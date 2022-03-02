(function () {
  var d = document,
    srcset = function ($el) {
      [].forEach.call(
        $el.querySelectorAll('source[data-srcset]'),
        function (img) {
          if (isInViewport(img)) {
            // set image to nothing to clear, then load new
            var attrib = img.getAttribute('data-srcset');
            img.setAttribute('srcset', '');
            img.removeAttribute('data-srcset');
            img.setAttribute('srcset', attrib);
          }
        },
      );
    },
    src = function ($el) {
      [].forEach.call($el.querySelectorAll('img[data-src]'), function (img) {
        if (isInViewport(img)) {
          // set image to nothing to clear, then load new
          var attrib = img.getAttribute('data-src');
          img.setAttribute('src', '');
          img.removeAttribute('data-src');
          img.setAttribute('src', attrib);
        }
      });
    },
    load = function () {
      // start with "picture" so all elements are updated together.
      [].forEach.call(d.querySelectorAll('picture'), function (picture) {
        src(picture);
        srcset(picture);
      });

      // then global any leftovers
      src(d);
      srcset(d);
    };

  var isInViewport = function isInViewport(elem) {
    var bounding = elem.getBoundingClientRect(),
      padding = 600;
    return (
      bounding.top >= 0 - padding &&
      bounding.left >= 0 &&
      bounding.bottom - elem.clientHeight - padding <=
        (document.documentElement.clientHeight ||
          d.documentElement.clientHeight) &&
      bounding.right - padding - elem.clientWidth <=
        (document.documentElement.clientWidth ||
          d.documentElement.clientWidth) &&
      // is visible
      elem.offsetParent !== null
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
