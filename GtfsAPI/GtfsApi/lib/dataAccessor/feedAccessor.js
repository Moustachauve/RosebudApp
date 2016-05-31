var sqlHelper = require('../sqlHelper/sqlHelper.js')



var exports = module.exports = {}

exports.getAllFeeds = function (callback) {
	sqlHelper.acquire(function (err, client) {
		if (err) return callback(err)
		
		client.query("select `feed_id`, `short_name`, `long_name`, `last_update` from `my_bus`.`feed` WHERE data_valid = 1", [], function (err, rows) {
			sqlHelper.release(client);
			
			if (err) return callback(err)
			
			callback(null, rows)
		});
	});
}