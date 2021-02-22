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
(function () {
  var d = document;
  d.addEventListener("ajax", function () {
    setTimeout(function () {
      b();
    }, 0);
  });

  var b = function b() {
    var t = d.title.indexOf("-") != -1 ? d.title.split("-")[0] : d.title,
      u = window.location.href,
      j = {},
      c = sessionStorage.getItem(btoa("crumbs"));
    c = c !== null ? JSON.parse(c) : [];

    if (c.length == 0 || !(c[c.length - 1].t == t && c[c.length - 1].u == u)) {
      j.t = t;
      j.u = u;

      if (
        c.length > 0 &&
        c[c.length - 1].t.startsWith("Search") &&
        j.t.startsWith("Search")
      ) {
        c.pop();
      }

      c.push(j);
      sessionStorage.setItem(btoa("crumbs"), JSON.stringify(c));
    }

    var el = d.getElementsByClassName("nb-cmbs")[0];
    var q = el.offsetHeight;
    el.innerHTML = h(c);
    el.style.opacity = 1;
  };

  var h = function h(c) {
    var x = 0,
      l = "";
    c = c.slice(Math.max(c.length - 7, 0));
    c.reverse();
    l = "</a></li><li>" + c[0].t;
    for (x = 1; x < c.length - 1; x++) {
      l += '<li><a href="' + c[x].u + '" class="">' + c[x].t;
    }

    return l;
  };

  b();
})();
