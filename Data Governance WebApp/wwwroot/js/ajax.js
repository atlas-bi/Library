(function () {
  var q,
      d = document;

  function loadPage(url, cacheType, type) {
    var start = new Date();
    document.dispatchEvent(new CustomEvent('progress-reset'));
    document.dispatchEvent(new CustomEvent('progress-start'));

    if (type === '' || typeof type === 'undefined') {
      history.pushState({
        state: 'ajax'
      }, null, url);
    }

    if (cache.exists(url)) {
      a(url, cache.get(url), start, type);
    } else {
      try {
        q = new XMLHttpRequest();
        q.open('get', url, true);
        q.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded; charset=UTF-8');
        q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
        q.send();

        q.onreadystatechange = function (e) {
          if (this.readyState == 4 && this.status == 200) {
            var ccHeader = q.getResponseHeader('Cache-Control') != null ? (q.getResponseHeader('Cache-Control').match(/\d+/) || [null])[0] : null;
            a(url, this.responseText, start, type);

            if (cacheType || ccHeader) {
              cache.set(url, this.responseText, ccHeader);
            }
          }
        };
      } catch (e) {
        window.location = url;
      }
    }
  }

  var a = function a(url, t, start, type) {
    var d = document,
        m,
        sc,
        el = d.createElement('div');
    el.innerHTML = t; // clear ad boxes

    d.getElementById('AdColTwo').innerHMTL = ''; // hide or open search box
    // load new html

    m = d.getElementsByClassName('body-mainCtn')[0];
    m.innerHTML = el.innerHTML;
    sc = Array.prototype.slice.call(m.querySelectorAll('script:not([type="application/json"])'));
    for (var x = 0; x < sc.length; x++) {
      var q = document.createElement("script");
      q.innerHTML = sc[x].innerHTML;
      q.type = "text/javascript";
      q.setAttribute('async', 'true');
      m.appendChild(q);
      sc[x].parentElement.removeChild(sc[x]);
    } // get page title


    var matches = el.innerHTML.match(/<title>(.*?)<\/title>/); // update history

    d.title = matches ? matches[1] : 'Atlas of Information Management';

    if (type === '' || typeof type === 'undefined') {
      history.replaceState({
        state: 'ajax'
      }, d.title, url);
    } //scrol to top


    document.documentElement.scrollTop = document.body.scrollTop = 0;
    document.dispatchEvent(new CustomEvent('progress-finish'));
    document.dispatchEvent(new CustomEvent('analytics-post', {
      cancelable: true,
      detail: {
        value: new Date().getTime() - start.getTime(),
        type: 'newpage'
      }
    }));
    oldHref = window.location.href;
    oldPath = window.location.pathname;
    currentPathname = document.location.pathname;
    window.oldPopState = document.location.pathname;

    if (document.location.hash !== "" && document.location.hash !== null) {
      d.dispatchEvent(new CustomEvent("tab-open", {
        cancelable: true,
        detail: {
          el: d.querySelector('.tab-lnk[href="' + document.location.hash.replace("#", "") + '"], .tab-lnk[href="' + document.location.hash + '"]')
        }
      }));
    }

    if (window.location.pathname == '/') {
      d.dispatchEvent(new CustomEvent("clps-open", {
        cancelable: true,
        detail: {
          el: d.getElementById('sr-cls')
        }
      }));
    } else {
      d.dispatchEvent(new CustomEvent("clps-close", {
        cancelable: true,
        detail: {
          el: d.getElementById('sr-cls')
        }
      }));
    }

    d.querySelector('.sr-grp input').value = ''; // after load

    document.dispatchEvent(new CustomEvent("ajax"));
    document.dispatchEvent(new CustomEvent("ajax-page"));
    document.dispatchEvent(new CustomEvent("load-charts"));
    document.dispatchEvent(new CustomEvent("load-ajax-content"));
    document.dispatchEvent(new CustomEvent("code-highlight"));
    document.dispatchEvent(new CustomEvent("live-editor"));
  };

  d.addEventListener('click', function (e) {
    if (e.target.closest('.ajax')) {
      var t = e.target.closest('.ajax');
      e.preventDefault();
      d.dispatchEvent(new CustomEvent("modal-close")); // move all contents into one container.

      var div = d.createElement('div'),
          mcnt = d.getElementsByClassName('.mail-notification-container')[0];

      if (typeof mcnt !== 'undefined') {
        me = div.innerHTML = mcnt.outerHTML;
        mcnt.innerHTML = me;

        if (me.length > 0) {
          me.stop().animate({
            "margin-right": "-=500"
          }).delay(500).queue(function (nextone) {
            me.parentElement.removeChild(me);
            nextone();
          });
        }
      }

      var url = t.getAttribute('href'); // if a hash

      if (/^#/.test(url)) {
        // .test() returns a boolean
        d.querySelector('.nav-links.tabs a[href="' + document.location.hash + '"]').click();
        history.pushState({
          state: 'ajax'
        }, document.title, window.location.href.split('#')[0] + url);
      } else {
        var el = d.getElementsByClassName('nb-cmbs')[0];
        el.style.opacity = 0;
        var q = el.offsetHeight;
        loadPage(url, t.hasAttribute('cache'));
        window.oldPopState = window.newPopState;
      }
    }
  });
  d.addEventListener('load-ajax', function (e) {
    if (e.detail) loadPage(e.detail, false);
  }); // if the back button is  pressed try to get last page back

  window.oldPopState = document.location.pathname;
  window.newPopState = '';

  window.onpopstate = function (e) {

    window.newPopState = document.location.pathname;
    if (window.newPopState != window.oldPopState) {
      loadPage(document.location, false, 'replace');
      if (e.state !== null && e.state.search) {
        d.querySelector('.sr-grp input').value = e.state.search;
      }
    } else {
      // hash change;
      return !1;
    }
  };
})();