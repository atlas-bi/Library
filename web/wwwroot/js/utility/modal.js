(function () {
  function closeModals(el) {
    if (typeof el == 'undefined') {
      // hide all other modals
      [].forEach.call(document.getElementsByClassName('mdl-o'), function (i) {
        i.classList.remove('mdl-o');
        i.dispatchEvent(new CustomEvent('mdl-close'));
      });
    } else {
      el.classList.remove('mdl-o');
      el.dispatchEvent(new CustomEvent('mdl-close'));
    }

    if (!(document.getElementsByClassName('mdl-o').length > 0)) {
      document.getElementsByTagName('body')[0].classList.remove('b-mdl');
    }
  }

  function showModal(el, e) {
    var d = document;
    d.getElementsByTagName('body')[0].classList.toggle('b-mdl');
    el.classList.toggle('mdl-o');
    el.dispatchEvent(
      new CustomEvent('mdl-open', {
        cancelable: true,
        detail: {
          e: e,
        },
      }),
    );
  }

  document.addEventListener(
    'click',
    function (e) {
      var el,
        d = document; // open modal
      if (
        e.target.closest('[data-toggle]') &&
        e.target.closest('[data-toggle]').getAttribute('data-toggle') == 'mdl'
      ) {
        el = d.getElementById(
          e.target.closest('[data-toggle]').getAttribute('data-target'),
        );
        showModal(el, e.target.closest('[data-toggle]'));
      } // close modal
      else if (
        (e.target.closest('[data-dismiss]') &&
          e.target.closest('[data-dismiss]').getAttribute('data-dismiss') ==
            'mdl') ||
        //e.target.closest('.mdl:not(#editModal)') ||
        e.target.closest('.editorMdl-close')
      ) {
        // close closest modal
        closeModals(e.target.closest('.mdl'));
      } else if (e.target.closest('.pop')) {
        d.getElementsByClassName('imagepreview')[0].setAttribute(
          'src',
          e.target.closest('.pop').children[0].getAttribute('src'),
        );
        showModal(d.getElementById('image-modal'), null);
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
  function openModal($el) {
    $el.classList.add('is-active');
    document.dispatchEvent(new CustomEvent('modal-open'));
  }

  function closeModal($el) {
    $el.classList.remove('is-active');
  }

  function closeAllModals() {
    (document.querySelectorAll('.modal') || []).forEach(($modal) => {
      closeModal($modal);
    });
  }

  // Add a keyboard event to close all modals
  document.addEventListener('keydown', (event) => {
    const e = event || window.event;

    if (e.keyCode === 27) {
      // Escape key
      closeAllModals();
    }
  });

  // global event to close modals
  document.addEventListener('modal-close', function () {
    closeAllModals();
  });

  // global click event to capture dynamically added modals.
  document.addEventListener('click', ($e) => {
    var $trigger = $e.target,
      data,
      q,
      url,
      $target;

    // Add a click event on buttons to open a specific modal
    if ($trigger.closest('.js-modal-trigger')) {
      $target = $trigger.closest('.js-modal-trigger');
      openModal(document.getElementById($target.dataset.target));
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

    // share feedback
    else if ($trigger.closest('.modal button.share-feedback')) {
      $target = $trigger.closest('.modal button.share-feedback');
      var textarea = $target.parentNode.getElementsByTagName('textarea')[0];

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
      ShowMessageBox('Thanks for the feedback.');
      textarea.value = '';
      closeAllModals();
    }

    // request access
    else if ($trigger.closest('.modal button.request-access')) {
      $target = $trigger.closest('.modal button.request-access');
      data = {
        reportName: $target.getAttribute('report-name'),
        directorName: $target.closest('.modal').querySelector('.director-name')
          .value,
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
      ShowMessageBox('Your request has been submitted.');
      closeAllModals();
    }
  });
})();
