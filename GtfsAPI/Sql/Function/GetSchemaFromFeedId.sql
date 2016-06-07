DROP FUNCTION IF EXISTS `my_bus`.`GetSchemaFromFeedId`;
DELIMITER $$

CREATE FUNCTION `my_bus`.`GetSchemaFromFeedId`
(
	pFeedId INT
)
RETURNS VARCHAR(20)
BEGIN
	DECLARE return_value VARCHAR(20);

	SELECT 
		`database_name` INTO return_value
	FROM `my_bus`.`feed` 
	WHERE `feed_id` = pFeedId;

	RETURN return_value;
    
END$$

DELIMITER ;
