var sqlConfig = require('./sqlConfig.json')
var genericPool = require('generic-pool').Pool
var sqlClient = require('mariasql')
var fs = require("fs")

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
	// if true, logs via console.log - can also be a function
	log : true
})

var exports = module.exports = pool
