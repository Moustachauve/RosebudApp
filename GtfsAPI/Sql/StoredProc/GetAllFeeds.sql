DROP PROCEDURE IF EXISTS `my_bus`.`GetAllFeeds`;
DELIMITER $$

CREATE PROCEDURE `my_bus`.`GetAllFeeds`
()
BEGIN
    
    set @stmt = 
  (select group_concat(concat(
                 'SELECT feed_id,',
					 'last_update,',
					 'agency_name,',
					 'agency_url,',
					 'agency_timezone,',
					 'agency_lang,',
					 'agency_phone,',
					 'agency_fare_url,',
					 'agency_email ',
                 'FROM `my_bus`.`feed` feed ',
                 'JOIN `',database_name,'`.`agency` agency ', 
                 'ON feed.database_name = ''',database_name,''' ') 
                 SEPARATOR ' union all ')
   FROM (SELECT database_name 
         FROM `my_bus`.`feed`
         WHERE data_valid = 1
         GROUP BY database_name) AS dbdata);

prepare stmt from @stmt;
execute stmt;

END$$

DELIMITER ;
