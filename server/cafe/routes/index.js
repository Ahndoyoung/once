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
			res.render('menu', {results : results.results});
		})
});


router.get('/notice/:page', function( req, res) {
	var page = req.params.page;
	noticeDB.notice(page, function(results) {
		res.render('notice',{notice : results.results});
	});
});


//이런식으로 index에 추가하면 해당 결로가 잡힘.
//index의 경로가 그냥 접속 주소이기 때문에
// 이 경로는 주소/notice로 됨.
// localhost로 주소 잡으면 localhost/notice로 됨.
router.get('/notice', function(req, res, next) {
	res.render('notice', {});		//
	//res.render는 ejs파일로 연결시켜주는 거.
	//notice는 ejs파일 경로. render를 하면 경로가 바로 view로 잡히기 때문에 따로 안잡아 줘도 됨.
	// 만약 view/insta에 있는 ejs파일로 연결 시킬거면
	//			insta/notice 이런식으로 하면 됨.
	//뒤에 {}는 넘겨줄 객체, 근데 넘겨줄게 없어서 빈 객체
});

router.get('/introduction', function(req, res, next) {
	res.render('introduction', {});		//
});

module.exports = router;
