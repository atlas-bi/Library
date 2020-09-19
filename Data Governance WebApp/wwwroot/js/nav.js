/*
    Atlas of Information Management business intelligence library and documentation database.
    Copyright (C) 2020  Riverside Healthcare, Kankakee, IL

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

(function (root, factory) {
    if (typeof define === 'function' && define.amd) {
        // AMD. Register as an anonymous module.
        define(['b'], factory);
    } else {
        // Browser globals
        root.Nav = factory(root.b);
    }
}(typeof self !== 'undefined' ? self : this, function (b) {

  var x = function(){
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
  };
  console.log('nav scripts loaded');
  return x;
  }));
Nav();