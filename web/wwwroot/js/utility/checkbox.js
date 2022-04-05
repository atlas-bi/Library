(function () {
  const d = document;
  const checkbox = function (element) {
    element.classList.add('loaded');
    const check = element.querySelector('#editModal input[type=checkbox]');
    const input = element.querySelector('#editModal input[type=hidden');

    if (check && input) {
      check.checked = input.value === 'Y';
      check.addEventListener('change', function () {
        input.value = check.checked ? 'Y' : 'N';
      });
    }
  };

  const loadCheckbox = function () {
    const els = d.querySelectorAll('.form-check:not(.loaded)');
    for (let x = 0; x < els.length; x++) {
      checkbox(els[x]);
    }
  };

  loadCheckbox();
  d.addEventListener('ajax', function () {
    loadCheckbox();
  });
})();

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
