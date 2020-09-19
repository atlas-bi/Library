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
  /*
  [data-toggle="clps"][data-target="#el"]
  #el
  */
  document.addEventListener('click', function (e) {
    var el; // collapse

    if (e.target.getAttribute('data-toggle') == 'clps') {
      var d = document;
      el = d.getElementById(e.target.getAttribute('data-target').replace("#", ''));

      if (el == null) {
        return;
      }

      if (el.style.maxHeight || el.classList.contains('clps-o')) {
        c(el);
      } else {
        o(el);
      }
    } else if (e.target.closest('.clps:not(.clps-o)')){
      el = e.target.closest('.clps:not(.clps-o)');
      if (el.style.maxHeight || el.classList.contains('clps-o')) {
        c(el);
      } else {
        o(el);
      }
    }
  });

  function c(el) {
    el.style.maxHeight = null;
    el.classList.remove('clps-o');
  }

  function o(el) {
    el.style.maxHeight = el.scrollHeight + 'px';
    el.classList.add('clps-o');
    document.dispatchEvent(new CustomEvent("collapse-opened"));
  }

  document.addEventListener('clps-close', function (e) {
    if (typeof e.detail !== 'undefined') {
      c(e.detail.el);
    }
  }, false);
  document.addEventListener('clps-open', function (e) {
    if (typeof e.detail !== 'undefined') {
      o(e.detail.el);
    }
  }, false);
})();