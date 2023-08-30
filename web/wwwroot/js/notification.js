(function () {
  function addNotification(message) {
    const d = document;
    const notificationWrapper = d.querySelectorAll(
      '.fixed-notification-wrapper',
    )[0];

    const notification = document.createElement('div');
    notification.classList.add('notification', 'is-info', 'py-2');
    const button = document.createElement('button');
    button.classList.add('delete');

    notification.append(button);
    notification.insertAdjacentHTML('beforeend', DOMPurify.sanitize(message));
    notificationWrapper.insertBefore(
      notification,
      notificationWrapper.childNodes[0],
    );

    setTimeout(function () {
      notification.remove();
    }, 4000);

    button.addEventListener('mouseup', () => {
      notification.remove();
    });
  }

  document.addEventListener(
    'notification',
    function (event) {
      if (
        typeof event.detail !== 'undefined' &&
        Boolean(event.detail) &&
        Boolean(event.detail.value)
      ) {
        addNotification(event.detail.value);
      }
    },
    false,
  );
})();
