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
        root.Page = factory(root.b);
    }
}(typeof self !== 'undefined' ? self : this, function (b) {

  var x = function(){
    window.addEventListener('scroll', function() {
        debounce(function() {
            scrollHead();
        }(), 100);
    }, { passive: true });

    window.addEventListener('resize', function() {
        debounce(function() {
            scrollHead();
        }(), 100);
    });
    
    var title = document.querySelector('.pageTitle:not(.loose)');
    if(title){
        l = document.getElementsByClassName('location');
        for(x=0;x<l.length;x++){
            l[x].style.top = '-' + title.clientHeight + 'px';
        }
    }

    function scrollHead() {
        var title = document.querySelector('.pageTitle:not(.loose)');
        if (title) {
            var w = title.clientWidth;

            if (title.getBoundingClientRect().top > 0 || window.pageYOffset < title.clientHeight) {
                title.classList.remove('pageTitle--sticky');
                title.style.removeProperty('width');

                title.nextElementSibling.style.removeProperty('padding-top');
            } else {
                title.classList.add('pageTitle--sticky');
                title.style.width = w + 'px';
             //   title.nextElementSibling.style.paddingTop = title.clientHeight + 'px';   
            }
        }
    }
    scrollHead();
};
console.log('page scripts loaded');
return x;
}));
Page();