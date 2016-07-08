var sqlHelper = require('../sqlHelper/sqlHelper.js')
var log = require('../log/log.js')
naturalCompare = require("natural-compare-lite")

var exports = module.exports = {}

exports.getAllRoutes = function (feedId, callback) {
	sqlHelper.acquire(function (err, client) {
		if (err) return callback(err)
		
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

exports.getRouteDetails = function (feedId, routeId, date, callback) {
	sqlHelper.acquire(function (err, client) {
		if (err) return callback(err)
		
		client.query("CALL `my_bus`.`GetTripsForRoute`(?, ?, ?)", [feedId, routeId, date], function (err, rows) {
			sqlHelper.release(client)
			if (err) return callback(err)
			
			var result = {
				trips: rows[0]
			}

			callback(null, result)
		})
	})
}
