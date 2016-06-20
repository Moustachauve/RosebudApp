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
    
    SET dateDayOfWeek = (SELECT DAYOFWEEK(pDayDate));
    
    CALL ExecuteQuery(CONCAT(
		'SELECT `service_id` INTO @serviceId ',
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
		'END = 1'));
    
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
									'`note_fr`, ',
									'`note_en`, ',
                                    '(',
										'SELECT `departure_time` ',
										'FROM `',schemaName,'`.`stop_times` ',
										'WHERE `stop_times`.`trip_id` = `trips`.`trip_id` ',
										'ORDER BY `stop_sequence` ASC ',
										'LIMIT 1 ',
									') AS start_time, ',
									'( ',
										'SELECT `departure_time` ',
										'FROM `',schemaName,'`.`stop_times` ',
										'WHERE `stop_times`.`trip_id` = `trips`.`trip_id` ',
										'ORDER BY `stop_sequence` DESC ',
										'LIMIT 1 ',
									') AS end_time ',
		'FROM `',schemaName,'`.`trips` ',
        'LEFT JOIN `',schemaName,'`.`calendar_dates` ON `calendar_dates`.`service_id` = `trips`.`service_id` ',
		'JOIN `',schemaName,'`.`routes` ON `routes`.`route_id` = `trips`.`route_id` ',
		'WHERE `routes`.`route_id` = \'',pRouteId,'\' ',
			'AND (',
				'   (`calendar_dates`.`date` = \'',pDayDate,'\' AND `calendar_dates`.`exception_type` != 2) ',
				'OR (`calendar_dates`.`date` = \'',pDayDate,'\' AND `calendar_dates`.`exception_type` = 1) ',
				'OR (`trips`.`service_id` = \'',@serviceId,'\') ',
			')',
		'GROUP BY `trip_id`',
		'ORDER BY `start_time`'));

END$$

DELIMITER ;

/*
	CALL `my_bus`.`GetTripsForRoute`(4, '4', '20160620')
*/