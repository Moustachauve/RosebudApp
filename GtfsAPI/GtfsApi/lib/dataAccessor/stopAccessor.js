var sqlHelper = require('../sqlHelper/sqlHelper.js')
var log = require('../log/log.js')

var exports = module.exports = {}

exports.getStopsForTrip = function (feedId, tripId, callback) {
	sqlHelper.acquire(function (err, client) {
		if (err) return callback(err)
		
		client.query("CALL `my_bus`.`GetStopsForTrip`(?, ?)", [feedId, tripId], function (err, rows) {
			sqlHelper.release(client)
			if (err) return callback(err)
			
			var result = {
				stops: rows[0],
				shape: rows[1]
			}

			callback(null, result)
		})
	})
}
