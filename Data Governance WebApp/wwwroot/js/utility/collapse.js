(function () {
  /*
  [data-toggle="clps"][data-target="#el"]
  #el
  */
  var d = document;
  d.addEventListener('click', function (e) {
    var el,
        d = document; // collapse

    if (e.target.getAttribute('data-toggle') == 'clps') {
      el = d.getElementById(e.target.getAttribute('data-target').replace("#", ''));

      if (el == null) {
        return;
      }

      if (el.style.maxHeight || el.classList.contains('clps-o')) {
        c(el);
      } else {
        o(el);
      }
    }
  });

  function c(el) {
    el.style.maxHeight = null;
    el.classList.remove('clps-o');
  }

  function o(el) {
    el.style.maxHeight = el.scrollHeight + 'px';
    el.classList.add('clps-o');
    document.dispatchEvent(new CustomEvent("collapse-opened"));
  }

  d.addEventListener('clps-close', function (e) {
    if (typeof e.detail !== 'undefined') {
      c(e.detail.el);
    }
  }, false);
  d.addEventListener('clps-open', function (e) {
    if (typeof e.detail !== 'undefined') {
      o(e.detail.el);
    }
  }, false);
})();