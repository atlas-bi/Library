(function () {
  window.ajaxOn = true;
  var timeOnPage = new Date(),
    sessionTimerId,
    analitycsUpdateTimeoutId,
    buildAnalyticsPackage = function (loadTime) {
      var a = {},
        n = navigator,
        w = window,
        d = document,
        l = w.location;
      a.appCodeName = n.appCodeName;
      a.appName = n.appName;
      a.appVersion = n.appVersion;
      a.cookieEnabled = n.cookieEnabled;
      a.language = n.language;
      a.oscpu = n.oscpu;
      a.platform = n.platform;
      a.userAgent = n.userAgent;
      a.host = l.host;
      a.hostname = l.hostname;
      a.href = l.href;
      a.protocol = l.protocol;
      a.search = l.search;
      a.pathname = l.pathname;
      a.hash = l.hash;
      a.screenHeight = d.documentElement.clientHeight;
      a.screenWidth = d.documentElement.clientWidth;
      a.origin = w.origin;
      a.title = d.title;
      a.referrer = d.referrer;
      a.loadTime =
        loadTime ||
        w.performance.timing.domContentLoadedEventEnd -
          w.performance.timing.navigationStart;
      a.zoom = w.devicePixelRatio;
      a.sessionId = getOrResetSessionId();
      a.pageId = getOrResetPageId();
      a.pageTime = new Date().getTime() - timeOnPage.getTime();
      return a;
    },
    getOrResetSessionId = function (reset) {
      if (reset == 'clear') {
        sessionStorage.removeItem('_sid');
        return false;
      }

      if (reset == 'reset' || typeof sessionStorage._sid === 'undefined') {
        sessionStorage._sid = btoa(new Date().toString());
        getOrResetPageId('reset');
        timeOnPage = new Date();
      }

      return sessionStorage._sid;
    },
    getOrResetPageId = function (reset) {
      if (reset == 'reset' || typeof sessionStorage._pid === 'undefined') {
        timeOnPage = new Date();
        sessionStorage._pid = btoa(new Date().toString());
      }

      return sessionStorage._pid;
    },
    postAnalytics = function (loadTime, type) {
      if (type == 'newpage') {
        getOrResetPageId('reset');
        window.ajaxOn = true;
      }

      if (window.ajaxOn === true) {
        if (navigator.sendBeacon) {
          navigator.sendBeacon(
            '/analytics?handler=Beacon',
            JSON.stringify(buildAnalyticsPackage(loadTime)),
          );
        } else {
          var s = new XMLHttpRequest();
          s.open('post', '/analytics?handler=Beacon', true);
          s.setRequestHeader('Content-Type', 'text/plain;charset=UTF-8`');
          s.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
          s.send(JSON.stringify(buildAnalyticsPackage(loadTime)));
        }
      }

      resetAnalyticsTimer();
    },
    startPageTimer = function () {
      var sessionTimeout = 2 * 60000;
      sessionTimerId = window.setTimeout(doInactive, sessionTimeout);
      getOrResetSessionId();
    },
    resetPageTimer = function () {
      debounce(
        (function () {
          window.clearTimeout(sessionTimerId);
          startPageTimer();
          window.ajaxOn = true;
        })(),
        500,
      );
    },
    doInactive = function () {
      window.ajaxOn = false;
      getOrResetSessionId('clear');
      sessionStorage.clear();
    },
    setupPageTimer = function () {
      document.addEventListener('mousemove', resetPageTimer, false);
      document.addEventListener('mousedown', resetPageTimer, false);
      document.addEventListener('keypress', resetPageTimer, false);
      document.addEventListener('touchmove', resetPageTimer, false);
      document.addEventListener(
        'scroll',
        resetPageTimer,
        {
          passive: true,
        },
        false,
      );
      startPageTimer();
    },
    startAnalyticsTimer = function () {
      var analitycsUpdateTimeout = 0.5 * 60000;
      analitycsUpdateTimeoutId = window.setTimeout(
        postAnalytics,
        analitycsUpdateTimeout,
      );
    },
    resetAnalyticsTimer = function () {
      window.clearTimeout(analitycsUpdateTimeoutId);
      startAnalyticsTimer();
    };

  // if document is already loaded
  if (document.readyState == 'complete') {
    setupPageTimer();
    postAnalytics();
  }
  // if document has not loaded yet
  window.addEventListener(
    'load',
    function () {
      setupPageTimer();
      postAnalytics();
    },
    false,
  );

  document.addEventListener('analytics-post', function (e) {
    if (typeof e.detail !== 'undefined') {
      postAnalytics(e.detail.value, e.detail.type);
    }
  });
  /*
    1. create sessionid, store in browser.
    2. clear sessionid  when inactive
    3. to find session time > sum pagetime, group by sessionid + userid
    4. create pageid, store in browser
    5. reset pageid when url changes
    6. timer for pageid, clear timer when page changes
    7. analtic for page is updated, so there will only be 1 row in db per page + session
    8. when session becomes inactive stop all ajax requests
    9. when session becomes active resume all ajax requests
     while ajax = on
      send analitycs update every timeout interval.
      if update is sent because of page load, reset intereval (so we don't have dup data sends)
    */
})();

/* analytics page js for charts */
(function(){
    document.addEventListener('click', function($e){
        if($e.target.closest('.dropdown.is-select .dropdown-item')){
            ($e.target.closest('.dropdown').querySelectorAll('.dropdown-item.is-active') || []).forEach(($el) => {$el.classList.remove('is-active');})
            $e.target.classList.add('is-active');
            var $target = $e.target.closest('.dropdown.is-select .dropdown-item');

            $target.closest('.dropdown').querySelector('.select-value').innerText = $target.innerText

            /* build date parameters */

            document.getElementById($target.closest('.dropdown').dataset.target).setAttribute('data-param', "type="+$target.innerText)

            document.getElementById($target.closest('.dropdown').dataset.target).dispatchEvent(new CustomEvent('reload'));
        }
    })
})();
