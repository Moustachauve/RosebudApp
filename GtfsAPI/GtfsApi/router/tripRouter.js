var express = require('express')
var tripAccessor = require('../lib/dataAccessor/tripAccessor.js')
var router = express.Router({mergeParams: true})

router.get('/', function (req, res, next) {
	tripAccessor.getTripsForRoute(req.params.feedId, req.params.routeId, req.query.date, function (err, data) {
		if (err) return next(err)
		
		res.json(data)
	})
})

module.exports = router