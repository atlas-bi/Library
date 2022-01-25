(function () {
  // Functions to open and close a mini
  function openMini($el) {
    $el.classList.add('is-active');
  }

  function closeMini($el) {
    $el.classList.remove('is-active');
  }

  function closeAllMinis() {
    (document.querySelectorAll('.mini') || []).forEach(($mini) => {
      closeMini($mini);
    });
  }

  function load($mini, $data, $hidden, $input, $preload) {
    var data = JSON.parse($data);
    if (data.length === 0) {
      $mini.innerHTML = '<div class="mini-waiting">No matches found.</div>';
    } else {
      $mini.innerText = '';
      var result = document.createElement('div');
      for (var el of data) {
        var hiddenClass = '';
        // if (active.indexOf(el.ObjectId) !== -1) {
        //   hiddenClass = 'hidden';
        // }
        var a = document.createElement('a');
        a.classList.add('mini-item');
        a.innerText = el.Name;
        a.setAttribute('value', el.ObjectId || el.Description);

        a.addEventListener('click', function (event) {
          if ($hidden.tagName == 'INPUT') {
            $input.value = event.target.innerText;
            $hidden.value = event.target.getAttribute('value');
            closeAllMinis();
          } else if ($hidden.tagName == 'SELECT') {
            // add it to the select
            var opt = document.createElement('option');
            opt.setAttribute('selected', 'selected');
            opt.value = event.target.getAttribute('value');
            $hidden.appendChild(opt);

            // add it to the tag list
            var control = document.createElement('div');
            control.classList.add('control');

            var group = document.createElement('div');
            group.classList.add('tags', 'has-addons');

            var tag = document.createElement('span');
            tag.classList.add('tag', 'is-link');
            tag.innerText = event.target.innerText;

            var del = document.createElement('a');
            del.classList.add('tag', 'is-delete');
            del.setAttribute('value', event.target.getAttribute('value'));

            del.addEventListener('click', function (event) {
              var $this = event.target;
              console.log(
                $hidden.querySelector(
                  'option[value="' + $this.getAttribute('value') + '"]',
                ),
              );
              $hidden.removeChild(
                $hidden.querySelector(
                  'option[value="' + $this.getAttribute('value') + '"]',
                ),
              );
              control.parentElement.removeChild(control);
            });

            control.appendChild(group);
            group.appendChild(tag);
            group.appendChild(del);

            var taglist =
              $input.parentNode.parentNode.parentNode.querySelector(
                '.mini-tags',
              );
            if (taglist != undefined) {
              taglist.appendChild(control);
            }

            $input.value = '';
          }
        });

        $mini.appendChild(a);
      }

      if ($mini.querySelectorAll('.mini-item:not(.hidden)').length < 1) {
        $mini.innerHTML += '<div class="mini-waiting">No matches found.</div>';
      }
    }
  }
  var loadMiniAjax = null;
  function loadMini($input, $mini, $sa, $hidden) {
    const value = $input.value;

    // run query if txt
    if ($input.value.length > 0) {
      if (loadMiniAjax !== null) {
        loadMiniAjax.abort();
      }

      loadMiniAjax = new XMLHttpRequest();
      loadMiniAjax.open('post', '/Search?handler=' + $sa + '&s=' + value, true);
      loadMiniAjax.setRequestHeader(
        'Content-Type',
        'application/x-www-form-urlencoded; charset=UTF-8',
      );
      loadMiniAjax.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
      loadMiniAjax.send();

      loadMiniAjax.onload = function () {
        var $data = loadMiniAjax.responseText;
        load($mini, $data, $hidden, $input);
      };
    }
    // else reset it to loader
    else {
      $mini.innerHTML = `<div class="mini-waiting">
                    <span class="icon">
                        <i class="fas fa-circle-notch fa-spin"></i>
                    </span>
                </div>`;
    }
  }

  function preloadMini($input, $mini, $searchArea, $hidden) {
    var q = new XMLHttpRequest();
    q.open('post', '/Search?handler=ValueList&s=' + $searchArea, true);
    q.setRequestHeader(
      'Content-Type',
      'application/x-www-form-urlencoded; charset=UTF-8',
    );
    q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
    q.send();

    q.onload = function () {
      var l = q.responseText;
      load($mini, l, $hidden, $input);
    };
  }

  var searchTimeout = 250,
    searchTimerId = null;

  // Add a click event on buttons to open a specific mini
  (document.querySelectorAll('.input-mini') || []).forEach(($input) => {
    // open it when clicking
    const $mini = $input.parentElement.querySelector('.mini');
    const $hidden = $input.parentElement.querySelector(
      'input[type="hidden"], select.is-hidden',
    );
    const $clear =
      $input.parentElement.parentElement.querySelector('.mini-clear');

    $input.addEventListener('click', () => {
      closeAllMinis();
      openMini($mini);
    });

    // open when typing in the related input
    if ($input.hasAttribute('search-area')) {
      $input.addEventListener('input', () => {
        window.clearTimeout(searchTimerId);
        searchTimerId = window.setTimeout(function () {
          loadMini($input, $mini, $input.getAttribute('search-area'), $hidden);
          window.clearTimeout(searchTimerId);
        }, searchTimeout);
      });
    } else if ($input.hasAttribute('lookup-area')) {
      preloadMini($input, $mini, $input.getAttribute('lookup-area'), $hidden);
    }

    if ($clear != undefined) {
      $clear.addEventListener('click', function () {
        $hidden.value = '';
        $input.value = '';
      });
    }
  });

  // Add a click event on various child elements to close the parent mini
  (
    document.querySelectorAll(
      '.mini-background, .mini-close, .mini-card-head .delete, .mini-card-foot .button',
    ) || []
  ).forEach(($close) => {
    const $target = $close.closest('.mini');

    $close.addEventListener('click', () => {
      closeMini($target);
    });
  });

  window.onclick = function (event) {
    if (!event.target.closest('.mini,.input-mini')) {
      closeAllMinis();
    }
  };

  // Add a keyboard event to close all minis
  document.addEventListener('keydown', (event) => {
    const e = event || window.event;

    if (e.keyCode === 27) {
      // Escape key
      closeAllMinis();
    }
  });

  // add event to remove mini tags that were preloaded.
  (document.querySelectorAll('.mini-tags a.is-delete[value]') || []).forEach(
    ($delete) => {
      $delete.addEventListener('click', function (e) {
        var $hidden =
          $delete.parentNode.parentNode.parentNode.parentNode.querySelector(
            'select.is-hidden',
          );
        var $control = $delete.parentNode.parentNode;
        $hidden.removeChild(
          $hidden.querySelector(
            'option[value="' + $delete.getAttribute('value') + '"]',
          ),
        );
        $control.parentNode.removeChild($control);
      });
    },
  );
})();
