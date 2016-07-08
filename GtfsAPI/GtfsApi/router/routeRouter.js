var express = require('express')
var routeAccessor = require('../lib/dataAccessor/routeAccessor.js')
var router = express.Router({mergeParams: true})

router.get('/', function (req, res, next) {
	routeAccessor.getAllRoutes(req.params.feedId, function (err, data) {
		if (err) return next(err)
		
		res.json(data)
	})
})

router.get('/:routeId/', function (req, res, next) {
	routeAccessor.getRouteDetails(req.params.feedId, req.params.routeId, req.query.date, function (err, data) {
		if (err) return next(err)
		
		res.json(data)
	})
})

module.exports = router