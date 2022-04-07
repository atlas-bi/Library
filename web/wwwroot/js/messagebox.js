(function () {
  function showMessageBox(message) {
    const d = document;
    const w = d.querySelectorAll('.message-wrapper')[0];
    d.querySelector('.message-container .message-inner').innerHTML =
      DOMPurify.sanitize(message);
    w.classList.remove('hidden');
    setTimeout(function () {
      w.classList.add('hidden');
    }, 2000);
  }

  document.addEventListener('click', function (event) {
    if (
      event.target.matches('.message-close') &&
      event.target.parentElement.matches('.message-wrapper')
    ) {
      event.target.parentElement.classList.add('hidden');
    }
  });

  document.addEventListener(
    'show-message',
    function (event) {
      if (
        typeof event.detail !== 'undefined' &&
        Boolean(event.detail) &&
        Boolean(event.detail.value)
      ) {
        showMessageBox(event.detail.value);
      }
    },
    false,
  );
})();
