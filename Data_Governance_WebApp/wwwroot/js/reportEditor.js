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

  /* details */
  d.getElementById("report-edit-details-form").addEventListener(
    "submit",
    function (e) {
      e.preventDefault();
      var form = e.target,
        q = new XMLHttpRequest();
      q.open("post", form.getAttribute("action") + "&" + serialize(form), true);
      q.setRequestHeader("Content-Type", "text/plain;charset=UTF-8`");
      q.setRequestHeader("X-Requested-With", "XMLHttpRequest");
      q.send();
      q.onload = function () {
        d.getElementById("editorMdl-titleSave").style.visibility = "visible";
        setTimeout(function () {
          d.getElementById("editorMdl-titleSave").style.removeProperty(
            "visibility"
          );
        }, 750);
      };
    }
  );

  // maintenance
  d.getElementById("report-edit-new-maintenance").addEventListener(
    "submit",
    function (e) {
      e.preventDefault();
      var form = e.target,
        q = new XMLHttpRequest();
      q.open("post", form.getAttribute("action") + "&" + serialize(form), true);
      q.setRequestHeader("Content-Type", "text/plain;charset=UTF-8`");
      q.setRequestHeader("X-Requested-With", "XMLHttpRequest");
      q.send();
      q.onload = function () {
        d.getElementById("editorMdl-titleSave").style.visibility = "visible";
        setTimeout(function () {
          d.getElementById("editorMdl-titleSave").style.removeProperty(
            "visibility"
          );
        }, 750);
      };
      form.reset();
    }
  );

  /* me tickets */
  d.querySelector("#editModal").addEventListener("submit", function (e) {
    if (
      e.target.closest("#report-edit-new-meticket") ||
      e.target.closest("#report-editor-remove-meticket-form")
    ) {
      e.preventDefault();
      var form =
          e.target.closest("#report-edit-new-meticket") ||
          e.target.closest("#report-editor-remove-meticket-form"),
        q = new XMLHttpRequest();
      q.open("post", form.getAttribute("action") + "&" + serialize(form), true);
      q.setRequestHeader("Content-Type", "text/plain;charset=UTF-8`");
      q.setRequestHeader("X-Requested-With", "XMLHttpRequest");
      q.send();
      q.onload = function () {
        d.getElementById("report-editor-metickets-container").innerHTML =
          q.responseText;
        form.reset();
      };
    }
  });

  /* images */
  d.querySelector("#editModal").addEventListener("change", function (e) {
    if (
      e.target.matches("#report-add-image-input") &&
      e.target.closest("#report-add-image-input").value != ""
    ) {
      var form = e.target.closest("form"),
        id = form.querySelector('[name="Id"]').value;
      if (id.value == "") {
        return;
      }
      q = new XMLHttpRequest();
      q.open("post", form.getAttribute("action"), true);
      q.withCredentials = true;
      q.setRequestHeader("X-Requested-With", "XMLHttpRequest");
      q.send(new FormData(form));
      q.upload.addEventListener("progress", function (e) {
        d.dispatchEvent(new CustomEvent("progress-start"));
      });
      q.onload = function () {
        form
          .closest(".editorImg-img")
          .insertAdjacentHTML("beforebegin", q.responseText);
        e.target.closest("#report-add-image-input").setAttribute("value", "");
        e.target.closest("#report-add-image-input").value = null;
      };
    }
  });

  d.querySelector("#editModal").addEventListener("submit", function (e) {
    if (e.target.closest(".report-remove-image")) {
      e.preventDefault();
      var url,
        el = e.target.closest(".editorImg-img"),
        id = el.querySelector('[name="Id"]').value,
        imgId = el.querySelector('[name="ImgId"]').value,
        data = {};
      data.Id = id;
      data.ImgId = imgId;

      url = Object.keys(data)
        .map(function (k) {
          return encodeURIComponent(k) + "=" + encodeURIComponent(data[k]);
        })
        .join("&");

      q = new XMLHttpRequest();
      q.open("post", "reports?handler=RemoveImage&" + url, true);
      q.setRequestHeader(
        "Content-Type",
        "application/x-www-form-urlencoded; charset=UTF-8"
      );
      q.setRequestHeader("X-Requested-With", "XMLHttpRequest");
      q.send();

      q.onload = function () {
        el.parentElement.removeChild(el);
      };
    }
  });

  /* terms */
  d.querySelector("#editModal").addEventListener("change", function (e) {
    if (e.target.closest("#NewTermLink_TermId")) {
      var t = e.target.closest("#NewTermLink_TermId"),
        q = new XMLHttpRequest();
      q.open(
        "post",
        "/Reports?handler=CurrentTermDetails&TermId=" + t.getAttribute("value"),
        true
      );
      q.setRequestHeader("Content-Type", "text/plain;charset=UTF-8`");
      q.setRequestHeader("X-Requested-With", "XMLHttpRequest");
      q.send();
      q.onload = function () {
        if (q.responseText) {
          data = JSON.parse(q.responseText);
          d.getElementsByClassName("new-term-summary")[0].innerHTML =
            '<div class="markdown noleft">' + data.summary || "n/a" + "</div>";
          d.getElementsByClassName("new-term-def")[0].innerHTML =
            '<div class="markdown noleft">' + data.technicalDefinition ||
            "n/a" + "</div>";
          // render markdown
          d.dispatchEvent(new CustomEvent("code-highlight"));
        }
      };
    }
  });

  d.querySelector("#editModal").addEventListener("submit", function (e) {
    var q,
      data,
      form,
      url,
      termsContainer = d.getElementById("report-editor-currentterms-container");
    if (
      e.target.closest("#report-editor-remove-term-form") ||
      e.target.closest("#report-editor-add-term-form") ||
      e.target.closest("#report-editor-add-new-term-form")
    ) {
      e.preventDefault();

      form =
        e.target.closest("#report-editor-remove-term-form") ||
        e.target.closest("#report-editor-add-term-form") ||
        e.target.closest("#report-editor-add-new-term-form");
      url = form.getAttribute("action");

      q = new XMLHttpRequest();
      q.open("post", form.getAttribute("action") + "&" + serialize(form), true);
      q.setRequestHeader("Content-Type", "text/plain;charset=UTF-8`");
      q.setRequestHeader("X-Requested-With", "XMLHttpRequest");
      q.send();
      q.onload = function () {
        data = q.responseText;

        if (data.trim().length == 0) {
          termsContainer.innerHTML = "";
        } else {
          termsContainer.innerHTML = data;
          // render markdown
          d.dispatchEvent(new CustomEvent("code-highlight"));
        }
        d.getElementById("editorMdl-titleSave").style.visibility = "visible";
        setTimeout(function () {
          d.getElementById("editorMdl-titleSave").style.removeProperty(
            "visibility"
          );
        }, 750);
      };
      form.reset();
      d.getElementsByClassName("new-term-summary")[0].innerHTML = "";
      d.getElementsByClassName("new-term-def")[0].innerHTML = "";
    }
  });
})();
