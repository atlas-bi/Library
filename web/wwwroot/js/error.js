(function () {
  const vars = getUrlVars();
  let message;

  function updateUrl($key) {
    delete vars[$key];
    const $hash = document.location.hash ? '#' + document.location.hash : '';

    let $parameters = Object.keys(vars)
      .map(function (x) {
        return x + '=' + vars[x];
      })
      .join('&');

    $parameters =
      $parameters !== undefined && $parameters !== '' ? '?' + $parameters : '';

    const $newUrl =
      window.location.origin + window.location.pathname + $parameters + $hash;

    history.replaceState({}, document.title, $newUrl);
  }

  function addMessage($class, $message) {
    const div = document.createElement('div');
    div.classList.add(
      'notification',
      'is-light',
      $class,
      'has-text-centered',
      'my-0',
    );

    const button = document.createElement('button');

    button.classList.add('delete');
    button.addEventListener('click', function () {
      div.remove();
    });

    div.innerHTML = '<p><b>' + DOMPurify.sanitize($message) + '</b></p>';

    div.insertBefore(button, div.firstChild);

    document
      .querySelector('body')
      .insertBefore(div, document.querySelector('body').firstChild);
  }

  if (vars.error) {
    message = decodeURI(vars.error).replace(/\+/g, / /);
    addMessage('is-danger', message);
    updateUrl('error');
  }

  if (vars.success) {
    message = decodeURI(vars.success).replace(/\+/g, / /);
    addMessage('is-success', message);
    updateUrl('success');
  }

  if (vars.warning) {
    message = decodeURI(vars.warning).replace(/\+/g, / /);
    addMessage('is-warning', message);
    updateUrl('warning');
  }
})();
