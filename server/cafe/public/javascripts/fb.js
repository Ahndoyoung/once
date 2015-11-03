 window.fbAsyncInit = function() {
      FB.init({
        appId      : '1513743232271985', // App ID
        status     : true, // check login status
        cookie     : true, // enable cookies to allow the server to access the session
        xfbml      : true  // parse XFBML
      });

      // First we need to check if the user is already logged in

      FB.getLoginStatus(function(response) {
          if (response.status === 'connected') {

              // If user is logged in call the callAPI() function
              callAPI();

          } else if (response.status === 'not_authorized') {

              // If the user is not logged in call the login() function
              login();

          } else {

              // if the status is unknown do something, in this case we call the login() function
              login();
          }
      });

    };

      //login the user
      function login() {
          FB.login(function(response) {
            if (response.authResponse) {

                // If user is accepted the permission call the callAPI() function
                callAPI();

            } else {

                  // If the user declined the permission do something, in this case lets refresh the page.
                  //window.parent.location = "http://myhost.com";
                  alert("no login");

            }
          },{scope: 'publish_actions'}); // in some cases you need to specify the permissions, a nice list is here: 
                                                  http://developers.facebook.com/docs/reference/login/#permissions

      }

      //If the user accapted the premission this is the function waht will be called
      function callAPI() {
      FB.api('/me', function(response) {

          //now we can access the the user informations like this:

          response.id // get users Facebook ID
          response.username // get users Facebook username

      }