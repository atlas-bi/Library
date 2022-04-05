(function () {
  document.addEventListener('click', function (event) {
    if (
      event.target.matches('.role-permissions[type="checkbox"]') &&
      event.target.tagName === 'INPUT'
    ) {
      const i = event.target;
      let type = 1;

      if (i.hasAttribute('checked')) {
        i.removeAttribute('checked');
        type = 2;
      } else {
        i.setAttribute('checked', 'checked');
      }

      const data = {
        RoleId: i.getAttribute('roleid'),
        PermissionId: i.getAttribute('permissionid'),
        Type: type,
      };
      const url = Object.keys(data)
        .map(function (k) {
          return encodeURIComponent(k) + '=' + encodeURIComponent(data[k]);
        })
        .join('&');
      const q = new XMLHttpRequest();
      q.open('post', '/Settings/Roles/?handler=UpdatePermissions&' + url, true);
      q.setRequestHeader('Content-Type', 'text/html;charset=UTF-8`');
      q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
      q.send();

      q.addEventListener('readystatechange', function () {
        if (this.readyState === 4 && this.status === 200) {
          console.log('Changes saved.');
        }
      });
    }
  });
})();
