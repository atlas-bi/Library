function debounce(func, wait, immediate) {
  var timeout;
  return function () {
    var context = this,
      args = arguments;

    var later = function later() {
      timeout = null;
      if (!immediate) func.apply(context, args);
    };

    var callNow = immediate && !timeout;
    clearTimeout(timeout);
    timeout = setTimeout(later, wait);
    if (callNow) func.apply(context, args);
  };
}

function getUrlVars() {
  var vars = {};
  window.location.href
    .split('#')[0]
    .replace(/[?&]+([^=&]+)=([^&]*)/gi, function (m, key, value) {
      vars[key] = value;
    });
  return vars;
}

function getOffset(element) {
  if (!element.getClientRects().length) {
    return {
      top: 0,
      left: 0,
    };
  }

  var rect = element.getBoundingClientRect();

  return {
    top: rect.top,
    left: rect.left,
  };
}

function serialize(form) {
  // Setup our serialized data
  var serialized = []; // Loop through each field in the form

  for (var i = 0; i < form.elements.length; i++) {
    var field = form.elements[i]; // Don't serialize fields without a name, submits, buttons, file and reset inputs, and disabled fields

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
      for (var n = 0; n < field.options.length; n++) {
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

function setCookie(name, value, days) {
  var expires = '',
    date = new Date();

  days = days || 1;
  date.setTime(date.getTime() + days * 24 * 60 * 60 * 1000);
  expires = '; expires=' + date.toUTCString();

  document.cookie = name + '=' + (value || '') + expires + '; path=/';
}

function getCookie(name) {
  var nameEQ = name + '=';
  var ca = document.cookie.split(';');

  for (var i = 0; i < ca.length; i++) {
    var c = ca[i];

    while (c.charAt(0) == ' ') {
      c = c.substring(1, c.length);
    }

    if (c.indexOf(nameEQ) === 0) return c.substring(nameEQ.length, c.length);
  }

  return null;
}

//  function cache = {
//   timeout: 30 * 60,
//   // mins
//   data: {},
//   remove: function remove(url) {
//     try {
//       sessionStorage.removeItem(btoa(url));
//     } catch (e) {}
//   },
//   exists: function exists(url) {
//     try {
//       return (
//         !!sessionStorage.getItem(btoa(url)) &&
//         JSON.parse(sessionStorage.getItem(btoa(url)))._ > new Date().getTime()
//       );
//     } catch (e) {
//       return false;
//     }
//   },
//   get: function get(url) {
//     return JSON.parse(sessionStorage.getItem(btoa(url))).data;
//   },
//   set: function set(url, data, t) {
//     try {
//       sessionStorage.removeItem(btoa(url));
//       sessionStorage.setItem(
//         btoa(url),
//         JSON.stringify({
//           data: data,
//           _: new Date().getTime() + (t || cache.timeout) * 1000,
//         }),
//       );
//     } catch (e) {}
//   },
// };
