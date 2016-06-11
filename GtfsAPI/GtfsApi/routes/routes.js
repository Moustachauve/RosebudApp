var express = require('express')
var routeAccessor = require('../lib/dataAccessor/routeAccessor.js')
var router = express.Router()

router.get('/:feedId', function (req, res, next) {
	routeAccessor.getAllRoutes(req.params.feedId, function (err, data) {
		if (err) return next(err)
		
		res.json(data)
	})
})

module.exports = router