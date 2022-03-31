(function () {
  if (getUrlVars().EPIC === 1 || getCookie('EPIC') === 1) {
    setCookie('EPIC', 1, 99);
  }
})();
