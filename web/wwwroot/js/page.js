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
  window.addEventListener(
    "scroll",
    function () {
      debounce(
        (function () {
          scrollHead();
        })(),
        100
      );
    },
    {
      passive: true,
    }
  );

  window.addEventListener("resize", function () {
    debounce(
      (function () {
        scrollHead();
      })(),
      100
    );
  });

  var title = document.querySelector(".pageTitle:not(.loose)"),
    controls = document.querySelector(".sideNav"),
    sibling,
    sticky,
    siblingPadding;

  document.addEventListener("click", function (e) {
    if (e.target.closest(".site-messageClose")) {
      document
        .querySelector(".site-message")
        .parentElement.removeChild(document.querySelector(".site-message"));
      if (title) {
        sticky = getOffset(title).top;
      }
    }
  });

  if (title) {
    l = document.getElementsByClassName("location");
    for (x = 0; x < l.length; x++) {
      l[x].style.top = "-" + title.clientHeight + "px";
    }

    sticky = getOffset(title).top;
    sibling = title.nextElementSibling;
    siblingPadding = parseInt(
      window.getComputedStyle(sibling, null).getPropertyValue("padding-top"),
      10
    );
  }

  var scrollHead = function () {
    // cant add the extra margin if the page height - window height < new padding or page will jump.
    var body = document.body,
      html = document.documentElement;

    var pageHeight = Math.max(
      body.scrollHeight,
      body.offsetHeight,
      html.clientHeight,
      html.scrollHeight,
      html.offsetHeight
    );

    if (
      title &&
      (pageHeight - window.innerHeight > sticky * 2 ||
        pageHeight - window.innerHeight <= sticky)
    ) {
      var w = title.clientWidth;
      if (window.pageYOffset >= sticky) {
        // only add padding if first time adding class
        if (!title.classList.contains("sticky")) {
          sibling.style.paddingTop = siblingPadding + title.clientHeight + "px";
        }
        title.classList.add("sticky");
        title.style.width = w + "px";
      } else {
        title.classList.remove("sticky");
        title.style.removeProperty("width");
        sibling.style.removeProperty("padding-top");
      }
    }
    if (controls) {
      if (window.pageYOffset >= sticky) {
        var l = controls.getBoundingClientRect().left;
        controls.classList.add("sticky");
        controls.style.left = l + "px";
      } else {
        controls.classList.remove("sticky");
        controls.style.removeProperty("left");
      }
    }
  };
  scrollHead();

  // related reports
  function relatedReports() {
    var links = document.querySelectorAll(".body-main a[href]"),
      ids = [],
      re = /[R|r]eports\?id=(\d+)/;

    for (var x = 0; x < links.length; x++) {
      var m = links[x].getAttribute("href").match(re);
      if (m) {
        ids.push(m[1]);
      }
    }

    if (ids.length > 0) {
      var div = document.getElementById("related-reports");
      if (!div) {
        div = document.createElement("div");
        div.setAttribute("id", "related-reports");
        document.getElementById("AdColTwo").appendChild(div);
      }
      div.setAttribute(
        "data-url",
        "/Reports/?handler=RelatedReports&id=" + ids.slice(0, 5).join(",")
      );
      div.setAttribute("data-ajax", "yes");

      document.dispatchEvent(new CustomEvent("load-ajax-content"));
    }
  }
  relatedReports();

  document.addEventListener("related-reports", function () {
    relatedReports();
  });
})();
