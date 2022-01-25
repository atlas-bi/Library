(function () {
  document.addEventListener('click', function (e) {
    if (
      e.target.matches('.role-permissions[type="checkbox"]') &&
      e.target.tagName == 'INPUT'
    ) {
      var p = e.target.parentElement,
        i = e.target,
        type = 1,
        q,
        url,
        data;

      if (i.hasAttribute('checked')) {
        i.removeAttribute('checked');
        type = 2;
      } else {
        i.setAttribute('checked', 'checked');
      }

      data = {
        RoleId: p.getAttribute('roleid'),
        PermissionId: p.getAttribute('permissionid'),
        Type: type,
      };
      url = Object.keys(data)
        .map(function (k) {
          return encodeURIComponent(k) + '=' + encodeURIComponent(data[k]);
        })
        .join('&');
      q = new XMLHttpRequest();
      q.open('post', '/AccessControl?handler=UpdatePermissions&' + url, true);
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
