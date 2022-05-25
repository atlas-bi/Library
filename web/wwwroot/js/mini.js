// Functions to open and close a mini
(function () {
  function openMini($element) {
    $element.classList.add('is-active');
  }

  function closeMini($element) {
    $element.classList.remove('is-active');
    // If the mini input has remaining text, add red outline

    const $input = $element.parentElement.querySelector(
      'input.input-mini.multiselect',
    );
    if ($input && $input.value) {
      $input.classList.add('is-danger');
    }
  }

  function closeAllMinis() {
    (document.querySelectorAll('.mini') || []).forEach(($mini) => {
      closeMini($mini);
    });
  }

  function updateId(taglist) {
    const $hiddenInputs = taglist.querySelectorAll(
      'input[type="hidden"][name]:not(.drag)',
    );
    for (let x = 0; x < $hiddenInputs.length; x++) {
      $hiddenInputs[x].setAttribute(
        'name',
        $hiddenInputs[x].getAttribute('name').replace(/\[\d*?]/, '[' + x + ']'),
      );
    }
  }

  function load($mini, $data, $hidden, $input) {
    const data = JSON.parse($data);
    if (data.length === 0) {
      $mini.innerHTML = '<div class="mini-waiting">No matches found.</div>';
    } else {
      $mini.textContent = '';

      for (const element of data) {
        // Var hiddenClass = '';
        // if (active.indexOf(el.ObjectId) !== -1) {
        //   hiddenClass = 'hidden';
        // }
        const a = document.createElement('a');
        a.classList.add('mini-item');
        a.textContent = element.Name;
        a.setAttribute('value', element.ObjectId || element.Description);

        a.addEventListener('click', function (event) {
          if (!$input.classList.contains('multiselect')) {
            $input.value = event.target.textContent;
            $hidden.value = event.target.getAttribute('value');
            closeAllMinis();
          } else if ($input.classList.contains('multiselect')) {
            const taglist = $input
              .closest('.field')
              .parentNode.querySelector('.mini-tags');
            // Add it to the tag list
            const control = document.createElement('div');
            control.classList.add('control');

            const input = document.createElement('input');
            input.setAttribute('type', 'hidden');
            input.setAttribute('value', event.target.getAttribute('value'));
            if ($input.hasAttribute('data-name')) {
              input.setAttribute('name', $input.getAttribute('data-name'));
            }

            const group = document.createElement('div');
            group.classList.add('tags', 'has-addons');

            const tag = document.createElement('span');
            tag.classList.add('tag', 'is-link');
            tag.textContent = event.target.textContent;

            const del = document.createElement('a');
            del.classList.add('tag', 'is-delete');

            del.addEventListener('click', function () {
              control.remove();
              updateId(taglist);
            });

            if (taglist.classList.contains('reorder')) {
              control.classList.add('drg');
              tag.classList.add('drg-hdl');
            }

            control.append(group);
            group.append(tag);
            group.append(input);
            group.append(del);

            if (taglist !== undefined) {
              taglist.append(control);
              // Update index for c#
              updateId(taglist);
            }

            $input.value = '';

            // If it is a static list then clear the filter
            if ($input.hasAttribute('lookup-area')) {
              inputFilter($input, $mini);
            }

            if ($input.classList.contains('mini-close-fast')) {
              closeAllMinis();
            }
          }
        });

        $mini.append(a);
      }

      if ($mini.querySelectorAll('.mini-item:not(.hidden)').length === 0) {
        $mini.innerHTML += '<div class="mini-waiting">No matches found.</div>';
      }
    }
  }

  let loadMiniAjax = null;
  function loadMini($input, $mini, $sa, $hidden) {
    const value = $input.value;

    // Run query if txt
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

      loadMiniAjax.addEventListener('load', function () {
        const $data = loadMiniAjax.responseText;
        load($mini, $data, $hidden, $input);
      });
    }
    // Else reset it to loader
    else {
      $mini.innerHTML = `<div class="mini-waiting">
                    <span class="icon">
                        <i class="fas fa-circle-notch fa-spin"></i>
                    </span>
                </div>`;
    }
  }

  function preloadMini($input, $mini, $searchArea, $hidden) {
    const q = new XMLHttpRequest();
    q.open('post', '/Search?handler=ValueList&s=' + $searchArea, true);
    q.setRequestHeader(
      'Content-Type',
      'application/x-www-form-urlencoded; charset=UTF-8',
    );
    q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
    q.send();

    q.addEventListener('load', function () {
      const l = q.responseText;
      load($mini, l, $hidden, $input);
    });
  }

  function inputFilter($input, $mini) {
    const $options = $mini.querySelectorAll('.mini-item');
    $options.forEach(($option) => {
      if (
        $input.value === '' ||
        $option.textContent.toLowerCase().includes($input.value.toLowerCase())
      ) {
        $option.style.display = '';
      } else {
        $option.style.display = 'none';
      }
    });
  }

  const searchTimeout = 250;
  let searchTimerId = null;

  function loadMinis() {
    // Delete event for existing tags
    (document.querySelectorAll('.mini-tags .tag.is-delete') || []).forEach(
      ($tag) => {
        $tag.addEventListener('click', function () {
          const $control = $tag.closest('.control');
          const $taglist = $tag.closest('.mini-tags');
          $control.remove();
          updateId($taglist);
        });
      },
    );

    // Add a click event on buttons to open a specific mini
    (document.querySelectorAll('.input-mini:not(.loaded)') || []).forEach(
      ($input) => {
        // Open it when clicking
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

        // Open when typing in the related input
        if ($input.hasAttribute('search-area')) {
          $input.addEventListener('input', () => {
            window.clearTimeout(searchTimerId);
            $input.classList.remove('is-danger');
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

          // If it is a multiselect, we can use typing to filter the static list
          if ($input.classList.contains('multiselect')) {
            $input.addEventListener('input', () => {
              $input.classList.remove('is-danger');
              inputFilter($input, $mini);
            });
          }
        }

        if ($clear) {
          $clear.addEventListener('click', function () {
            $hidden.value = '';
            $input.value = '';
            $input.classList.remove('is-danger');
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

    window.addEventListener('click', function (event) {
      if (!event.target.closest('.mini,.input-mini')) {
        closeAllMinis();
      }
    });

    // Add a keyboard event to close all minis
    document.addEventListener('keydown', (event) => {
      event = event || window.event;

      if (Number(event.keyCode) === 27) {
        // Escape key
        closeAllMinis();
      }
    });

    // On reorder events, update id
    (
      document.querySelectorAll('.mini-tags.reorder:not(.loaded)') || []
    ).forEach(($tag) => {
      $tag.classList.add('loaded');
      $tag.addEventListener('reorder', function () {
        updateId($tag);
      });
    });
  }

  loadMinis();
  document.addEventListener('ajax', () => {
    loadMinis();
  });
})();
