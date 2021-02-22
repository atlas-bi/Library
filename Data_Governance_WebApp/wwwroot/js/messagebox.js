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

function ShowMessageBox(message) {
  var d = document,
    w = d.getElementsByClassName("message-wrapper")[0],
    c = w.getElementsByClassName("message-close");
  d.querySelector(".message-container .message-inner").innerHTML = message;
  w.classList.remove("hidden");
  setTimeout(function () {
    w.classList.add("hidden");
  }, 2000);
}

document.addEventListener("click", function (e) {
  if (
    e.target.matches(".message-close") &&
    e.target.parentElement.matches(".message-wrapper")
  ) {
    e.target.parentElement.classList.add("hidden");
  }
});

document.addEventListener(
  "show-message",
  function (e) {
    if (typeof e.detail !== "undefined" && !!e.detail && !!e.detail.value) {
      ShowMessageBox(e.detail.value);
    }
  },
  false
);
