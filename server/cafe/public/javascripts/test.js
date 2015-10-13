$(document).ready(function(){
  var te1 =  $.get("/v1/menu/coffee", function(data, status){
      console.log(data);
      return data;
 });
  
});

function temp(){

}