
var mysql = require('mysql');
var crypto =require('crypto');

//52.26.40.174

var pool = mysql.createPool({
   connectionLimit : 150,
   host : "52.26.40.174",
   port : 3025,
   user: 'root',
   password: 'once1234',
   database: 'OnceDB'
});
var ip = "http://52.26.40.174";

category_num_set = function (category_name,callback) {

 var category = category_name.toLowerCase();
 var menu_category_num;
   switch(category){
      case 'coffee':
        menu_category_num=1;
         break;
      case 'non-coffee':
         menu_category_num=2;
         break;
      case 'tea':
        menu_category_num=3;
         break;
      case 'side':
        menu_category_num=4;
         break;
      case 'cocktail':
        menu_category_num=5;
         break;
      default :
         menu_category_num=0;
   }

   callback(menu_category_num);

}


exports.menu = function(menu_category, callback){
   var results = {};
   var err_results = {};
   err_results.success = 2;
   err_results.message = "menu fail";
   err_results.results = [];
   pool.getConnection(function(err, conn){
      if(err) {
         console.error('db menu err = ', err);
         conn.release();
         callback(err_results);
      } else {
         var menuSQL = 'SELECT MENU.MENU_NUM AS menuNum, MENU_NAME AS menuName, MENU_PRICE AS menuPrice, MENU_CATEGORY_NAME AS menuCategory, MENU_IMAGE_PATH AS menuImagePath FROM MENU, MENU_CATEGORY,MENU_IMAGE WHERE MENU.MENU_CATEGORY_NUM=MENU_CATEGORY.MENU_CATEGORY_NUM AND MENU_CATEGORY.MENU_CATEGORY_NAME = ? AND MENU_IMAGE.MENU_NUM = MENU.MENU_NUM GROUP BY MENU.MENU_NUM';
         conn.query(menuSQL,menu_category, function(menuErr, menuRow) {
            if(menuErr){
               console.error('menuSQL error = ', menuErr);
               conn.release();
               callback(err_results);
            } else {
               results.success = 1;
               results.message = "menu success";
               results.results = menuRow;
               conn.release();
               callback(results);
            }
         })
      }
   })
}


exports.revise = function(menu_num, callback) {
   var results = {};
   var err_results = {};
   err_results.success = 2;
   err_results.message = "menu revise access fail";
   err_results.results = [];
   pool.getConnection(function(err, conn) {
      if(err){
         console.error('db menu revise err = ', err);
         conn.release();
         callback(err_results);
      } else {
         var menuReviseSQL = 'SELECT MENU.MENU_NUM AS menuNum, MENU_NAME AS menuName, MENU_PRICE AS menuPrice, MENU_CATEGORY_NAME AS menuCategory, MENU_IMAGE_PATH AS menuImagePath FROM MENU, MENU_CATEGORY,MENU_IMAGE WHERE MENU.MENU_CATEGORY_NUM=MENU_CATEGORY.MENU_CATEGORY_NUM AND MENU.MENU_NUM=? AND MENU.MENU_NUM=MENU_IMAGE.MENU_NUM  GROUP BY MENU.MENU_NUM';
         conn.query(menuReviseSQL, menu_num, function(menuRErr, menuRow) {
            if(menuRErr) {
               console.error('menu revise SQL error = ', menuRErr);
               conn.release();
               callback(err_results);
            } else {
               results.success = 1;
               results.message = "menu revise access success";
               results.results = menuRow[0];
               conn.release();
               callback(results);
            }
         })
      }
   })
}
//   var datas = [menu_num, menu_category, menu_name, menu_price, menu_image];

exports.revise_menu = function(datas, callback) {
   console.log('datas = ', datas);
   var results = {};
   var err_results = {};
   err_results.success = 2;
   err_results.message = "menu revise fail";
   err_results.results = [];
   category_num_set(datas[1], function(menu_category_num) {
      if(menu_category_num == 0){
         results.success = 2;
         results.message = "menu category is unvalid";
         results.results = [];
         callback(results);
      } else {
         pool.getConnection(function(err, conn) {
            if(err){
               console.error('db menu revise_menu err = ', err);
               conn.release();
               callback(err_results);
            } else {
               var reviseData = [datas[2], datas[3], menu_category_num, datas[0]];
               var reviseMenuSQL = 'UPDATE MENU  SET MENU_NAME=?, MENU_PRICE=?, MENU_CATEGORY_NUM=?   WHERE MENU_NUM = ?';   //update문
               conn.query(reviseMenuSQL, reviseData, function(reviseErr, reviseRow) {
                  if(reviseErr){
                     console.error('db menu revise err = ', reviseErr);
                     conn.release();
                     callback(err_results);
                  } else {         //업데이트 쿼리 성공
                     var image_path = ip+datas[4].path.substring(6);
                     var imageReviseSQL = 'UPDATE MENU_IMAGE SET MENU_IMAGE_PATH=? WHERE MENU_NUM=?';
                     conn.query(imageReviseSQL,[image_path,datas[0]], function(imageReviseErr, imageReviseRow) {
                        if(imageReviseErr){
                           console.error('db menu revise image err = ', imageReviseErr);
                           conn.release();
                           callback(err_results);
                        } else {   //사진 업데이트 성공
                           results.success = 1;
                           results.message = "revise menu success";
                           conn.release();
                           callback(results);
                        }
                     });
                  }
               });
            }
         })
      }

   });
}

//var datas = [menu_category, menu_name, menu_price, user, menu_image];

exports.menuUpload = function (datas, callback) {
   var results = {};
   var err_results = {};
   err_results.success = 2;
   err_results.message = "menu upload fail";
   err_results.results = [];

   pool.getConnection( function (connErr, conn) {
      if(connErr) {
         console.error('connErr = ', connErr);
         conn.release();
         callback(err_results);
      } else {
         category_num_set(datas[0], function (category_num) {
            if(category_num == 0){
               results.success = 2;
               results.message = "menu category is unvalid";
               results.results = [];
               conn.release();
               callback(results);
            } else {
               var miSQL = 'INSERT INTO MENU(MENU_CATEGORY_NUM, MENU_NAME, MENU_PRICE) VALUES(?,?,?)';
               conn.query(miSQL, [category_num, datas[1], datas[2]], function (miErr, miRow) {
                  if(miErr) {
                     console.error('miErr = ', miErr);
                     conn.release();
                     callback(err_results);
                  } else {
                     var mSQL = 'SELECT MENU_NUM FROM MENU WHERE MENU_NAME=? AND MENU_PRICE=?';
                     conn.query(mSQL, [datas[1], datas[2]], function (mErr, mRow) {
                        if(mErr) {
                           console.error('mErr = ', mErr);
                           conn.release();
                           callback(err_results);
                        } else {
                           var miiSQL = 'INSERT INTO MENU_IMAGE(MENU_IMAGE_PATH,MENU_NUM) VALUES(?,?)';
                           var image_path = ip+datas[4].path.substring(6);
                           conn.query(miiSQL, [image_path, mRow[0].MENU_NUM], function (miiErr, miiRow) {
                              if(miiErr) {
                                 console.error('miiErr =',miiErr );
                                 conn.release();
                                 callback(err_results);
                              } else {
                                 results.success =1 ;
                                 results.message = "menu upload success"
                                 results.results = {};
                                 conn.release();
                                 callback(results);
                              }
                           });
                        }
                     });
                  }
               });
            }
         });
      }
   });
}

