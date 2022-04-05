(function () {
  const d = document;

  d.addEventListener('click', function (event) {
    if (event.target.closest('.favorites-show-all')) {
      showall();
    } else if (event.target.closest('.favorites-folder')) {
      const $target = event.target.closest('.favorites-folder');

      // Close other folders
      (
        document.querySelectorAll(
          '.favorites-folder.is-active,.favorites-show-all.is-active',
        ) || []
      ).forEach(($element) => {
        $element.classList.remove('is-active');
      });

      (
        document.querySelectorAll(
          '.favorites-folder .fa-folder-open,.favorites-show-all .fa-folder-open',
        ) || []
      ).forEach(($element) => {
        $element.classList.remove('fa-folder-open');
        $element.classList.add('fa-folder');
      });

      // Update icon
      if ($target.querySelector('.fa-folder')) {
        const $icon = $target.querySelector('.fa-folder');
        $icon.classList.remove('fa-folder');
        $icon.classList.add('fa-folder-open');
      }

      // Make active
      $target.classList.add('is-active');

      // Filter favs
      document.querySelectorAll('.favorites .favorite').forEach(($element) => {
        const newLocal = $target.dataset.folderid === $element.dataset.folderid;
        $element.style.display = newLocal ? '' : 'None';
      });
    }
  });

  function showall() {
    // Show everything
    (document.querySelectorAll('.favorites .favorite') || []).forEach(
      ($element) => {
        $element.style.display = '';
      },
    );

    // Clear filters
    (document.querySelectorAll('a.favorites-filter.is-active') || []).forEach(
      ($element) => {
        $element.classList.remove('is-active');
      },
    );

    // Clear input
    document.querySelector('input.favorites-filter').value = '';

    // Clear folders
    (document.querySelectorAll('.favorites-folder.is-active') || []).forEach(
      ($element) => {
        $element.classList.remove('is-active');
      },
    );

    // Reset icon
    if (document.querySelector('.favorites-show-all .fa-folder')) {
      const $element = document.querySelector('.favorites-show-all .fa-folder');
      $element.classList.remove('fa-folder');
      $element.classList.add('fa-folder-open');
    }

    (
      document.querySelectorAll('.favorites-folder .fa-folder-open') || []
    ).forEach(($element) => {
      $element.classList.remove('fa-folder-open');
      $element.classList.add('fa-folder');
    });
  }

  document.addEventListener('click', function (event) {
    if (event.target.closest('.favorite-folder-delete')) {
      event.preventDefault();
      const $target = event.target.closest('.favorite-folder-delete');
      const q = new XMLHttpRequest();
      q.open(
        'post',
        '/users/favorites?handler=DeleteFolder&id=' + $target.dataset.folderid,
        true,
      );
      q.setRequestHeader(
        'Content-Type',
        'application/x-www-form-urlencoded; charset=UTF-8',
      );
      q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
      q.send();
      const folder = event.target.closest('.favorites-folder');
      folder.remove();
      // Open all
      document.querySelector('.favorites-show-all').click();
    }
  });
  d.addEventListener(
    'submit',
    function (event) {
      // No event for adding a new folder. we allow a refresh in that case.
      let $target;
      let q;
      if (event.target.closest('.favorite-folder-new')) {
        event.preventDefault();

        $target = event.target.closest('.favorite-folder-new');
        q = new XMLHttpRequest();
        q.open(
          'post',
          $target.getAttribute('action') +
            '&name=' +
            $target.querySelector('input').value,
          true,
        );
        q.setRequestHeader(
          'Content-Type',
          'application/x-www-form-urlencoded; charset=UTF-8',
        );
        q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
        q.send();
        window.location.reload();
      }

      if (event.target.closest('.favorite-folder-rename')) {
        event.preventDefault();
        $target = event.target.closest('.favorite-folder-rename');
        document.querySelector(
          '.favorite-folders .favorites-folder[data-folderid="' +
            $target.dataset.folderid +
            '"] .favorite-folder-name',
        ).textContent = $target.querySelector('input').value;
        q = new XMLHttpRequest();
        q.open(
          'post',
          $target.getAttribute('action') +
            '&name=' +
            $target.querySelector('input').value,
          true,
        );
        q.setRequestHeader(
          'Content-Type',
          'application/x-www-form-urlencoded; charset=UTF-8',
        );
        q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
        q.send();
        document.dispatchEvent(new CustomEvent('modal-close'));
      }
    },
    false,
  );

  d.addEventListener(
    'reorder',
    function (event) {
      const array = [];
      let i = 0;
      let q;
      if (event.target.closest('.favorite-folders.reorder')) {
        document.querySelectorAll('.favorites-folder').forEach(($element) => {
          const item = {};
          item.FolderId = $element.dataset.folderid;
          item.FolderRank = i;
          i++;
          array.push(item);
        });

        q = new XMLHttpRequest();
        q.open('post', '/Users/Favorites?handler=ReorderFolders', true);
        q.setRequestHeader('Content-Type', 'application/json; charset=utf-8');
        q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
        q.send(JSON.stringify(array));
      } else if (event.target.closest('.favorites .reorder')) {
        document.querySelectorAll('.favorite').forEach(($element) => {
          const item = {};
          item.FavoriteId = $element.dataset.favoriteid;
          item.FavoriteRank = i;
          item.FavoriteType = $element.dataset.type;

          i++;
          array.push(item);
        });

        q = new XMLHttpRequest();
        q.open('post', '/Users/Favorites?handler=ReorderFavorites', true);
        q.setRequestHeader('Content-Type', 'application/json; charset=utf-8');
        q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
        q.send(JSON.stringify(array));
      }
    },
    false,
  );

  // Remove hover style
  d.addEventListener('dragEnd', function (event) {
    if (typeof event.detail !== 'undefined') {
      const element = getHoveredFolder(
        event.detail.el,
        event.detail.x,
        event.detail.y,
      );
      if (event.target.closest('.favorite') && element) {
        let $count;
        // Decrment current folder count
        const $folderid = event.detail.el.dataset.folderid;
        const $folder = document.querySelector(
          '.favorite-folders .favorites-folder[data-folderid="' +
            $folderid +
            '"]',
        );

        if ($folderid && $folder !== null) {
          $count = $folder.querySelector('.fav-count');
          $count.textContent = parseInt($count.textContent, 10) - 1;
        }

        updateFavFolder(event.detail.el, element);

        if (element.dataset.folderid !== '0') {
          $count = element.querySelector('.fav-count');
          $count.textContent = parseInt($count.textContent, 10) + 1;
        }

        element.click();
      }
    }

    document
      .querySelectorAll('.favorite-folders .is-hover')
      .forEach(($element) => {
        $element.classList.remove('is-hover');
      });
  });
  d.addEventListener(
    'dragMove',
    function (event) {
      if (typeof event.detail !== 'undefined') {
        const i = d.querySelectorAll(
          '.favorite-folders .favorites-show-all,.favorite-folders .favorites-folder',
        );
        let l;

        for (l = 0; l < i.length; l++) {
          i[l].classList.remove('is-hover');
        }

        const element = getHoveredFolder(
          event.detail.el,
          event.detail.x,
          event.detail.y,
        );

        if (element && element !== null) {
          element.classList.add('is-hover');
        }
      }
    },
    false,
  );

  function getHoveredFolder(element, x, y) {
    if (element.classList.contains('favorite')) {
      const i = d.querySelectorAll(
        '.favorite-folders .favorites-show-all,.favorite-folders .favorites-folder',
      );
      let l;
      let g;
      let o;
      let top;
      let bottom;
      let left;
      let right;

      for (l = 0; l < i.length; l++) {
        g = i[l];
        o = getOffset(g);
        top = o.top;
        bottom = o.top + g.offsetHeight;
        left = o.left;
        right = o.left + g.offsetWidth;

        if (y > top && y < bottom && x > left && x < right) {
          return g;
        }
      }

      return false;
    }
  }

  function updateFavFolder($favorite, $folder) {
    const item = {};
    $favorite.dataset.folderid = $folder.dataset.folderid;
    item.FavoriteId = $favorite.dataset.favoriteid;
    item.FolderId = $folder.dataset.folderid;
    item.FavoriteType = $favorite.dataset.type;
    const q = new XMLHttpRequest();
    q.open('post', '/Users/Favorites?handler=UpdateFavoriteFolder', true);
    q.setRequestHeader('Content-Type', 'application/json; charset=utf-8');
    q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
    q.send(JSON.stringify(item));
  }

  document.addEventListener('click', function (event) {
    let $target;
    let $props;
    if (event.target.closest('a.star')) {
      event.preventDefault();
      $target = event.target.closest('a.star');
      $props = $target.dataset;

      const $ajax = new XMLHttpRequest();
      $ajax.open('get', $props.href, true);
      $ajax.send();
      $ajax.addEventListener('load', function () {
        // Swap classes and update count.
        if ($target.querySelector('.fa-star').classList.contains('fas')) {
          $target.querySelector('.fa-star').classList.remove('fas');
          $target.querySelector('.fa-star').classList.add('far');

          $target.querySelector('.icon').classList.remove('has-text-gold');
          $target.querySelector('.icon').classList.add('has-text-grey');
        } else {
          $target.querySelector('.fa-star').classList.remove('far');
          $target.querySelector('.fa-star').classList.add('fas');

          $target.querySelector('.icon').classList.remove('has-text-grey');
          $target.querySelector('.icon').classList.add('has-text-gold');
        }

        if (
          $target.querySelector('.star-count') &&
          $ajax.responseText !== 'error'
        ) {
          $target.querySelector('.star-count').textContent = $ajax.responseText;
        }

        // If we are on the fav's page, should we refresh the page, or delete the element?
        // pop it
        if ($target.closest('.favorite') !== null) {
          const $fav = $target.closest('.favorite');
          const $folderid = $target.closest('.favorite').dataset.folderid;
          $fav.remove();

          const $folder = document.querySelector(
            '.favorite-folders .favorites-folder[data-folderid="' +
              $folderid +
              '"]',
          );

          if ($folderid !== null && $folderid !== '0' && $folder !== null) {
            $folder.querySelector('.fav-count').textContent =
              parseInt($folder.querySelector('.fav-count').textContent, 10) - 1;
          }

          // Decrment all fav count
          document.querySelector('.favorites-show-all .fav-count').textContent =
            parseInt(
              document.querySelector('.favorites-show-all .fav-count')
                .textContent,
              10,
            ) - 1;
        }
      });
    } else if (event.target.closest('a.favorites-filter.is-active')) {
      $target = event.target.closest('a.favorites-filter.is-active');
      $props = $target.dataset;
      $target.classList.remove('is-active');
      // Unhide everything
      document.querySelectorAll('.favorites .favorite').forEach(($element) => {
        $element.style.display = '';
      });
    } else if (event.target.closest('a.favorites-filter')) {
      $target = event.target.closest('a.favorites-filter');
      $props = $target.dataset;
      // Remove other active filters
      document
        .querySelectorAll('a.favorites-filter.is-active')
        .forEach(($element) => {
          $element.classList.remove('is-active');
        });

      // Reset input
      document.querySelector('input.favorites-filter').value = '';
      // Active
      $target.classList.add('is-active');

      document.querySelectorAll('.favorites .favorite').forEach(($element) => {
        $element.style.display = $element.classList.contains($props.type)
          ? ''
          : 'None';
      });
    }
  });

  function fuzzysearch(needle, haystack) {
    const words = needle.toLowerCase().split(' ');
    const haystackWords = haystack.toLowerCase().split(' ');
    return haystackWords.some(($element) => {
      return (
        words.includes($element) ||
        words.some(($sel) => {
          return $element.includes($sel);
        })
      );
    });
  }

  document.addEventListener('input', function (event) {
    let $target;
    if (event.target.closest('input.favorites-filter')) {
      $target = event.target;
      // Clear other filters
      document
        .querySelectorAll('a.favorites-filter.is-active')
        .forEach(($element) => {
          $element.classList.remove('is-active');
        });

      document.querySelectorAll('.favorites .favorite').forEach(($element) => {
        if ($target.value.trim() === '') {
          $element.style.display = '';
        } else if (fuzzysearch($target.value, $element.textContent)) {
          $element.style.display = '';
        } else {
          $element.style.display = 'None';
        }
      });
    }
  });
})();
