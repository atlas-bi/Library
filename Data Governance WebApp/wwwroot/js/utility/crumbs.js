(function () {
  var d = document;
  d.addEventListener('ajax', function () {
    setTimeout(function () {
      b();
    }, 0);
  });

  var b = function b() {
    var t = d.title.split(' -')[0].length > 15 ? d.title.split(' -')[0].substring(0, 15) + '..' : d.title.split(' -')[0],
        u = window.location.href,
        j = {},
        c = sessionStorage.getItem(btoa('crumbs'));
    c = c !== null ? JSON.parse(c) : [];

    if (c.length == 0 || !(c[c.length - 1].t == t && c[c.length - 1].u == u)) {
      j.t = t;
      j.u = u;

      if (c.length > 0 && c[c.length - 1].t.startsWith('Search') && j.t.startsWith('Search')) {
        c.pop();
      }

      c.push(j);
      sessionStorage.setItem(btoa('crumbs'), JSON.stringify(c));
    }

    var el = d.getElementsByClassName('nb-cmbs')[0];
    var q = el.offsetHeight;
    el.innerHTML = h(c);
    el.style.opacity = 1;
  };

  var h = function h(c) {
    var x = 0,
        l = '';
    c = c.slice(Math.max(c.length - 10, 0));

    for (x; x < c.length - 1; x++) {
      l += '<li><a href="' + c[x].u + '" class="ajax">' + c[x].t;
    }

    l += '</a></li><li>' + c[c.length - 1].t;
    return l;
  };

  b();
  /*
    add url + name to session storage on load and ajax
  */
})();