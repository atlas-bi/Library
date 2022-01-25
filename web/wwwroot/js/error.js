(function () {
  var vars = getUrlVars();

  function addMessage($class, $message) {
    var div = document.createElement('div');
    div.classList.add(
      'notification',
      'is-light',
      $class,
      'has-text-centered',
      'my-0',
    );

    var button = document.createElement('button');

    button.classList.add('delete');
    button.addEventListener('click', function (e) {
      div.parentNode.removeChild(div);
    });

    div.innerHTML = '<p><b>' + $message + '</b></p>';

    div.insertBefore(button, div.firstChild);

    document
      .querySelector('body')
      .insertBefore(div, document.querySelector('body').firstChild);
  }

  if (vars['error']) {
    var message = decodeURI(vars['error']).replaceAll('+', ' ');
    addMessage('is-danger', message);
  }

  if (vars['success']) {
    var message = decodeURI(vars['success']).replaceAll('+', ' ');
    addMessage('is-success', message);
  }
  if (vars['warning']) {
    var message = decodeURI(vars['warning']).replaceAll('+', ' ');
    addMessage('is-warning', message);
  }
})();
