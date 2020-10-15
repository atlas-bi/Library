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

function getUrlVars() {
      var vars = {};
      var parts = window.location.href.split("#")[0].replace(/[?&]+([^=&]+)=([^&]*)/gi, function (m, key, value) {
        vars[key] = value;
      });
      return vars;
    }

(function (root, factory) {
    if (typeof define === 'function' && define.amd) {
        // AMD. Register as an anonymous module.
        define(['b'], factory);
    } else {
        // Browser globals
        root.Hyperspace = factory(root.b);
    }
}(typeof self !== 'undefined' ? self : this, function (b) {

  var x = function(){
   

    var is_epic = getUrlVars().EPIC;

    if (is_epic == 1 || getCookie('EPIC') == 1) {
      setCookie('EPIC', 1, 99);
    };
  };
  console.log('hyperspace scripts loaded.');
  return x;
}));

Hyperspace();