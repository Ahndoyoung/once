var express = require('express');
var router = express.Router();
var once_db = require('../models/db_menu');

router.get('/', function(req, res, next) {
<<<<<<< HEAD
   once_db.menu('coffee', function(coffee){
      once_db.menu('non-coffee', function(non){
         once_db.menu('tea', function(tea){
            once_db.menu('side', function(side){
               once_db.menu('cocktail', function(cocktail){
                  var coffee1 = coffee.results;
                  var non1 = non.results;
                  var tea1 = tea.results;
                  var side1 = side.results;
                  var cocktail1 = cocktail.results;
                  var results = {
                     coffee : coffee1,
                     non : non1,
                     tea : tea1,
                     side : side1,
                     cocktail : cocktail1
                  };
                  console.log('results = ', results);
                  res.render('menu', {
                     results : results
                     })
               });
            });
         });
      });
   });
=======
	once_db.menu('coffee', function(coffee){
		once_db.menu('non-coffee', function(non){
			once_db.menu('tea', function(tea){
				once_db.menu('side', function(side){
					once_db.menu('cocktail', function(cocktail){
						var coffee1 = coffee.results;
						var non1 = non.results;
						var tea1 = tea.results;
						var side1 = side.results;
						var cocktail1 = cocktail.results;
						var results = {
							coffee : coffee1,
							non : non1,
							tea : tea1,
							side : side1,
							cocktail : cocktail1
						};
console.log('results = ', results);
						res.render('menu', {
							results : results
							})
					});
				});
			});
		});
	});
>>>>>>> 7518ea40cdf1ab4de0d85f7bd52c7ecf01774630
});

//메뉴 확인 페이지
router.get('/:menu', function(req, res) {
   var menu_category = req.params.menu;
   menu_category = menu_category.toLowerCase();
      once_db.menu(menu_category, function(results){
         res.render('menu', {results : results.results});
      })
});

//수정 페이지
router.get('/revise/:num', function(req, res) {
   var menu_num = req.params.num;
      once_db.revise(menu_num, function(results){
         res.json(results);
      })
});

//수정 요청 페이지
router.post('/revise', function (req, res) {
   var menu_category = req.body.menuCategory;
   var menu_num = req.body.menuNum;
   var menu_name = req.body.menuName;
   var menu_price = req.body.menuPrice;
   var menu_image = req.files.menuImage;
   var nonImage = {};
   nonImage.success = 0;
   nonImage.message = "need image";
   nonImage.results = {};
   if(menu_image){
      var datas = [menu_num, menu_category, menu_name, menu_price, menu_image];
         once_db.revise_menu(datas, function(results) {
            res.json(results);
         })
   } else {
      res.json(nonImage);
   }
});

router.post('/menuUpload', function (req, res, next) {
   var menu_category = req.body.menuCategory;
   var menu_name = req.body.menuName;
   var menu_price = req.body.menuPrice;
   var menu_image = req.files.menuImage;
   //var user = req.seession.user;
   var user = 'snaos';
   var nonImage = {};
   nonImage.success = 0;
   nonImage.message = "need image";
   nonImage.results = {};
   if(menu_image){
      var datas = [ menu_category, menu_name, menu_price, user, menu_image];
         once_db.menuUpload(datas, function(results) {
            res.json(results);
         })
   } else {
      res.json(nonImage);
   }
});



module.exports = router;