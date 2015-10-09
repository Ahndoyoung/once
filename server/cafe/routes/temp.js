var express = require('express');
var router = express.Router();

var once_db = require('../models/db_main');

/* GET home page. */
router.get('/', function(req, res, next) {
		once_db.main(function(results){
			res.json(results);
		});
});

router.post('/signUp', function ( req, res, next) {
	var userId = req.body.managerId;
	var pw = req.body.pw;

	var datas = [userId, pw];

	once_db.signUp(datas, function  (results) {
		res.json(results);
	});
});

router.post('/manager', function(req, res) {
	var managerID = req.body.managerId;
	var managerPassword = req.body.pw;
	var datas = [managerID, managerPassword];
	once_db.manager(datas, function(results){
		res.json(results);
	});
})

module.exports = router;
