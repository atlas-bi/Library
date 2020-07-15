function getUrlVars() {
  var vars = {};
  var parts = window.location.href.split("#")[0].replace(/[?&]+([^=&]+)=([^&]*)/gi, function (m, key, value) {
    vars[key] = value;
  });
  return vars;
}

var is_epic = getUrlVars().EPIC;

if (is_epic == 1 || getCookie('EPIC') == 1) {
  setCookie('EPIC', 1, 99);
}