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
  document.addEventListener("click", function (e) {
    if (
      e.target.matches('.role-permissions[type="checkbox"]') &&
      e.target.tagName == "INPUT"
    ) {
      var p = e.target.parentElement,
        i = e.target,
        type = 1,
        q,
        url,
        data;

      if (i.getAttribute("checked") == "checked") {
        i.removeAttribute("checked");
        type = 2;
      } else {
        i.setAttribute("checked", "checked");
      }

      data = {
        RoleId: p.getAttribute("roleid"),
        PermissionId: p.getAttribute("permissionid"),
        Type: type,
      };
      url = Object.keys(data)
        .map(function (k) {
          return encodeURIComponent(k) + "=" + encodeURIComponent(data[k]);
        })
        .join("&");
      q = new XMLHttpRequest();
      q.open("post", "/AccessControl?handler=UpdatePermissions&" + url, true);
      q.setRequestHeader("Content-Type", "text/html;charset=UTF-8`");
      q.setRequestHeader("X-Requested-With", "XMLHttpRequest");
      q.send();

      q.onreadystatechange = function (e) {
        if (this.readyState == 4 && this.status == 200) {
          ShowMessageBox("Changes saved.");
        }
      };
    }
  });
})();
