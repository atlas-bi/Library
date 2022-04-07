// Dropdowns
(function () {
  function closeDropdowns() {
    (document.querySelectorAll('.dropdown:not(.is-hoverable)') || []).forEach(
      function ($element) {
        $element.classList.remove('is-active');
      },
    );
  }

  document.addEventListener('click', function (event) {
    if (!event.target.closest('.dropdown.is-active')) {
      closeDropdowns();
    }
  });

  // Close dropdowns if ESC pressed
  document.addEventListener('keydown', function (event) {
    event = event || window.event;
    if (Number(event.keyCode) === 27) {
      closeDropdowns();
    }
  });
  document.addEventListener('click', (event) => {
    if (event.target.closest('.dropdown:not(.is-hoverable)')) {
      const $element = event.target.closest('.dropdown:not(.is-hoverable)');
      event.stopPropagation();

      if ($element.classList.contains('is-active')) {
        $element.classList.remove('is-active');
      } else {
        $element.classList.add('is-active');
      }
    }
  });
})();
