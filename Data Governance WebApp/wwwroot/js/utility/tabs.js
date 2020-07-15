(function () {
  var d = document;
  d.addEventListener('click', function (e) {
    if (e.target.closest('.tab-lnk')) {
      e.preventDefault();
      o(e.target.closest('.tab-lnk'));
    }
  });
  d.addEventListener('tab-open', function (e) {
    if (typeof e.detail !== 'undefined' && !!e.detail.el) {
      o(e.detail.el);
    }
  }, false);

  function o(el) {
    var l = [].slice.call(el.parentElement.children).filter(function (el) {
      return el.classList.contains('tab-lnk');
    }),
        c = d.getElementById(el.getAttribute('href').replace('#', '')),
        t = [].slice.call(c.parentElement.children).filter(function (el) {
      return el.classList.contains('tab-dta');
    }); // close tabs

    [].forEach.call(t, function (s) {
      s.classList.remove('active');
    });
    [].forEach.call(l, function (s) {
      s.classList.remove('active');
    }); // open tab

    c.classList.add('active'); // add style to tab

    el.classList.add('active');
    d.dispatchEvent(new CustomEvent("tab-opened"));
  }
  /*
    div.tab
      div.tab-lnk.active data-target=tab1
      div.tab-lnk data-target=tab2
    div.tab-cnt
      div.tab-dta#tab1.tab-o
      div.tab-dta#tab2
    */

})();