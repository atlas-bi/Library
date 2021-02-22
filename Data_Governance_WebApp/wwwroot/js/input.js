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
  var d = document,
    buildInputs = function buildInputs() {
      var inputs = d.querySelectorAll(
        'input[type="nice-input"], textarea[type="nice-input"]'
      );

      for (var x = 0; x < inputs.length; x++) {
        new build(inputs[x], x);
      }
    };

  function build(input, i) {
    // build inputs
    this.hiddenInput = d.createElement(input.tagName);
    this.hiddenInput.style.display = "none";
    this.hiddenInput.setAttribute("tabindex", -1);
    this.hiddenInput.setAttribute("id", input.getAttribute("id"));
    this.hiddenInput.setAttribute("name", input.getAttribute("name"));
    this.hiddenInput.setAttribute("inputId", i);
    this.hiddenInput.setAttribute("value", input.getAttribute("value"));
    this.hiddenInput.setAttribute("multiple", "true");
    this.span = d.createElement("span");
    this.span.setAttribute("class", input.getAttribute("class"));
    this.span.classList.add("input-span");
    this.span.setAttribute("contenteditable", "true");
    this.span.setAttribute(
      "data-initial-content",
      input.getAttribute("data-initial-content")
    );
    this.span.textContent = input.getAttribute("value");
    this.wrapper = d.createElement("div");
    this.wrapper.classList.add("input-newWrapper");
    this.wrapper.appendChild(this.span);
    this.wrapper.appendChild(this.hiddenInput);

    if (input.classList.contains("submit")) {
      this.button = d.createElement("button");
      this.button.classList.add("input-submit");
      this.button.setAttribute("type", "submit");
      this.button.setAttribute("name", "submit");
      this.button.innerHTML = "<span class='fas fa-paper-plane'></span>";
      this.wrapper.appendChild(this.button);
    }

    this.container = d.createElement("div");
    this.container.classList.add("input-ctn");

    if (input.classList.contains("slim")) {
      this.container.classList.add("slim");
    }

    this.container.appendChild(this.wrapper);
    input.parentNode.replaceChild(this.container, input);
    this.span.addEventListener("focus", this.addFocus.bind(this), false);
    this.span.addEventListener("blur", this.addBlur.bind(this), false);
    this.span.addEventListener("input", this.updateInput.bind(this), false);
    this.span.addEventListener("keypress", this.updateInput.bind(this), false);
    this.span.addEventListener("paste", this.updateInput.bind(this), false);
    this.span.addEventListener("change", this.updateInput.bind(this), false);
  }

  var k =
    d.requestAnimationFrame ||
    d.setImmediate ||
    function (b) {
      return setTimeout(b, 0);
    };

  build.prototype = {
    addFocus: function addFocus(b) {
      var w = this.wrapper;
      k(function () {
        w.classList.add("input-newWrapper-focus");
      });
    },
    addBlur: function addBlur(b) {
      var w = this.wrapper,
        s = this.span;
      k(function () {
        w.classList.remove("input-newWrapper-focus");
        s.innerHTML = s.innerHTML;
      });
    },
    updateInput: function updateInput(b) {
      var s = this.span,
        i = this.hiddenInput;
      k(function () {
        i.value = s.innerHTML;
      });
    },
  };
  buildInputs();
  d.addEventListener("ajax", function () {
    buildInputs();
  });
  d.addEventListener("build-inputs", function () {
    buildInputs();
  });
})();
