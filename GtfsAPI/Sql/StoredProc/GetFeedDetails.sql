DROP PROCEDURE IF EXISTS `my_bus`.`GetFeedDetails`;
DELIMITER $$

CREATE PROCEDURE `my_bus`.`GetFeedDetails`
(
	IN pFeedId INT
)
BEGIN

	DECLARE schemaName VARCHAR(20) DEFAULT `my_bus`.`GetSchemaFromFeedId`(pFeedId);

	SET @sqlQuery= CONCAT('
		SELECT
			`agency_id`,
			`agency_name`,
			`agency_url`,
			`agency_timezone`,
			`agency_lang`,
			`agency_phone`,
			`agency_fare_url`,
			`agency_email`
		FROM `', schemaName,'`.`agency`');

    PREPARE stmt FROM @sqlQuery;
    EXECUTE stmt;
    DEALLOCATE PREPARE stmt;

END$$

DELIMITER ;
