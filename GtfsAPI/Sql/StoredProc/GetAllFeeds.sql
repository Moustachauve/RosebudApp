DROP PROCEDURE IF EXISTS `my_bus`.`GetAllFeeds`;
DELIMITER $$

CREATE PROCEDURE `my_bus`.`GetAllFeeds`
()
BEGIN

	SELECT 
		`feed_id`,
		`short_name`,
		`long_name`,
		`last_update`
	FROM `my_bus`.`feed`
	WHERE data_valid = '1';

END$$

DELIMITER ;
