var express = require('express')
var feedAccessor = require('../lib/dataAccessor/feedAccessor.js')
var router = express.Router()

router.get('/', function (req, res, next) {
	feedAccessor.getAllFeeds(function (err, data) {
		if (err) return next(err)

		res.json(data)
	})
})

module.exports = router