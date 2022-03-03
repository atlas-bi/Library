(function () {
  var d = document; // adding favorites

  d.addEventListener(
    'click',
    function (e) {
      if (
        e.target.closest('.favorite:not(.disabled)') ||
        e.target.closest('.favorite-search:not(.disabled)') ||
        e.target.closest('.fav-star:not(.disabled)') ||
        e.target.closest('[fav-type]')
      ) {
        e.preventDefault();
        e.stopPropagation();
        var t = e.target.closest('.favorite-search:not(.disabled)')
            ? e.target.getElementsByTagName('i')[0]
            : e.target,
          x,
          el,
          q,
          data,
          url,
          inFavBox = t.closest('.favs') == null ? false : true,
          hasFavBox =
            document.getElementsByClassName('favs')[0] == null ? false : true,
          actionType = 1,
          favoriteType = t.getAttribute('fav-type'),
          objectId = t.getAttribute('object-id'),
          objectName = t.getAttribute('object-name'),
          l = d.querySelectorAll(
            '[fav-type="' + favoriteType + '"][object-id="' + objectId + '"]',
          );

        for (x = 0; x < l.length; x++) {
          el = l[x];

          if (el.classList.contains('favorite')) {
            el.classList.remove('favorite');
            actionType = 0;
          } else {
            el.classList.add('favorite');
          }
        }

        if (inFavBox) {
          if (d.querySelectorAll('.favs div[folder-id]').length <= 1) {
            el = d.getElementById('favs-none');
            el.style.opacity = 0;
            el.style.removeProperty('display');
            el.style.transition = 'opacity 0.3s ease-in-out';

            // eslint-disable-next-line no-unused-vars
            var a = el.offsetHeight; // clear css cache

            el.style.opacity = 1;
          }

          for (x = 0; x < l.length; x++) {
            el = l[x].closest('.fav');
            el.parentElement.removeChild(el);
          }
        }

        data = {
          actionType: actionType,
          favoriteType: favoriteType,
          objectId: objectId,
          objectName: objectName,
        };
        url = Object.keys(data)
          .map(function (k) {
            return encodeURIComponent(k) + '=' + encodeURIComponent(data[k]);
          })
          .join('&');
        q = new XMLHttpRequest();
        q.open('post', '/users?handler=EditFavorites&' + url, true);
        q.setRequestHeader(
          'Content-Type',
          'application/x-www-form-urlencoded; charset=UTF-8',
        );
        q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
        q.send();

        q.onload = function () {
          if (hasFavBox) {
            d.dispatchEvent(new CustomEvent('reload-favs'));
          }
        };
      }
    },
    false,
  );

  d.addEventListener('click', function (e) {
    if (e.target.closest('.fav-show-all')) {
      showall();
    } else if (e.target.closest('.fav-folder-new')) {
      return false;
    } else if (e.target.closest('.fav-folder')) {
      if (e.target.classList.contains('active')) {
        showall();
      } else {
        showall(e.target.closest('.fav-folder'));
      }
    }
  });

  function showall(me) {
    var t = me || d.getElementsByClassName('fav-show-all')[0],
      i,
      x,
      el,
      si,
      a,
      folderId = t.getAttribute('folder-id'),
      nr = d.getElementById('favs-none');
    nr.style.display = 'none';
    i = t.parentElement.getElementsByClassName('active');

    for (x = 0; x < i.length; x++) {
      el = i[x];
      el.classList.remove('active');
      si = el.getElementsByTagName('i')[0];
      si.classList.remove('fa-folder-open');
      si.classList.add('fa-folder');
    }

    if (folderId !== null) {
      i = d.querySelectorAll(
        '.favs div[folder-id]:not([folder-id="' + folderId + '"])',
      );

      for (x = 0; x < i.length; x++) {
        el = i[x];
        el.style.display = 'none';
      }

      i = d.querySelectorAll('.favs div[folder-id="' + folderId + '"]');
    } else {
      i = d.querySelectorAll('.favs div[folder-id]');
    }

    for (x = 0; x < i.length; x++) {
      el = i[x];
      el.style.opacity = 0;
      el.style.removeProperty('display');
      el.style.transition = 'opacity 0.1s ease-in-out';
      a = el.offsetHeight; // clear css cache

      el.style.opacity = 1;
    }

    t.classList.add('active');
    i = t.getElementsByTagName('i')[0];
    i.classList.remove('fa-folder');
    i.classList.add('fa-folder-open'); // check if there  0 items showing and give a message

    if (
      (folderId !== null &&
        d.querySelectorAll('.favs div[folder-id="' + folderId + '"]').length ==
          0) ||
      (folderId == null &&
        d.querySelectorAll('.favs div[folder-id]').length == 0)
    ) {
      i = nr.childNodes;

      for (x = 0; x < i.length; x++) {
        el = i[x];
        el.style.removeProperty('display');
      }

      nr.style.opacity = 0;
      nr.style.removeProperty('display');
      nr.style.transition = 'opacity 0.1s ease-in-out';

      // eslint-disable-next-line no-unused-vars
      a = nr.offsetHeight; // clear css cache

      nr.style.opacity = 1;
      d.getElementById('Folder_UserFavoriteFolderId').value = folderId;
    }
  }

  d.addEventListener(
    'submit',
    function (e) {
      var q, url;

      if (e.target.closest('#CreateFolderForm')) {
        e.preventDefault();
        var i = e.target.getElementsByTagName('input')[0],
          s = e.target.getElementsByTagName('span')[0],
          v = i.value,
          f = d.getElementById('fav-folders');

        url = serialize(e.target);
        q = new XMLHttpRequest();
        q.open('post', e.target.getAttribute('action') + '&' + url, true);
        q.setRequestHeader(
          'Content-Type',
          'application/x-www-form-urlencoded; charset=UTF-8',
        );
        q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
        q.send();
        // clear input
        i.value = '';
        e.target.querySelector('.submit').innerHTML = '';
        q.onload = function () {
          var div = d.createElement('div');
          div.classList.add('fav-folder');
          div.classList.add('drg');
          div.setAttribute('folder-id', q.responseText);
          div.innerHTML =
            '<i class="fas fa-folder"></i><span>' +
            v +
            '</span><div class="fav-count">0</div><div class="folder-grip drg-hdl"><i class="fas fa-grip-lines"></i></div>';
          var nf = f.getElementsByClassName('fav-folder-new')[0];
          nf.parentElement.insertBefore(div, nf);
          s.innerHTML = '';
          document.dispatchEvent(
            new CustomEvent('clps-close', {
              cancelable: true,
              detail: {
                el: d.getElementById('fav-folder-new'),
              },
            }),
          );
        };
      } else if (e.target.closest('#DeleteFolderForm')) {
        e.preventDefault();
        var folderId = d.getElementById('Folder_UserFavoriteFolderId').value;
        url = serialize(e.target);
        q = new XMLHttpRequest();
        q.open('post', e.target.getAttribute('action') + '&' + url, true);
        q.setRequestHeader(
          'Content-Type',
          'application/x-www-form-urlencoded; charset=UTF-8',
        );
        q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
        q.send();

        q.onload = function () {
          var m = d
            .getElementById('fav-folders')
            .querySelector("div[folder-id][folder-id='" + folderId + "']");
          m.parentElement.removeChild(m);
          showall();
        };
      }
    },
    false,
  );
  d.addEventListener(
    'dragEnd',
    function (e) {
      if (typeof e.detail !== 'undefined') {
        Reorder(e.detail.el, e.detail.x, e.detail.y);
      }
    },
    false,
  );

  function Reorder(el, x, y) {
    var e, r, i, l;

    if (el.classList.contains('fav-folder')) {
      e = el.parentElement.querySelectorAll(
        '.fav-folder:not(.fav-folder-new):not(.fav-show-all',
      );
      r = Array.from(e).sort(function (a, b) {
        return getOffset(a).top - getOffset(b).top;
      });
      var nf = el.parentElement.getElementsByClassName('fav-folder-new')[0];

      for (i = 0; i < r.length; i++) {
        nf.parentElement.insertBefore(r[i], nf);
      }

      UpdateFolderRank();
    } else if (el.classList.contains('fav')) {
      e = getHoveredFolder(el, x, y);

      if (e && e !== null) {
        UpdateFavFolder(el.getAttribute('fav-id'), e.getAttribute('folder-id'));
        el.setAttribute('folder-id', e.getAttribute('folder-id'));
        showall(d.querySelector('.fav-folder.active'));
      } else {
        e = el.parentElement.getElementsByClassName('fav');
        r = Array.from(e).sort(function (a, b) {
          return getOffset(a).top - getOffset(b).top;
        });

        for (i = 0; i < r.length; i++) {
          el.parentElement.appendChild(r[i]);
        }

        UpdateFavRank();
      }
    }

    el.style.transition = 'top 0.3s; left 0.3s;';
    el.style.top = 0;
    el.style.left = 0; // remove hover class from folders

    i = d.querySelectorAll('#fav-folders .fav-folder:not(.fav-folder-new)');

    for (l = 0; l < i.length; l++) {
      i[l].classList.remove('hover');
    }
  }

  d.addEventListener(
    'dragMove',
    function (e) {
      if (typeof e.detail !== 'undefined') {
        var i = d.querySelectorAll(
            '#fav-folders .fav-folder:not(.fav-folder-new)',
          ),
          l,
          el;

        for (l = 0; l < i.length; l++) {
          i[l].classList.remove('hover');
        }

        el = getHoveredFolder(e.detail.el, e.detail.x, e.detail.y);

        if (el && el !== null) {
          el.classList.add('hover');
        }
      }
    },
    false,
  );

  function getHoveredFolder(el, x, y) {
    if (el.classList.contains('fav')) {
      var i = d.querySelectorAll(
          '#fav-folders .fav-folder:not(.fav-folder-new)',
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

  function UpdateFolderRank() {
    var array = [],
      g,
      s = d.querySelectorAll('#fav-folders .fav-folder:not(.drag-source)'),
      q;

    for (g = 0; g < s.length; g++) {
      if (s[g].hasAttribute('folder-id')) {
        var item = {};
        item.FolderId = s[g].getAttribute('folder-id');
        item.FolderRank = g + 1;
        array.push(item);
      }
    }

    q = new XMLHttpRequest();
    q.open('post', '/Users?handler=ReorderFolders', true);
    q.setRequestHeader('Content-Type', 'application/json; charset=utf-8');
    q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
    q.send(JSON.stringify(array));
  }

  function UpdateFavRank() {
    var array = [],
      g,
      s = d.querySelectorAll('.favs div[folder-id]:not(.drag-source)'),
      q;

    for (g = 0; g < s.length; g++) {
      if (s[g].hasAttribute('folder-id')) {
        var item = {};
        item.FavoriteId = s[g].getAttribute('fav-id');
        item.FavoriteRank = g + 1;
        array.push(item);
      }
    }

    q = new XMLHttpRequest();
    q.open('post', '/Users?handler=ReorderFavorites', true);
    q.setRequestHeader('Content-Type', 'application/json; charset=utf-8');
    q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
    q.send(JSON.stringify(array));
  }

  function UpdateFavFolder(FavoriteId, FolderId) {
    var item = {},
      q;
    item.FavoriteId = FavoriteId;
    item.FolderId = FolderId;
    q = new XMLHttpRequest();
    q.open('post', '/Users?handler=UpdateFavoriteFolder', true);
    q.setRequestHeader('Content-Type', 'application/json; charset=utf-8');
    q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
    q.send(JSON.stringify(item));
  }
})();

(function () {
  document.addEventListener('click', function ($e) {
    if ($e.target.closest('a.star')) {
      $e.preventDefault();
      var $target = $e.target.closest('a.star');
      var $props = $target.dataset;

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
      };
    }
  });
})();
