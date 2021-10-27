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
  if (document.querySelector("#editModal") !== null) {
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

    document
      .querySelector("#editModal")
      .addEventListener("click", function (e) {
        var q,
          data,
          form,
          url,
          termsContainer = document.getElementById(
            "collection-editor-currentterms-container"
          );
        if (e.target.closest("#collection-editor-remove-term-form")) {
          e.preventDefault();

          form = e.target.closest("#collection-editor-remove-term-form");
          url = form.getAttribute("href");

          q = new XMLHttpRequest();
          q.open("get", url, true);
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
              document.dispatchEvent(new CustomEvent("code-highlight"));
            }
            document.getElementById("editorMdl-titleSave").style.visibility =
              "visible";
            setTimeout(function () {
              document
                .getElementById("editorMdl-titleSave")
                .style.removeProperty("visibility");
            }, 750);
          };
        }
      });

    document
      .querySelector("#editModal")
      .addEventListener("click", function (e) {
        var q,
          data,
          form,
          url,
          termsContainer = document.getElementById(
            "collection-editor-currentreports-container"
          );
        if (e.target.closest("#collection-editor-remove-report-form")) {
          e.preventDefault();

          form = e.target.closest("#collection-editor-remove-report-form");
          url = form.getAttribute("href");

          q = new XMLHttpRequest();
          q.open("get", url, true);
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
              document.dispatchEvent(new CustomEvent("code-highlight"));
            }
            document.getElementById("editorMdl-titleSave").style.visibility =
              "visible";
            setTimeout(function () {
              document
                .getElementById("editorMdl-titleSave")
                .style.removeProperty("visibility");
            }, 750);
          };
        }
      });

    document
      .querySelector("#editModal")
      .addEventListener("submit", function (e) {
        var q,
          data,
          form,
          url,
          termsContainer = document.getElementById(
            "collection-editor-currentterms-container"
          );
        if (
          e.target.closest("#collection-editor-edit-term-form") ||
          e.target.closest("#collection-editor-add-term-form")
        ) {
          e.preventDefault();

          form =
            e.target.closest("#collection-editor-edit-term-form") ||
            e.target.closest("#collection-editor-add-term-form");
          url = form.getAttribute("action");

          q = new XMLHttpRequest();
          q.open(
            "post",
            form.getAttribute("action") + "&" + serialize(form),
            true
          );
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
              document.dispatchEvent(new CustomEvent("code-highlight"));
            }
            document.getElementById("editorMdl-titleSave").style.visibility =
              "visible";
            setTimeout(function () {
              document
                .getElementById("editorMdl-titleSave")
                .style.removeProperty("visibility");
            }, 750);
            document.dispatchEvent(new CustomEvent("ajax"));
            document
              .querySelector("#editModal")
              .dispatchEvent(new CustomEvent("mdl-open"));
          };
          form.reset();
          form.querySelector(".dd-vsbl").value = "";
          form.querySelector("#DpTermAnnotation_Rank").value = "";
          if (form.querySelector(".CodeMirror")) {
            form.querySelector(".CodeMirror").CodeMirror.getDoc().setValue("");
          }
          // document.getElementsByClassName("new-term-summary")[0].innerHTML = "";
          // document.getElementsByClassName("new-term-def")[0].innerHTML = "";
        }
      });

    document
      .querySelector("#editModal")
      .addEventListener("submit", function (e) {
        var q,
          data,
          form,
          url,
          termsContainer = document.getElementById(
            "collection-editor-currentreports-container"
          );
        if (
          e.target.closest("#collection-editor-edit-report-form") ||
          e.target.closest("#collection-editor-add-report-form")
        ) {
          e.preventDefault();

          form =
            e.target.closest("#collection-editor-edit-report-form") ||
            e.target.closest("#collection-editor-add-report-form");
          url = form.getAttribute("action");

          q = new XMLHttpRequest();
          q.open(
            "post",
            form.getAttribute("action") + "&" + serialize(form),
            true
          );
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
              document.dispatchEvent(new CustomEvent("code-highlight"));
            }
            document.getElementById("editorMdl-titleSave").style.visibility =
              "visible";
            setTimeout(function () {
              document
                .getElementById("editorMdl-titleSave")
                .style.removeProperty("visibility");
            }, 750);
            document.dispatchEvent(new CustomEvent("ajax"));
            document
              .querySelector("#editModal")
              .dispatchEvent(new CustomEvent("mdl-open"));
          };
          form.reset();
          form.querySelector(".dd-vsbl").value = "";
          form.querySelector("#DpReportAnnotation_Rank").value = "";
          if (form.querySelector(".CodeMirror")) {
            form.querySelector(".CodeMirror").CodeMirror.getDoc().setValue("");
          }
          // document.getElementsByClassName("new-term-summary")[0].innerHTML = "";
          // document.getElementsByClassName("new-term-def")[0].innerHTML = "";
        }
      });
  }
})();
