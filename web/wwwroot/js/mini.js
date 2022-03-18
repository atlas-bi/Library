// Functions to open and close a mini
(function () {
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

  function updateId(taglist) {
    var $hiddenInputs = taglist.querySelectorAll(
      'input[type="hidden"][name]:not(.drag)',
    );
    for (var x = 0; x < $hiddenInputs.length; x++) {
      $hiddenInputs[x].setAttribute(
        'name',
        $hiddenInputs[x]
          .getAttribute('name')
          .replace(/\[\d*?\]/, '[' + x + ']'),
      );
    }
  }

  function load($mini, $data, $hidden, $input) {
    var data = JSON.parse($data);
    if (data.length === 0) {
      $mini.innerHTML = '<div class="mini-waiting">No matches found.</div>';
    } else {
      $mini.innerText = '';

      for (var el of data) {
        //var hiddenClass = '';
        // if (active.indexOf(el.ObjectId) !== -1) {
        //   hiddenClass = 'hidden';
        // }
        var a = document.createElement('a');
        a.classList.add('mini-item');
        a.innerText = el.Name;
        a.setAttribute('value', el.ObjectId || el.Description);

        a.addEventListener('click', function (event) {
          if (!$input.classList.contains('multiselect')) {
            $input.value = event.target.innerText;
            $hidden.value = event.target.getAttribute('value');
            closeAllMinis();
          } else if ($input.classList.contains('multiselect')) {
            var taglist = $input
              .closest('.field')
              .parentNode.querySelector('.mini-tags');
            // add it to the tag list
            var control = document.createElement('div');
            control.classList.add('control');

            var input = document.createElement('input');
            input.setAttribute('type', 'hidden');
            input.setAttribute('value', event.target.getAttribute('value'));
            if ($input.hasAttribute('data-name')) {
              input.setAttribute('name', $input.getAttribute('data-name'));
            }
            var group = document.createElement('div');
            group.classList.add('tags', 'has-addons');

            var tag = document.createElement('span');
            tag.classList.add('tag', 'is-link');
            tag.innerText = event.target.innerText;

            var del = document.createElement('a');
            del.classList.add('tag', 'is-delete');

            del.addEventListener('click', function () {
              control.parentElement.removeChild(control);
              updateId(taglist);
            });

            if (taglist.classList.contains('reorder')) {
              control.classList.add('drg');
              tag.classList.add('drg-hdl');
            }

            control.appendChild(group);
            group.appendChild(tag);
            group.appendChild(input);
            group.appendChild(del);

            if (taglist != undefined) {
              taglist.appendChild(control);
              // update index for c#
              updateId(taglist);
            }

            $input.value = '';

            // if it is a static list then clear the filter
            if ($input.hasAttribute('lookup-area')) {
              inputFilter($input, $mini);
            }

            if ($input.classList.contains('mini-close-fast')) {
              closeAllMinis();
            }
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

  function inputFilter($input, $mini) {
    var $options = $mini.querySelectorAll('.mini-item');
    $options.forEach(($option) => {
      if (
        $input.value === '' ||
        $option.innerText.toLowerCase().indexOf($input.value.toLowerCase()) !=
          -1
      ) {
        $option.style.display = '';
      } else {
        $option.style.display = 'none';
      }
    });
  }

  var searchTimeout = 250,
    searchTimerId = null;

  function load_minis() {
    // delete event for existing tags
    (document.querySelectorAll('.mini-tags .tag.is-delete') || []).forEach(
      ($tag) => {
        $tag.addEventListener('click', function () {
          var $control = $tag.closest('.control');
          var $taglist = $tag.closest('.mini-tags');
          $control.parentElement.removeChild($control);
          updateId($taglist);
        });
      },
    );

    // Add a click event on buttons to open a specific mini
    (document.querySelectorAll('.input-mini:not(.loaded)') || []).forEach(
      ($input) => {
        // open it when clicking
        $input.classList.add('loaded');
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
              loadMini(
                $input,
                $mini,
                $input.getAttribute('search-area'),
                $hidden,
              );
              window.clearTimeout(searchTimerId);
            }, searchTimeout);
          });
        } else if ($input.hasAttribute('lookup-area')) {
          preloadMini(
            $input,
            $mini,
            $input.getAttribute('lookup-area'),
            $hidden,
          );

          // if it is a multiselect, we can use typing to filter the static list
          if ($input.classList.contains('multiselect')) {
            $input.addEventListener('input', () => {
              inputFilter($input, $mini);
            });
          }
        }

        if ($clear != undefined) {
          $clear.addEventListener('click', function () {
            $hidden.value = '';
            $input.value = '';
          });
        }
      },
    );

    // Add a click event on various child elements to close the parent mini
    (
      document.querySelectorAll(
        '.mini-background:not(.loaded), .mini-close:not(.loaded), .mini-card-head:not(.loaded) .delete:not(.loaded), .mini-card-foot:not(.loaded) .button:not(.loaded)',
      ) || []
    ).forEach(($close) => {
      const $target = $close.closest('.mini');
      $close.classList.add('loaded');
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
    (
      document.querySelectorAll('.mini-tags a.is-delete[value]:not(.loaded)') ||
      []
    ).forEach(($delete) => {
      $delete.classList.add('loaded');
      $delete.addEventListener('click', function () {
        var $hidden = $delete
          .closest('.field:not(.mini-tags)')
          .querySelector('select.is-hidden');
        var $control = $delete.closest('.control');

        $hidden.removeChild(
          $hidden.querySelector(
            'option[value="' + $delete.getAttribute('value') + '"]',
          ),
        );
        $control.parentNode.removeChild($control);
      });
    });

    // on reorder events, update id
    (
      document.querySelectorAll('.mini-tags.reorder:not(.loaded)') || []
    ).forEach(($tag) => {
      $tag.classList.add('loaded');
      $tag.addEventListener('reorder', function () {
        updateId($tag);
      });
    });
  }

  load_minis();
  document.addEventListener('ajax', () => {
    load_minis();
  });
})();
