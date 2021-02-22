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
  /*
     div.tab
       div.tab-lnk.active data-target=tab1
       div.tab-lnk data-target=tab2
     div.tab-cnt
       div.tab-dta#tab1.tab-o
       div.tab-dta#tab2
     */

  var d = document;
  d.addEventListener(
    "click",
    function (e) {
      if (e.target.closest(".tab-lnk")) {
        e.preventDefault();
        o(e.target.closest(".tab-lnk"));
      }
    },
    false
  );
  d.addEventListener(
    "tab-open",
    function (e) {
      if (typeof e.detail !== "undefined" && !!e.detail.el) {
        o(e.detail.el);
      }
    },
    false
  );

  function o(el) {
    var l = [].slice.call(el.parentElement.children).filter(function (el) {
        return el.classList.contains("tab-lnk");
      }),
      c = d.getElementById(el.getAttribute("href").replace("#", "")),
      t = [].slice.call(c.parentElement.children).filter(function (el) {
        return el.classList.contains("tab-dta");
      }); // close tabs

    [].forEach.call(t, function (s) {
      s.classList.remove("active");
    });
    [].forEach.call(l, function (s) {
      s.classList.remove("active");
    }); // open tab

    c.classList.add("active"); // add style to tab

    el.classList.add("active");
    d.dispatchEvent(new CustomEvent("tab-opened"));

    // change url hash. use pushstate not window.location.hash to prevent scrolling.
    if (history.pushState) {
      history.pushState(
        null,
        null,
        "#" + el.getAttribute("href").replace("#", "")
      );
    }
  }

  // onload open tab that is url
  if (document.location.hash !== "" && document.location.hash !== null) {
    document.dispatchEvent(
      new CustomEvent("tab-open", {
        cancelable: true,
        detail: {
          el: document.querySelector(
            '.tab-lnk[href="' +
              document.location.hash.replace("#", "") +
              '"], .tab-lnk[href="' +
              document.location.hash +
              '"]'
          ),
        },
      })
    );
  }
})();
