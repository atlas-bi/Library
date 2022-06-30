(function () {
  // Document
  //   .querySelector('.user-search')
  //   .addEventListener('keydown', function (e) {
  //     if (
  //       e.target.closest('input.dd-vsbl') &&
  //       !e.target.closest('.dd-wrp-show') &&
  //       (e.keyCode == 13 || e.keyCode == 3)
  //     ) {
  //       e.target.closest('form').submit();
  //     }
  //   });

  // report search

  let start;
  const d = document;
  const w = window;
  const grp = d.querySelector('#search-form');
  const m = d.querySelectorAll('.body-mainCtn')[0];
  // Hst = d.getElementsByClassName('sr-hst')[0];

  // if (grp == undefined) {
  //   // no search on the current page
  //   return !1;
  // }

  const i = grp.querySelectorAll('input')[0];
  let sAjx = null;
  let atmr;
  const a = document.createElement('a');

  /**
   * Scroll to top when clicking search button
   */
  document.addEventListener('click', function (event) {
    if (event.target.closest('#nav-search')) {
      document.documentElement.scrollTop = 0;
      document.body.scrollTop = 0;
    }
  });

  const oldHref = w.location.pathname.toLowerCase().startsWith('/search')
    ? '/'
    : w.location.href;

  function replaceUrlParameter(url, parameterName, parameterValue) {
    if (parameterValue === null) {
      parameterValue = '';
    }

    const pattern = new RegExp('\\b(' + parameterName + '=).*?(&|#|$)');
    if (url.search(pattern) >= 0) {
      if (parameterValue === '') {
        return url.replace(pattern, '');
      }

      return url.replace(pattern, '$1' + parameterValue + '$2');
    }

    url = url.replace(/[?#]$/, '');
    if (parameterValue === '') {
      return url;
    }

    return (
      url +
      (url.indexOf('?') > 0 ? '&' : '?') +
      parameterName +
      '=' +
      parameterValue
    );
  }

  function getQueryStringParameters(parameters, url) {
    // First decode URL to get readable data
    const href = decodeURIComponent(url || window.location.href);
    // Regular expression to get value
    const regEx = new RegExp('[?&]' + parameters + '=([^&#]*)', 'i');
    const value = regEx.exec(href);
    // Return the value if exist
    return value ? value[1] : null;
  }

  function ajaxSearch(value, url) {
    // Show loader here
    if (document.querySelector('.body-mainCtn')) {
      document.querySelector('.body-mainCtn').style.opacity = 0.5;
    }

    // Attempt to get existing search params from url
    const urlPath = window.location.pathname.toLowerCase() === '/search';

    let s = url;

    // If we are on the search page already, keep filters.
    if (urlPath) {
      value = encodeURIComponent(
        value || getQueryStringParameters('Query', url),
      );
      s =
        url ||
        replaceUrlParameter(
          replaceUrlParameter(
            window.location.href.replace(window.location.origin, ''),
            'Query',
            value,
          ),
          'PageIndex',
          '',
        );
    } else if (url) {
      s = url;
    } else {
      s = '/Search?Query=' + value;

      // Add default filters for type depending on url
      if (window.location.pathname.toLowerCase() === '/users') {
        s += '&type=users';
      } else if (window.location.pathname.toLowerCase() === '/groups') {
        s += '&type=groups';
      }
    }

    const u = s.replace('/Search?Query=', '');

    start = new Date();
    if (
      (typeof value !== 'undefined' && value !== null && value.length > 0) ||
      typeof url !== 'undefined'
    ) {
      document.documentElement.scrollTop = 0;
      document.body.scrollTop = 0;

      if (typeof atmr !== 'undefined') clearTimeout(atmr);

      a.href = url || oldHref;

      history.pushState(
        {
          state: 'ajax',
          search: value,
        },
        document.title,
        w.location.origin + '/Search?Query=' + encodeURI(decodeURI(u)),
      );

      if (sAjx !== null) {
        sAjx.abort();
      }

      sAjx = new XMLHttpRequest();
      sAjx.open('get', s, true);
      sAjx.setRequestHeader(
        'Content-Type',
        'application/x-www-form-urlencoded; charset=UTF-8',
      );
      sAjx.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
      sAjx.send();

      sAjx.addEventListener('load', function () {
        l(sAjx.responseText, a, m, d, atmr, s, u, value);
      });
    } else {
      d.dispatchEvent(
        new CustomEvent('load-ajax', {
          detail: oldHref,
        }),
      );
    }
  }

  const l = function (t, a, m, d, atmr, s, u, value) {
    // Hst.style.display = 'none';
    // remove nav links
    if (document.querySelector('.side-links')) {
      document.querySelector('.side-links').innerHTML = '';
    }

    // Clear ads
    if (document.querySelector('#AdColTwo')) {
      document.querySelector('#AdColTwo').innerHTML = '';
    }

    m.style.visibility = 'visible';
    m.style.removeProperty('overflow');
    m.innerHTML = t;
    const sc = Array.prototype.slice.call(
      m.querySelectorAll('script:not([type="application/json"])'),
    );

    for (const element of sc) {
      const q = document.createElement('script');
      q.innerHTML = element.innerHTML;
      q.type = 'text/javascript';
      q.setAttribute('async', 'true');
      m.append(q);
      element.remove();
    }

    d.title = 'Search: ' + value + ' | Atlas BI Library';

    history.replaceState(
      {
        state: 'ajax',
        search: value,
      },
      document.title,
      w.location.origin + '/Search?Query=' + encodeURI(decodeURI(u)),
    );

    atmr = setTimeout(function () {
      document.dispatchEvent(
        new CustomEvent('analytics-post', {
          cancelable: true,
          detail: {
            value: Date.now() - start.getTime(),
            type: 'newpage',
          },
        }),
      );
    }, 3000);

    document.dispatchEvent(new CustomEvent('related-reports'));
    document.dispatchEvent(new CustomEvent('ajax'));
    document.dispatchEvent(new CustomEvent('ss-load'));
    document.dispatchEvent(new CustomEvent('code-highlight'));

    /* Remove loader here */
    if (document.querySelector('.body-mainCtn')) {
      document.querySelector('.body-mainCtn').style.opacity = '';
    }
  };

  grp.addEventListener('click', function (event) {
    event.stopPropagation();
  });
  grp.addEventListener('submit', function (event) {
    event.preventDefault();
  });

  // Only search if the user has stopped typing for 1/5 second.
  const searchTimeout = 250;
  let searchTimerId = null;
  i.addEventListener('input', function () {
    window.clearTimeout(searchTimerId);
    searchTimerId = window.setTimeout(function () {
      if (i.value.trim() !== '') {
        ajaxSearch(i.value, null);
        window.clearTimeout(searchTimerId);
      }
    }, searchTimeout);
  });

  i.addEventListener('keydown', (event) => {
    if (Number(event.keyCode) === 13 && i.value.trim() !== '') {
      ajaxSearch(i.value, null);
    }
  });

  d.addEventListener('click', function (event) {
    // Hst.style.display = 'none';

    if (event.target.matches('.search-filter input')) {
      event.preventDefault();
      submit(event.target.closest('.search-filter input').value);
      return !1;
    }

    if (event.target.matches('.page-link')) {
      if (event.target.closest('.search-filter input')) {
        submit(event.target.closest('.search-filter input').value);
      }

      return !1;
    }
  });

  function submit(l) {
    ajaxSearch(null, l);
  }
})();
