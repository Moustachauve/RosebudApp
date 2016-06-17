DROP PROCEDURE IF EXISTS `my_bus`.`GetAllFeeds`;
DELIMITER $$

CREATE PROCEDURE `my_bus`.`GetAllFeeds`
()
BEGIN
    SET SESSION group_concat_max_len = 10000;
    SET @stmt = 
	(SELECT CONCAT(GROUP_CONCAT(CONCAT(
                 'SELECT `feed_id`,',
					 '`last_update`,',
					 '`keywords`,',
					 '`agency_name`,',
					 '`agency_url`,',
					 '`agency_timezone`,',
					 '`agency_lang`,',
					 '`agency_phone`,',
					 '`agency_fare_url`,',
					 '`agency_email` ',
                 'FROM `my_bus`.`feed` feed ',
                 'JOIN `',database_name,'`.`agency` agency ', 
                 'ON feed.database_name = ''',database_name,''' ')
                 SEPARATOR ' union all '),
                 ' ORDER BY `agency_name`')
			
	FROM (SELECT database_name 
         FROM `my_bus`.`feed`
         WHERE data_valid = 1
         GROUP BY database_name) AS dbdata);
	#SELECT @stmt;
	PREPARE stmt FROM @stmt;
	EXECUTE stmt;

END$$

DELIMITER ;
