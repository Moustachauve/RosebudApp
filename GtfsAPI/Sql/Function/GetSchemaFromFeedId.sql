DROP FUNCTION IF EXISTS `my_bus`.`GetSchemaFromFeedId`;
DELIMITER $$

CREATE FUNCTION `my_bus`.`GetSchemaFromFeedId`
(
	pFeedId INT
)
RETURNS VARCHAR(20)
BEGIN
	DECLARE database_name VARCHAR(20);
	SET database_name = 0;

	SELECT 
		`database_name` INTO database_name
	FROM `my_bus`.`feed` 
	WHERE `feed_id` = pFeedId;

	RETURN database_name;
    
END$$

DELIMITER ;
