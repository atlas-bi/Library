(function () {
  var enableAdmin = document.querySelector('input#enable-administrator');
  if (enableAdmin !== null) {
    enableAdmin.addEventListener('change', function (event) {
      window.location = `/Users?handler=ChangeRole&Id=${
        event.target.value
      }&Url=${window.location.href.replace(window.location.origin, '')}`;
    });
  }

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
