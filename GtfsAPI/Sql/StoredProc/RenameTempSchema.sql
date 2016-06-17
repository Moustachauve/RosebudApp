DROP PROCEDURE IF EXISTS `my_bus`.`RenameTempSchema`;
DELIMITER $$

CREATE PROCEDURE `my_bus`.`RenameTempSchema`
(
	IN pFeedId INT,
    IN pTempSchemaName VARCHAR(25)
)
BEGIN
        DECLARE schemaName VARCHAR(20) DEFAULT `my_bus`.`GetSchemaFromFeedId`(pFeedId);
        
        CALL ExecuteQuery(CONCAT('DROP DATABASE IF EXISTS ', schemaName));
        CALL ExecuteQuery(CONCAT('CREATE DATABASE ', schemaName));
		CALL ExecuteQuery((SELECT CONCAT('RENAME TABLE ', GROUP_CONCAT( table_schema,'.',table_name, ' TO ', schemaName, '.',table_name,' '),';') as stmt FROM information_schema.TABLES WHERE table_schema LIKE pTempSchemaName GROUP BY table_schema));
        CALL ExecuteQuery(CONCAT('DROP DATABASE ', pTempSchemaName));
END$$

DELIMITER ;