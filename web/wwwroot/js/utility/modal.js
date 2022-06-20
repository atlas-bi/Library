(function () {
  function closeModals(element) {
    if (typeof element === 'undefined') {
      // Hide all other modals
      Array.prototype.forEach.call(
        document.querySelectorAll('.mdl-o'),
        function (i) {
          i.classList.remove('mdl-o');
          i.dispatchEvent(new CustomEvent('mdl-close'));
        },
      );
    } else {
      element.classList.remove('mdl-o');
      element.dispatchEvent(new CustomEvent('mdl-close'));
    }

    if (document.querySelectorAll('.mdl-o').length === 0) {
      document.querySelectorAll('body')[0].classList.remove('b-mdl');
    }
  }

  function showModal(element, child) {
    const d = document;
    d.querySelectorAll('body')[0].classList.toggle('b-mdl');
    element.classList.toggle('mdl-o');
    element.dispatchEvent(
      new CustomEvent('mdl-open', {
        cancelable: true,
        detail: {
          child,
        },
      }),
    );
  }

  document.addEventListener(
    'click',
    function (event) {
      let element;
      const d = document; // Open modal
      if (
        event.target.closest('[data-toggle]') &&
        event.target.closest('[data-toggle]').getAttribute('data-toggle') ===
          'mdl'
      ) {
        element = d.querySelector(
          `#${event.target
            .closest('[data-toggle]')
            .getAttribute('data-target')}`,
        );
        showModal(element, event.target.closest('[data-toggle]'));
      } // Close modal
      else if (
        (event.target.closest('[data-dismiss]') &&
          event.target
            .closest('[data-dismiss]')
            .getAttribute('data-dismiss') === 'mdl') ||
        // E.target.closest('.mdl:not(#editModal)') ||
        event.target.closest('.editorMdl-close')
      ) {
        // Close closest modal
        closeModals(event.target.closest('.mdl'));
      } else if (event.target.closest('.pop')) {
        d.querySelectorAll('.imagepreview')[0].setAttribute(
          'src',
          event.target.closest('.pop').children[0].getAttribute('src'),
        );
        showModal(d.querySelector('#image-modal'), null);
      }
    },
    true,
  );

  document.addEventListener('modal-close', function () {
    closeModals();
  });
})();

(function () {
  // Functions to open and close a modal
  function openModal($element) {
    $element.classList.add('is-active');
    document.dispatchEvent(new CustomEvent('modal-open'));
  }

  function closeModal($element) {
    $element.classList.remove('is-active');
  }

  function closeAllModals() {
    (document.querySelectorAll('.modal') || []).forEach(($modal) => {
      closeModal($modal);
    });
  }

  // Add a keyboard event to close all modals
  document.addEventListener('keydown', (event) => {
    event = event || window.event;

    if (Number(event.keyCode) === 27) {
      // Escape key
      closeAllModals();
    }
  });

  // global event to close modals
  document.addEventListener('modal-close', function () {
    closeAllModals();
  });

  // global click event to capture dynamically added modals.
  document.addEventListener('click', (event) => {
    const $trigger = event.target;
    let data;
    let q;
    let url;
    let $target;

    // Add a click event on buttons to open a specific modal
    if ($trigger.closest('.js-modal-trigger')) {
      $target = $trigger.closest('.js-modal-trigger');
      openModal(document.querySelector(`#${$target.dataset.target}`));
    }

    // Add a click event on various child elements to close the parent modal
    else if (
      $trigger.closest(
        '.modal-background, .modal-close, .modal-card-head .delete, .modal-card-foot .button',
      )
    ) {
      $target = $trigger.closest(
        '.modal-background, .modal-close, .modal-card-head .delete, .modal-card-foot .button',
      );
      closeModal($target.closest('.modal'));
    }

    // Share feedback
    else if ($trigger.closest('.modal button.share-feedback')) {
      $target = $trigger.closest('.modal button.share-feedback');
      const textarea = $target.parentNode.querySelectorAll('textarea')[0];

      data = {
        reportName: $target.hasAttribute('data-name')
          ? $target.getAttribute('data-name')
          : document.title,
        description: textarea.value,
        reportUrl: $target.hasAttribute('data-url')
          ? $target.getAttribute('data-url')
          : window.location.href,
      };
      url = Object.keys(data)
        .map(function (k) {
          return encodeURIComponent(k) + '=' + encodeURIComponent(data[k]);
        })
        .join('&');
      q = new XMLHttpRequest();
      q.open('post', '/Requests?handler=ShareFeedback&' + url, true);
      q.setRequestHeader('Content-Type', 'text/html;charset=UTF-8`');
      q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
      q.send();
      // ShowMessageBox('Thanks for the feedback.');
      textarea.value = '';
      closeAllModals();
    }

    // Request access
    else if ($trigger.closest('.modal button.request-access')) {
      $target = $trigger.closest('.modal button.request-access');

      const director = $target
        .closest('.modal')
        .querySelector('.director-name');

      console.log(director.value);

      if (director.value === null || director.value === '') {
        console.log('blank');
        const label = director.closest('.field.pt-5').querySelector('label');
        if (label) {
          label.insertAdjacentHTML(
            'afterend',
            '<p class="help is-danger">Recipients are required.</p>',
          );
        }

        return false;
      }

      data = {
        reportName: $target.getAttribute('report-name'),
        directorName: director.value,
        reportUrl: window.location.href,
      };
      url = Object.keys(data)
        .map(function (k) {
          return encodeURIComponent(k) + '=' + encodeURIComponent(data[k]);
        })
        .join('&');
      q = new XMLHttpRequest();
      q.open('post', '/Requests?handler=AccessRequest&' + url, true);
      q.setRequestHeader('Content-Type', 'text/html;charset=UTF-8`');
      q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
      q.send();
      // ShowMessageBox('Your request has been submitted.');
      closeAllModals();
    }
  });
})();
