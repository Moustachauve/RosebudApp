DROP PROCEDURE IF EXISTS `my_bus`.`GetTripDetails`;
DELIMITER $$

CREATE PROCEDURE `my_bus`.`GetTripDetails`
(
	IN pFeedId INT,
	IN pTripId varchar(30))
BEGIN

	DECLARE schemaName VARCHAR(20) DEFAULT `my_bus`.`GetSchemaFromFeedId`(pFeedId);
    
	CALL ExecuteQuery(CONCAT('SELECT * ',
		'FROM `', schemaName, '`.`stops` ',
		'JOIN `stop_times` ON `stop_times`.`stop_id` = `stops`.`stop_id` ',
		'WHERE `stop_times`.`trip_id` = \'', pTripId, '\' ',
		'GROUP BY `stops`.`stop_id` ',
		'ORDER BY `stop_sequence`'));
    
    
END$$

DELIMITER ;
