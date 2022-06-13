// Get data elements
//  1. enable ajax
//   data-ajax=yes
//  2. get url
//   data-url=?handler=la
//  3. get any params
//   data-parameters=d=1
//  4. if we need to refresh somteims
//   data-refresh=5
(function () {
  const d = document;

  const isInViewport = function (element) {
    const bounding = element.getBoundingClientRect();
    const padding = 400;
    return (
      bounding.top >= 0 &&
      bounding.left >= 0 &&
      bounding.bottom - element.clientHeight - padding <=
        (document.documentElement.clientHeight ||
          d.documentElement.clientHeight) &&
      bounding.right - padding - element.clientWidth <=
        (document.documentElement.clientWidth || d.documentElement.clientWidth)
    );
  };

  const sendAjax = function (element) {
    let u = element.getAttribute('data-url');
    const p = element.getAttribute('data-parameters');
    const page = element.getAttribute('data-page');
    const l = element.getAttribute('data-loadtag');

    if (!element.classList.contains('no-loader')) {
      element.innerHTML =
        '<div class="ajaxLoader"><img class="ajaxLoader-img" src="/img/loader.gif" /></div>';
    }

    if (element.classList.contains('ajax-fade')) {
      element.style.opacity = '.5';
      // eslint-disable-next-line no-unused-vars
      const clear = element.clientHeight; // Clear js cache
    }

    if (p !== null && p !== '') {
      u += u.includes('?') ? '&' : '?';

      u += p.replace(/^(&|\?)+/gm, '');
    }

    // For paginated ajax boxes
    if (page !== null && page !== '') {
      u += u.includes('?') ? '&' : '?';

      u += page.replace(/^(&|\?)+/gm, '');
    }

    if (element.q !== undefined && element.q !== null) {
      element.q.abort();
    }

    element.q = new XMLHttpRequest();
    element.q.open('get', u, true);
    element.q.setRequestHeader(
      'Content-Type',
      'application/x-www-form-urlencoded; charset=UTF-8',
    );
    element.q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
    element.q.send();

    element.q.addEventListener('load', function () {
      a(element, l, element.q.responseText);
      if (element.classList.contains('ajax-fade')) {
        element.style.opacity = '1';
      }
    });
  };

  const loadAjaxContent = function () {
    Array.prototype.forEach.call(
      d.querySelectorAll('[data-ajax="yes"]'),
      function (element) {
        if (
          !element.closest('#AdColOne') &&
          !element.closest('#AdColTwo [data-ajax="yes"]') &&
          isInViewport(element) &&
          // Is visible
          element.offsetParent !== null
        ) {
          sendAjax(element);
          element.removeAttribute('data-ajax');
          // Allow events to reload container
          element.addEventListener('reload', function () {
            sendAjax(element);
          });
        }
      },
    );
  };

  const a = function (element, l, t) {
    if (element === undefined) {
      return;
    }

    try {
      // E.style.opacity = 0;
      element.style.visibility = 'hidden';
      element.style.transition = 'visibility 0.3s ease-in-out';

      if (!element.parentNode) return;
      let sc;
      let newElement = d.createElement('div');
      newElement.innerHTML = t;

      if (l !== null && l !== '') {
        newElement = element.querySelector(l);
        newElement.setAttribute('data-loadtag', l);
      } else {
        newElement = newElement.children[0];
      }

      element.innerHTML = newElement.innerHTML;

      if (element.querySelector('script:not([type="application/json"])')) {
        sc = Array.prototype.slice.call(
          element.querySelectorAll('script:not([type="application/json"])'),
        );

        for (let x = 0; x < sc.length; x++) {
          const element_ = sc[x];
          const s = d.createElement('script');
          s.innerHTML = element_.innerHTML;
          s.type = 'text/javascript';
          if (element_.hasAttribute('src')) {
            s.setAttribute('src', element_.hasAttribute('src'));
            s.setAttribute('async', 'true');
          }

          element.append(s);
          element_.remove();
        }
      }

      element.style.visibility = 'visible';

      d.dispatchEvent(new CustomEvent('ajax'));
    } catch (error) {
      console.log(error);
    }
  };

  const loadAdAjaxContent = function () {
    Array.prototype.forEach.call(
      d.querySelectorAll('#AdColTwo [data-ajax="yes"]'),
      function (element) {
        if (isInViewport(element)) {
          let u = element.getAttribute('data-url');
          const p = element.getAttribute('data-parameters');
          const l = element.getAttribute('data-loadtag');

          if (!element.classList.contains('no-loader')) {
            element.innerHTML =
              '<div class="ajaxLoader"><img class="ajaxLoader-img" src="/img/loader.gif" /></div>';
          }

          if (p !== null && p !== '') {
            u += u.includes('?') ? '&' : '?';

            u += p.replace(/^(&|\?)+/gm, '');
            console.log(u);
          }

          element.style.visibility = 'hidden';
          element.removeAttribute('data-ajax');

          const q = new XMLHttpRequest();
          q.open('get', u, true);
          q.setRequestHeader(
            'Content-Type',
            'application/x-www-form-urlencoded; charset=UTF-8',
          );
          q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
          q.send();

          q.addEventListener('load', function () {
            a(element, l, q.responseText);
          });
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

  // Trigger reload on paginated boxes
  document.addEventListener('click', (event) => {
    if (
      event.target.closest('[data-url][data-page]') &&
      event.target.closest('.pagination-link[data-page]')
    ) {
      event.target
        .closest('[data-url][data-page]')
        .setAttribute(
          'data-page',
          event.target
            .closest('.pagination-link[data-page]')
            .getAttribute('data-page'),
        );
      event.target
        .closest('[data-url][data-page]')
        .dispatchEvent(new CustomEvent('reload'));
    }
  });
})();
