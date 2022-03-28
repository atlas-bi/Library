// get data elements
//  1. enable ajax
//   data-ajax=yes
//  2. get url
//   data-url=?handler=la
//  3. get any params
//   data-param=d=1
//  4. if we need to refresh somteims
//   data-refresh=5
(function () {
  var d = document;

  var isInViewport = function isInViewport(elem) {
    var bounding = elem.getBoundingClientRect(),
      padding = 400;
    return (
      bounding.top >= 0 &&
      bounding.left >= 0 &&
      bounding.bottom - elem.clientHeight - padding <=
        (document.documentElement.clientHeight ||
          d.documentElement.clientHeight) &&
      bounding.right - padding - elem.clientWidth <=
        (document.documentElement.clientWidth || d.documentElement.clientWidth)
    );
  };

  var sendAjax = function (e) {
    var u = e.getAttribute('data-url'),
      p = e.getAttribute('data-param'),
      page = e.getAttribute('data-page'),
      l = e.getAttribute('data-loadtag'),
      q;

    if (!e.classList.contains('no-loader')) {
      e.innerHTML =
        '<div class="ajaxLoader"><img class="ajaxLoader-img" src="/img/loader.gif" /></div>';
    }

    if (p !== null && p !== '') {
      if (u.indexOf('?') != -1) {
        u += '&';
      } else {
        u += '?';
      }

      u += p;
    }
    // for paginated ajax boxes
    if (page !== null && page !== '') {
      if (u.indexOf('?') != -1) {
        u += '&';
      } else {
        u += '?';
      }
      u += page;
    }

    q = new XMLHttpRequest();
    q.open('get', u, true);
    q.setRequestHeader(
      'Content-Type',
      'application/x-www-form-urlencoded; charset=UTF-8',
    );
    q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
    q.send();

    q.onload = function () {
      a(e, l, q.responseText);
    };
  };
  var loadAjaxContent = function () {
    [].forEach.call(d.querySelectorAll('[data-ajax="yes"]'), function (e) {
      if (
        !e.closest('#AdColOne') &&
        !e.closest('#AdColTwo [data-ajax="yes"]') &&
        isInViewport(e) &&
        // is visible
        e.offsetParent !== null
      ) {
        sendAjax(e);
        e.removeAttribute('data-ajax');
        // allow events to reload container
        e.addEventListener('reload', function () {
          sendAjax(e);
        });
      }
    });
  };

  var a = function (e, l, t) {
    if (e == undefined) {
      return;
    }
    try {
      //e.style.opacity = 0;
      e.style.visibility = 'hidden';
      e.style.transition = 'visibility 0.3s ease-in-out';

      if (!e.parentNode) return;
      var sc,
        el = d.createElement('div');
      el.innerHTML = t;

      if (l !== null && l !== '') {
        el = el.querySelector(l);
        el.setAttribute('data-loadtag', l);
      } else {
        el = el.children[0];
      }

      e.innerHTML = el.innerHTML;

      if (e.querySelector('script:not([type="application/json"])')) {
        sc = Array.prototype.slice.call(
          e.querySelectorAll('script:not([type="application/json"])'),
        );

        for (var x = 0; x < sc.length; x++) {
          var s = d.createElement('script');
          s.innerHTML = sc[x].innerHTML;
          s.type = 'text/javascript';
          if (sc[x].hasAttribute('src')) {
            s.setAttribute('src', sc[x].hasAttribute('src'));
            s.setAttribute('async', 'true');
          }

          e.appendChild(s);
          sc[x].parentNode.removeChild(sc[x]);
        }
      }

      e.style.visibility = 'visible';

      d.dispatchEvent(new CustomEvent('ajax'));
    } catch (e) {
      console.log(e);
    }
  };

  var loadAdAjaxContent = function () {
    [].forEach.call(
      d.querySelectorAll('#AdColTwo [data-ajax="yes"]'),
      function (e) {
        if (isInViewport(e)) {
          var u = e.getAttribute('data-url'),
            p = e.getAttribute('data-param'),
            l = e.getAttribute('data-loadtag'),
            q;

          if (!e.classList.contains('no-loader')) {
            e.innerHTML =
              '<div class="ajaxLoader"><img class="ajaxLoader-img" src="/img/loader.gif" /></div>';
          }

          if (p !== null && p !== '') {
            if (u.indexOf('?') != -1) {
              u += '&';
            } else {
              u += '?';
            }

            u += p;
          }

          e.style.visibility = 'hidden';
          e.removeAttribute('data-ajax');

          q = new XMLHttpRequest();
          q.open('get', u, true);
          q.setRequestHeader(
            'Content-Type',
            'application/x-www-form-urlencoded; charset=UTF-8',
          );
          q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
          q.send();

          q.onload = function () {
            a(e, l, q.responseText);
          };
        }
      },
    );
  };

  loadAjaxContent();
  loadAdAjaxContent();
  d.addEventListener('load-ajax-content', function () {
    loadAjaxContent();
    loadAdAjaxContent();
  });

  d.addEventListener('modal-open', function () {
    loadAjaxContent();
  });
  d.addEventListener('panel-tab-opened', function () {
    loadAjaxContent();
  });
  d.addEventListener('step-tab-opened', function () {
    loadAjaxContent();
  });

  d.addEventListener('tab-opened', function () {
    loadAjaxContent();
  });

  d.addEventListener(
    'scroll',
    function () {
      debounce(loadAjaxContent(), 100);
      debounce(loadAdAjaxContent(), 100);
    },
    {
      passive: true,
    },
  );

  // trigger reload on paginated boxes
  document.addEventListener('click', ($e) => {
    if (
      $e.target.closest('[data-url][data-page]') &&
      $e.target.closest('.pagination-link[data-page]')
    ) {
      $e.target
        .closest('[data-url][data-page]')
        .setAttribute(
          'data-page',
          $e.target
            .closest('.pagination-link[data-page]')
            .getAttribute('data-page'),
        );
      $e.target
        .closest('[data-url][data-page]')
        .dispatchEvent(new CustomEvent('reload'));
    }
  });
})();
