(function () {
  if (document.querySelector('#editModal') !== null) {
    document
      .querySelector('#editModal')
      .addEventListener('click', function (e) {
        var q,
          data,
          form,
          url,
          termsContainer = document.getElementById(
            'collection-editor-currentterms-container',
          );
        if (e.target.closest('#collection-editor-remove-term-form')) {
          e.preventDefault();

          form = e.target.closest('#collection-editor-remove-term-form');
          url = form.getAttribute('href');

          q = new XMLHttpRequest();
          q.open('get', url, true);
          q.setRequestHeader('Content-Type', 'text/plain;charset=UTF-8`');
          q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
          q.send();
          q.onload = function () {
            data = q.responseText;

            if (data.trim().length == 0) {
              termsContainer.innerHTML = '';
            } else {
              termsContainer.innerHTML = data;
              // render markdown
              document.dispatchEvent(new CustomEvent('code-highlight'));
            }
            document.getElementById('editorMdl-titleSave').style.visibility =
              'visible';
            setTimeout(function () {
              document
                .getElementById('editorMdl-titleSave')
                .style.removeProperty('visibility');
            }, 750);
          };
        }
      });

    document
      .querySelector('#editModal')
      .addEventListener('click', function (e) {
        var q,
          data,
          form,
          url,
          termsContainer = document.getElementById(
            'collection-editor-currentreports-container',
          );
        if (e.target.closest('#collection-editor-remove-report-form')) {
          e.preventDefault();

          form = e.target.closest('#collection-editor-remove-report-form');
          url = form.getAttribute('href');

          q = new XMLHttpRequest();
          q.open('get', url, true);
          q.setRequestHeader('Content-Type', 'text/plain;charset=UTF-8`');
          q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
          q.send();
          q.onload = function () {
            data = q.responseText;

            if (data.trim().length == 0) {
              termsContainer.innerHTML = '';
            } else {
              termsContainer.innerHTML = data;
              // render markdown
              document.dispatchEvent(new CustomEvent('code-highlight'));
            }
            document.getElementById('editorMdl-titleSave').style.visibility =
              'visible';
            setTimeout(function () {
              document
                .getElementById('editorMdl-titleSave')
                .style.removeProperty('visibility');
            }, 750);
          };
        }
      });

    document
      .querySelector('#editModal')
      .addEventListener('submit', function (e) {
        var q,
          data,
          form,
          url,
          termsContainer = document.getElementById(
            'collection-editor-currentterms-container',
          );
        if (
          e.target.closest('#collection-editor-edit-term-form') ||
          e.target.closest('#collection-editor-add-term-form')
        ) {
          e.preventDefault();

          form =
            e.target.closest('#collection-editor-edit-term-form') ||
            e.target.closest('#collection-editor-add-term-form');
          url = form.getAttribute('action');

          q = new XMLHttpRequest();
          q.open(
            'post',
            form.getAttribute('action') + '&' + serialize(form),
            true,
          );
          q.setRequestHeader('Content-Type', 'text/plain;charset=UTF-8`');
          q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
          q.send();
          q.onload = function () {
            data = q.responseText;

            if (data.trim().length == 0) {
              termsContainer.innerHTML = '';
            } else {
              termsContainer.innerHTML = data;
              // render markdown
              document.dispatchEvent(new CustomEvent('code-highlight'));
            }
            document.getElementById('editorMdl-titleSave').style.visibility =
              'visible';
            setTimeout(function () {
              document
                .getElementById('editorMdl-titleSave')
                .style.removeProperty('visibility');
            }, 750);
            document.dispatchEvent(new CustomEvent('ajax'));
            document
              .querySelector('#editModal')
              .dispatchEvent(new CustomEvent('mdl-open'));
          };
          form.reset();
          form.querySelector('.dd-vsbl').value = '';
          form.querySelector('#DpTermAnnotation_Rank').value = '';
          if (form.querySelector('.CodeMirror')) {
            form.querySelector('.CodeMirror').CodeMirror.getDoc().setValue('');
          }
          // document.getElementsByClassName("new-term-summary")[0].innerHTML = "";
          // document.getElementsByClassName("new-term-def")[0].innerHTML = "";
        }
      });

    document
      .querySelector('#editModal')
      .addEventListener('submit', function (e) {
        var q,
          data,
          form,
          url,
          termsContainer = document.getElementById(
            'collection-editor-currentreports-container',
          );
        if (
          e.target.closest('#collection-editor-edit-report-form') ||
          e.target.closest('#collection-editor-add-report-form')
        ) {
          e.preventDefault();

          form =
            e.target.closest('#collection-editor-edit-report-form') ||
            e.target.closest('#collection-editor-add-report-form');
          url = form.getAttribute('action');

          q = new XMLHttpRequest();
          q.open(
            'post',
            form.getAttribute('action') + '&' + serialize(form),
            true,
          );
          q.setRequestHeader('Content-Type', 'text/plain;charset=UTF-8`');
          q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
          q.send();
          q.onload = function () {
            data = q.responseText;

            if (data.trim().length == 0) {
              termsContainer.innerHTML = '';
            } else {
              termsContainer.innerHTML = data;
              // render markdown
              document.dispatchEvent(new CustomEvent('code-highlight'));
            }
            document.getElementById('editorMdl-titleSave').style.visibility =
              'visible';
            setTimeout(function () {
              document
                .getElementById('editorMdl-titleSave')
                .style.removeProperty('visibility');
            }, 750);
            document.dispatchEvent(new CustomEvent('ajax'));
            document
              .querySelector('#editModal')
              .dispatchEvent(new CustomEvent('mdl-open'));
          };
          form.reset();
          form.querySelector('.dd-vsbl').value = '';
          form.querySelector('#DpReportAnnotation_Rank').value = '';
          if (form.querySelector('.CodeMirror')) {
            form.querySelector('.CodeMirror').CodeMirror.getDoc().setValue('');
          }
          // document.getElementsByClassName("new-term-summary")[0].innerHTML = "";
          // document.getElementsByClassName("new-term-def")[0].innerHTML = "";
        }
      });
  }
})();
