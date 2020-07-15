(function () {
  var load = debounce(function () {
    [].forEach.call(document.querySelectorAll('img[data-src]'), function (img) {
      if (isInViewport(img)) {
        img.setAttribute('src', img.getAttribute('data-src'));
        img.removeAttribute('data-src');
      }
    });
  }, 250);

  var isInViewport = function isInViewport(elem) {
    var bounding = elem.getBoundingClientRect(),
        padding = 400;
    return bounding.top >= 0 && bounding.left >= 0 && bounding.bottom - elem.clientHeight - padding <= (window.innerHeight || document.documentElement.clientHeight) && bounding.right - padding - elem.clientWidth <= (window.innerWidth || document.documentElement.clientWidth);
  };

  load();
  document.addEventListener('ajax', function () {
    setTimeout(function () {
      load();
    }, 0);
  });
  document.addEventListener('scroll', function () {
    setTimeout(function () {
      load();
    }, 0);
  }, {passive: true});
})();