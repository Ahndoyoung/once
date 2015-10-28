 $(document).ready(function() {
        $("#coffee").click(function(event){
           $.get('http://localhost:8080/v1/menu/coffee', function(results) {
            $('#stage').html();
            for (var i =0; i < results.results.length; i++) {
              $('#stage').append('<p> Name: ' + results.results[i].menuName + '</p>');
              $('#stage').append('<p>Price : ' + results.results[i].menuPrice+ '</p>');
              $('#stage').append('<img src='+results.results[i].menuImagePath+'>');
            };
           });
        });
        $("#non-coffee").click(function(event){
           $.get('http://localhost:8080/v1/menu/noncoffee', function(results) {
            for (var i =0; i <= results.results.length - 1; i++) {
                         $('#stage').html('<p> Name: ' + results.results[i].menuName + '</p>');
                         $('#stage').append('<p>Price : ' + results.results[i].menuPrice+ '</p>');
                        $('#stage').append('<img src='+results.results[i].menuImagePath+'>');
                       };
           });
        });
        $("#tea").click(function(event){
           $.get('localhost:8080/v1/menu/tea', function(results) {
             for (var i =0; i <= results.results.length - 1; i++) {
                          $('#stage').html('<p> Name: ' + results.results[i].menuName + '</p>');
                          $('#stage').append('<p>Price : ' + results.results[i].menuPrice+ '</p>');
                          $('#stage').append('<img src='+results.results[i].menuImagePath+'>');
                        };
           });
        });
        $("#side").click(function(event){
           $.get('localhost:8080/v1/menu/side', function(results) {
              for (var i =0; i <= results.results.length - 1; i++) {
                           $('#stage').html('<p> Name: ' + results.results[i].menuName + '</p>');
                           $('#stage').append('<p>Price : ' + results.results[i].menuPrice+ '</p>');
                          $('#stage').append('<img src='+results.results[i].menuImagePath+'>');
                         };
           });
        });

        $("#cocktail").click(function(event){
           $.get('localhost:8080/v1/menu/cocktail', function(results) {
              for (var i =0; i <= results.results.length - 1; i++) {
                           $('#stage').html('<p> Name: ' + results.results[i].menuName + '</p>');
                           $('#stage').append('<p>Price : ' + results.results[i].menuPrice+ '</p>');
                           $('#stage').append('<img src='+results.results[i].menuImagePath+'>');
                         };
           });
        });
});