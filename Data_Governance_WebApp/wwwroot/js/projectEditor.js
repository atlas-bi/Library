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
  function UpdateMilestoneChecklistItems(e) {
    var t = e.closest("form");

    if (
      null == e.nextElementSibling ||
      (null !== e.nextElementSibling &&
        !e.nextElementSibling.classList.contains("checklistitem") &&
        e.nextElementSibling.innerHTML !== "Task")
    ) {
      var n = e.cloneNode(!0),
        l = document.createElement("label");
      l.innerHTML = "Task";
      n.value = "";
      e.parentNode.insertBefore(n, e.nextSibling);
      n.parentNode.insertBefore(l, n);
    }

    var i = t.querySelector('input[name="DpChecklist"]'),
      h = "",
      c = 0;

    [].map.call(t.getElementsByClassName("checklistitem"), function (e) {
      var t = e.value.trim();
      if ("" != t) {
        h += '"' + (c += 1) + '":"' + t + '",';
      }
    });
    if (h.length > 0) {
      i.value = "{" + h.substring(0, h.length - 1) + "}";
    }
  }
  document.addEventListener("click", function (e) {
    if (e.target.matches(".checklistitem"))
      UpdateMilestoneChecklistItems(e.target);
  });
  document.addEventListener("focus", function (e) {
    if (e.target.matches(".checklistitem"))
      UpdateMilestoneChecklistItems(e.target);
  });
  document.addEventListener("change", function (e) {
    if (e.target.matches(".checklistitem"))
      UpdateMilestoneChecklistItems(e.target);
  });
  document.addEventListener("keyup", function (e) {
    if (e.target.matches(".checklistitem"))
      UpdateMilestoneChecklistItems(e.target);
  });

  var cli = document.querySelectorAll("form#AddMilestone .checklistitem");
  for (var x = 0; x < cli.length; x++) {
    UpdateMilestoneChecklistItems(cli[x]);
  }
})();
