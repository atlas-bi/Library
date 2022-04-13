(function () {
  /*
    [data-toggle="clps"][data-target="#el"]
    #el
    */
  const d = document;
  d.addEventListener('click', (event) => {
    let element;

    if (event.target.closest('[data-toggle="clps"]')) {
      element = d.querySelector(
        `#${event.target
          .closest('[data-toggle]')
          .getAttribute('data-target')
          .replace('#', '')}`,
      );

      if (element === null) {
        return;
      }

      if (element.style.maxHeight || element.classList.contains('clps-o')) {
        c(element);
      } else {
        o(element);
      }
    } else if (event.target.closest('.clps:not(.clps-o')) {
      o(event.target.closest('.clps:not(.clps-o'));
    }
  });

  function h(element) {
    element.style.maxHeight = element.scrollHeight + 20 + 'px';
  }

  function c(element) {
    element.style.maxHeight = '';
    element.style.overflow = 'hidden';
    element.classList.remove('clps-o');
  }

  function o(element) {
    element.classList.add('clps-o');
    h(element);
    let l = element;
    while ((l = l.parentElement.closest('.clps-o'))) {
      l.style.removeProperty('max-height');
    }

    d.dispatchEvent(new CustomEvent('collapse-opened'));

    // Close siblings
    let o = element.parentElement.querySelector('.clps-o');
    const r = [];

    while (o) {
      if (o !== element && o.nodeType === Node.ELEMENT_NODE) r.push(o);
      o = o.nextElementSibling || o.nextSibling;
    }

    for (let x = 0; x < r.length; x++) c(r[x]);

    // After animation finished add max-height back
    window.setTimeout(function () {
      let l = element.parentElement.closest('.clps-o');
      while (l) {
        h(l);
        l = l.parentElement.closest('.clps-o');
      }

      if (element.classList.contains('clps-o')) {
        element.style.overflow = 'visible';
      }
    }, 300);
  }

  d.addEventListener('change', function (event) {
    if (event.target.closest('.clps-o')) {
      let l = event.target.closest('.clps-o');
      while (l) {
        l.style.removeProperty('max-height');
        l = l.parentElement.closest('.clps-o');
      }

      // After animation finished add max-height back
      window.setTimeout(function () {
        let l = event.target.closest('.clps-o');
        while (l) {
          h(l);
          l = l.parentElement.closest('.clps-o');
        }
      }, 300);
    }
  });

  d.addEventListener(
    'clps-close',
    function (event) {
      if (typeof event.detail !== 'undefined') {
        c(event.detail.el);
      }
    },
    false,
  );
  d.addEventListener(
    'clps-open',
    function (event) {
      if (typeof event.detail !== 'undefined') {
        o(event.detail.el);
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
  const d = document;
  d.addEventListener('click', function (event) {
    let element;
    let src;

    if (event.target.closest('[data-action="collapse"]')) {
      src = event.target.closest('[data-action="collapse"]');
      element = d.querySelector(
        `#${src.getAttribute('data-target').replace('#', '')}`,
      );
      if (element === null) {
        return;
      }

      if (element.style.maxHeight || element.classList.contains('is-active')) {
        element.classList.remove('is-active');
        src.classList.remove('is-active');
      } else {
        element.classList.add('is-active');
        src.classList.add('is-active');
      }
    }
  });
})();
