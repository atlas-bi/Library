
(function () {
  var d = document,
    load = debounce(function () {
      [].forEach.call(d.querySelectorAll("img[data-src]"), function (img) {
        if (isInViewport(img)) {
          img.setAttribute("src", img.getAttribute("data-src"));
          img.removeAttribute("data-src");
        }
      });
    }, 250);

  var isInViewport = function isInViewport(elem) {
    var bounding = elem.getBoundingClientRect(),
      padding = 400;
    return (
      bounding.top >= 0 &&
      bounding.left >= 0 &&
      bounding.bottom - elem.clientHeight - padding <=
        (document.documentElement.clientHeight ||
          d.documentElement.clientHeight) &&
      bounding.right - padding - elem.clientWidth <=
        (document.documentElement.clientWidth || d.documentElement.clientWidth)
    );
  };

  load();
  d.addEventListener("ajax", function () {
    setTimeout(function () {
      load();
    }, 0);
  });
  d.addEventListener(
    "scroll",
    function () {
      debounce(load(), 100);
    },
    {
      passive: true,
    }
  );
})();
