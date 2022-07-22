(function () {
  document
    .querySelector('#enable_share_notification')
    .addEventListener('change', (event) => {
      const q = new XMLHttpRequest();
      q.open(
        'get',
        '/users/settings?handler=EnableShareNotification&value=' +
          event.target.value,
        true,
      );
      q.setRequestHeader(
        'Content-Type',
        'application/x-www-form-urlencoded; charset=UTF-8',
      );
      q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
      q.send();
    });
})();
