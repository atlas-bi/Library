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
  var load = debounce(function () {
    [].forEach.call(document.querySelectorAll('img[data-src]'), function (img) {
      if (isInViewport(img)) {
        img.setAttribute('src', img.getAttribute('data-src'));
        img.removeAttribute('data-src');
      }
    });
  }, 250);

  var isInViewport = function isInViewport(elem) {
    var bounding = elem.getBoundingClientRect(),
        padding = 400;
    return bounding.top >= 0 && bounding.left >= 0 && bounding.bottom - elem.clientHeight - padding <= (window.innerHeight || document.documentElement.clientHeight) && bounding.right - padding - elem.clientWidth <= (window.innerWidth || document.documentElement.clientWidth);
  };

  load();
  document.addEventListener('ajax', function () {
    setTimeout(function () {
      load();
    }, 0);
  });
  document.addEventListener('scroll', function () {
    setTimeout(function () {
      load();
    }, 0);
  }, {passive: true});
})();