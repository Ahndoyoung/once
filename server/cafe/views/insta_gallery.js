var express = require('express');
var app = express();
var instagram = require('instagram-node').instagram();

app.use(express.static(__dirname + '/public'));

app.set('view engine', 'ejs');

instagram.use({
	client_id: '691493c610ae41d6a4017b8d0df1236a',
	client_secret: '408bcba50eb648eb88a91a16c0a98095'
});

app.get('/', function(req, res) {
	if(instagram.user_media_recent('2042470368', function(err, result, pagination, remaining, limit) {
		res.render('insta/insta_index', { grams: result });
	}));
});


app.listen(8080, function(err) {
	if(err){
		console.log("Error");
	} else {
		console.log("Listening on port 8080");
	}
});