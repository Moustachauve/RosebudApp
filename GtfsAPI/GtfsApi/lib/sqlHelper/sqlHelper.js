var sqlConfig = require('./sqlConfig.json')
var genericPool = require('generic-pool').Pool
var sqlClient = require('mariasql')
var fs = require("fs")
var log = require('../log/log.js')

var pool = new genericPool({
	name: 'sql',
	create: function (callback) {
		var client = new sqlClient({
			host: sqlConfig.host,
			user: sqlConfig.user,
			password: sqlConfig.password,
			multiStatements: true,
			local_infile: true
		})
		
		callback(null, client)
	},
	destroy  : function (client) { client.end() },
	max      : 10,
	min      : 2,
	idleTimeoutMillis : 30000,
	/*log : function (logString, logLevel) {
		log.log(logLevel, logString)
	}*/
})

var exports = module.exports = pool
