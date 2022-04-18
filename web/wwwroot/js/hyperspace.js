(function () {
  if (getUrlVars().EPIC === 1 || Number(getCookie('EPIC')) === 1) {
    setCookie('EPIC', 1, 99);
  }
})();
