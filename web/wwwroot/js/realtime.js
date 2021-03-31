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
  var e = document,
    t = null,
    getActiveUsers = function () {
      if (t !== null) {
        t.abort();
      }

      t = new XMLHttpRequest();
      t.open("get", "/analytics?handler=LiveUsers", !0);
      t.setRequestHeader(
        "Content-Type",
        "application/x-www-form-urlencoded; charset=UTF-8"
      );
      t.send();
      t.onload = function () {
        if (e.getElementById("active-users")) {
          e.getElementById("active-users").innerHTML = t.responseText;
          e.dispatchEvent(new CustomEvent("ajax"));
        }
      };
    };

  getActiveUsers();
  setInterval(function () {
    if (e.getElementById("active-users")) {
      getActiveUsers();
    }
  }, 1e4);
});
