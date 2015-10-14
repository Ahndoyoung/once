 $(document).ready(function() {
  console.log("aaaa");
        $("#t1").click(function(event){
          console.log("ddddd");
           $.get('http://localhost:8080/v1/menu/coffee', function(jd) {
              $('#stage').html('<p> Name: ' + jd.results[0].menuName + '</p>');
              $('#stage').append('<p>Price : ' + jd.results[0].menuPrice+ '</p>');
              $('#stage').append('<p> Image: ' + jd.results[0].menuImagePath+ '</p>');
           });
        });
        $("#t2").click(function(event){
           $.get('http://localhost:8080/v1/menu/noncoffee', function(jd) {
              $('#stage').html('<p> Name: ' + jd.name + '</p>');
              $('#stage').append('<p>Age : ' + jd.age+ '</p>');
              $('#stage').append('<p> Sex: ' + jd.sex+ '</p>');
           });
        });
        $("#t3").click(function(event){
           $.get('localhost:8080/v1/menu/tea', function(jd) {
              $('#stage').html('<p> Name: ' + jd.name + '</p>');
              $('#stage').append('<p>Age : ' + jd.age+ '</p>');
              $('#stage').append('<p> Sex: ' + jd.sex+ '</p>');
           });
        });
        $("#t4").click(function(event){
           $.get('localhost:8080/v1/menu/side', function(jd) {
              $('#stage').html('<p> Name: ' + jd.name + '</p>');
              $('#stage').append('<p>Age : ' + jd.age+ '</p>');
              $('#stage').append('<p> Sex: ' + jd.sex+ '</p>');
           });
        });

        $("#t5").click(function(event){
           $.get('localhost:8080/v1/menu/cocktail', function(jd) {
              $('#stage').html('<p> Name: ' + jd.name + '</p>');
              $('#stage').append('<p>Age : ' + jd.age+ '</p>');
              $('#stage').append('<p> Sex: ' + jd.sex+ '</p>');
           });
        });
});