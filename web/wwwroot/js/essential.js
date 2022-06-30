(function () {
  document.addEventListener('change', function (event) {
    if (event.target.closest('#change-role')) {
      event.target.closest('form').querySelector('#MyRole_Url').value =
        window.location.href;
      event.target.closest('form').submit();
    }
  });

  function showScrollToTop() {
    if (window.pageYOffset > 50) {
      document.querySelector('#back-to-top').style.visibility = 'visible';
    } else {
      document.querySelector('#back-to-top').style.visibility = 'hidden';
    }
  }

  showScrollToTop();
  document.addEventListener('click', function (event) {
    if (event.target.closest('#back-to-top')) {
      document.documentElement.scrollTop = 0;
      document.body.scrollTop = 0;
      return false;
    }
  });

  document.addEventListener(
    'scroll',
    function () {
      debounce(showScrollToTop(), 250);
    },
    {
      passive: true,
    },
  );
})();
