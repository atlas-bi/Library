(function () {
  /*
    [data-toggle="clps"][data-target="#el"]
    #el
    */
  var d = document;
  d.addEventListener('click', function (e) {
    var el;

    if (e.target.closest('[data-toggle="clps"]')) {
      el = d.getElementById(
        e.target
          .closest('[data-toggle]')
          .getAttribute('data-target')
          .replace('#', ''),
      );

      if (el == null) {
        return;
      }

      if (el.style.maxHeight || el.classList.contains('clps-o')) {
        c(el);
      } else {
        o(el);
      }
    } else if (e.target.closest('.clps:not(.clps-o')) {
      o(e.target.closest('.clps:not(.clps-o'));
    }
  });

  function h(el) {
    el.style.maxHeight = el.scrollHeight + 20 + 'px';
  }

  function c(el) {
    el.style.maxHeight = '';
    el.style.overflow = 'hidden';
    el.classList.remove('clps-o');
  }

  function o(el) {
    el.classList.add('clps-o');
    h(el);
    var l = el;
    while ((l = l.parentElement.closest('.clps-o'))) {
      l.style.removeProperty('max-height');
    }
    d.dispatchEvent(new CustomEvent('collapse-opened'));

    // close siblings
    var o = el.parentElement.querySelector('.clps-o'),
      r = [];

    while (o) {
      if (o !== el && o.nodeType === Node.ELEMENT_NODE) r.push(o);
      o = o.nextElementSibling || o.nextSibling;
    }

    for (var x = 0; x < r.length; x++) c(r[x]);

    // after animation finished add max-height back
    window.setTimeout(function () {
      var l = el.parentElement.closest('.clps-o');
      while (l) {
        h(l);
        l = l.parentElement.closest('.clps-o');
      }
      if (el.classList.contains('clps-o')) {
        el.style.overflow = 'visible';
      }
    }, 300);
  }

  d.addEventListener('change', function (e) {
    if (e.target.closest('.clps-o')) {
      var l = e.target.closest('.clps-o');
      while (l) {
        l.style.removeProperty('max-height');
        l = l.parentElement.closest('.clps-o');
      }
      // after animation finished add max-height back
      window.setTimeout(function () {
        var l = e.target.closest('.clps-o');
        while (l) {
          h(l);
          l = l.parentElement.closest('.clps-o');
        }
      }, 300);
    }
  });

  d.addEventListener(
    'clps-close',
    function (e) {
      if (typeof e.detail !== 'undefined') {
        c(e.detail.el);
      }
    },
    false,
  );
  d.addEventListener(
    'clps-open',
    function (e) {
      if (typeof e.detail !== 'undefined') {
        o(e.detail.el);
      }
    },
    false,
  );
})();

(function () {
  /*
    [data-action="collapse"][data-target="#el"]
    #el
    */
  var d = document;
  d.addEventListener('click', function (e) {
    var el, src;

    if (e.target.closest('[data-action="collapse"]')) {
      src = e.target.closest('[data-action="collapse"]');
      el = d.getElementById(src.getAttribute('data-target').replace('#', ''));
      if (el == null) {
        return;
      }

      if (el.style.maxHeight || el.classList.contains('is-active')) {
        el.classList.remove('is-active');
        src.classList.remove('is-active');
      } else {
        el.classList.add('is-active');
        src.classList.add('is-active');
      }
    }
  });
})();
