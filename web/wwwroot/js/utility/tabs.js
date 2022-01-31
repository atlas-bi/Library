(function () {
  /*
     div.tab
       div.tab-lnk.active data-target=tab1
       div.tab-lnk data-target=tab2
     div.tab-cnt
       div.tab-dta#tab1.tab-o
       div.tab-dta#tab2
     */

  var d = document;
  d.addEventListener(
    'click',
    function (e) {
      if (e.target.closest('.tab-lnk')) {
        e.preventDefault();
        o(e.target.closest('.tab-lnk'));
      }
    },
    false,
  );
  d.addEventListener(
    'tab-open',
    function (e) {
      if (typeof e.detail !== 'undefined' && !!e.detail.el) {
        o(e.detail.el);
      }
    },
    false,
  );

  function o(el) {
    var l = [].slice.call(el.parentElement.children).filter(function (el) {
        return el.classList.contains('tab-lnk');
      }),
      c = d.getElementById(el.getAttribute('href').replace('#', '')),
      t = [].slice.call(c.parentElement.children).filter(function (el) {
        return el.classList.contains('tab-dta');
      }); // close tabs

    [].forEach.call(t, function (s) {
      s.classList.remove('active');
    });
    [].forEach.call(l, function (s) {
      s.classList.remove('active');
    }); // open tab

    c.classList.add('active'); // add style to tab

    el.classList.add('active');
    d.dispatchEvent(new CustomEvent('tab-opened'));

    // change url hash. use pushstate not window.location.hash to prevent scrolling.
    if (history.pushState) {
      history.pushState(
        null,
        null,
        '#' + el.getAttribute('href').replace('#', ''),
      );
    }
  }

  // onload open tab that is url
  if (document.location.hash !== '' && document.location.hash !== null) {
    document.dispatchEvent(
      new CustomEvent('tab-open', {
        cancelable: true,
        detail: {
          el: document.querySelector(
            '.tab-lnk[href="' +
              document.location.hash.replace('#', '') +
              '"], .tab-lnk[href="' +
              document.location.hash +
              '"]',
          ),
        },
      }),
    );
  }
})();

(function () {
  /*
     div.tab
       div.tab-lnk.active data-target=tab1
       div.tab-lnk data-target=tab2
     div.tab-cnt
       div.tab-dta#tab1.tab-o
       div.tab-dta#tab2
     */

  (document.querySelectorAll('.panel-tab[href]') || []).forEach(function ($el) {
    $el.addEventListener(
      'click',
      function ($event) {
        $event.preventDefault();
        console.log('clicked');
        o($el);
      },
      false,
    );
  });

  var d = document;

  d.addEventListener(
    'panel-tab-open',
    function (e) {
      if (typeof e.detail !== 'undefined' && !!e.detail.el) {
        o(e.detail.el);
      }
    },
    false,
  );

  function o(el) {
    var l = [].slice.call(el.parentElement.children).filter(function (el) {
        return el.classList.contains('panel-tab');
      }),
      c = d.getElementById(el.getAttribute('href').replace('#', '')),
      t = [].slice.call(c.parentElement.children).filter(function (el) {
        return el.classList.contains('panel-tab-data');
      }); // close tabs

    [].forEach.call(t, function (s) {
      s.classList.remove('is-active');
    });
    [].forEach.call(l, function (s) {
      s.classList.remove('is-active');
    }); // open tab

    c.classList.add('is-active'); // add style to tab

    el.classList.add('is-active');
    d.dispatchEvent(new CustomEvent('panel-tab-opened'));

    // change url hash. use pushstate not window.location.hash to prevent scrolling.
    if (history.pushState) {
      history.pushState(
        null,
        null,
        '#' + el.getAttribute('href').replace('#', ''),
      );
    }
  }

  // onload open tab that is url
  if (document.location.hash !== '' && document.location.hash !== null) {
    document.dispatchEvent(
      new CustomEvent('panel-tab-open', {
        cancelable: true,
        detail: {
          el: document.querySelector(
            '.panel-tab[href="' +
              document.location.hash.replace('#', '') +
              '"], .panel-tab[href="' +
              document.location.hash +
              '"]',
          ),
        },
      }),
    );
  }
})();

(function () {
  /*
     div.tab
       div.tab-lnk.active data-target=tab1
       div.tab-lnk data-target=tab2
     div.tab-cnt
       div.tab-dta#tab1.tab-o
       div.tab-dta#tab2
     */

  (document.querySelectorAll('.step-tab[href]') || []).forEach(function ($el) {
    $el.addEventListener(
      'click',
      function ($event) {
        $event.preventDefault();
        console.log('clicked');
        o($el);
      },
      false,
    );
  });

  var d = document;

  d.addEventListener(
    'step-tab-open',
    function (e) {
      if (typeof e.detail !== 'undefined' && !!e.detail.el) {
        o(e.detail.el);
      }
    },
    false,
  );

  function o(el) {
    var l = [].slice.call(el.parentElement.children).filter(function (el) {
        return el.classList.contains('step-tab');
      }),
      c = d.getElementById(el.getAttribute('href').replace('#', '')),
      t = [].slice.call(c.parentElement.children).filter(function (el) {
        return el.classList.contains('step-tab-data');
      }); // close tabs

    [].forEach.call(t, function (s) {
      s.classList.remove('is-active');
    });
    [].forEach.call(l, function (s) {
      s.classList.remove('is-active');
    }); // open tab

    c.classList.add('is-active'); // add style to tab

    el.classList.add('is-active');
    d.dispatchEvent(new CustomEvent('step-tab-opened'));

    // change url hash. use pushstate not window.location.hash to prevent scrolling.
    if (history.pushState) {
      history.pushState(
        null,
        null,
        '#' + el.getAttribute('href').replace('#', ''),
      );
    }
  }

  // onload open tab that is url
  if (document.location.hash !== '' && document.location.hash !== null) {
    document.dispatchEvent(
      new CustomEvent('step-tab-open', {
        cancelable: true,
        detail: {
          el: document.querySelector(
            '.step-tab[href="' +
              document.location.hash.replace('#', '') +
              '"], .step-tab[href="' +
              document.location.hash +
              '"]',
          ),
        },
      }),
    );
  }
})();
