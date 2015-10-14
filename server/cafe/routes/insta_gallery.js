var express = require('express');
var app = express();
var instagram = require('instagram-node').instagram();
var router = express.Router();


instagram.use({
	client_id: '691493c610ae41d6a4017b8d0df1236a',
	client_secret: '408bcba50eb648eb88a91a16c0a98095'
});

router.get('/', function(req, res, next) {
	if(instagram.user_media_recent('2042470368', function(err, result, pagination, remaining, limit) {
		res.render('gallery', { grams: result , title : 'once'});
	}));
});





module.exports = router;
