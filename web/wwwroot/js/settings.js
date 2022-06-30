(function () {
  let q;
  document.addEventListener('click', function (event) {
    if (event.target.matches('button.report-tags-etl-reset')) {
      q = new XMLHttpRequest();
      q.open('get', '/Settings/?handler=DefaultEtl', true);
      q.setRequestHeader('Content-Type', 'text/html;charset=UTF-8`');
      q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
      q.send();

      q.addEventListener('readystatechange', function () {
        if (this.readyState === 4 && this.status === 200) {
          document.querySelector('.report-tags-etl textarea').value =
            q.responseText;
        }
      });
    } else if (
      event.target.matches('.settings-search-visiblity[type="checkbox"]') &&
      event.target.tagName === 'INPUT'
    ) {
      const p = event.target.parentElement;
      const i = event.target;
      let type = 1;

      if (i.hasAttribute('checked')) {
        i.removeAttribute('checked');
        type = 2;
      } else {
        i.setAttribute('checked', 'checked');
      }

      const data = {
        TypeId: p.getAttribute('typeId'),
        GroupId: p.hasAttribute('groupId') ? p.getAttribute('groupId') : '',
        Type: type,
      };
      const url = Object.keys(data)
        .map(function (k) {
          return encodeURIComponent(k) + '=' + encodeURIComponent(data[k]);
        })
        .join('&');
      q = new XMLHttpRequest();
      q.open(
        'post',
        '/Settings/Search/?handler=SearchUpdateVisibility&' + url,
        true,
      );
      q.setRequestHeader('Content-Type', 'text/html;charset=UTF-8`');
      q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
      q.send();

      q.addEventListener('readystatechange', function () {
        if (this.readyState === 4 && this.status === 200) {
          showMessageBox('Changes saved.');
        }
      });
    } else if (event.target.matches('a.settings-search-name')) {
      const input = event.target
        .closest('.field')
        .querySelector('input[groupId]');
      if (input === undefined) return !1;
      q = new XMLHttpRequest();
      q.open(
        'post',
        '/Settings/Search/?handler=SearchUpdateText&id=' +
          input.getAttribute('groupId') +
          '&text=' +
          input.value,
        true,
      );
      q.setRequestHeader('Content-Type', 'text/html;charset=UTF-8`');
      q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
      q.send();

      q.addEventListener('readystatechange', function () {
        if (this.readyState === 4 && this.status === 200) {
          showMessageBox('Changes saved.');
        }
      });
    }
  });
})();
