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

var serialize = function serialize(form) {
  // Setup our serialized data
  var serialized = []; // Loop through each field in the form

  for (var i = 0; i < form.elements.length; i++) {
    var field = form.elements[i]; // Don't serialize fields without a name, submits, buttons, file and reset inputs, and disabled fields

    if (
      !field.name ||
      field.disabled ||
      field.type === "file" ||
      field.type === "reset" ||
      field.type === "submit" ||
      field.type === "button"
    )
      continue; // If a multi-select, get all selections

    if (field.type === "select-multiple") {
      for (var n = 0; n < field.options.length; n++) {
        if (!field.options[n].selected) continue;
        serialized.push(
          encodeURIComponent(field.name) +
            "=" +
            encodeURIComponent(field.options[n].value)
        );
      }
    } // Convert field data to a query string
    else if (
      (field.type !== "checkbox" && field.type !== "radio") ||
      field.checked
    ) {
      serialized.push(
        encodeURIComponent(field.name) + "=" + encodeURIComponent(field.value)
      );
    }
  }

  return serialized.join("&");
};

var debounce = function debounce(func, wait, immediate) {
  var timeout;
  return function () {
    var context = this,
      args = arguments;

    var later = function later() {
      timeout = null;
      if (!immediate) func.apply(context, args);
    };

    var callNow = immediate && !timeout;
    clearTimeout(timeout);
    timeout = setTimeout(later, wait);
    if (callNow) func.apply(context, args);
  };
};

function setCookie(name, value, days) {
  var expires = "",
    date = new Date();

  days = days || 1;
  date.setTime(date.getTime() + days * 24 * 60 * 60 * 1000);
  expires = "; expires=" + date.toUTCString();

  document.cookie = name + "=" + (value || "") + expires + "; path=/";
}

function getCookie(name) {
  var nameEQ = name + "=";
  var ca = document.cookie.split(";");

  for (var i = 0; i < ca.length; i++) {
    var c = ca[i];

    while (c.charAt(0) == " ") {
      c = c.substring(1, c.length);
    }

    if (c.indexOf(nameEQ) === 0) return c.substring(nameEQ.length, c.length);
  }

  return null;
}

function getOffset(element) {
  if (!element.getClientRects().length) {
    return {
      top: 0,
      left: 0,
    };
  }

  var rect = element.getBoundingClientRect();
  var win = element.ownerDocument.defaultView;
  return {
    top: rect.top + win.pageYOffset,
    left: rect.left + win.pageXOffset,
  };
}

function getUrlVars(url) {
  url = url || window.location.href;
  var vars = {},
    parts = url.replace(/[?&]+([^=&]+)=([^&]*)/gi, function (m, key, value) {
      vars[key] = value;
    });
  return vars;
}

var cache = {
  timeout: 30 * 60,
  // mins
  data: {},
  remove: function remove(url) {
    try {
      sessionStorage.removeItem(btoa(url));
    } catch (e) {}
  },
  exists: function exists(url) {
    try {
      return (
        !!sessionStorage.getItem(btoa(url)) &&
        JSON.parse(sessionStorage.getItem(btoa(url)))._ > new Date().getTime()
      );
    } catch (e) {
      return false;
    }
  },
  get: function get(url) {
    return JSON.parse(sessionStorage.getItem(btoa(url))).data;
  },
  set: function set(url, data, t) {
    try {
      sessionStorage.removeItem(btoa(url));
      sessionStorage.setItem(
        btoa(url),
        JSON.stringify({
          data: data,
          _: new Date().getTime() + (t || cache.timeout) * 1000,
        })
      );
    } catch (e) {}
  },
};

(function () {
  document.addEventListener("change", function (e) {
    if (e.target.closest("#change-role")) {
      e.target.closest("form").querySelector("#MyRole_Url").value =
        window.location.href;
      e.target.closest("form").submit();
    }
  });

  try {
    new Function("async () => {}")();
  } catch (error) {
    // import polyfill for IE 11
    loadScripts(document.getElementsByClassName("polyfillScripts"));
  }

  function loadScripts(els) {
    els = els ? Array.prototype.slice.call(els) : [];

    for (var x = 0; x < els.length; x++) {
      var el = els[x];

      var l = document.createElement("div");

      l.innerHTML = el.value;

      el.parentElement.removeChild(el);

      var scripts = l.querySelectorAll("script");

      for (var y = 0; y < scripts.length; y++) {
        var i = scripts[y],
          q = document.createElement("script");
        q.src = i.src;
        q.innerHTML = i.innerHTML;
        q.type = "text/javascript";
        document.body.appendChild(q);
      }
    }
  }

  function showScrollToTop() {
    if (window.pageYOffset > 50) {
      document.getElementById("back-to-top").style.visibility = "visible";
    } else {
      document.getElementById("back-to-top").style.visibility = "hidden";
    }
  }

  showScrollToTop();
  document.addEventListener("click", function (e) {
    if (e.target.closest("#back-to-top")) {
      document.documentElement.scrollTop = document.body.scrollTop = 0;
      return false;
    }
  });

  function downloadJSAtOnload() {
    loadScripts(
      Array.prototype.slice.call(
        document.getElementsByClassName("postLoadScripts")
      )
    );
    if (!!document.querySelector(".tab-lnk"))
      loadScripts(document.getElementsByClassName("tabScripts"));
    if (!!document.querySelector("table.sort"))
      loadScripts(document.getElementsByClassName("tableScripts"));
    if (!!document.querySelector('[data-toggle="clps"]'))
      loadScripts(document.getElementsByClassName("collapseScripts"));
    if (!!document.querySelector(".crsl"))
      loadScripts(document.getElementsByClassName("carouselScripts"));
    if (!!document.querySelector(".drg"))
      loadScripts(document.getElementsByClassName("dragScripts"));
    if (!!document.querySelector(".atlas-chart"))
      loadScripts(document.getElementsByClassName("chartScripts"));
    if (
      !!document.querySelector(
        'input[type="nice-input"], textarea[type="nice-input"]'
      )
    )
      loadScripts(document.getElementsByClassName("inputScripts"));
    if (!!document.querySelector(".comments-form"))
      loadScripts(document.getElementsByClassName("commentsScripts"));
    if (!!document.querySelector('[type="dynamic-dropdown"]'))
      loadScripts(document.getElementsByClassName("dropdownScripts"));
  }

  window.addEventListener(
    "load",
    function () {
      downloadJSAtOnload();
    },
    false
  );

  document.addEventListener("ajax", function () {
    downloadJSAtOnload();
  });

  document.addEventListener("tab-opened", function () {
    debounce(downloadJSAtOnload(), 250);
  });

  document.addEventListener(
    "scroll",
    function () {
      debounce(downloadJSAtOnload(), 250);
      debounce(showScrollToTop(), 250);
    },
    {
      passive: true,
    }
  );
})();
