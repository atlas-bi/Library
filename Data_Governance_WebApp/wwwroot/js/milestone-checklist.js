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

  [].forEach.call(
    d.getElementsByClassName("complete-checklist-form"),
    function (el) {
      var items = el.getElementsByClassName("form-check"),
        select = el.getElementsByTagName("select")[0];

      if (!select.children.length) {
        [].forEach.call(items, function (e) {
          var check = e.querySelector('input[type="checkbox"]');
          if (
            check.hasAttribute("checked") &&
            check.getAttribute("checked") == "checked"
          ) {
            check.checked = true;
          }
          completed =
            e.getElementsByClassName("completed").length > 0
              ? 'selected="selected"'
              : "";
          select.innerHTML +=
            '<option value="' +
            e.getAttribute("check-id") +
            '" ' +
            completed +
            "></option";
        });
      }
    }
  );

  d.addEventListener("click", function (e) {
    if (e.target.closest(".complete-checklist-form .form-check")) {
      var i = e.target.closest(".complete-checklist-form .form-check"),
        type = 1,
        q,
        url,
        data,
        f = i.closest("form"),
        option;

      if (i.getAttribute("checked") == "checked") {
        type = 2;
      }

      option = f.querySelector(
        'select option[value="' + i.getAttribute("check-id") + '"]'
      );

      if (option) {
        if (option.hasAttribute("selected")) {
          option.removeAttribute("selected");
        } else {
          option.setAttribute("selected", "selected");
        }

        url = serialize(f);
        q = new XMLHttpRequest();
        q.open("post", f.getAttribute("action") + "&" + url, true);
        q.setRequestHeader("Content-Type", "text/html;charset=UTF-8`");
        q.setRequestHeader("X-Requested-With", "XMLHttpRequest");
        q.send();

        q.onreadystatechange = function (e) {
          if (this.readyState == 4 && this.status == 200) {
            ShowMessageBox("Changes saved.");
          }
        };
      }
    }
  });
})();
