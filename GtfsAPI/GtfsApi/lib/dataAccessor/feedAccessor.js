var sqlHelper = require('../sqlHelper/sqlHelper.js')
var log = require('../log/log.js')


var exports = module.exports = {}

exports.getAllFeeds = function (callback) {
	sqlHelper.acquire(function (err, client) {
		if (err) return callback(err)
		
		client.query("select `feed_id`, `short_name`, `long_name`, `last_update` FROM `my_bus`.`feed` WHERE data_valid = 1", [], function (err, rows) {
			sqlHelper.release(client)
			
			if (err) return callback(err)
			
			callback(null, rows)
		})
	})
}

exports.getSchemaFromFeedId = function (feedId, callback) {
	sqlHelper.acquire(function (err, client) {
		if (err) return callback(err)
		
		client.query("select `database_name` FROM `my_bus`.`feed` WHERE feed_id = ?", [feedId], function (err, rows) {
			sqlHelper.release(client)
			
			if (err) return callback(err)
			
			callback(null, rows[0].database_name)
		})
	})

}

exports.getFeedDetails = function (feedId, callback) {
	exports.getSchemaFromFeedId(feedId, function (err, result) {
		sqlHelper.acquire(function (err, client) {
			if (err) return callback(err)

			client.query("select `agency_id`, `agency_name`, `agency_url`, `agency_timezone`, `agency_lang`, `agency_phone`, `agency_fare_url`, `agency_email` FROM `" + result + "`.`agency`", [], function (err, rows) {
				sqlHelper.release(client)
				
				if (err) return callback(err)
				
				callback(null, rows)
			})
		})
	})
}