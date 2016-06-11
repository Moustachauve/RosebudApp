var sqlHelper = require('../sqlHelper/sqlHelper.js')
var log = require('../log/log.js')


var exports = module.exports = {}

exports.getAllRoutes = function (feedId, callback) {
	sqlHelper.acquire(function (err, client) {
		if (err) return callback(err)
		
		//client.query("select `feed_id`, `short_name`, `long_name`, `last_update` FROM `my_bus`.`feed` WHERE data_valid = 1", [], function (err, rows) {
		client.query("CALL `my_bus`.`GetAllRoutes`(?)", [feedId], function (err, rows) {
			sqlHelper.release(client)
			
			if (err) return callback(err)
			
			callback(null, rows[0])
		})
	})
}
