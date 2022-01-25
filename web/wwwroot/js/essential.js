function getOffset(element) {
  if (!element.getClientRects().length) {
    return {
      top: 0,
      left: 0,
    };
  }

  var rect = element.getBoundingClientRect();
  var win = element.ownerDocument.defaultView;
  return {
    top: rect.top + win.pageYOffset,
    left: rect.left + win.pageXOffset,
  };
}

(function () {
  document.addEventListener('change', function (e) {
    if (e.target.closest('#change-role')) {
      e.target.closest('form').querySelector('#MyRole_Url').value =
        window.location.href;
      e.target.closest('form').submit();
    }
  });

  function loadScripts(els) {
    els = els ? Array.prototype.slice.call(els) : [];

    for (var x = 0; x < els.length; x++) {
      var el = els[x];

      var l = document.createElement('div');

      l.innerHTML = el.value;

      el.parentElement.removeChild(el);

      var scripts = l.querySelectorAll('script');

      for (var y = 0; y < scripts.length; y++) {
        var i = scripts[y],
          q = document.createElement('script');
        q.src = i.src;
        q.innerHTML = i.innerHTML;
        q.type = 'text/javascript';
        document.body.appendChild(q);
      }
    }
  }

  function showScrollToTop() {
    if (window.pageYOffset > 50) {
      document.getElementById('back-to-top').style.visibility = 'visible';
    } else {
      document.getElementById('back-to-top').style.visibility = 'hidden';
    }
  }

  showScrollToTop();
  document.addEventListener('click', function (e) {
    if (e.target.closest('#back-to-top')) {
      document.documentElement.scrollTop = document.body.scrollTop = 0;
      return false;
    }
  });

  function downloadJSAtOnload() {
    loadScripts(
      Array.prototype.slice.call(
        document.getElementsByClassName('postLoadScripts'),
      ),
    );
  }

  window.addEventListener(
    'load',
    function () {
      downloadJSAtOnload();
    },
    false,
  );

  document.addEventListener('ajax', function () {
    downloadJSAtOnload();
  });

  document.addEventListener('tab-opened', function () {
    debounce(downloadJSAtOnload(), 250);
  });

  document.addEventListener(
    'scroll',
    function () {
      debounce(downloadJSAtOnload(), 250);
      debounce(showScrollToTop(), 250);
    },
    {
      passive: true,
    },
  );
})();
