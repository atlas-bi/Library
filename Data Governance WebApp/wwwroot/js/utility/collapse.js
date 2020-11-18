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
    /*
    [data-toggle="clps"][data-target="#el"]
    #el
    */
    var d = document;
    d.addEventListener('click', function(e) {
        var el;

        if (e.target.closest('[data-toggle="clps"]')) {
            el = d.getElementById(e.target.closest('[data-toggle]').getAttribute('data-target').replace("#", ''));

            if (el == null) {
                return;
            }

            if (el.style.maxHeight || el.classList.contains('clps-o')) {
                c(el);
            } else {
                o(el);
            }
        }
    });

    function h(el) {
        el.style.maxHeight = (el.scrollHeight + 20) + 'px';
    }

    function c(el) {
        el.style.maxHeight = null;
        el.style.overflow = 'hidden';
        el.classList.remove('clps-o');

    }

    function o(el) {
        el.classList.add('clps-o');
        h(el);
        var l = el;
        while (l = l.parentElement.closest('.clps-o')) {
            l.style.removeProperty('max-height');
        }
        d.dispatchEvent(new CustomEvent("collapse-opened"));

        // close siblings
        var o = el.parentElement.querySelector('.clps-o'),
            r = [];

        while (o) {
            if (o !== el && o.nodeType === Node.ELEMENT_NODE)
                r.push(o);
            o = o.nextElementSibling || o.nextSibling;
        }

        for (var x = 0; x < r.length; x++) c(r[x]);

        // after animation finished add max-height back
        window.setTimeout(function() {
            var l = el.parentElement.closest('.clps-o');
            while (l) {
                h(l);
                l = l.parentElement.closest('.clps-o');
            }
            if (el.classList.contains('clps-o')) {
                el.style.overflow = 'visible';
            }
        }, 300);
    }

    d.addEventListener('change', function(e) {
        if (e.target.closest('.clps-o')) {
            var l = e.target.closest('.clps-o');
            while (l) {
                l.style.removeProperty('max-height');
                l = l.parentElement.closest('.clps-o');
            }
            // after animation finished add max-height back
            window.setTimeout(function() {
                var l = e.target.closest('.clps-o');
                while (l) {
                    h(l);
                    l = l.parentElement.closest('.clps-o');
                }
            }, 300);
        }
    });

    d.addEventListener('clps-close', function(e) {
        if (typeof e.detail !== 'undefined') {
            c(e.detail.el);
        }
    }, false);
    d.addEventListener('clps-open', function(e) {
        if (typeof e.detail !== 'undefined') {
            o(e.detail.el);
        }
    }, false);
})();