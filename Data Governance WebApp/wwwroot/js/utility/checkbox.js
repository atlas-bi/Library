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

(function() {
    var d = document,
        checkbox = function(el) {
            el.classList.add('loaded');
            var check = el.querySelector('#editModal input[type=checkbox]'),
                input = el.querySelector('#editModal input[type=hidden');

            if (check && input) {
                check.checked = input.value == "Y" ? true : false;
                check.addEventListener('change', function(e) {
                    input.value = check.checked ? "Y" : "N";
                });
            }
        },

        loadCheckbox = function() {
            var els = d.querySelectorAll('.form-check:not(.loaded)');
            for (var x = 0; x < els.length; x++) {
                checkbox(els[x]);
            }
        };

    loadCheckbox();
    d.addEventListener("ajax", function() {
        loadCheckbox();
    });

})();
