(function () {
  const d = document;
  const srcset = function ($element) {
    Array.prototype.forEach.call(
      $element.querySelectorAll('source[data-srcset]'),
      function (img) {
        if (isInViewport(img)) {
          // Set image to nothing to clear, then load new
          const attrib = img.dataset.srcset;
          img.setAttribute('srcset', '');
          img.removeAttribute('srcset');
          img.setAttribute('srcset', attrib);
        }
      },
    );
  };

  const src = function ($element) {
    Array.prototype.forEach.call(
      $element.querySelectorAll('img[data-src]'),
      function (img) {
        if (isInViewport(img)) {
          // Set image to nothing to clear, then load new
          const attrib = img.dataset.src;
          img.setAttribute('src', '');
          delete img.dataset.src;
          img.setAttribute('src', attrib);
        }
      },
    );
  };

  const load = function () {
    // Start with "picture" so all elements are updated together.
    Array.prototype.forEach.call(
      d.querySelectorAll('picture'),
      function (picture) {
        src(picture);
        srcset(picture);
      },
    );

    // Then global any leftovers
    src(d);
    srcset(d);
  };

  const isInViewport = function (element) {
    const bounding = element.getBoundingClientRect();
    const padding = 600;
    return (
      bounding.top >= 0 - padding &&
      bounding.left >= 0 &&
      bounding.bottom - element.clientHeight - padding <=
        (document.documentElement.clientHeight ||
          d.documentElement.clientHeight) &&
      bounding.right - padding - element.clientWidth <=
        (document.documentElement.clientWidth ||
          d.documentElement.clientWidth) &&
      // Is visible
      element.offsetParent !== null
    );
  };

  load();
  d.addEventListener('ajax', function () {
    setTimeout(function () {
      load();
    }, 0);
  });
  d.addEventListener('modal-open', function () {
    load();
  });
  d.addEventListener(
    'scroll',
    function () {
      debounce(load(), 100);
    },
    {
      passive: true,
    },
  );
})();
