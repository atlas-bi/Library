(function () {
  const d = document;
  const checkbox = function (element) {
    element.classList.add('loaded');
    const check = element.querySelector('input[type=checkbox]');
    const input = element.querySelector('input[type=hidden');

    if (check && input) {
      check.checked = input.value === 'Y';
      check.addEventListener('change', function () {
        input.value = check.checked ? 'Y' : 'N';
        input.dispatchEvent(new CustomEvent('change'));
      });
    }
  };

  const loadCheckbox = function () {
    const els = d.querySelectorAll('.toggle');
    for (let x = 0; x < els.length; x++) {
      checkbox(els[x]);
    }
  };

  loadCheckbox();
  d.addEventListener('ajax', function () {
    loadCheckbox();
  });
})();
