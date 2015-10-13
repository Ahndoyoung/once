

var mysql = require('mysql');
var crypto =require('crypto');

//52.26.40.174
var iterations = 1000;      //암호화 반복 횟수
var keylen = 24;    //암호화 후 생성되는 key 길이 설정

var pool = mysql.createPool({
	connectionLimit : 150,
	host : "52.26.40.174",
	port : 3025,
	user: 'root',
	password: 'once1234',
	database: 'OnceDB'
});

var ip = "http://52.26.40.174";

exports.main = function(callback){
	var results = {};
	var err_results = {};
	err_results.success = 2;
	err_results.message = "main fail";
	err_results.notice = [];
	err_results.gallery = [];
	pool.getConnection(function(err, conn) {
		if(err){
			console.error('db notice err = ', err);
		} else {
			var noticeSQL = 'SELECT NOTICE_NUM AS noticeNum, NOTICE_TITLE AS noticeTile, NOTICE_CATEGORY_NAME AS noticeCategory, DATE_FORMAT(NOTICE_TIME, "%y-%m-%d %h:%i:%s") AS noticeTime FROM NOTICE, NOTICE_CATEGORY WHERE NOTICE.NOTICE_CATEGORY_NUM=NOTICE_CATEGORY.NOTICE_CATEGORY_NUM ORDER BY NOTICE_NUM DESC LIMIT 10';			//공지사항 관련 데이터
			var gallerySQL = 'SELECT GALLERY_IMAGE_THUMBNAIL_PATH AS imagePath FROM GALLERY ORDER BY GALLERY_NUM DESC LIMIT 9'		//갤러리 사진 관련 데이터
			conn.query(noticeSQL,function(noticeErr, noticeRow) {
				if(noticeErr){
					console.error('noticeSQL error = ', noticeErr);
					conn.release();
					callback(err_results);
				} else {
					conn.query(gallerySQL, function(galleryErr, galleryRow){
						if(galleryErr){
							console.error('gallerySQL error = ', galleryErr);
							conn.release();
							callback(err_results);
						} else {
							results.success = 1;
							results.message = "main success";
							results.notice = noticeRow;
							results.gallery = galleryRow;
							callback(results);
						}
					})
				}
			})
		}
	})
}

// datas = [managerID, managerPassword]
exports.manager = function(datas, callback){
	var id = datas[0];
	var pw = datas[1] + "";
	var callback_data = {};
	var err_results = {};
	err_results.success = 2;
	err_results.message = "manager fail";

	pool.getConnection(function(err, conn){
		if(err) {
			console.error('manager login err = ', err);
			callback(err_results);
		} else {
			var managerSQL = 'SELECT ACCOUNT_NUM, ACCOUNT_PASSWORD, ACCOUNT_ADMIN FROM ACCOUNT WHERE ACCOUNT_ID =?';
			conn.query(managerSQL, id, function(managerErr, managerRow){
				if(managerErr){
					console.error('managerErr = ', managerErr);
					conn.release();
					callback(err_results);
				} else {//아이디 찾앗고 로그인

				}
			})
		}
	});
}

//datas = [id, pw]
exports.signUp = function (datas, callback ){
	var results = {};
	var pw = datas[1] + "";
	var salt = Math.round((new Date().valueOf() * Math.random())) + '';//salt값 생성
	var key = crypto.pbkdf2Sync(pw, salt, iterations, keylen);  //암호화
	var pw_cryp = Buffer(key, 'binary').toString('hex');     //암호화된 값 생성.
	datas[1] = pw_cryp;
	datas.push(salt);
	datas.push('N');

	pool.getConnection(function ( connErr, conn) {
		if(connErr) {
			console.error('connErr = ', connErr);
			results.success = 1;
			results.message  = 'get connection error';
			results.results = {};
			callback(results);
		} else {
			var idSQL = 'SELECT * FROM ACCOUNT WHERE ACCOUNT_ID=?';
			conn.query(idSQL, datas[0], function (idErr, idRow) {
				if(idErr){
					console.error('idErr = ', idErr);
					conn.release();
					results.success = 2;
					results.message = 'sign up fail';
					callback(results);
				} else if(idRow.length == 0){
					var signUpSQL = 'INSERT INTO ACCOUNT(ACCOUNT_ID, ACCOUNT_PASSWORD, ACCOUNT_SALT, ACCOUNT_ADMIN) VALUES(?,?,?,?)';
					conn.query(signUpSQL, datas, function (siErr, siRow) {
						if(siErr){
							console.error('siErr = ', siErr);
							conn.release();
							results.success = 2;
							results.message = 'sign up fail';
							callback(results);
						} else {
							results.success = 1;
							results.message = 'sign up success';
							conn.release();
							callback(results);
						}
					});
				} else {
					conn.release();
					results.success = 3;
					results.message = 'id exist';
					callback(results);
				}
			});
		}
	});
}

exports.manager = function (datas, callback) {
	var results = {};
	pool.getConnection( function (connErr, conn) {
		if(connErr) {
			console.error('connErr = ', connErr);
			results.success = 1;
			results.message  = 'get connection error';
			results.results = {};
			callback(results);
		} else {
			var userSQL = 'SELECT ACCOUNT_SALT, ACCOUNT_ADMIN, ACCOUNT_NUM, ACCOUNT_PASSWORD FROM ACCOUNT WHERE ACCOUNT_ID=?';
			conn.query(userSQL, datas[0], function (userErr, userRow) {
				if(userErr) {
					console.error('userErr = ', userErr);
					conn.release();
					results.success = 2;
					results.message = 'login fail';
					callback(results);
				} else if(userRow.length == 1 ) {
					var pw = datas[1] + "";
					var key = crypto.pbkdf2Sync(pw, userRow[0].ACCOUNT_SALT, iterations, keylen);  //암호화
					var pw_cryp = Buffer(key, 'binary').toString('hex');     //암호화된 값 생성.
					if(pw_cryp == userRow[0].ACCOUNT_PASSWORD) { //비번 맞음
						conn.release();
						results.success = 1;
						results.message = 'login success';
						callback(results);
					} else {		//비번 틀림
						conn.release();
						results.success = 4;
						results.message = 'check pw';
						callback(results);
					}
				} else {
					conn.release();
					results.success = 3;
					results.message = 'check id';
					callback(results);
				}
			});
		}
	});
}