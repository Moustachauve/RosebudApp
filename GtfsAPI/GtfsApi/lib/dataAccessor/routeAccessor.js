var sqlHelper = require('../sqlHelper/sqlHelper.js')
var log = require('../log/log.js')
naturalCompare = require("natural-compare-lite")

var exports = module.exports = {}

exports.getAllRoutes = function (feedId, callback) {
	sqlHelper.acquire(function (err, client) {
		if (err) return callback(err)
		
		//client.query("select `feed_id`, `short_name`, `long_name`, `last_update` FROM `my_bus`.`feed` WHERE data_valid = 1", [], function (err, rows) {
		client.query("CALL `my_bus`.`GetAllRoutes`(?)", [feedId], function (err, rows) {
			sqlHelper.release(client)
			if (err) return callback(err)
			
			rows[0].sort(function(a, b){
				return naturalCompare(a.route_short_name, b.route_short_name)
			})

			callback(null, rows[0])
		})
	})
}
