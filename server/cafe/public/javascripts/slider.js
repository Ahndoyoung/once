$(document).ready(function(){
$("#volume").slider({
    min: 0,
    max: 100,
    value: 0,
		range: "min",
		animate: true,
    slide: function(event, ui) {
      setVolume((ui.value) / 100);
    }
  });

  var myMedia = document.createElement('audio');
  $('#player').append(myMedia);
  myMedia.id = "myMedia";
	playAudio('http://c7.inlive.co.kr:2830/;stream.nsv&type=mp3', 0);
  	

});

function playAudio(fileName, myVolume) {
	//var canPlayogg = !!myMedia.canPlayType & myMedia.canPlayType('myMedia/ogg;codes="vorbis"')!=="";
    myMedia.src = fileName;
    setVolume(myVolume);
    myMedia.play();
}

function setVolume(myVolume) {
    var myMedia = document.getElementById('myMedia');
    myMedia.volume = myVolume;
}