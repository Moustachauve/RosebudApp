DROP PROCEDURE IF EXISTS `my_bus`.`GetStopsForTrip`;
DELIMITER $$

CREATE PROCEDURE `my_bus`.`GetStopsForTrip`
(
	IN pFeedId INT,
	IN pTripId varchar(30)
)
BEGIN
	
	DECLARE schemaName VARCHAR(20) DEFAULT `my_bus`.`GetSchemaFromFeedId`(pFeedId);

	CALL ExecuteQuery(CONCAT(
	'SELECT `stops`.`stop_id`, ',
			'`stop_code`, ',
            '`stop_name`, ',
            '`stop_desc`, ',
            '`stop_lat`, ',
            '`stop_lon`, ',
            '`zone_id`, ',
            '`stop_url`, ',
            '`location_type`, ',
            '`parent_station`, ',
            '`stop_timezone`, ',
            '`wheelchair_boarding`, ',
            '`arrival_time`, ',
            '`departure_time`, ',
            '`stop_sequence`, ',
            '`pickup_type`, ',
            '`drop_off_type`, ',
            '`shape_dist_traveled`, ',
            '`timepoint` ',
    'FROM `',schemaName,'`.`stops` ',
    'JOIN `',schemaName,'`.`stop_times` ON `stop_times`.`stop_id` = `stops`.`stop_id` ',
    'WHERE `stop_times`.`trip_id` = \'',pTripId,'\' ',
    'ORDER BY `stop_sequence` '
    ));
    
    CALL ExecuteQuery(CONCAT(
	'SELECT `shapes`.`shape_id`, ',
			'`shape_pt_lat`, ',
			'`shape_pt_lon`, ',
			'`shape_pt_sequence`, ',
			'`shape_dist_traveled` ',
    'FROM `',schemaName,'`.`trips` ',
    'JOIN `',schemaName,'`.`shapes` ON `shapes`.`shape_id` = `trips`.`shape_id` ',
    'WHERE `trip_id` = \'',pTripId,'\' ',
    'ORDER BY `shape_pt_sequence` '
    ));

END$$

DELIMITER ;

/*
CALL `my_bus`.`GetStopsForTrip`(1, '34_3_7_merged_102031044')
*/

