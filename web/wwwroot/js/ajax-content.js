(function () {
  // get data elements
  //  1. enable ajax
  //   data-ajax=yes
  //  2. get url
  //   data-url=?handler=la
  //  3. get any params
  //   data-param=d=1
  //  4. if we need to refresh somteims
  //   data-refresh=5

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

  var loadAjaxContent = function () {
    [].forEach.call(d.querySelectorAll('[data-ajax="yes"]'), function (e) {
      if (
        !e.matches('#AdColOne') &&
        !e.matches('#AdColTwo [data-ajax="yes"]') &&
        isInViewport(e) &&
        // is visible
        e.offsetParent !== null
      ) {
        var u = e.getAttribute('data-url'),
          p = e.getAttribute('data-param'),
          l = e.getAttribute('data-loadtag'),
          id = e.getAttribute('id'),
          q;
        e.removeAttribute('data-ajax');

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

        if (cache.exists(u)) {
          a(e, l, p, cache.get(u), u, id);
        } else {
          q = new XMLHttpRequest();
          q.open('get', u, true);
          q.setRequestHeader(
            'Content-Type',
            'application/x-www-form-urlencoded; charset=UTF-8',
          );
          q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
          q.send();

          q.onload = function () {
            var ccHeader =
              q.getResponseHeader('Cache-Control') != null
                ? (q.getResponseHeader('Cache-Control').match(/\d+/) || [
                    null,
                  ])[0]
                : null;

            a(e, l, p, q.responseText, u, id);

            if (e.hasAttribute('cache') || ccHeader) {
              cache.set(u, q.responseText, ccHeader);
            }
          };
        }
      }
    });
  };

  var a = function (e, l, p, t, u, id) {
    if (e == undefined) {
      return;
    }
    try {
      e.style.opacity = 0;
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

      if (id) el.setAttribute('id', id);

      el.style.visibility = 'hidden';
      el.style.transition = 'visibility 0.3s ease-in-out';
      var q = el.clientHeight;
      el.setAttribute('data-url', u);
      el.setAttribute('data-param', p);

      e.parentNode.replaceChild(el, e);

      if (el.querySelector('script:not([type="application/json"])')) {
        sc = Array.prototype.slice.call(
          el.querySelectorAll('script:not([type="application/json"])'),
        );

        for (var x = 0; x < sc.length; x++) {
          var s = d.createElement('script');
          s.innerHTML = sc[x].innerHTML;
          s.type = 'text/javascript';
          s.setAttribute('async', 'true');
          el.appendChild(s);
          sc[x].parentNode.removeChild(sc[x]);
        }
      }

      el.style.visibility = 'visible';
      d.dispatchEvent(new CustomEvent('ajax'));
    } catch (e) {
      console.log(e);
    }
  };

  var loadAdAjaxContent = function () {
    [].forEach.call(
      d.querySelectorAll('#AdColOne, #AdColTwo [data-ajax="yes"]'),
      function (e) {
        if (isInViewport(e)) {
          var u = e.getAttribute('data-url'),
            p = e.getAttribute('data-param'),
            l = e.getAttribute('data-loadtag'),
            id = e.getAttribute('id'),
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

          if (cache.exists(u)) {
            a(e, l, p, cache.get(u), u, id);
          } else {
            q = new XMLHttpRequest();
            q.open('get', u, true);
            q.setRequestHeader(
              'Content-Type',
              'application/x-www-form-urlencoded; charset=UTF-8',
            );
            q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
            q.send();

            q.onload = function () {
              var ccHeader =
                q.getResponseHeader('Cache-Control') != null
                  ? (q.getResponseHeader('Cache-Control').match(/\d+/) || [
                      null,
                    ])[0]
                  : null;
              a(e, l, p, q.responseText, u, id);

              if (e.hasAttribute('cache') || ccHeader) {
                cache.set(u, q.responseText, ccHeader);
              }
            };
          }
        }
      },
    );
  };
  var reloadFavs = function () {
    var c = d.getElementsByClassName('favs-cntr'),
      q;
    [].forEach.call(c, function (e) {
      q = new XMLHttpRequest();
      q.open('get', e.getAttribute('data-url'), true);
      q.setRequestHeader(
        'Content-Type',
        'application/x-www-form-urlencoded; charset=UTF-8',
      );
      q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
      q.send();

      q.onload = function () {
        a(e, null, null, q.responseText, e.getAttribute('data-url'));
      };
    });
  };

  loadAjaxContent();
  loadAdAjaxContent();
  d.addEventListener('load-ajax-content', function () {
    loadAjaxContent();
    loadAdAjaxContent();
  });
  d.addEventListener('reload-favs', function () {
    reloadFavs();
  });

  d.addEventListener('modal-open', function () {
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
})();
