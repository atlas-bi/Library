(function () {
  /*
    Example configuration
     div.tab
       div.tab-lnk.active data-target=tab1
       div.tab-lnk data-target=tab2
     div.tab-cnt
       div.tab-dta#tab1.tab-o
       div.tab-dta#tab2
     */

  const d = document;
  d.addEventListener(
    'click',
    function (event) {
      if (event.target.closest('.tab-lnk')) {
        event.preventDefault();
        o(event.target.closest('.tab-lnk'));
      }
    },
    false,
  );
  d.addEventListener(
    'tab-open',
    function (event) {
      if (typeof event.detail !== 'undefined' && Boolean(event.detail.el)) {
        o(event.detail.el);
      }
    },
    false,
  );

  function o(element) {
    const l = Array.prototype.slice
      .call(element.parentElement.children)
      .filter(function (element) {
        return element.classList.contains('tab-lnk');
      });
    const c = d.querySelector(
      `#${element.getAttribute('href').replace('#', '')}`,
    );
    const t = Array.prototype.slice
      .call(c.parentElement.children)
      .filter(function (element) {
        return element.classList.contains('tab-dta');
      });
    Array.prototype.forEach.call(t, function (s) {
      s.classList.remove('active');
    });
    Array.prototype.forEach.call(l, function (s) {
      s.classList.remove('active');
    });

    c.classList.add('active');

    element.classList.add('active');
    d.dispatchEvent(new CustomEvent('tab-opened'));

    // Change url hash. use pushstate not window.location.hash to prevent scrolling.
    if (history.pushState) {
      history.pushState(
        null,
        null,
        '#' + element.getAttribute('href').replace('#', ''),
      );
    }
  }

  // Onload open tab that is url
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
     Div.tab
       div.tab-lnk.active data-target=tab1
       div.tab-lnk data-target=tab2
     div.tab-cnt
       div.tab-dta#tab1.tab-o
       div.tab-dta#tab2
     */

  (document.querySelectorAll('.panel-tab[href]') || []).forEach(function (
    $element,
  ) {
    $element.addEventListener(
      'click',
      function ($event) {
        $event.preventDefault();

        o($element);
      },
      false,
    );
  });

  const d = document;

  d.addEventListener(
    'panel-tab-open',
    function (event) {
      if (typeof event.detail !== 'undefined' && Boolean(event.detail.el)) {
        o(event.detail.el);
      }
    },
    false,
  );

  function o(element) {
    const l = Array.prototype.slice
      .call(element.parentElement.children)
      .filter(function (element) {
        return element.classList.contains('panel-tab');
      });
    const c = d.querySelector(
      `#${element.getAttribute('href').replace('#', '')}`,
    );
    const t = Array.prototype.slice
      .call(c.parentElement.children)
      .filter(function (element_) {
        return element_.classList.contains('panel-tab-data');
      });

    Array.prototype.forEach.call(t, function (s) {
      s.classList.remove('is-active');
    });
    Array.prototype.forEach.call(l, function (s) {
      s.classList.remove('is-active');
    });

    c.classList.add('is-active');

    element.classList.add('is-active');
    d.dispatchEvent(new CustomEvent('panel-tab-opened'));

    // Change url hash. use pushstate not window.location.hash to prevent scrolling.
    if (history.pushState) {
      history.pushState(
        null,
        null,
        '#' + element.getAttribute('href').replace('#', ''),
      );
    }
  }

  // Onload open tab that is url
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
    Example
     div.tab
       div.tab-lnk.active data-target=tab1
       div.tab-lnk data-target=tab2
     div.tab-cnt
       div.tab-dta#tab1.tab-o
       div.tab-dta#tab2
     */

  (document.querySelectorAll('.step-tab[href]') || []).forEach(function (
    $element,
  ) {
    $element.addEventListener(
      'click',
      function ($event) {
        $event.preventDefault();

        o($element);
      },
      false,
    );
  });

  const d = document;

  d.addEventListener(
    'step-tab-open',
    function (event) {
      if (typeof event.detail !== 'undefined' && Boolean(event.detail.el)) {
        o(event.detail.el);
      }
    },
    false,
  );

  function o(element) {
    const l = Array.prototype.slice.call(
      element.closest('.steps').querySelectorAll('.steps-segment'),
    );
    const c = d.querySelector(
      `#${element.getAttribute('href').replace('#', '')}`,
    );
    const t = Array.prototype.slice
      .call(c.parentElement.children)
      .filter(function (element) {
        return element.classList.contains('step-tab-data');
      });

    Array.prototype.forEach.call(t, function (s) {
      s.classList.remove('is-active');
    });
    Array.prototype.forEach.call(l, function (s) {
      s.classList.remove('is-active');
    });

    c.classList.add('is-active');

    element.closest('.steps-segment').classList.add('is-active');
    d.dispatchEvent(new CustomEvent('step-tab-opened'));

    // Change url hash. use pushstate not window.location.hash to prevent scrolling.
    if (history.pushState) {
      history.pushState(
        null,
        null,
        '#' + element.getAttribute('href').replace('#', ''),
      );
    }
  }

  // Onload open tab that is url
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
