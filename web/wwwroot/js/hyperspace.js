function getUrlVars() {
  var vars = {};
  var parts = window.location.href
    .split('#')[0]
    .replace(/[?&]+([^=&]+)=([^&]*)/gi, function (m, key, value) {
      vars[key] = value;
    });
  return vars;
}

(function () {
  if (getUrlVars().EPIC == 1 || getCookie('EPIC') == 1) {
    setCookie('EPIC', 1, 99);
  }
})();
