(function () {
  var d = document,
    checkbox = function (el) {
      el.classList.add('loaded');
      var check = el.querySelector('#editModal input[type=checkbox]'),
        input = el.querySelector('#editModal input[type=hidden');

      if (check && input) {
        check.checked = input.value == 'Y' ? true : false;
        check.addEventListener('change', function (e) {
          input.value = check.checked ? 'Y' : 'N';
        });
      }
    },
    loadCheckbox = function () {
      var els = d.querySelectorAll('.form-check:not(.loaded)');
      for (var x = 0; x < els.length; x++) {
        checkbox(els[x]);
      }
    };

  loadCheckbox();
  d.addEventListener('ajax', function () {
    loadCheckbox();
  });
})();

(function () {
  var d = document,
    checkbox = function (el) {
      el.classList.add('loaded');
      var check = el.querySelector('input[type=checkbox]'),
        input = el.querySelector('input[type=hidden');

      if (check && input) {
        check.checked = input.value == 'Y' ? true : false;
        check.addEventListener('change', function (e) {
          input.value = check.checked ? 'Y' : 'N';
        });
      }
    },
    loadCheckbox = function () {
      var els = d.querySelectorAll('.toggle');
      for (var x = 0; x < els.length; x++) {
        checkbox(els[x]);
      }
    };

  loadCheckbox();
  d.addEventListener('ajax', function () {
    loadCheckbox();
  });
})();
