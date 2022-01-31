(function () {
  var q;
  document.addEventListener('click', function (e) {
    if (
      e.target.matches('.settings-search-visiblity[type="checkbox"]') &&
      e.target.tagName == 'INPUT'
    ) {
      var p = e.target.parentElement,
        i = e.target,
        type = 1,
        url,
        data;

      if (i.hasAttribute('checked')) {
        i.removeAttribute('checked');
        type = 2;
      } else {
        i.setAttribute('checked', 'checked');
      }

      data = {
        TypeId: p.getAttribute('typeId'),
        GroupId: p.hasAttribute('groupId') ? p.getAttribute('groupId') : '',
        Type: type,
      };
      url = Object.keys(data)
        .map(function (k) {
          return encodeURIComponent(k) + '=' + encodeURIComponent(data[k]);
        })
        .join('&');
      q = new XMLHttpRequest();
      q.open('post', '/Settings?handler=SearchUpdateVisibility&' + url, true);
      q.setRequestHeader('Content-Type', 'text/html;charset=UTF-8`');
      q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
      q.send();

      q.onreadystatechange = function (e) {
        if (this.readyState == 4 && this.status == 200) {
          ShowMessageBox('Changes saved.');
        }
      };
    } else if (e.target.matches('a.settings-search-name')) {
      var input = e.target.closest('.field').querySelector('input[groupId]');
      if (input === undefined) return !1;
      q = new XMLHttpRequest();
      q.open(
        'post',
        '/Settings?handler=SearchUpdateText&id=' +
          input.getAttribute('groupId') +
          '&text=' +
          input.value,
        true,
      );
      q.setRequestHeader('Content-Type', 'text/html;charset=UTF-8`');
      q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
      q.send();

      q.onreadystatechange = function (e) {
        if (this.readyState == 4 && this.status == 200) {
          ShowMessageBox('Changes saved.');
        }
      };
    }
  });
})();
