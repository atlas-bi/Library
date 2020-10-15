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
        root.Video = factory(root.b);
    }
}(typeof self !== 'undefined' ? self : this, function (b) {

  var x = function(){
    var d = document,
        q;
    d.addEventListener('click', function (e) {
      var t = e.target,
          v = d.getElementsByTagName("video")[0],
          w = d.getElementsByClassName('video')[0];

      function on() {
        var s = w.getElementsByTagName('source')[0];

        if (s.hasAttribute('data-src')) {
          s.setAttribute('src', s.getAttribute('data-src'));
          s.removeAttribute('data-src');
          v.appendChild(s);
        }

        v.play();
        q = new XMLHttpRequest();
        q.open('post', '/Users?handler=WelcomeToAtlasState&State=1', true);
        q.setRequestHeader('Content-Type', 'text/html;charset=UTF-8`');
        q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
        q.send();
      }

      function off() {
        w.classList.remove('video-large');
        w.classList.add('video-closed');
        v.pause();
        q = new XMLHttpRequest();
        q.open('post', '/Users?handler=WelcomeToAtlasState&State=0', true);
        q.setRequestHeader('Content-Type', 'text/html;charset=UTF-8`');
        q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
        q.send();
      }

      if (t.closest('.video-open')) {
        w.classList.remove('video-closed');
        w.classList.add('video-large');
        on();
      } else if (t.closest('.video-min')) {
        w.classList.remove('video-large');
        w.classList.remove('video-closed');
        on();
      } else if (t.closest('.video-close')) {
        off();
      } else if (t.closest('.video')) {
        w.classList.remove('video-closed');
        on();
      } else {
        if (w) {
          w.classList.remove('video-large');
        }
      }
    });
  };
  console.log('video script loaded')
  return x;

}));

Video();