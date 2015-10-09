

var mysql = require('mysql');
var crypto =require('crypto');

//52.26.40.174

var pool = mysql.createPool({
	connectionLimit : 150,
	host : "127.0.0.1",
	port : 3025,
	user: 'root',
	password: 'once1234',
	database: 'OnceDB'
});

var page_size = 10;

categoryConfirm = function(categoryName) {
	console.log('categoryName = ',categoryName);
	if(categoryName == 'inform'){
		return 1;
	} else if(categoryName == 'event'){
		return 2;
	} else {
		return 0;
	}
}

exports.notice = function(page, callback) {
	var results = {};
	var callback_data = {};
	var err_results = {};
	err_results.success = 2;
	err_results.message = "notice fail";
	err_results.results = [];
	pool.getConnection(function(err, conn){
		if(err) {
			console.error('db notice err = ', err);
			callback(err_results);
		} else {
			var noticeSQL = 'SELECT NOTICE_NUM AS noticeNum, NOTICE_TITLE AS noticeTitle, NOTICE_CATEGORY_NAME AS noticeCategory, DATE_FORMAT(NOTICE_TIME, "%y-%m-%d %h:%i:%s") AS noticeTime, NOTICE_HIT AS noticeHit FROM NOTICE, NOTICE_CATEGORY WHERE NOTICE.NOTICE_CATEGORY_NUM = NOTICE_CATEGORY.NOTICE_CATEGORY_NUM';
			conn.query(noticeSQL, function(noticeErr, noticeRow) {
				if(noticeErr){
					console.error('notice sql err = ', noticeErr);
					conn.release();
					callback(err_results);
				} else {
					var total_cnt = noticeRow.length;
					var total_page = Math.ceil(total_cnt / page_size);
					var start_num = (page-1)*page_size;
					var end_num = (page * page_size) -1;
					callback_data.results =[];
					if(page == 1 && total_page >0){
						for(var i = 0; (i<page_size)&& i<(total_cnt);i++ ){
							callback_data.results[i] = noticeRow[i];
						}
						callback_data.success = 1;
						callback_data.message = "notice success";
						conn.release();
						callback(callback_data);
					} else if(page > total_page || total_page==0) {	//페이지가 없음
						conn.release();
						err_results.message = "page over";
						callback(err_results);
					} else {
						if(end_num > total_cnt) {
							end_num = total_cnt-1;
						}
						for(var i = 0; i < end_num -start_num+1;i++){
							callback_data.results[i] = noticeRow[i+start_num];
						}
						callback_data.success = 1;
						callback_data.message = "notice success";
						conn.release();
						callback(callback_data);
					}
				}
			});
		}
	})
}

exports.noticeDetail = function(notice_num, callback) {
	var results = {};
	var err_results = {};
	err_results.success = 2;
	err_results.message = "notice detail fail";
	err_results.results = {};
	pool.getConnection(function(err, conn){
		if(err) {
			console.error('notice detail err', err);
			conn.release();
			callback(err_results);
		} else {
			var noticeDetailSQL = 'SELECT NOTICE_NUM AS noticeNum, NOTICE_TITLE AS noticeTitle, NOTICE_CONTENT AS noticeContent, NOTICE_CATEGORY_NAME AS noticeCategory, DATE_FORMAT(NOTICE_TIME, "%y-%m-%d %h:%i:%s") AS noticeTime, NOTICE_HIT AS noticeHit FROM NOTICE, NOTICE_CATEGORY WHERE NOTICE.NOTICE_CATEGORY_NUM = NOTICE_CATEGORY.NOTICE_CATEGORY_NUM AND NOTICE.NOTICE_NUM =?';
			conn.query(noticeDetailSQL, [notice_num], function(noticeErr, noticeRow) {
				if( noticeErr){
					console.error('notice sql err = ', noticeErr);
					conn.release();
					callback(err_results);
				} else {
					var noticeHitSQL = 'UPDATE NOTICE SET NOTICE_HIT=NOTICE_HIT+1 WHERE NOTICE_NUM=?';
					conn.query(noticeHitSQL, notice_num, function(hitErr, hitRow){
						if(hitErr){
							console.error('notice hit sql err = ', hitErr);
							conn.release();
							callback(err_results);
						} else {
							results.success = 1;
							results.message = "notice detail success";
							results.results = noticeRow[0];
							conn.release();
							callback(results);
						}

					});
				}
			});
		}
	});
}


//	var datas = [notice_title, notice_category, notice_content];
exports.newNotice = function(datas, callback) {
	var results = {};
	var category_num = categoryConfirm(datas[1]);
	var err_results = {};

	err_results.success = 2;
	err_results.message = "new notice fail";
	err_results.results = {};

	if(category_num == 0){
		results.success = 0;
		results.message = "category name is unvalid";
		results.results = {};
		callback(results);
	} else {
		datas[1] = category_num;
		pool.getConnection(function(err, conn) {
			if(err) {
				console.error('new notice err', err);
				conn.release();
				callback(err_results);
			} else {
				var newNoticeSQL = 'INSERT INTO NOTICE(NOTICE_TIME, NOTICE_TITLE, NOTICE_CATEGORY_NUM, NOTICE_CONTENT) VALUES(NOW(), ?, ?, ?)';
				conn.query(newNoticeSQL, datas, function(newNoticeErr, newNoticeRow) {
					if(newNoticeErr) {
						console.error(newNoticeErr);
						conn.release();
						callback(err_results);
					} else {
						var callback_data = {};
						callback_data.success = 1;
						callback_data.message = 'new notice success';
						conn.release();
						callback(callback_data);
					}
				});
			}
		});
	}
}


//var datas = [notice_title, notice_category, notice_content, notice_num];
exports.reviseNotice = function(datas, callback) {
	var callback_data = {};
	var category_num = categoryConfirm(datas[1]);

	var err_results = {};
	err_results.success = 2;
	err_results.message = "revise notice fail";
	err_results.results = {};

	datas[1] = category_num;
	if(category_num == 0){
		results.success = 2;
		results.message = "category name is unvalid";
		results.results = {};
		callback(results);
	} else {
		pool.getConnection(function(err, conn){
			if(err) {
				console.error('revise notice err =',err);
				conn.release();
				callback(err_results);
			} else {
				reviseNoticeSQL = 'UPDATE NOTICE SET NOTICE_TITLE=?, NOTICE_CATEGORY_NUM=?, NOTICE_CONTENT=? WHERE NOTICE_NUM=?';
				conn.query(reviseNoticeSQL, datas, function(reviseNoticeErr, reviseNoticeRow){
					if(reviseNoticeErr){
						console.error('reviseNoticeErr = ', reviseNoticeErr);
						conn.release();
						callback(callback_data);
					} else {
						callback_data.success = 1;
						callback_data.message = "revise notice success";
						conn.release();
						callback(callback_data);
					}
				});
			}
		});
	}
}
