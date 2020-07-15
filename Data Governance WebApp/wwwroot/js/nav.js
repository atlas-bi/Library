(function () {
  var n = document.getElementsByClassName('nb')[0],
      b = document.getElementById('brs-b'),
      f = document.getElementById('brs-f'),
      ddl = n.getElementsByClassName('nb-ddl'),
      ddm = n.getElementsByClassName('nb-dd-ddm');

  if (b) {
    b.addEventListener('click', function () {
      history.back();
      b.blur();
    });
  }

  if (f) {
    f.addEventListener('click', function () {
      history.forward();
      f.blur();
    });
  }

  function m(l) {
    event.preventDefault();

    for (var q = 0; q < ddm.length; q++) {
      var g = ddm[q];
      if (g.classList.contains('dd-o') && g != l.nextElementSibling) g.classList.remove('dd-o');
    }

    l.nextElementSibling.classList.toggle("dd-o");
  }

  window.addEventListener('click', function (e) {
    if (e.target.classList && !e.target.classList.contains('nb-ddl')) {
      for (var q = 0; q < ddm.length; q++) {
        var l = ddm[q];
        if (l.classList.contains('dd-o')) l.classList.remove('dd-o');
      }
    }
  });

  for (var c = 0; c < ddl.length; c++) {
    var l = ddl[c];
    ddl[c].addEventListener('click', m.bind(this, l), false);
  }
})();