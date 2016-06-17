DROP FUNCTION IF EXISTS `my_bus`.`GenerateRenameQuery`;
DELIMITER $$

CREATE FUNCTION `my_bus`.`GenerateRenameQuery`
(
	pSchemaName VARCHAR(20),
    pTempSchemaName VARCHAR(25)
)
RETURNS VARCHAR(500)
BEGIN
	RETURN CONCAT('SELECT CONCAT(\'RENAME TABLE ', pTempSchemaName, '.\',table_name, \' TO ', pSchemaName, '.\',table_name, \';\')',
			' FROM information_schema.TABLES',
			' WHERE table_schema=\'', pTempSchemaName ,'\'');
    
END$$

DELIMITER ;
