var express = require('express');
var router = express.Router();

var once_db = require('../models/db_notice');

router.get('/', function(req, res, next) {
		res.render('index', {title : 'notice관련 페이지'});
});

router.get('/:page', function( req, res) {
	var page = req.params.page;

	once_db.notice(page, function(results) {
		res.json(results);
	});
});

router.get('/detail/:num', function(req, res) {
	var notice_num = req.params.num;
	once_db.noticeDetail(notice_num, function(results){
		res.json(results);
	});
});

router.post('/newNotice', function(req, res) {
	var notice_title = req.body.noticeTitle;
	var notice_content = req.body.noticeContent;
	var notice_category = req.body.noticeCategory;
	var datas = [notice_title, notice_category, notice_content];
	once_db.newNotice(datas, function(results) {
		res.json(results);
	});
});

router.post('/revise', function(req, res) {
	var notice_num = req.body.noticeNum;
	var notice_title = req.body.noticeTitle;
	var notice_category = req.body.noticeCategory;
	var notice_content = req.body.noticeContent;
	var datas = [notice_title, notice_category, notice_content, notice_num];
	once_db.reviseNotice(datas, function(results) {
		res.json(results);
	});
});

module.exports = router;
