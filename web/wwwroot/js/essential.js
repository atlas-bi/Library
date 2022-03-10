(function () {
  document.addEventListener('change', function (e) {
    if (e.target.closest('#change-role')) {
      e.target.closest('form').querySelector('#MyRole_Url').value =
        window.location.href;
      e.target.closest('form').submit();
    }
  });

  function showScrollToTop() {
    if (window.pageYOffset > 50) {
      document.getElementById('back-to-top').style.visibility = 'visible';
    } else {
      document.getElementById('back-to-top').style.visibility = 'hidden';
    }
  }

  showScrollToTop();
  document.addEventListener('click', function (e) {
    if (e.target.closest('#back-to-top')) {
      document.documentElement.scrollTop = document.body.scrollTop = 0;
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
