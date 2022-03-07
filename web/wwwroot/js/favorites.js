(function () {
  var d = document;

  d.addEventListener('click', function (e) {
    if (e.target.closest('.favorites-show-all')) {
      showall();
    } else if (e.target.closest('.favorites-folder')) {
      var $target = e.target.closest('.favorites-folder');

      // close other folders
      (
        document.querySelectorAll(
          '.favorites-folder.is-active,.favorites-show-all.is-active',
        ) || []
      ).forEach(($el) => {
        $el.classList.remove('is-active');
      });

      (
        document.querySelectorAll(
          '.favorites-folder .fa-folder-open,.favorites-show-all .fa-folder-open',
        ) || []
      ).forEach(($el) => {
        $el.classList.remove('fa-folder-open');
        $el.classList.add('fa-folder');
      });

      // update icon
      if ($target.querySelector('.fa-folder')) {
        var $icon = $target.querySelector('.fa-folder');
        $icon.classList.remove('fa-folder');
        $icon.classList.add('fa-folder-open');
      }

      // make active
      $target.classList.add('is-active');

      // filter favs
      document.querySelectorAll('.favorites .favorite').forEach(($el) => {
        if ($target.dataset.folderid == $el.dataset.folderid) {
          $el.style.display = null;
        } else {
          $el.style.display = 'None';
        }
      });
    }
  });

  function showall() {
    // show everything
    (document.querySelectorAll('.favorites .favorite') || []).forEach(($el) => {
      $el.style.display = null;
    });

    // clear filters
    (document.querySelectorAll('a.favorites-filter.is-active') || []).forEach(
      ($el) => {
        $el.classList.remove('is-active');
      },
    );

    // clear input
    document.querySelector('input.favorites-filter').value = '';

    // clear folders
    (document.querySelectorAll('.favorites-folder.is-active') || []).forEach(
      ($el) => {
        $el.classList.remove('is-active');
      },
    );

    // reset icon
    if (document.querySelector('.favorites-show-all .fa-folder')) {
      var $el = document.querySelector('.favorites-show-all .fa-folder');
      $el.classList.remove('fa-folder');
      $el.classList.add('fa-folder-open');
    }

    (
      document.querySelectorAll('.favorites-folder .fa-folder-open') || []
    ).forEach(($el) => {
      $el.classList.remove('fa-folder-open');
      $el.classList.add('fa-folder');
    });
  }

  document.addEventListener('click', function (e) {
    if (e.target.closest('.favorite-folder-delete')) {
      e.preventDefault();
      var $target = e.target.closest('.favorite-folder-delete');
      var q = new XMLHttpRequest();
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
      var folder = e.target.closest('.favorites-folder');
      folder.parentElement.removeChild(folder);
      // open all
      document.querySelector('.favorites-show-all').click();
    }
  });
  d.addEventListener(
    'submit',
    function (e) {
      // no event for adding a new folder. we allow a refresh in that case.
      var $target, q;
      if (e.target.closest('.favorite-folder-new')) {
        e.preventDefault();

        $target = e.target.closest('.favorite-folder-new');
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

      if (e.target.closest('.favorite-folder-rename')) {
        e.preventDefault();
        $target = e.target.closest('.favorite-folder-rename');
        document.querySelector(
          '.favorite-folders .favorites-folder[data-folderid="' +
            $target.dataset.folderid +
            '"] .favorite-folder-name',
        ).innerText = $target.querySelector('input').value;
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
    function (e) {
      var array = [],
        i = 0,
        q;
      if (e.target.closest('.favorite-folders.reorder')) {
        document.querySelectorAll('.favorites-folder').forEach(($el) => {
          var item = {};
          item.FolderId = $el.dataset.folderid;
          item.FolderRank = i;
          i++;
          array.push(item);
        });

        q = new XMLHttpRequest();
        q.open('post', '/Users/Favorites?handler=ReorderFolders', true);
        q.setRequestHeader('Content-Type', 'application/json; charset=utf-8');
        q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
        q.send(JSON.stringify(array));
      } else if (e.target.closest('.favorites .reorder')) {
        document.querySelectorAll('.favorite').forEach(($el) => {
          var item = {};
          item.FavoriteId = $el.dataset.favoriteid;
          item.FavoriteRank = i;
          item.FavoriteType = $el.dataset.type;

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

  // remove hover style
  d.addEventListener('dragEnd', function (e) {
    if (typeof e.detail !== 'undefined') {
      var el = getHoveredFolder(e.detail.el, e.detail.x, e.detail.y);
      if (e.target.closest('.favorite') && el) {
        var $count;
        // decrment current folder count
        var $folderid = e.detail.el.dataset.folderid;
        var $folder = document.querySelector(
          '.favorite-folders .favorites-folder[data-folderid="' +
            $folderid +
            '"]',
        );

        if ($folderid && $folder !== null) {
          $count = $folder.querySelector('.fav-count');
          $count.innerText = parseInt($count.innerText) - 1;
        }

        UpdateFavFolder(e.detail.el, el);

        if (el.dataset.folderid !== '0') {
          $count = el.querySelector('.fav-count');
          $count.innerText = parseInt($count.innerText) + 1;
        }
        el.click();
      }
    }

    document.querySelectorAll('.favorite-folders .is-hover').forEach(($el) => {
      $el.classList.remove('is-hover');
    });
  });
  d.addEventListener(
    'dragMove',
    function (e) {
      if (typeof e.detail !== 'undefined') {
        var i = d.querySelectorAll(
            '.favorite-folders .favorites-show-all,.favorite-folders .favorites-folder',
          ),
          l,
          el;

        for (l = 0; l < i.length; l++) {
          i[l].classList.remove('is-hover');
        }

        el = getHoveredFolder(e.detail.el, e.detail.x, e.detail.y);

        if (el && el !== null) {
          el.classList.add('is-hover');
        }
      }
    },
    false,
  );

  function getHoveredFolder(el, x, y) {
    if (el.classList.contains('favorite')) {
      var i = d.querySelectorAll(
          '.favorite-folders .favorites-show-all,.favorite-folders .favorites-folder',
        ),
        l,
        g,
        o,
        top,
        bottom,
        left,
        right;

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

  function UpdateFavFolder($favorite, $folder) {
    var item = {},
      q;
    $favorite.dataset.folderid = $folder.dataset.folderid;
    item.FavoriteId = $favorite.dataset.favoriteid;
    item.FolderId = $folder.dataset.folderid;
    item.FavoriteType = $favorite.dataset.type;
    q = new XMLHttpRequest();
    q.open('post', '/Users/Favorites?handler=UpdateFavoriteFolder', true);
    q.setRequestHeader('Content-Type', 'application/json; charset=utf-8');
    q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
    q.send(JSON.stringify(item));
  }
})();

(function () {
  document.addEventListener('click', function ($e) {
    var $target, $props;
    if ($e.target.closest('a.star')) {
      $e.preventDefault();
      $target = $e.target.closest('a.star');
      $props = $target.dataset;

      var $ajax = new XMLHttpRequest();
      $ajax.open('get', $props.href, true);
      $ajax.send();
      $ajax.onload = function () {
        // swap classes and update count.
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
          $target.querySelector('.star-count').innerText = $ajax.responseText;
        }
        // if we are on the fav's page, should we refresh the page, or delete the element?
        // pop it
        if ($target.closest('.favorite') != null) {
          var $fav = $target.closest('.favorite');
          var $folderid = $target.closest('.favorite').dataset.folderid;
          $fav.parentElement.removeChild($fav);

          var $folder = document.querySelector(
            '.favorite-folders .favorites-folder[data-folderid="' +
              $folderid +
              '"]',
          );

          if ($folderid !== null && $folderid != '0' && $folder != null) {
            $folder.querySelector('.fav-count').innerText =
              parseInt($folder.querySelector('.fav-count').innerText) - 1;
          }

          // decrment all fav count
          document.querySelector('.favorites-show-all .fav-count').innerText =
            parseInt(
              document.querySelector('.favorites-show-all .fav-count')
                .innerText,
            ) - 1;
        }
      };
    } else if ($e.target.closest('a.favorites-filter.is-active')) {
      $target = $e.target.closest('a.favorites-filter.is-active');
      $props = $target.dataset;
      $target.classList.remove('is-active');
      // unhide everything
      document.querySelectorAll('.favorites .favorite').forEach(($el) => {
        $el.style.display = null;
      });
    } else if ($e.target.closest('a.favorites-filter')) {
      $target = $e.target.closest('a.favorites-filter');
      $props = $target.dataset;
      // remove other active filters
      document
        .querySelectorAll('a.favorites-filter.is-active')
        .forEach(($el) => {
          $el.classList.remove('is-active');
        });

      // reset input
      document.querySelector('input.favorites-filter').value = '';
      // active
      $target.classList.add('is-active');

      document.querySelectorAll('.favorites .favorite').forEach(($el) => {
        if ($el.classList.contains($props.type)) {
          $el.style.display = null;
        } else {
          $el.style.display = 'None';
        }
      });
    }
  });

  function fuzzysearch(needle, haystack) {
    var words = needle.toLowerCase().split(' ');
    var haystack_words = haystack.toLowerCase().split(' ');
    return (
      haystack_words.filter(($el) => {
        return (
          words.indexOf($el) != -1 ||
          words.filter(($sel) => {
            return $el.indexOf($sel) != -1;
          }).length > 0
        );
      }).length > 0
    );
  }

  document.addEventListener('input', function ($e) {
    var $target;
    if ($e.target.closest('input.favorites-filter')) {
      $target = $e.target;
      // clear other filters
      document
        .querySelectorAll('a.favorites-filter.is-active')
        .forEach(($el) => {
          $el.classList.remove('is-active');
        });

      document.querySelectorAll('.favorites .favorite').forEach(($el) => {
        if ($target.value.trim() === '') {
          $el.style.display = null;
        } else {
          if (fuzzysearch($target.value, $el.textContent)) {
            $el.style.display = null;
          } else {
            $el.style.display = 'None';
          }
        }
      });
    }
  });
})();
