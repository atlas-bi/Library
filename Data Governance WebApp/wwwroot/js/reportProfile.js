(function () {
  window.profileLoad = function () {
    var i = 0,
        stateCheck = setInterval(function () {
      if (document.readyState === 'complete' && i < 1) {
        i++;
        clearInterval(stateCheck); // document ready

        a("/Profile?id=" + getUrlVars().id);
      }
    }, 100);

    function a(url) {
      if (cache.exists(url)) {
        load(cache.get(url));
      } else {
        var q = new XMLHttpRequest();
        q.open('get', url, true);
        q.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded; charset=UTF-8');
        q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
        q.send();

        q.onload = function () {
          load(q.responseText);
          var ccHeader = q.getResponseHeader('Cache-Control') != null ? (q.getResponseHeader('Cache-Control').match(/\d+/) || [null])[0] : null;

          if (ccHeader) {
            cache.set(url, q.responseText, ccHeader);
          }
        };
      }

      window.profileLoad = undefined;
    }

    function load(d) {
      if(document.getElementsByClassName('profile-buttonHidden')[0]){
        document.getElementsByClassName('profile-buttonHidden')[0].classList.remove('profile-buttonHidden');
      }
      document.querySelector('#profile-modal .mdl-b').innerHTML = d;
      document.dispatchEvent(new CustomEvent('ajax'));
      document.dispatchEvent(new CustomEvent('load-charts'));
    }
  };

  window.profileLoad();
})();