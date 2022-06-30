(function () {
  document.execCommand('defaultParagraphSeparator', false, 'div');
  const getNewMail = null;
  const d = document;
  let newMesQ = null;
  let loadMesQ = null;
  let saveDraftQ = null;
  let deleteMes = null;
  let sendMes = null;
  const pollForNewMessages = function () {
    const div = document.createElement('div');

    if (newMesQ !== null) {
      newMesQ.abort();
    }

    newMesQ = new XMLHttpRequest();
    newMesQ.open('post', '/mail?handler=CheckForMail', true);
    newMesQ.setRequestHeader(
      'Content-Type',
      'application/x-www-form-urlencoded; charset=UTF-8',
    );
    newMesQ.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
    newMesQ.send();
    newMesQ.addEventListener('load', function () {
      div.innerHTML = DOMPurify.sanitize(newMesQ.responseText);
      parseNewMailData(div);
    });
  };

  const parseNewMailData = function (element) {
    const unreadMessage = d.querySelectorAll('.mail-unread-cnt');
    const allMessage = d.querySelectorAll('.mail-all-cnt');
    let x = 0;
    const newUnreadMessage = element.querySelector(
      '#unread_message_count',
    ).innerHTML;
    const newAllMessage = element.querySelector(
      '#total_message_count',
    ).innerHTML;
    const draftMessage = element.querySelector(
      '#draft_message_count',
    ).innerHTML;
    const not = element.querySelector('#new_message_alerts');

    if (d.querySelector('#mlbx-fldrsDrafts .mail-draft-cnt')) {
      d.querySelector('#mlbx-fldrsDrafts .mail-draft-cnt').innerHTML =
        draftMessage > 0 ? draftMessage : '';
    }

    for (x = 0; x < unreadMessage.length; x++) {
      if (newUnreadMessage > 0) {
        unreadMessage[x].style.display = 'inline-block';
        unreadMessage[x].innerHTML = DOMPurify.sanitize(newUnreadMessage);
      } else {
        unreadMessage[x].style.display = 'none';
        unreadMessage[x].innerHTML = '';
      }
    }

    for (x = 0; x < allMessage.length; x++) {
      allMessage[x].innerHTML = DOMPurify.sanitize(newAllMessage);
    }

    // Show new mail notification.
    // if (not.innerHTML.length > 0) {
    //   const notBox = document.querySelectorAll(
    //     '.mail-notification-container',
    //   )[0];
    //   notBox.innerHTML = DOMPurify.sanitize(not.innerHTML);
    //   notBox.style.transition = 'margin-right .5s';
    //   notBox.style.marginRight = '500px';

    //   // eslint-disable-next-line no-unused-vars
    //   const q = notBox.clientWidth; // Clear js cache
    //   setTimeout(function () {
    //     notBox.style.marginRight = '0px';
    //   }, 10_000);
    // }

    // Add new message previews, if they do not already exist.
    // sometimes mail check requests can cross paths.
    // only if in inbox
    if (d.querySelector('#mlbx-fldrsInbox.active') !== null) {
      const div = document.createElement('div');
      div.innerHTML = DOMPurify.sanitize(
        element.querySelector('#new_message_previews').innerHTML,
      );

      if (div.innerHTML.length > 0) {
        // 1 get existing repid

        const message = Array.prototype.slice.call(
          d.querySelectorAll('.mlbx-msgsMsg[data-rep-id]'),
        );
        let element;
        for (x = 0; x < message.length; x++) {
          element = div.querySelector(
            '.mlbx-msgsMsg[data-rep-id="' +
              message[x].getAttribute('data-rep-id') +
              '"]',
          );
          if (element !== null) {
            element.nextElementSibling.remove();
            element.remove();
          }
        }
      }
      // If(d.querySelector('.mlbx-msgsMsg.active')){

      if (div.querySelector('.mlbx-msgsMsg.active')) {
        div.querySelector('.mlbx-msgsMsg.active').classList.remove('active');
      }

      if (d.querySelector('.mlbx-msgs .ss-content').children.length > 0) {
        d.querySelector(
          '.mlbx-msgs .ss-content',
        ).children[0].insertAdjacentHTML(
          'beforebegin',
          DOMPurify.sanitize(div.innerHTML),
        );
      } else {
        d.querySelector('.mlbx-msgs .ss-content').innerHTML =
          DOMPurify.sanitize(div.innerHTML);
      }
    }
  };

  const loadFolder = function (folder, folderId) {
    d.querySelectorAll('.mlbx-rdr')[0].innerHTML = '';

    if (loadMesQ !== null) loadMesQ.abort();
    loadMesQ = new XMLHttpRequest();
    if (folder !== null)
      loadMesQ.open('get', 'mail?handler=Messages&folder=' + folder, true);
    if (typeof folderId !== 'undefined' && folderId !== null)
      loadMesQ.open('get', 'mail?handler=Messages?folderId=' + folderId, true);
    loadMesQ.setRequestHeader(
      'Content-Type',
      'application/x-www-form-urlencoded; charset=UTF-8',
    );
    loadMesQ.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
    loadMesQ.send();
    loadMesQ.addEventListener('load', function () {
      const previousBox = d.querySelectorAll('.mlbx-msgsScroll')[0];
      previousBox.classList.remove('ss-container');
      previousBox.innerHTML = DOMPurify.sanitize(loadMesQ.responseText);
      document.dispatchEvent(
        new CustomEvent('ss-load', {
          cancelable: true,
          detail: {
            el: previousBox,
          },
        }),
      );

      // Load first message
      if (
        folder === 'inbox' ||
        folder === 'deleted' ||
        folder === 'sent' ||
        (typeof folderId !== 'undefined' && folderId !== null)
      ) {
        if (d.querySelector('.mlbx-msgs .mlbx-msgsMsg.active'))
          loadMessageDetails(
            d
              .querySelector('.mlbx-msgs .mlbx-msgsMsg.active')
              .getAttribute('data-rep-id'),
          );
      } else if (folder === 'drafts') {
        if (d.querySelector('.mlbx-msgs .mlbx-msgsMsg.active')) {
          loadDraft(
            d
              .querySelector('.mlbx-msgs .mlbx-msgsMsg.active')
              .getAttribute('data-draft-id'),
          );
        } else {
          startNewMessage();
        }
      }
    });
  };

  const saveDraft = function (form) {
    if (saveDraftQ !== null && form.querySelector('input[name="draftId')) {
      saveDraftQ.abort();
    }

    if (saveDraftQ === null || form.querySelector('input[name="draftId')) {
      const data = {
        To: form.querySelector('.dd-cntr .dd-hdn').innerHTML,
        Subject: form.querySelector('.mlbx-newMsgSubjIpt').innerHTML,
        Message: form.querySelector('.mlbx-newMsgMsg').innerHTML,
        Text: form
          .querySelector('.mlbx-newMsgMsgBx')
          .textContent.replace(/\n/g, ' '),
        MsgId: null,
        ConvId: null,
      };
      if (form.querySelector('input[name="draftId')) {
        data.DraftId = form
          .querySelector('input[name="draftId')
          .getAttribute('value');
      }

      saveDraftQ = new XMLHttpRequest();
      saveDraftQ.open('post', 'mail?handler=SaveDraft', true);
      saveDraftQ.setRequestHeader(
        'Content-Type',
        'application/x-www-form-urlencoded; charset=UTF-8',
      );
      saveDraftQ.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
      saveDraftQ.send(JSON.stringify(data));
      saveDraftQ.addEventListener('load', function () {
        if (form.querySelector('input[name="draftId') === null) {
          const ipt = d.createElement('input');
          ipt.setAttribute('name', 'draftId');
          ipt.setAttribute('value', saveDraftQ.responseText);
          ipt.style.display = 'none';
          form.append(ipt);
        }
      });
    }

    /* 1 if form has not id send to db and get id
               2 add id to form for reference
               3 save to session storage w/ id
               4 send to server sometimes > when checking for mail send drafts
            */
  };

  const loadDraft = function (id) {
    /*
            1 open blank message with params from storage (or server) */
    saveDraftQ = null;
    if (loadMesQ) {
      loadMesQ.abort();
    }

    loadMesQ = new XMLHttpRequest();
    loadMesQ.open('get', 'mail?handler=DraftDetails&id=' + id, true);
    loadMesQ.setRequestHeader(
      'Content-Type',
      'application/x-www-form-urlencoded; charset=UTF-8',
    );
    loadMesQ.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
    loadMesQ.send();
    loadMesQ.addEventListener('load', function () {
      const div = d.createElement('div');
      div.innerHTML = loadMesQ.responseText;
      const ipt = d.createElement('input');
      ipt.setAttribute('name', 'draftId');
      ipt.setAttribute('value', id);
      ipt.style.display = 'none';
      div.querySelector('form').append(ipt);

      d.querySelectorAll('.mlbx-rdr')[0].innerHTML = DOMPurify.sanitize(
        div.querySelectorAll('.mlbx-newMsg')[0].outerHTML,
      );
      document.dispatchEvent(
        new CustomEvent('dropdown', {
          cancelable: true,
          detail: {
            el: d.querySelector('#new_msg_dynamic-dropdown'),
          },
        }),
      );
      document.dispatchEvent(
        new CustomEvent('ss-load', {
          cancelable: true,
          detail: {
            el: d.querySelector('.mlbx-rdr .mlbx-newMsg'),
          },
        }),
      );
      d.querySelector('.mlbx-newMsg form.mlbx-newMsgForm').addEventListener(
        'input',
        function () {
          saveDraft(this);
        },
      );
      d.querySelector('.mlbx-newMsg form.mlbx-newMsgForm').addEventListener(
        'change',
        function () {
          saveDraft(this);
        },
      );
    });
  };

  const markMessageRead = function (id) {
    const q = new XMLHttpRequest();
    q.open('post', 'mail?handler=markMessageRead&id=' + id, true);
    q.setRequestHeader(
      'Content-Type',
      'application/x-www-form-urlencoded; charset=UTF-8',
    );
    q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
    q.send();
  };

  const startNewMessage = function (to, subject, message, convid) {
    saveDraftQ = null;
    // Mark current message as read.
    const act = d.querySelectorAll('.mlbx-msgsMsg.active.unread');
    let x = 0;
    for (x; x < act.length; x++) {
      act[x].classList.remove('unread');
      markMessageRead(act[x].getAttribute('data-rep-id'));
    }

    // Unmark active message
    if (d.querySelector('.mlbx-msgsMsg.active'))
      d.querySelector('.mlbx-msgsMsg.active').classList.remove('active');
    const messagebox = d.querySelectorAll('.mlbx-rdr')[0];
    messagebox.innerHTML = '';
    messagebox.append(
      d.querySelector('.new_message_template .mlbx-newMsg').cloneNode(true),
    );

    messagebox
      .querySelector('form.mlbx-newMsgForm')
      .addEventListener('input', function () {
        saveDraft(this);
      });
    messagebox
      .querySelector('form.mlbx-newMsgForm')
      .addEventListener('change', function () {
        saveDraft(this);
      });

    document.dispatchEvent(
      new CustomEvent('dropdown', {
        cancelable: true,
        detail: {
          el: messagebox.querySelector('#new_msg_dynamic-dropdown'),
        },
      }),
    );

    if (typeof convid !== 'undefined') {
      messagebox.querySelector('#convid').value = convid;
    }

    if (typeof subject !== 'undefined') {
      messagebox.querySelector('.mlbx-newMsgSubjIpt').innerHTML =
        DOMPurify.sanitize(subject);
    }

    if (typeof message !== 'undefined') {
      messagebox.querySelector('.mlbx-newMsgMsg').innerHTML =
        DOMPurify.sanitize(message.innerHTML);
    }

    if (typeof to !== 'undefined' && to !== null) {
      // Build input values
      const ddw = messagebox.querySelector('.dd-cntr .dd-wrp');
      const dds = messagebox.querySelector('.dd-cntr select.dd-hdn');
      to.forEach(function (t) {
        dds.innerHTML +=
          '<option selected="selected" class="' +
          DOMPurify.sanitize(t[2]) +
          '" value="' +
          DOMPurify.sanitize(t[0]) +
          '">' +
          DOMPurify.sanitize(t[1]) +
          '</option>';
        ddw.children[0].insertAdjacentHTML(
          'beforebegin',
          '<div class="dd-itm ' +
            DOMPurify.sanitize(t[2]) +
            '">' +
            DOMPurify.sanitize(t[1]) +
            '</div>',
        );
        // Move placeholder after input
        // dd.find('.dynamic-dropdown-input-container').after(dd.find('.placeholder'))
      });
    }

    document.dispatchEvent(
      new CustomEvent('ss-load', {
        cancelable: true,
        detail: {
          el: messagebox.querySelector('.mlbx-newMsg'),
        },
      }),
    );
  };

  const loadMessageDetails = function (id) {
    if (!d.querySelectorAll('.mlbx-rdr')[0]) {
      return false;
    }

    if (loadMesQ) {
      loadMesQ.abort();
    }

    loadMesQ = new XMLHttpRequest();
    loadMesQ.open('get', 'mail?handler=MessageDetails&id=' + id, true);
    loadMesQ.setRequestHeader(
      'Content-Type',
      'application/x-www-form-urlencoded; charset=UTF-8',
    );
    loadMesQ.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
    loadMesQ.send();
    loadMesQ.addEventListener('load', function () {
      if (d.querySelectorAll('.mlbx-rdr')[0])
        d.querySelectorAll('.mlbx-rdr')[0].innerHTML = DOMPurify.sanitize(
          loadMesQ.responseText,
        );
      document.dispatchEvent(new CustomEvent('ss-load'));
    });
  };

  const getNextMessage = function (element, selector) {
    // Get the next sibling element
    let sibling = element.nextElementSibling;

    // If there's no selector, return the first sibling
    if (!selector) return sibling;

    // If the sibling matches our selector, use it
    // If not, jump to the next sibling and continue the loop
    while (sibling) {
      if (sibling.matches(selector)) return sibling;
      sibling = sibling.nextElementSibling;
    }

    // Get the next sibling element
    sibling = element.previousElementSibling;

    // If there's no selector, return the first sibling
    if (!selector) return sibling;

    // If the sibling matches our selector, use it
    // If not, jump to the next sibling and continue the loop
    while (sibling) {
      if (sibling.matches(selector)) return sibling;
      sibling = sibling.previousElementSibling;
    }
  };

  const changeActiveMessage = function (me) {
    // If active folder is inbox

    const act = d.querySelectorAll('.mlbx-msgsMsg.active:not(.deleted');
    let x = 0;
    for (x; x < act.length; x++) {
      act[x].classList.remove('active');
      if (
        d.querySelector('#mlbx-fldrsInbox.active') &&
        act[x].classList.contains('unread')
      ) {
        act[x].classList.remove('unread');
        markMessageRead(act[x].getAttribute('data-rep-id'));
      }
    }

    me.classList.add('active');

    if (
      d.querySelector('#mlbx-fldrsInbox.active') &&
      me.classList.contains('unread')
    ) {
      me.classList.remove('unread');
      markMessageRead(me.getAttribute('data-rep-id'));
    }

    // Load message contents
    if (!d.querySelector('#mlbx-fldrsDrafts.active')) {
      loadMessageDetails(me.getAttribute('data-rep-id'));
    } else {
      loadDraft(me.getAttribute('data-draft-id'));
    }

    // Update mail icon
    pollForNewMessages();
  };

  setInterval(function () {
    if (typeof window.ajaxOn === 'undefined' || window.ajaxOn === true) {
      pollForNewMessages();
    }
  }, 10_000); // 10000 milliseconds = 10 seconds

  document.addEventListener('loadMessage', function (event) {
    if (typeof event.detail !== 'undefined') {
      loadMessageDetails(event.detail.id);
    }
  });

  d.addEventListener('click', function (event) {
    if (event.target.closest('.mlbx-msgsMsg')) {
      changeActiveMessage(event.target.closest('.mlbx-msgsMsg'));
    } else if (event.target.closest('.mail-new-message')) {
      const from = event.target
        .closest('.mail-new-message')
        .getAttribute('data-from');
      const fromId = event.target
        .closest('.mail-new-message')
        .getAttribute('data-fromid');
      if (from && fromId) {
        startNewMessage([[fromId, from]]);
      } else {
        startNewMessage();
      }
    } else if (event.target.closest('.mlbx-usrDrp')) {
      // User dropdown
      event.target
        .closest('.mlbx-usrDrp')
        .querySelector('.mlbx-usrDrpCtr').style.display = 'block';
      event.target
        .closest('.mlbx-usrDrp')
        .querySelector('.mlbx-usrDrpCtr')
        .addEventListener('mouseleave', function mouseleave() {
          this.style.removeProperty('display');
          this.removeEventListener('mouseleave', mouseleave, false);
        });
    } else if (event.target.closest('#mlbx-fldrsDrafts')) {
      loadFolder('drafts');
      document
        .querySelector('.mlbx-fldrsFldr.active')
        .classList.remove('active');
      event.target.closest('#mlbx-fldrsDrafts').classList.add('active');
    } else if (event.target.closest('#mlbx-fldrsInbox')) {
      loadFolder('inbox');
      document
        .querySelector('.mlbx-fldrsFldr.active')
        .classList.remove('active');
      event.target.closest('#mlbx-fldrsInbox').classList.add('active');
    } else if (event.target.closest('#mlbx-fldrsSent')) {
      loadFolder('sent');
      document
        .querySelector('.mlbx-fldrsFldr.active')
        .classList.remove('active');
      event.target.closest('#mlbx-fldrsSent').classList.add('active');
    } else if (event.target.closest('#mlbx-fldrsDeleted')) {
      loadFolder('deleted');
      document
        .querySelector('.mlbx-fldrsFldr.active')
        .classList.remove('active');
      event.target.closest('#mlbx-fldrsDeleted').classList.add('active');
    } else if (event.target.closest('.mail-delete-message')) {
      if (getNewMail !== null) getNewMail.abort();

      // Get repid, or draft if draft
      const message = d.querySelector('.mlbx-msgsMsg.active:not(.deleted)');
      if (message === null) return;
      const nextMessage = getNextMessage(message, '.mlbx-msgsMsg');

      if (message.hasAttribute('data-draft-id')) {
        // Delete draft
        deleteMes = new XMLHttpRequest();
        deleteMes.open(
          'post',
          'mail?handler=DeleteDraft&id=' +
            message.getAttribute('data-draft-id'),
          true,
        );
        deleteMes.setRequestHeader(
          'Content-Type',
          'application/x-www-form-urlencoded; charset=UTF-8',
        );
        deleteMes.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
        deleteMes.send();

        // Show next message
        if (typeof nextMessage !== 'undefined') {
          loadDraft(nextMessage.getAttribute('data-draft-id'));
          nextMessage.classList.add('active');
        } else {
          d.querySelector('.mlbx-rdr').innerHTML = '';
        }
      } else if (message.hasAttribute('data-rep-id')) {
        // Delete message
        deleteMes = new XMLHttpRequest();
        deleteMes.open(
          'post',
          'mail?handler=DeleteMessage&id=' +
            message.getAttribute('data-rep-id'),
          true,
        );
        deleteMes.setRequestHeader(
          'Content-Type',
          'application/x-www-form-urlencoded; charset=UTF-8',
        );
        deleteMes.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
        deleteMes.send();

        // Show next message
        if (typeof nextMessage !== 'undefined') {
          loadMessageDetails(nextMessage.getAttribute('data-rep-id'));
          nextMessage.classList.add('active');
        } else {
          d.querySelector('.mlbx-rdr').innerHTML = '';
        }
      }

      const w = parseFloat(message.clientWidth);
      const h = parseFloat(message.clientHeight);
      message.style.transition =
        'margin-left 150ms, margin-right 150ms, margin-top 150ms';
      message.style.zIndex = '-1';

      // eslint-disable-next-line no-unused-vars
      const clear = message.clientHeight; // Clear js cache

      message.style.marginLeft = -w + 'px';
      message.style.marginRight = w + 'px';
      message.style.marginTop = -h + 'px';
      message.classList.add('deleted');
      setTimeout(function () {
        if (message !== null) {
          message.nextElementSibling.remove();
          message.remove();
        }
      }, 150);
    }
  });

  d.addEventListener(
    'submit',
    function (event) {
      if (event.target.closest('.mlbx-newMsgForm')) {
        event.preventDefault();

        const form = event.target.closest('.mlbx-newMsgForm');

        // To
        const to = [];
        const recp = form.querySelectorAll(
          '.mlbx-newMsgRecip select option, .mail-to input[type="hidden"]',
        );

        if (recp !== null && recp.length > 0) {
          for (let x = 0; x < recp.length; x++) {
            const g = recp[x].classList.contains('group') ? 'g' : '';
            to.push({
              UserId: recp[x].value,
              Type: g,
            });
          }
        } else {
          const label = form
            .querySelector('.mlbx-newMsgRecip,.mail-to')
            .parentElement.querySelector('label');
          if (label) {
            label.insertAdjacentHTML(
              'afterend',
              '<p class="help is-danger">Recipients are required.</p>',
            );
          }

          return false;
        }

        // Subject
        const subject = form.querySelector('.mlbx-newMsgSubjIpt')
          ? form.querySelector('.mlbx-newMsgSubjIpt').textContent
          : form.querySelector('input.mail-subject').value;

        // Message
        let message;
        let text;
        if (form.querySelector('.mlbx-newMsgMsg')) {
          message = form.querySelector('.mlbx-newMsgMsg').innerHTML;
          text = form
            .querySelector('.mlbx-newMsgMsg')
            .textContent.replace(/\n/g, ' ');
        } else {
          message = form.querySelector('textarea[name="mail-message"]').value;
          text = message;
        }

        const share = form.querySelector('.mlbx-share') ? '1' : '0';
        const shareName = form.querySelector('.mlbx-shareName')
          ? form.querySelector('.mlbx-shareName').value
          : '';
        const shareUrl = form.querySelector('.mlbx-shareUrl')
          ? form.querySelector('.mlbx-shareUrl').value
          : window.location.href;

        const data = {
          To: JSON.stringify(to),
          Subject: subject,
          Message: message,
          Text: text,
          Share: share,
          ShareName: shareName,
          ShareUrl: shareUrl,
        };

        if (form.querySelector('input[name="draftId"]') !== null) {
          const draftId = form.querySelector('input[name="draftId"]').value;
          data.DraftId = draftId;
          if (
            d.querySelector('.mlbx-msgsMsg[data-draft-id="' + draftId + '"]')
          ) {
            d.querySelector(
              '.mlbx-msgsMsg[data-draft-id="' + draftId + '"]',
            ).remove();
          }
        }

        sendMes = new XMLHttpRequest();
        sendMes.open('post', form.getAttribute('action'), true);
        sendMes.setRequestHeader(
          'Content-Type',
          'application/x-www-form-urlencoded; charset=UTF-8',
        );
        sendMes.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
        sendMes.send(JSON.stringify(data));
        sendMes.addEventListener('load', function () {
          pollForNewMessages();
        });
        // If we are in drafts folder, remove draft.

        document.dispatchEvent(
          new CustomEvent('show-message', {
            cancelable: true,
            detail: {
              value: 'Messages sent! ¯\\_(ツ)_/¯',
            },
          }),
        );

        if (d.querySelector('.mlbx-rdr') !== null) {
          d.querySelector('.mlbx-rdr').innerHTML = '';
        }
        // Close modal if its open

        d.dispatchEvent(new CustomEvent('modal-close'));
      }
    },
    false,
  );

  d.addEventListener('click', function (event) {
    let m;
    let convid;
    let subject;
    let f;
    let from;
    let x;
    let message;
    let messageLiner;
    let dbox;
    if (
      event.target.closest('.mail-reply') &&
      d.querySelector('#mlbx-fldrsInbox.active') !== null
    ) {
      m = d.querySelector('.mlbx-rdrHead');
      convid = m.getAttribute('data-conv-id');
      subject = 'Re: ' + m.querySelector('.mailbox-subject').textContent;
      f = m.querySelector('div.mailbox-sender-user');
      from = [
        [
          f.getAttribute('data-user-id'),
          f.querySelector('span').textContent,
          '',
        ],
      ];
      message = document.createElement('div');
      messageLiner = document.createElement('div');
      messageLiner.classList.add('message-reply');
      messageLiner.innerHTML = DOMPurify.sanitize(
        d.querySelector('.mlbx-rdr .ss-content').innerHTML,
      );

      messageLiner.querySelector('.mlbx-rdrHead-controls').remove();

      dbox = messageLiner.querySelectorAll('.mlbx-usrDrp');
      for (x = 0; x < dbox.length; x++) {
        dbox[x].remove();
      }

      message.innerHTML = '<br><br><br>';
      message.append(messageLiner);

      startNewMessage(from, subject, message, convid);
    } else if (
      event.target.closest('.mail-reply-all') &&
      d.querySelector('#mlbx-fldrsInbox.active') !== null
    ) {
      m = d.querySelector('.mlbx-rdrHead');
      convid = m.getAttribute('data-conv-id');
      subject = 'Re: ' + m.querySelector('.mailbox-subject').textContent;
      f = m.querySelectorAll('div.mailbox-sender-user');
      const t = m.querySelectorAll('div.mailbox-to-user');
      from = [];
      for (x = 0; x < f.length; x++) {
        from.push([
          f[x].getAttribute('data-user-id'),
          f[x].querySelector('span').textContent,
          f[x].classList.contains('group') ? 'group' : '',
        ]);
      }

      for (x = 0; x < t.length; x++) {
        from.push([
          t[x].getAttribute('data-user-id'),
          t[x].querySelector('span').textContent,
          t[x].classList.contains('group') ? 'group' : '',
        ]);
      }
      // Remove dups

      const seen = {};
      const returnValueArray = [];
      for (let i = 0; i < from.length; i++) {
        if (!(from[i] in seen)) {
          returnValueArray.push(from[i]);
          seen[from[i]] = true;
        }
      }

      from = returnValueArray;
      message = document.createElement('div');
      messageLiner = document.createElement('div');
      messageLiner.classList.add('message-reply');
      messageLiner.innerHTML = DOMPurify.sanitize(
        d.querySelector('.mlbx-rdr .ss-content').innerHTML,
      );

      messageLiner.querySelector('.mlbx-rdrHead-controls').remove();

      dbox = messageLiner.querySelectorAll('.mlbx-usrDrp');
      for (x = 0; x < dbox.length; x++) {
        dbox[x].remove();
      }

      message.innerHTML = '<br><br><br>';
      message.append(messageLiner);

      startNewMessage(from, subject, message, convid);
    }
  });
})();
