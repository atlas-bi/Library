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

document.addEventListener("DOMContentLoaded", function () {
  var d = document,
    a = function (url) {
      if (cache.exists(url)) {
        load(cache.get(url));
      } else {
        var q = new XMLHttpRequest();
        q.open("get", url, true);
        q.setRequestHeader(
          "Content-Type",
          "application/x-www-form-urlencoded; charset=UTF-8"
        );
        q.setRequestHeader("X-Requested-With", "XMLHttpRequest");
        q.send();

        q.onload = function () {
          load(q.responseText);
          var ccHeader =
            q.getResponseHeader("Cache-Control") != null
              ? (q.getResponseHeader("Cache-Control").match(/\d+/) || [null])[0]
              : null;

          if (ccHeader) {
            cache.set(url, q.responseText, ccHeader);
          }
        };
      }

      window.profileLoad = undefined;
    },
    load = function (text) {
      if (d.getElementsByClassName("profile-buttonHidden")[0]) {
        d.getElementsByClassName("profile-buttonHidden")[0].classList.remove(
          "profile-buttonHidden"
        );
      }
      d.querySelector("#profile-modal .mdl-b").innerHTML = text;
      d.dispatchEvent(new CustomEvent("load-charts"));
    };
  a("/Profile?id=" + getUrlVars().id);
});
