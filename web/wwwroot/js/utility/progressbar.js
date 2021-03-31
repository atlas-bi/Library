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
  var s = 0.5, // the smaller this is the slower the progress bar
    d = document,
    p = d.getElementsByClassName("prg")[0],
    b = p.getElementsByTagName("div")[0],
    w = parseFloat(b.style.width.replace("%", "")),
    utm,
    rtm,
    st = function st(max) {
      setTimeout(function () {
        max = max || 70;
        if (w >= 100) r();
        if (typeof utm !== "undefined") clearInterval(utm);
        if (typeof rtm !== "undefined") clearTimeout(rtm);
        utm = setInterval(function () {
          w += s;
          var i =
            Math.round((Math.atan(w) / (Math.PI / 2)) * 100 * 1000) / 1000;
          b.style.width = i + "%";

          if (i >= 100) {
            clearInterval(utm);
          } else if (i >= max) {
            s = 0.1;
          }
        }, 100);
      }, 0);
    },
    f = function f() {
      setTimeout(function () {
        if (typeof utm !== "undefined") clearInterval(utm);
        b.style.width = "100%";
        w = 100;
        rtm = setTimeout(e, 2000);
      }, 0);
    },
    e = function e() {
      setTimeout(function () {
        if (typeof utm !== "undefined" && w >= 100) {
          clearInterval(utm);
          b.style.transition = "width 0s, opacity 0.8s";
          b.style.opacity = "0";
        }
      }, 0);
    },
    r = function r() {
      setTimeout(function () {
        if (typeof utm !== "undefined") clearInterval(utm);
        b.style.display = "none";
        b.style.transition = "width 0s";
        b.style.width = "0%";
        var a = b.offsetHeight; // clear css cache

        b.style.removeProperty("display");
        b.style.removeProperty("transition");
        b.style.removeProperty("opacity");
        w = 0;
      }, 0);
    };

  d.addEventListener(
    "progress-start",
    function (e) {
      if (typeof e.detail !== "undefined" && !!e.detail && !!e.detail.value) {
        st(e.detail.value);
      } else {
        st();
      }
    },
    false
  );
  d.addEventListener(
    "progress-finish",
    function () {
      f();
    },
    false
  );
  d.addEventListener(
    "progress-reset",
    function () {
      r();
    },
    false
  );
})();
