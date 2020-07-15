function textfill() {
  var el = document.getElementsByClassName('pageTitle-head')[0];

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

textfill();
document.addEventListener('ajax-page', function () {
  textfill();
});
window.addEventListener('resize', function () {
  textfill();
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