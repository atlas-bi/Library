(function () {
  (document.querySelectorAll('.navbar-burger') || []).forEach(function (
    element,
  ) {
    element.addEventListener('click', function () {
      const target = element.dataset.target;
      const $target = document.querySelector(`#${target}`);

      element.classList.toggle('is-active');
      $target.classList.toggle('is-active');
    });
  });
})();
