(function () {
  document.execCommand('defaultParagraphSeparator', false, 'div');
  var mail_check_rate = 30 * 1000,
    getMessageAjax = null,
    getNewMail = null,
    d = document,
    newMesQ = null,
    loadMesQ = null,
    saveDraftQ = null,
    deleteMes = null,
    sendMes = null,
    pollForNewMessages = function () {
      var div = document.createElement('div');

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
      newMesQ.onload = function () {
        div.innerHTML = newMesQ.responseText;
        parse_new_mail_data(div);
      };
    },
    parse_new_mail_data = function (e) {
      var unreadMsg = d.getElementsByClassName('mail-unread-cnt'),
        allMsg = d.getElementsByClassName('mail-all-cnt'),
        x = 0,
        newUnreadMsg = e.querySelector('#unread_message_count').innerHTML,
        newAllMsg = e.querySelector('#total_message_count').innerHTML,
        draftMsg = e.querySelector('#draft_message_count').innerHTML,
        not = e.querySelector('#new_message_alerts');

      if (d.querySelector('#mlbx-fldrsDrafts .mail-draft-cnt')) {
        d.querySelector('#mlbx-fldrsDrafts .mail-draft-cnt').innerHTML =
          draftMsg > 0 ? draftMsg : '';
      }

      for (var x; x < unreadMsg.length; x++) {
        if (newUnreadMsg > 0) {
          unreadMsg[x].style.display = 'inline-block';
          unreadMsg[x].innerHTML = newUnreadMsg;
        } else {
          unreadMsg[x].style.display = 'none';
          unreadMsg[x].innerHTML = '';
        }
      }

      for (var x; x < allMsg.length; x++) {
        allMsg[x].innerHTML = newAllMsg;
      }

      // show new mail notification.
      if (not.innerHTML.length > 0) {
        var notBox = document.getElementsByClassName(
          'mail-notification-container',
        )[0];
        notBox.innerHTML = not.innerHTML;
        notBox.style.transition = 'margin-right .5s';
        notBox.style.marginRight = '500px';
        var q = notBox.clientWidth;
        setTimeout(function () {
          notBox.style.marginRight = '0px';
        }, 5000);
      }

      // add new message previews, if they do not already exist.
      // sometimes mail check requests can cross paths.
      // only if in inbox
      if (d.querySelector('#mlbx-fldrsInbox.active') !== null) {
        var div = document.createElement('div');
        div.innerHTML = e.querySelector('#new_message_previews').innerHTML;

        if (div.innerHTML.length > 0) {
          //1 get existing repid

          var msg = Array.prototype.slice.call(
              d.querySelectorAll('.mlbx-msgsMsg[data-rep-id]'),
            ),
            el;
          for (var x = 0; x < msg.length; x++) {
            el = div.querySelector(
              '.mlbx-msgsMsg[data-rep-id="' +
                msg[x].getAttribute('data-rep-id') +
                '"]',
            );
            if (el !== null) {
              el.parentElement.removeChild(el.nextElementSibling);
              el.parentElement.removeChild(el);
            }
          }
        }
        //if(d.querySelector('.mlbx-msgsMsg.active')){

        if (div.querySelector('.mlbx-msgsMsg.active')) {
          div.querySelector('.mlbx-msgsMsg.active').classList.remove('active');
        }

        if (d.querySelector('.mlbx-msgs .ss-content').children.length > 0) {
          d.querySelector(
            '.mlbx-msgs .ss-content',
          ).children[0].insertAdjacentHTML('beforebegin', div.innerHTML);
        } else {
          d.querySelector('.mlbx-msgs .ss-content').innerHTML = div.innerHTML;
        }
      }
    },
    LoadFolder = function (folder, folderId) {
      d.getElementsByClassName('mlbx-rdr')[0].innerHTML = '';

      if (loadMesQ !== null) loadMesQ.abort();
      loadMesQ = new XMLHttpRequest();
      if (folder !== null)
        loadMesQ.open('get', 'mail?handler=Messages&folder=' + folder, true);
      if (typeof folderId !== 'undefined' && folderId !== null)
        loadMesQ.open(
          'get',
          'mail?handler=Messages?folderId=' + folderId,
          true,
        );
      loadMesQ.setRequestHeader(
        'Content-Type',
        'application/x-www-form-urlencoded; charset=UTF-8',
      );
      loadMesQ.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
      loadMesQ.send();
      loadMesQ.onload = function () {
        var prevBox = d.getElementsByClassName('mlbx-msgsScroll')[0];
        prevBox.classList.remove('ss-container');
        prevBox.innerHTML = loadMesQ.responseText;
        document.dispatchEvent(
          new CustomEvent('ss-load', {
            cancelable: true,
            detail: {
              el: prevBox,
            },
          }),
        );

        // load first message
        if (
          folder == 'inbox' ||
          folder == 'deleted' ||
          folder == 'sent' ||
          (typeof folderId !== 'undefined' && folderId !== null)
        ) {
          if (d.querySelector('.mlbx-msgs .mlbx-msgsMsg.active'))
            LoadMessageDetails(
              d
                .querySelector('.mlbx-msgs .mlbx-msgsMsg.active')
                .getAttribute('data-rep-id'),
            );
        } else if (folder == 'drafts') {
          if (d.querySelector('.mlbx-msgs .mlbx-msgsMsg.active')) {
            LoadDraft(
              d
                .querySelector('.mlbx-msgs .mlbx-msgsMsg.active')
                .getAttribute('data-draft-id'),
            );
          } else {
            StartNewMessage();
          }
        }
      };
    },
    SaveDraft = function (form) {
      if (saveDraftQ !== null && form.querySelector('input[name="draftId')) {
        saveDraftQ.abort();
      }

      if (saveDraftQ === null || form.querySelector('input[name="draftId')) {
        var data = {
          To: form.querySelector('.dd-cntr .dd-hdn').innerHTML,
          Subject: form.querySelector('.mlbx-newMsgSubjIpt').innerHTML,
          Message: form.querySelector('.mlbx-newMsgMsg').innerHTML,
          Text: form
            .querySelector('.mlbx-newMsgMsgBx')
            .innerText.replace(/\n/g, ' '),
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
        saveDraftQ.onload = function () {
          if (form.querySelector('input[name="draftId') == null) {
            var ipt = d.createElement('input');
            ipt.setAttribute('name', 'draftId');
            ipt.setAttribute('value', saveDraftQ.responseText);
            ipt.style.display = 'none';
            form.appendChild(ipt);
          }
        };
      }

      /* 1 if form has not id send to db and get id
               2 add id to form for reference
               3 save to session storage w/ id
               4 send to server sometimes > when checking for mail send drafts
            */
    },
    LoadDraft = function (id) {
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
      loadMesQ.onload = function () {
        var div = d.createElement('div');
        div.innerHTML = loadMesQ.responseText;
        var ipt = d.createElement('input');
        ipt.setAttribute('name', 'draftId');
        ipt.setAttribute('value', id);
        ipt.style.display = 'none';
        div.querySelector('form').appendChild(ipt);

        d.getElementsByClassName('mlbx-rdr')[0].innerHTML =
          div.getElementsByClassName('mlbx-newMsg')[0].outerHTML;
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
          function (e) {
            SaveDraft(this);
          },
        );
        d.querySelector('.mlbx-newMsg form.mlbx-newMsgForm').addEventListener(
          'change',
          function (e) {
            SaveDraft(this);
          },
        );
      };
    },
    MarkMessageRead = function (id) {
      q = new XMLHttpRequest();
      q.open('post', 'mail?handler=MarkMessageRead&id=' + id, true);
      q.setRequestHeader(
        'Content-Type',
        'application/x-www-form-urlencoded; charset=UTF-8',
      );
      q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
      q.send();
    },
    StartNewMessage = function (to, subject, message, convid) {
      saveDraftQ = null;
      // mark current message as read.
      var act = d.querySelectorAll('.mlbx-msgsMsg.active.unread'),
        x = 0,
        el,
        q;
      for (x; x < act.length; x++) {
        act[x].classList.remove('unread');
        MarkMessageRead(act[x].getAttribute('data-rep-id'));
      }
      // unmark active message
      if (d.querySelector('.mlbx-msgsMsg.active'))
        d.querySelector('.mlbx-msgsMsg.active').classList.remove('active');
      var messagebox = d.getElementsByClassName('mlbx-rdr')[0];
      messagebox.innerHTML = '';
      messagebox.appendChild(
        d.querySelector('.new_message_template .mlbx-newMsg').cloneNode(true),
      );

      messagebox
        .querySelector('form.mlbx-newMsgForm')
        .addEventListener('input', function (e) {
          SaveDraft(this);
        });
      messagebox
        .querySelector('form.mlbx-newMsgForm')
        .addEventListener('change', function (e) {
          SaveDraft(this);
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
        messagebox.querySelector('.mlbx-newMsgSubjIpt').innerHTML = subject;
      }
      if (typeof message !== 'undefined') {
        messagebox.querySelector('.mlbx-newMsgMsg').innerHTML =
          message.innerHTML;
      }
      if (typeof to !== 'undefined' && to != null) {
        // build input values
        var ddw = messagebox.querySelector('.dd-cntr .dd-wrp');
        var dds = messagebox.querySelector('.dd-cntr select.dd-hdn');
        to.forEach(function (t) {
          dds.innerHTML +=
            '<option selected="selected" class="' +
            t[2] +
            '" value="' +
            t[0] +
            '">' +
            t[1] +
            '</option>';
          ddw.children[0].insertAdjacentHTML(
            'beforebegin',
            '<div class="dd-itm ' + t[2] + '">' + t[1] + '</div>',
          );
          // move placeholder after input
          //dd.find('.dynamic-dropdown-input-container').after(dd.find('.placeholder'))
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
    },
    LoadMessageDetails = function (id) {
      if (!d.getElementsByClassName('mlbx-rdr')[0]) {
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
      loadMesQ.onload = function () {
        if (d.getElementsByClassName('mlbx-rdr')[0])
          d.getElementsByClassName('mlbx-rdr')[0].innerHTML =
            loadMesQ.responseText;
        document.dispatchEvent(new CustomEvent('ss-load'));
      };
    },
    getNextMessage = function (elem, selector) {
      // Get the next sibling element
      var sibling = elem.nextElementSibling;

      // If there's no selector, return the first sibling
      if (!selector) return sibling;

      // If the sibling matches our selector, use it
      // If not, jump to the next sibling and continue the loop
      while (sibling) {
        if (sibling.matches(selector)) return sibling;
        sibling = sibling.nextElementSibling;
      }

      // Get the next sibling element
      sibling = elem.previousElementSibling;

      // If there's no selector, return the first sibling
      if (!selector) return sibling;

      // If the sibling matches our selector, use it
      // If not, jump to the next sibling and continue the loop
      while (sibling) {
        if (sibling.matches(selector)) return sibling;
        sibling = sibling.previousElementSibling;
      }
    },
    ChangeActiveMessage = function (me) {
      // if active folder is inbox

      var act = d.querySelectorAll('.mlbx-msgsMsg.active:not(.deleted'),
        x = 0;
      for (x; x < act.length; x++) {
        act[x].classList.remove('active');
        if (d.querySelector('#mlbx-fldrsInbox.active')) {
          if (act[x].classList.contains('unread')) {
            act[x].classList.remove('unread');
            MarkMessageRead(act[x].getAttribute('data-rep-id'));
          }
        }
      }
      me.classList.add('active');

      if (
        d.querySelector('#mlbx-fldrsInbox.active') &&
        me.classList.contains('unread')
      ) {
        me.classList.remove('unread');
        MarkMessageRead(me.getAttribute('data-rep-id'));
      }

      // load message contents
      if (!d.querySelector('#mlbx-fldrsDrafts.active')) {
        LoadMessageDetails(me.getAttribute('data-rep-id'));
      } else {
        LoadDraft(me.getAttribute('data-draft-id'));
      }

      // update mail icon
      pollForNewMessages();
    };

  setInterval(function () {
    if (typeof window.ajaxOn == 'undefined' || window.ajaxOn == true) {
      pollForNewMessages();
    }
  }, 10000); //10000 milliseconds = 10 seconds

  document.addEventListener('loadMessage', function (e) {
    if (typeof e.detail !== 'undefined') {
      LoadMessageDetails(e.detail.id);
    }
  });

  d.addEventListener('click', function (e) {
    var q = null;
    if (e.target.closest('.mlbx-msgsMsg')) {
      ChangeActiveMessage(e.target.closest('.mlbx-msgsMsg'));
    } else if (e.target.closest('.mail-new-message')) {
      var from = e.target
        .closest('.mail-new-message')
        .getAttribute('data-from');
      var fromId = e.target
        .closest('.mail-new-message')
        .getAttribute('data-fromid');
      if (from && fromId) {
        StartNewMessage([[fromid, from]]);
      } else {
        StartNewMessage();
      }
    } else if (e.target.closest('.mlbx-usrDrp')) {
      // user dropdown
      e.target
        .closest('.mlbx-usrDrp')
        .querySelector('.mlbx-usrDrpCtr').style.display = 'block';
      e.target
        .closest('.mlbx-usrDrp')
        .querySelector('.mlbx-usrDrpCtr')
        .addEventListener('mouseleave', function mouseleave(e) {
          this.style.removeProperty('display');
          this.removeEventListener('mouseleave', mouseleave, false);
        });
    } else if (e.target.closest('#mlbx-fldrsDrafts')) {
      LoadFolder('drafts');
      document
        .querySelector('.mlbx-fldrsFldr.active')
        .classList.remove('active');
      e.target.closest('#mlbx-fldrsDrafts').classList.add('active');
    } else if (e.target.closest('#mlbx-fldrsInbox')) {
      LoadFolder('inbox');
      document
        .querySelector('.mlbx-fldrsFldr.active')
        .classList.remove('active');
      e.target.closest('#mlbx-fldrsInbox').classList.add('active');
    } else if (e.target.closest('#mlbx-fldrsSent')) {
      LoadFolder('sent');
      document
        .querySelector('.mlbx-fldrsFldr.active')
        .classList.remove('active');
      e.target.closest('#mlbx-fldrsSent').classList.add('active');
    } else if (e.target.closest('#mlbx-fldrsDeleted')) {
      LoadFolder('deleted');
      document
        .querySelector('.mlbx-fldrsFldr.active')
        .classList.remove('active');
      e.target.closest('#mlbx-fldrsDeleted').classList.add('active');
    } else if (e.target.closest('.mail-delete-message')) {
      if (getNewMail !== null) getNewMail.abort();

      // get repid, or draft if draft
      var msg = d.querySelector('.mlbx-msgsMsg.active:not(.deleted)');
      if (msg == null) return;
      var nextMsg = getNextMessage(msg, '.mlbx-msgsMsg');

      if (msg.hasAttribute('data-draft-id')) {
        // delete draft
        deleteMes = new XMLHttpRequest();
        deleteMes.open(
          'post',
          'mail?handler=DeleteDraft&id=' + msg.getAttribute('data-draft-id'),
          true,
        );
        deleteMes.setRequestHeader(
          'Content-Type',
          'application/x-www-form-urlencoded; charset=UTF-8',
        );
        deleteMes.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
        deleteMes.send();

        // show next message
        if (typeof nextMsg !== 'undefined') {
          LoadDraft(nextMsg.getAttribute('data-draft-id'));
          nextMsg.classList.add('active');
        } else {
          d.querySelector('.mlbx-rdr').innerHTML = '';
        }
      } else if (msg.hasAttribute('data-rep-id')) {
        // delete message
        deleteMes = new XMLHttpRequest();
        deleteMes.open(
          'post',
          'mail?handler=DeleteMessage&id=' + msg.getAttribute('data-rep-id'),
          true,
        );
        deleteMes.setRequestHeader(
          'Content-Type',
          'application/x-www-form-urlencoded; charset=UTF-8',
        );
        deleteMes.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
        deleteMes.send();

        // show next message
        if (typeof nextMsg !== 'undefined') {
          LoadMessageDetails(nextMsg.getAttribute('data-rep-id'));
          nextMsg.classList.add('active');
        } else {
          d.querySelector('.mlbx-rdr').innerHTML = '';
        }
      }

      var w = parseFloat(msg.clientWidth);
      var h = parseFloat(msg.clientHeight);
      msg.style.transition =
        'margin-left 150ms, margin-right 150ms, margin-top 150ms';
      msg.style.zIndex = '-1';

      var clear = msg.clientHeight;

      msg.style.marginLeft = -w + 'px';
      msg.style.marginRight = w + 'px';
      msg.style.marginTop = -h + 'px';
      msg.classList.add('deleted');
      setTimeout(function () {
        if (msg !== null) {
          msg.parentElement.removeChild(msg.nextElementSibling);
          msg.parentElement.removeChild(msg);
        }
      }, 150);
    }
  });

  d.addEventListener(
    'submit',
    function (e) {
      if (e.target.closest('.mlbx-newMsgForm')) {
        e.preventDefault();

        var form = e.target.closest('.mlbx-newMsgForm');

        //to
        var to = [];
        var recp = form.querySelectorAll(
          '.mlbx-newMsgRecip select option, .mail-to input[type="hidden"]',
        );

        if (recp != null) {
          console.log(recp);
          for (var x = 0; x < recp.length; x++) {
            var g = recp[x].classList.contains('group') ? 'g' : '';
            to.push({
              UserId: recp[x].value,
              Type: g,
            });
          }
        }

        //subject
        var subject;
        if (form.querySelector('.mlbx-newMsgSubjIpt')) {
          subject = form.querySelector('.mlbx-newMsgSubjIpt').innerText;
        } else {
          subject = form.querySelector('input.mail-subject').value;
        }
        //message
        var message, text;
        if (form.querySelector('.mlbx-newMsgMsg')) {
          message = form.querySelector('.mlbx-newMsgMsg').innerHTML;
          text = form
            .querySelector('.mlbx-newMsgMsg')
            .innerText.replace(/\n/g, ' ');
        } else {
          message = form.querySelector('textarea[name="mail-message"]').value;
          text = message;
        }
        var share = form.querySelector('.mlbx-share') ? '1' : '0';
        var shareName = form.querySelector('.mlbx-shareName')
          ? form.querySelector('.mlbx-shareName').value
          : '';
        var shareUrl = form.querySelector('.mlbx-shareUrl')
          ? form.querySelector('.mlbx-shareUrl').value
          : window.location.href;

        var data = {
          To: JSON.stringify(to),
          Subject: subject,
          Message: message,
          Text: text,
          Share: share,
          ShareName: shareName,
          ShareUrl: shareUrl,
        };

        if (form.querySelector('input[name="draftId"]') !== null) {
          var draftId = form.querySelector('input[name="draftId"]').value;
          data.DraftId = draftId;
          if (
            d.querySelector('.mlbx-msgsMsg[data-draft-id="' + draftId + '"]')
          ) {
            d.querySelector(
              '.mlbx-msgsMsg[data-draft-id="' + draftId + '"]',
            ).parentElement.removeChild(
              d.querySelector('.mlbx-msgsMsg[data-draft-id="' + draftId + '"]'),
            );
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
        sendMes.onload = function () {
          pollForNewMessages();
        };
        // if we are in drafts folder, remove draft.

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
        // close modal if its open

        d.dispatchEvent(new CustomEvent('modal-close'));
      }
    },
    false,
  );

  d.addEventListener('click', function (e) {
    var m, convid, subject, f, from, x, message, message_liner, dbox;
    if (
      e.target.closest('.mail-reply') &&
      d.querySelector('#mlbx-fldrsInbox.active') !== null
    ) {
      m = d.querySelector('.mlbx-rdrHead');
      convid = m.getAttribute('data-conv-id');
      subject = 'Re: ' + m.querySelector('.mailbox-subject').innerText;
      f = m.querySelector('div.mailbox-sender-user');
      from = [
        [f.getAttribute('data-user-id'), f.querySelector('span').innerText, ''],
      ];
      message = document.createElement('div');
      message_liner = document.createElement('div');
      message_liner.classList.add('message-reply');
      message_liner.innerHTML = d.querySelector(
        '.mlbx-rdr .ss-content',
      ).innerHTML;
      message_liner
        .querySelector('.mlbx-rdrHead-controls')
        .parentElement.removeChild(
          message_liner.querySelector('.mlbx-rdrHead-controls'),
        );
      dbox = message_liner.querySelectorAll('.mlbx-usrDrp');
      for (x = 0; x < dbox.length; x++) {
        dbox[x].parentElement.removeChild(dbox[x]);
      }
      message.innerHTML = '<br><br><br>';
      message.appendChild(message_liner);

      StartNewMessage(from, subject, message, convid);
    } else if (
      e.target.closest('.mail-reply-all') &&
      d.querySelector('#mlbx-fldrsInbox.active') !== null
    ) {
      m = d.querySelector('.mlbx-rdrHead');
      convid = m.getAttribute('data-conv-id');
      subject = 'Re: ' + m.querySelector('.mailbox-subject').innerText;
      f = m.querySelectorAll('div.mailbox-sender-user');
      t = m.querySelectorAll('div.mailbox-to-user');
      from = [];
      for (x = 0; x < f.length; x++) {
        from.push([
          f[x].getAttribute('data-user-id'),
          f[x].querySelector('span').innerText,
          f[x].classList.contains('group') ? 'group' : '',
        ]);
      }
      for (x = 0; x < t.length; x++) {
        from.push([
          t[x].getAttribute('data-user-id'),
          t[x].querySelector('span').innerText,
          t[x].classList.contains('group') ? 'group' : '',
        ]);
      }
      // remove dups

      var seen = {};
      var ret_arr = [];
      for (var i = 0; i < from.length; i++) {
        if (!(from[i] in seen)) {
          ret_arr.push(from[i]);
          seen[from[i]] = true;
        }
      }
      from = ret_arr;
      message = document.createElement('div');
      message_liner = document.createElement('div');
      message_liner.classList.add('message-reply');
      message_liner.innerHTML = d.querySelector(
        '.mlbx-rdr .ss-content',
      ).innerHTML;
      message_liner
        .querySelector('.mlbx-rdrHead-controls')
        .parentElement.removeChild(
          message_liner.querySelector('.mlbx-rdrHead-controls'),
        );
      dbox = message_liner.querySelectorAll('.mlbx-usrDrp');
      for (x = 0; x < dbox.length; x++) {
        dbox[x].parentElement.removeChild(dbox[x]);
      }
      message.innerHTML = '<br><br><br>';
      message.appendChild(message_liner);

      StartNewMessage(from, subject, message, convid);
    }
  });
})();
