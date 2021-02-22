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
  d.addEventListener(
    "submit",
    function (e) {
      if (e.target.matches(".comments-form")) {
        e.preventDefault();
        var t = e.target,
          c = d.getElementsByClassName("comments")[0],
          url,
          q;
        url = serialize(t);
        c.style.opacity = 0;
        c.style.transition = "opacity 0.1s ease-in-out";
        q = new XMLHttpRequest();
        q.open("post", t.getAttribute("action") + "&" + url, true);
        q.setRequestHeader(
          "Content-Type",
          "application/x-www-form-urlencoded; charset=UTF-8"
        );
        q.setRequestHeader("X-Requested-With", "XMLHttpRequest");
        q.send();

        q.onload = function () {
          c.innerHTML = q.responseText;
          c.style.opacity = 1;
          d.dispatchEvent(new CustomEvent("build-inputs"));
        };
      }
    },
    false
  );
})();
