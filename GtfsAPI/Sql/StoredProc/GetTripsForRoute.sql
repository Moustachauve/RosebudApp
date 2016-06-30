DROP PROCEDURE IF EXISTS `my_bus`.`GetTripsForRoute`;
DELIMITER $$

CREATE PROCEDURE `my_bus`.`GetTripsForRoute`
(
	IN pFeedId INT,
	IN prouteId varchar(30),
    IN pDayDate char(8)
)
BEGIN
	
	DECLARE schemaName VARCHAR(20) DEFAULT `my_bus`.`GetSchemaFromFeedId`(pFeedId);
    DECLARE dateDayOfWeek INT;
    SET @serviceId = '';
    
    SET dateDayOfWeek = (SELECT DAYOFWEEK(pDayDate));
    
    CALL ExecuteQuery(CONCAT(
		'SELECT GROUP_CONCAT(CONCAT(\'[\', `service_id`, \']\')) INTO @serviceId FROM ( ',
			'SELECT `service_id` ',
			'FROM `',schemaName,'`.`calendar_dates` ',
			'WHERE `date` = \'',pDayDate,'\' ',
				'AND `calendar_dates`.`exception_type` = 1 ',
			'UNION ',
			'SELECT `service_id` ',
			'FROM `',schemaName,'`.`calendar` ',
			'WHERE \'',pDayDate,'\' BETWEEN `start_date` AND `end_date` ',
			'AND CASE ',
				'WHEN ',dateDayOfWeek,' = 1 AND sunday = 1 THEN 1 ',
				'WHEN ',dateDayOfWeek,' = 2 AND monday = 1 THEN 1 ',
				'WHEN ',dateDayOfWeek,' = 3 AND tuesday = 1 THEN 1 ',
				'WHEN ',dateDayOfWeek,' = 4 AND wednesday = 1 THEN 1 ',
				'WHEN ',dateDayOfWeek,' = 5 AND thursday = 1 THEN 1 ',
				'WHEN ',dateDayOfWeek,' = 6 AND friday = 1 THEN 1 ',
				'WHEN ',dateDayOfWeek,' = 7 AND saturday = 1 THEN 1 ',
				'ELSE 0 ',
			'END = 1 ',
        ') AS `service`'));
    
    IF @serviceId IS NULL THEN
		SET @serviceId = '';
	END IF;
                
	CALL ExecuteQuery(CONCAT('SELECT `trips`.`trip_id`, ',
									'`trip_headsign`, ',
									'`trip_short_name`, ',
									'`direction_id`, ',
									'`block_id`, ',
									'`shape_id`, ',
									'`wheelchair_accessible`, ',
									'`bikes_allowed`, ',
									'`headway_secs`, ',
									'`note_fr`, ',
									'`note_en`, ',
                                    'CASE WHEN `headway_secs` IS NOT NULL THEN `frequencies`.`start_time`',
                                    'ELSE (',
										'SELECT `departure_time` ',
										'FROM `',schemaName,'`.`stop_times` ',
										'WHERE `stop_times`.`trip_id` = `trips`.`trip_id` ',
										'ORDER BY `stop_sequence` ASC ',
										'LIMIT 1 ',
									') END AS start_time, ',
                                    'CASE WHEN `headway_secs` IS NOT NULL THEN `frequencies`.`end_time`',
									'ELSE ( ',
										'SELECT `departure_time` ',
										'FROM `',schemaName,'`.`stop_times` ',
										'WHERE `stop_times`.`trip_id` = `trips`.`trip_id` ',
										'ORDER BY `stop_sequence` DESC ',
										'LIMIT 1 ',
									') END AS end_time ',
		'FROM `',schemaName,'`.`trips` ',
        'LEFT JOIN `',schemaName,'`.`calendar_dates` ON `calendar_dates`.`service_id` = `trips`.`service_id` AND `calendar_dates`.`date` = \'',pDayDate,'\' ',
        'LEFT JOIN `',schemaName,'`.`frequencies` ON `frequencies`.`trip_id` = `trips`.`trip_id` ',
		'JOIN `',schemaName,'`.`routes` ON `routes`.`route_id` = `trips`.`route_id` ',
		'WHERE `routes`.`route_id` = \'',pRouteId,'\' ',
		'AND \'',@serviceId,'\' LIKE CONCAT(\'%[\', `trips`.`service_id`, \']%\') ',
        'AND COALESCE(`exception_type`, 1) = 1 '
		'ORDER BY `direction_id`, `start_time`'));

END$$

DELIMITER ;

/*
	CALL `my_bus`.`GetTripsForRoute`(4, '1', '20160620')

	CALL `my_bus`.`GetTripsForRoute`(1, '34', '20160623')

	CALL `my_bus`.`GetTripsForRoute`(6, '8401', '20160620')
*/