var sqlHelper = require('../sqlHelper/sqlHelper.js')
var log = require('../log/log.js')

var exports = module.exports = {}

exports.getTripsForRoute = function (feedId, routeId, date, callback) {
	sqlHelper.acquire(function (err, client) {
		if (err) return callback(err)
		
		client.query("CALL `my_bus`.`GetTripsForRoute`(?, ?, ?)", [feedId, routeId, date], function (err, rows) {
			sqlHelper.release(client)
			if (err) return callback(err)
			
			callback(null, rows[0])
		})
	})
}
