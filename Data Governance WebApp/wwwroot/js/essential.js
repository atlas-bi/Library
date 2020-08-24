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
function textfill(el) {
  var el = el || document.getElementsByClassName('pageTitle-head')[0];

  if (el) {
    var s = el.getElementsByTagName('span')[0];

    if (s) {
      s.style.display = "inline-block";
      var styles = window.getComputedStyle(el);
      var padding = parseFloat(styles.paddingLeft) + parseFloat(styles.paddingRight);
      s.innerHTML = s.innerHTML.replace(/_/g, " ");
      var fontSize = parseFloat(window.getComputedStyle(el, null).getPropertyValue('font-size')),
          newSize = fontSize * ((el.clientWidth - padding - 100) / s.clientWidth - 0.05);
      s.style.fontSize = Math.min(newSize, fontSize) + 'px';
      s.style.removeProperty('display');

      if (newSize < fontSize) {
        s.style.marginTop = "auto";
      }
    }
  }
}
function titleTextfill() {
  var head = document.getElementsByClassName('resize-header');
  for(var x=0;x<head.length;x++){
    var el = head[x];

    if (el) {
      var s = el.getElementsByTagName('span')[0];

      if (s) {
        s.style.display = "inline-block";
        var styles = window.getComputedStyle(el);
        var padding = parseFloat(styles.paddingLeft) + parseFloat(styles.paddingRight);
        s.innerHTML = s.innerHTML.replace(/_/g, " ");
        var fontSize = parseFloat(window.getComputedStyle(el, null).getPropertyValue('font-size')),
            newSize = fontSize * ((el.clientWidth - padding ) / s.clientWidth - 0.05);
        s.style.fontSize = Math.min(newSize, fontSize) + 'px';
        s.style.removeProperty('display');

        if (newSize < fontSize) {
          s.style.marginTop = "auto";
        }
      }
    }
  }
}

textfill();
titleTextfill();
 
document.addEventListener('ajax-page', function () {
  textfill();
  titleTextfill();

});
window.addEventListener('resize', function () {
  textfill();
  titleTextfill();

});
/*
      $('body').on('change', '#MyRole_Id ~ input', function (e) {
              e.preventDefault();
              var form = $(this).parents('form');
              form.find('#MyRole_Url').val(window.location);
              form.submit();
      });
  */
// auto resize text to fit into area w/out overflow
// used for site banner when first accessed.

setCookie('alive', 'y');
window.addEventListener('scroll', function () {
  debounce(showScrollToTop(), 250);
}, {passive: true});

var showScrollToTop = function showScrollToTop() {
  if (window.pageYOffset > 50) {
    document.getElementById('back-to-top').style.visibility = 'visible';
  } else {
    document.getElementById('back-to-top').style.visibility = 'hidden';
  }
};

showScrollToTop();
document.addEventListener('click', function (e) {
  if (e.target.closest('#back-to-top')) {
    document.documentElement.scrollTop = document.body.scrollTop = 0;
    return false;
  }
});

function downloadJSAtOnload() {
  // get scripts

  var l = document.createElement('div'), el=document.getElementById('postLoadScripts');
  l.innerHTML = el.value;
  el.parentElement.removeChild(el);
  var scripts = l.querySelectorAll('script');

  for (var x = 0; x < scripts.length; x++) {
    var i = scripts[x],
        q = document.createElement("script");
    q.src = i.src;
    q.innerHTML = i.innerHTML;
    q.type = "text/javascript";
    document.body.appendChild(q);
  }
}

window.addEventListener("load", function () {
  downloadJSAtOnload();
}, false);
