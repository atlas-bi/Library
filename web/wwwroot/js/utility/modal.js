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
        q,
        data,
        url,
        d = document; // open modal
      console.log(e.target.closest('[data-toggle]'));
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
      } // request access
      else if (e.target.closest('.request-access')) {
        data = {
          reportName: document.querySelector('.report-name-clean').innerHTML,
          directorName: d.getElementById('director-name').value,
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
        closeModals();
      } // report a problem
      else if (e.target.closest('.share-feedback')) {
        data = {
          reportName: document.title,
          description:
            e.target.parentNode.previousElementSibling.getElementsByTagName(
              'textarea',
            )[0].value,
          reportUrl: window.location.href,
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
        e.target.parentNode.previousElementSibling.getElementsByTagName(
          'textarea',
        )[0].value = '';
        closeModals();
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
  document
    .getElementById('shareModal')
    .addEventListener('mdl-open', function (e) {
      var button = e.detail.e,
        name = button.getAttribute('data-name');

      if (
        !button.hasAttribute('data-url') ||
        button.getAttribute('data-url') == null ||
        button.getAttribute('data-url') == ''
      ) {
        url = window.location.href.replace(window.location.origin, '');
      } else {
        url = button.getAttribute('data-url');
      }

      var modal = this;
      modal.querySelector('.mlbx-newMsgSubjIpt').innerHTML = 'Share: ' + name;
      modal.querySelector('.mlbx-newMsgMsg').innerHTML =
        '<div>Hi!<br>I would like to share this ' +
        button.getAttribute('data-type') +
        " with you.</div><br><a href='" +
        url +
        "'>" +
        name +
        '</a><br><br><div>Check it out sometime!</div><br><div>Regards!<br>' +
        modal.querySelector('.mlbx-newMsgSender span:nth-child(2)').innerHTML;
      modal.querySelector('.mlbx-shareName').value = name;
      modal.querySelector('.mlbx-shareUrl').value = url;

      // remove an "to" recips.
      var toName = modal.querySelectorAll('.dd-itm');
      for (var x = 0; x < toName.length; x++) {
        toName[x].parentElement.removeChild(toName[x]);
      }
      var toId = modal.querySelectorAll('.dd-hdn option');
      for (x = 0; x < toId.length; x++) {
        toId[x].parentElement.removeChild(toId[x]);
      }
      document.dispatchEvent(
        new CustomEvent('dropdown', {
          cancelable: true,
          detail: {
            el: modal.querySelector('#new_msg_dynamic-dropdown:not(.dd-hdn)'),
          },
        }),
      );
    });
  document
    .getElementById('shareFeedback')
    .addEventListener('mdl-open', function (e) {
      var button = e.detail.e; // Button that triggered the modal

      var name = button.getAttribute('data-name'); // Extract info from data-* attributes

      var url = window.location.origin;

      if (
        !button.hasAttribute('data-url') ||
        button.getAttribute('data-url') == null ||
        button.getAttribute('data-url') == ''
      ) {
        url = window.location.href;
      } else {
        url += button.getAttribute('data-url');
      }

      var modal = this;
      modal.querySelector('.mdl-h h2').innerHTML =
        button.getAttribute('data-tooltip');
      modal.querySelector('.share-feedback').setAttribute('report-name', name);
    });

  document
    .getElementById('requestAccessModal')
    .addEventListener('mdl-open', function (e) {
      var button = e.detail.e;
      var name = button.getAttribute('data-name');
      var modal = this;

      modal.querySelector('.report-name-clean').innerHTML = name;
    });
  document.addEventListener('modal-close', function () {
    closeModals();
  });
})();
