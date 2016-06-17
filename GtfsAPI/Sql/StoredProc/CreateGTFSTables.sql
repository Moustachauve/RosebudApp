DROP PROCEDURE IF EXISTS `my_bus`.`CreateGTFSTables`;
DELIMITER $$

CREATE PROCEDURE `my_bus`.`CreateGTFSTables`
(
	IN pFeedId INT
)
BEGIN

	DECLARE schemaName VARCHAR(20) DEFAULT `my_bus`.`GetSchemaFromFeedId`(pFeedId);
	DECLARE schemaNameTemp VARCHAR(26) DEFAULT CONCAT(schemaName, '_temp');
	
	#Create temporary database structure
	CALL ExecuteQuery(CONCAT('DROP DATABASE IF EXISTS ', schemaNameTemp));
	CALL ExecuteQuery(CONCAT('CREATE DATABASE ', schemaNameTemp));

	CALL ExecuteQuery(CONCAT('CREATE TABLE IF NOT EXISTS `', schemaNameTemp,'`.`agency` 
	(
		`agency_id`				varchar(3)			NOT		NULL,
		`agency_name`			varchar(50)			NOT		NULL,
		`agency_url`			varchar(60)			NOT		NULL,
		`agency_timezone`		varchar(20)			NOT		NULL,
		`agency_lang`			char(2)				DEFAULT NULL,
		`agency_phone`			varchar(25)			DEFAULT NULL,
		`agency_fare_url`		varchar(255)		DEFAULT NULL,
		`agency_email`			varchar(100)		DEFAULT NULL,

		PRIMARY KEY				(`agency_id`)
	);'));

	CALL ExecuteQuery(CONCAT('CREATE TABLE IF NOT EXISTS `', schemaNameTemp,'`.`stops` 
	(
		`stop_id`				varchar(8)			NOT		NULL,
		`stop_code`				varchar(5)			DEFAULT NULL,
		`stop_name`				varchar(60)			NOT		NULL,
		`stop_desc`				varchar(150)		DEFAULT NULL,
		`stop_lat`				varchar(10)			NOT		NULL,
		`stop_lon`				varchar(10)			NOT		NULL,
		`zone_id`				varchar(20)			DEFAULT NULL,
		`stop_url`				varchar(60)			DEFAULT NULL,
		`location_type`			tinyint				DEFAULT NULL,
		`parent_station`		varchar(8)			DEFAULT NULL,
		`stop_timezone`			varchar(20)			DEFAULT NULL,
		`wheelchair_boarding`	tinyint				DEFAULT 0,

		PRIMARY KEY				(`stop_id`)
	);'));

	CALL ExecuteQuery(CONCAT('CREATE TABLE IF NOT EXISTS `', schemaNameTemp,'`.`routes` 
	(
		`route_id`				varchar(3)			NOT		NULL,
		`agency_id`				varchar(3)			DEFAULT NULL,
		`route_short_name`		varchar(10)			NOT		NULL,
		`route_long_name`		varchar(75)			NOT		NULL,
		`route_desc`			varchar(100)		DEFAULT NULL,
		`route_type`			tinyint				NOT		NULL,
		`route_url`				varchar(100)		DEFAULT NULL,
		`route_color`			char(6)				DEFAULT \'FFFFFF\',
		`route_text_color`		char(6)				DEFAULT \'000000\',

		PRIMARY KEY				(`route_id`)
	);'));

	CALL ExecuteQuery(CONCAT('CREATE TABLE IF NOT EXISTS `', schemaNameTemp,'`.`trips` 
	(
		`trip_id`				varchar(20)			NOT		NULL,
		`service_id`			varchar(3)			NOT		NULL,
		`route_id`				varchar(3)			NOT		NULL,
		`trip_headsign`			varchar(60)			DEFAULT NULL,
		`trip_short_name`		varchar(20)			DEFAULT NULL,
		`direction_id`			tinyint				DEFAULT NULL,
		`block_id`				varchar(20)			DEFAULT NULL,
		`shape_id`				varchar(15)			DEFAULT NULL,
		`wheelchair_accessible`	tinyint				DEFAULT 0,
		`bikes_allowed`			tinyint				DEFAULT 0,
		`note_fr`				varchar(255)		DEFAULT NULL,
		`note_en`				varchar(255)		DEFAULT NULL,

		PRIMARY KEY				(`trip_id`)
	);'));

	CALL ExecuteQuery(CONCAT('CREATE TABLE IF NOT EXISTS `', schemaNameTemp,'`.`stop_times` 
	(
		`trip_id`				varchar(20)			NOT		NULL,
		`arrival_time`			char(8)				NOT		NULL,
		`departure_time`		char(8)				NOT		NULL,
		`stop_id`				varchar(8)			NOT		NULL,
		`stop_sequence`			smallint UNSIGNED 	NOT		NULL,
		`stop_headsign`			varchar(50)			DEFAULT NULL,
		`pickup_type`			tinyint				DEFAULT 0,
		`drop_off_type`			tinyint				DEFAULT 0,
		`shape_dist_traveled`	varchar(10)			DEFAULT NULL,
		`timepoint`				tinyint				DEFAULT 1
	);'));

	CALL ExecuteQuery(CONCAT('CREATE TABLE IF NOT EXISTS `', schemaNameTemp,'`.`calendar` 
	(
		`service_id`			varchar(3)			NOT		NULL,
		`monday`				tinyint				NOT		NULL,
		`tuesday`				tinyint				NOT		NULL,
		`wednesday`				tinyint				NOT		NULL,
		`thursday`				tinyint				NOT		NULL,
		`friday`				tinyint				NOT		NULL,
		`saturday`				tinyint				NOT		NULL,
		`sunday`				tinyint				NOT		NULL,
		`start_date`			char(8)				NOT		NULL,
		`end_date`				char(8)				NOT		NULL,

		PRIMARY KEY				(`service_id`)
	);'));

	CALL ExecuteQuery(CONCAT('CREATE TABLE IF NOT EXISTS `', schemaNameTemp,'`.`calendar_dates` 
	(
		`service_id`			varchar(3)			NOT		NULL,
		`date`					char(8)				NOT		NULL,
		`exception_type`		tinyint				NOT		NULL,

		PRIMARY KEY				(`service_id`, `date`)
	);'));

	CALL ExecuteQuery(CONCAT('CREATE TABLE IF NOT EXISTS `', schemaNameTemp,'`.`fare_attributes` 
	(
		`fare_id`				varchar(15)			NOT		NULL,
		`price`					decimal(6,2)		NOT		NULL,
		`currency_type`			char(3)				NOT		NULL,
		`payment_method`		tinyint				NOT		NULL,
		`transfers`				tinyint				DEFAULT NULL,
		`transfer_duration`		int UNSIGNED 		DEFAULT NULL,

		PRIMARY KEY				(`fare_id`)
	);'));

	CALL ExecuteQuery(CONCAT('CREATE TABLE IF NOT EXISTS `', schemaNameTemp,'`.`fare_rules` 
	(
		`fare_id`				varchar(15)			NOT		NULL,
		`route_id`				varchar(3)			DEFAULT NULL,
		`origin_id`				varchar(15)			DEFAULT NULL,
		`destination_id`		varchar(15)			DEFAULT NULL,
		`contains_id`			varchar(15)			DEFAULT NULL
	);'));

	CALL ExecuteQuery(CONCAT('CREATE TABLE IF NOT EXISTS `', schemaNameTemp,'`.`shapes` 
	(
		`shape_id`				varchar(15)			NOT		NULL,
		`shape_pt_lat`			varchar(10)			NOT		NULL,
		`shape_pt_lon`			varchar(10)			NOT		NULL,
		`shape_pt_sequence`		smallint UNSIGNED 	NOT		NULL,
		`shape_dist_traveled`	varchar(20)			DEFAULT NULL
	);'));

	CALL ExecuteQuery(CONCAT('CREATE TABLE IF NOT EXISTS `', schemaNameTemp,'`.`frequencies` 
	(
		`trip_id`				varchar(20)			NOT		NULL,
		`start_time`			char(8)				NOT		NULL,
		`end_time`				char(8)				NOT		NULL,
		`headway_secs`			smallint UNSIGNED	NOT		NULL,
		`exact_times`			tinyint				DEFAULT 0
	);'));

	CALL ExecuteQuery(CONCAT('CREATE TABLE IF NOT EXISTS `', schemaNameTemp,'`.`transfers` 
	(
		`from_stop_id`			varchar(8)			NOT		NULL,
		`to_stop_id`			varchar(8)			NOT		NULL,
		`transfer_type`			tinyint				DEFAULT 0,
		`min_transfer_time`		int UNSIGNED 		DEFAULT 0
	);'));

	CALL ExecuteQuery(CONCAT('CREATE TABLE IF NOT EXISTS `', schemaNameTemp,'`.`feed_info` 
	(
		`feed_publisher_name`	varchar(50)			NOT		NULL,
		`feed_publisher_url`	varchar(100)		NOT		NULL,
		`feed_lang`				char(2)				NOT		NULL,
		`feed_start_date`		char(8)		 		DEFAULT NULL,
		`feed_end_date`			char(8)		 		DEFAULT NULL,
		`feed_version`			varchar(20)	 		DEFAULT NULL
	);'));

	SELECT schemaNameTemp;

END$$

DELIMITER ;

