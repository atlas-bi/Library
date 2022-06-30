// eslint-disable-next-line no-unused-vars
function debounce(func, wait, immediate) {
  let timeout;
  return function () {
    const context = this;
    const args = arguments;

    const later = function () {
      timeout = null;
      if (!immediate) func.apply(context, args);
    };

    const callNow = immediate && !timeout;
    clearTimeout(timeout);
    timeout = setTimeout(later, wait);
    if (callNow) func.apply(context, args);
  };
}

// eslint-disable-next-line no-unused-vars
function getUrlVars() {
  const vars = {};
  window.location.href
    .split('#')[0]
    .replace(/[?&]+([^=&]+)=([^&]*)/gi, function (m, key, value) {
      vars[key] = value;
    });
  return vars;
}

// eslint-disable-next-line no-unused-vars
function getOffset(element) {
  if (element.getClientRects().length === 0) {
    return {
      top: 0,
      left: 0,
    };
  }

  const rect = element.getBoundingClientRect();

  return {
    top: rect.top,
    left: rect.left,
  };
}

// eslint-disable-next-line no-unused-vars
function serialize(form) {
  // Setup our serialized data
  const serialized = []; // Loop through each field in the form

  for (let i = 0; i < form.elements.length; i++) {
    const field = form.elements[i]; // Don't serialize fields without a name, submits, buttons, file and reset inputs, and disabled fields

    if (
      !field.name ||
      field.disabled ||
      field.type === 'file' ||
      field.type === 'reset' ||
      field.type === 'submit' ||
      field.type === 'button'
    )
      continue; // If a multi-select, get all selections

    if (field.type === 'select-multiple') {
      for (let n = 0; n < field.options.length; n++) {
        if (!field.options[n].selected) continue;
        serialized.push(
          encodeURIComponent(field.name) +
            '=' +
            encodeURIComponent(field.options[n].value),
        );
      }
    } // Convert field data to a query string
    else if (
      (field.type !== 'checkbox' && field.type !== 'radio') ||
      field.checked
    ) {
      serialized.push(
        encodeURIComponent(field.name) + '=' + encodeURIComponent(field.value),
      );
    }
  }

  return serialized.join('&');
}

// eslint-disable-next-line no-unused-vars
function setCookie(name, value, days) {
  let expires = '';
  const date = new Date();

  days = days || 1;
  date.setTime(date.getTime() + days * 24 * 60 * 60 * 1000);
  expires = '; expires=' + date.toUTCString();

  document.cookie = name + '=' + (value || '') + expires + '; path=/';
}

// eslint-disable-next-line no-unused-vars
function getCookie(name) {
  const nameEQ = name + '=';
  const ca = document.cookie.split(';');

  for (let i = 0; i < ca.length; i++) {
    let c = ca[i];

    while (c.charAt(0) === ' ') {
      c = c.slice(1, c.length);
    }

    if (c.indexOf(nameEQ) === 0) return c.slice(nameEQ.length, c.length);
  }

  return null;
}
