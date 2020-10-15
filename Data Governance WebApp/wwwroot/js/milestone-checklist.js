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
(function (root, factory) {
    if (typeof define === 'function' && define.amd) {
        // AMD. Register as an anonymous module.
        define(['b'], factory);
    } else {
        // Browser globals
        root.Milestone = factory(root.b);
    }
}(typeof self !== 'undefined' ? self : this, function (b) {

  var x = function(){
    var d = document;

    function load() {
      // populate select lists intially
      [].forEach.call(d.getElementsByClassName('checkbox-list'), function (el) {
        var items = el.getElementsByClassName('checkbox-container'),
            select = el.getElementsByTagName('select')[0];

        if (!select.children.length) {
          [].forEach.call(items, function (e) {
            completed = e.getElementsByClassName('completed').length > 0 ? 'selected="selected"' : '';
            select.innerHTML += '<option value="' + e.getAttribute('check-id') + '" ' + completed + '></option';
          });
        }
      });
    }

    d.addEventListener('click', function (e) {
      if (e.target.matches('input.milestone[type="checkbox"]') && e.target.tagName == 'INPUT') {
        var p = e.target.parentElement,
            i = e.target,
            type = 1,
            l = i.closest('label'),
            q,
            url,
            data,
            f = i.closest('form'),
            option;

        if (i.getAttribute("checked") == "checked") {
          i.removeAttribute('checked');
          type = 2;
        } else {
          i.setAttribute('checked', 'checked');
        }

        option = f.querySelector('select option[value="' + l.getAttribute('check-id') + '"]');

        if (option) {
          if (option.hasAttribute("selected")) {
            option.removeAttribute("selected");
          } else {
            option.setAttribute("selected", "selected");
          }

          url = serialize(f);
          q = new XMLHttpRequest();
          q.open('post', f.getAttribute('action') + '&' + url, true);
          q.setRequestHeader('Content-Type', 'text/html;charset=UTF-8`');
          q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
          q.send();

          q.onreadystatechange = function (e) {
            if (this.readyState == 4 && this.status == 200) {
              ShowMessageBox("Changes saved.");
            }
          };
        }
      }
    });
    load();
    document.addEventListener('ajax', function (n) {
      load();
    });
  };
  console.log('milestone scripts loaded.');
  return x;
}));
Milestone();