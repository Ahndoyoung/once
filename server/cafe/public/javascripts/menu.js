 $(document).ready(function() {
        $("#coffee").click(function(event){
           $.get('http://localhost:8080/v1/menu/coffee', function(results) {
            $('#stage').html();
            for (var i =0; i < results.results.length; i++) {
              $('#stage').append('<div class= "stage-img">');
              $('#stage').append('<img src='+results.results[i].menuImagePath+'>');
              $('#stage').append('<p class="stage-desc"> Name: ' + results.results[i].menuName + '</p>');
              $('#stage').append('<p class="stage-desc"> Price : ' + results.results[i].menuPrice+ '</p>');
              $('#stage').append('</div>');
            };
           });
        });
        $("#non-coffee").click(function(event){
           $.get('http://localhost:8080/v1/menu/noncoffee', function(results) {
            for (var i =0; i <= results.results.length - 1; i++) {
                         $('#stage').append('<img src='+results.results[i].menuImagePath+'>');
                         $('#stage').html('<p> Name: ' + results.results[i].menuName + '</p>');
                         $('#stage').append('<p>Price : ' + results.results[i].menuPrice+ '</p>');
                       };
           });
        });
        $("#tea").click(function(event){
           $.get('localhost:8080/v1/menu/tea', function(results) {
             for (var i =0; i <= results.results.length - 1; i++) {
                          $('#stage').append('<img src='+results.results[i].menuImagePath+'>');
                          $('#stage').html('<p> Name: ' + results.results[i].menuName + '</p>');
                          $('#stage').append('<p>Price : ' + results.results[i].menuPrice+ '</p>');
                        };
           });
        });
        $("#side").click(function(event){
           $.get('localhost:8080/v1/menu/side', function(results) {
              for (var i =0; i <= results.results.length - 1; i++) {
                           $('#stage').append('<img src='+results.results[i].menuImagePath+'>');
                           $('#stage').html('<p> Name: ' + results.results[i].menuName + '</p>');
                           $('#stage').append('<p>Price : ' + results.results[i].menuPrice+ '</p>');
                         };
           });
        });

        $("#cocktail").click(function(event){
           $.get('localhost:8080/v1/menu/cocktail', function(results) {
              for (var i =0; i <= results.results.length - 1; i++) {
                           $('#stage').append('<img src='+results.results[i].menuImagePath+'>');
                           $('#stage').html('<p> Name: ' + results.results[i].menuName + '</p>');
                           $('#stage').append('<p>Price : ' + results.results[i].menuPrice+ '</p>');
                         };
           });
        });
});