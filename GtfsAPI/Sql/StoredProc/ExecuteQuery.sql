DROP PROCEDURE IF EXISTS `my_bus`.`ExecuteQuery`;
DELIMITER $$

CREATE PROCEDURE `my_bus`.`ExecuteQuery`
(
	IN pSqlQuery TEXT
)
BEGIN
	SET @sqlQuery = pSqlQuery;
	PREPARE stmt FROM @sqlQuery;
	EXECUTE stmt;
	DEALLOCATE PREPARE stmt;
END$$

DELIMITER ;