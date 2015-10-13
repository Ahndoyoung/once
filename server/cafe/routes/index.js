var express = require('express');
var router = express.Router();


/* GET home page. */
router.get('/', function(req, res, next) {
		res.render('index', { title: 'once', myValue: 'aaa' })
});

router.get('/test', function(req, res, next) {
		res.render('gallery', { title: 'once'})
});


module.exports = router;
