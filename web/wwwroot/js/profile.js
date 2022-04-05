(function () {
  const d = document;
  const a = function (url) {
    // If (cache.exists(url)) {
    //   load(cache.get(url));
    // } else {
    const q = new XMLHttpRequest();
    q.open('get', url, true);
    q.setRequestHeader(
      'Content-Type',
      'application/x-www-form-urlencoded; charset=UTF-8',
    );
    q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
    q.send();

    q.addEventListener('load', function () {
      load(q.responseText);
      // Var ccHeader =
      //   q.getResponseHeader('Cache-Control') != null
      //     ? (q.getResponseHeader('Cache-Control').match(/\d+/) || [null])[0]
      //     : null;

      // if (ccHeader) {
      //   cache.set(url, q.responseText, ccHeader);
      // }
      // };
    });

    window.profileLoad = undefined;
  };

  const load = function (text) {
    if (d.querySelectorAll('.profile-buttonHidden')[0]) {
      d.querySelectorAll('.profile-buttonHidden')[0].classList.remove(
        'profile-buttonHidden',
      );
    }

    const modal = d.querySelector('#profile-modal .mdl-b');
    if (modal) {
      modal.innerHTML = text;
      d.dispatchEvent(new CustomEvent('load-charts'));
    }
  };

  if (
    window.location.pathname.toLowerCase() === '/reports' &&
    getUrlVars().id
  ) {
    a('/Profile?id=' + getUrlVars().id);
  } else if (
    window.location.pathname.toLowerCase() === '/collections' &&
    getUrlVars().id
  ) {
    a('/Profile?handler=Collections&id=' + getUrlVars().id);
  } else if (
    window.location.pathname.toLowerCase() === '/terms' &&
    getUrlVars().id
  ) {
    a('/Profile?handler=Terms&id=' + getUrlVars().id);
  }
})();
