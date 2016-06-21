var sqlHelper = require('../sqlHelper/sqlHelper.js')
var log = require('../log/log.js')

var exports = module.exports = {}

exports.getTripsForRoute = function (feedId, routeId, callback) {
	sqlHelper.acquire(function (err, client) {
		if (err) return callback(err)
		
		client.query("CALL `my_bus`.`GetTripsForRoute`(?, ?, '20160620')", [feedId, routeId], function (err, rows) {
			sqlHelper.release(client)
			if (err) return callback(err)
			
			callback(null, rows[0])
		})
	})
}
