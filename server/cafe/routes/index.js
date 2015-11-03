var express = require('express');
var router = express.Router();

var menuDB = require('../models/db_menu');
var noticeDB = require('../models/db_notice');
var instagram = require('instagram-node').instagram();

instagram.use({
	client_id: '691493c610ae41d6a4017b8d0df1236a',
	client_secret: '408bcba50eb648eb88a91a16c0a98095'
});

/* GET home page. */
router.get('/', function(req, res, next) {
		res.render('index', { title: 'once', myValue: 'aaa' })
});

router.get('/test', function(req, res, next) {
		if(instagram.user_media_recent('2042470368', function(err, result, pagination, remaining, limit) {
		res.render('gallery', { grams: result , title : 'once'});
	}));
});

router.get('/menu/:menu', function(req, res) {
	var menu_category = req.params.menu;
	menu_category = menu_category.toLowerCase();
		menuDB.menu(menu_category, function(results){
			res.render('menu', {results: results.results});
		})
});
router.get('/notice/:page', function( req, res) {
   var page = req.params.page;
   noticeDB.notice(page, function(results) {
   	console.log("results:",results);
      res.render('notice',{notice : results.results});
   });
});

router.get('/introduction', function(req, res, next) {
   res.render('introduction', {});      
});


module.exports = router;
