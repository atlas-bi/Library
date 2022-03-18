(function () {
  function ShowMessageBox(message) {
    var d = document,
      w = d.getElementsByClassName('message-wrapper')[0];
    d.querySelector('.message-container .message-inner').innerHTML = message;
    w.classList.remove('hidden');
    setTimeout(function () {
      w.classList.add('hidden');
    }, 2000);
  }

  document.addEventListener('click', function (e) {
    if (
      e.target.matches('.message-close') &&
      e.target.parentElement.matches('.message-wrapper')
    ) {
      e.target.parentElement.classList.add('hidden');
    }
  });

  document.addEventListener(
    'show-message',
    function (e) {
      if (typeof e.detail !== 'undefined' && !!e.detail && !!e.detail.value) {
        ShowMessageBox(e.detail.value);
      }
    },
    false,
  );
})();
