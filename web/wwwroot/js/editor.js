/*
    Get all "editors"

    <div class="editor"><textarea>initial value</textarea></div>

    Editor should have a few props:
    	* class of "liveEditor"
    	* data-saveUrl (optional save url)
    	* data-inputName (optional name to assign to input when parent form is submitted for other uses)
    	* data-inputId (optional id to assign to input when parent form is submitted for other uses)

    */
(function () {
  let test = 0;

  function load() {
    // Wait for codemirror to load
    if (test === 50) {
      return false;
    }

    if (typeof CodeMirror === 'undefined') {
      setTimeout(function () {
        test++;
        load();
      }, 100);
    } else {
      test = 0;
      l();
    }
  }

  function l() {
    const editor = Array.prototype.slice.call(
      document.querySelectorAll('.editor:not(.loaded)'),
    );
    let x;

    for (x = 0; x < editor.length; x++) {
      // eslint-disable-next-line no-new
      new Loader(editor[x]);
    }
  }

  function Loader(b) {
    this.mdrequest = null;
    this.target = b;
    this.target.classList.add('loaded');
    // Editor input
    this.editorInput = document.createElement('div');
    this.editorInput.classList.add('liveEditor', 'box', 'p-0', 'pb-2');

    this.editorBody = document.createElement('div');
    this.editorBody.classList.add('liveEditor-body', 'p-2');
    this.editorText = document.createElement('textarea');
    this.editorText.classList.add('textarea');
    let initialValue = '';
    if (this.target.querySelectorAll('textarea').length > 0) {
      initialValue = this.target.querySelector('textarea').value;
      this.target.querySelector('textarea').remove();
    }

    this.editorText.setAttribute(
      'id',
      this.target.getAttribute('data-inputId'),
    );
    this.editorText.setAttribute(
      'name',
      this.target.getAttribute('data-inputName'),
    );

    this.editorBody.append(this.editorText);

    const newLocal = this;
    // Editor output
    newLocal.editorPrev = document.createElement('div');
    this.editorPrev.classList.add(
      'editor-liveEditorPrev',
      'is-hidden',
      'p-2',
      'content',
    );

    this.editorPrevText = document.createElement('div');
    this.editorPrev.append(this.editorPrevText);

    // Create buttons

    this.btn = document.createElement('div');
    this.btn.classList.add(
      'liveEditor-btnGrp',
      'has-background-white-ter',
      'is-flex',
      'is-justify-content-space-between',
    );

    this.btnLeft = document.createElement('span');
    this.btnLeft.classList.add('is-flex');
    this.btnRight = document.createElement('span');
    this.btnRight.classList.add('is-flex');

    this.btn.append(this.btnLeft);
    this.btn.append(this.btnRight);

    this.btnBold = document.createElement('button');
    this.btnBold.classList.add('liveEditor-btn', 'button', 'is-light');
    this.btnBold.setAttribute('type', 'button');
    this.btnBold.innerHTML =
      '<span class="icon"><i class="fas fa-bold"></i></span>';

    this.btnLeft.append(this.btnBold);

    this.btnBold.addEventListener('click', this.insertBold.bind(this), false);

    this.btnItalic = document.createElement('button');
    this.btnItalic.classList.add('liveEditor-btn', 'button', 'is-light');
    this.btnItalic.setAttribute('type', 'button');
    this.btnItalic.innerHTML =
      '<span class="icon"><i class="fas fa-italic"></i></span>';

    this.btnLeft.append(this.btnItalic);
    this.btnItalic.addEventListener(
      'click',
      this.insertItalics.bind(this),
      false,
    );

    this.btnHeading = document.createElement('button');
    this.btnHeading.classList.add('liveEditor-btn', 'button', 'is-light');
    this.btnHeading.setAttribute('type', 'button');
    this.btnHeading.innerHTML =
      '<span class="icon"><i class="fas fa-heading"></i></span>';

    this.btnLeft.append(this.btnHeading);
    this.btnHeading.addEventListener(
      'click',
      this.insertHeading.bind(this),
      false,
    );

    this.btnHeadingSep = document.createElement('span');
    this.btnHeadingSep.classList.add('liveEditor-btnSep');
    this.btnLeft.append(this.btnHeadingSep);

    this.btnQuote = document.createElement('button');
    this.btnQuote.classList.add('liveEditor-btn', 'button', 'is-light');
    this.btnQuote.setAttribute('type', 'button');
    this.btnQuote.innerHTML =
      '<span class="icon"><i class="fas fa-quote-left"></i></span>';

    this.btnLeft.append(this.btnQuote);
    this.btnQuote.addEventListener('click', this.insertQuote.bind(this), false);

    this.btnCode = document.createElement('button');
    this.btnCode.classList.add('liveEditor-btn', 'button', 'is-light');
    this.btnCode.setAttribute('type', 'button');
    this.btnCode.innerHTML =
      '<span class="icon"><i class="fas fa-code"></i></span>';

    this.btnLeft.append(this.btnCode);
    this.btnCode.addEventListener('click', this.insertCode.bind(this), false);

    this.btnCodeSep = document.createElement('span');
    this.btnCodeSep.classList.add('liveEditor-btnSep');
    this.btnLeft.append(this.btnCodeSep);

    this.btnUl = document.createElement('button');
    this.btnUl.classList.add('liveEditor-btn', 'button', 'is-light');
    this.btnUl.setAttribute('type', 'button');
    this.btnUl.innerHTML =
      '<span class="icon"><i class="fas fa-list-ul"></i></span>';

    this.btnLeft.append(this.btnUl);
    this.btnUl.addEventListener('click', this.insertUl.bind(this), false);

    this.btnOl = document.createElement('button');
    this.btnOl.classList.add('liveEditor-btn', 'button', 'is-light');
    this.btnOl.setAttribute('type', 'button');
    this.btnOl.innerHTML =
      '<span class="icon"><i class="fas fa-list-ol"></i></span>';

    this.btnLeft.append(this.btnOl);
    this.btnOl.addEventListener('click', this.insertOl.bind(this), false);

    this.btnOlSep = document.createElement('span');
    this.btnOlSep.classList.add('liveEditor-btnSep');
    this.btnLeft.append(this.btnOlSep);

    this.btnLink = document.createElement('button');
    this.btnLink.classList.add('liveEditor-btn', 'button', 'is-light');
    this.btnLink.setAttribute('type', 'button');
    this.btnLink.innerHTML =
      '<span class="icon"><i class="fas fa-link"></i></span>';

    this.btnLeft.append(this.btnLink);
    this.btnLink.addEventListener('click', this.insertLink.bind(this), false);

    this.editorInput.append(this.btn);

    if (this.target.getAttribute('data-saveUrl')) {
      this.btnSave = document.createElement('button');
      this.btnSave.classList.add('liveEditor-btn', 'button', 'is-light');
      this.btnSave.classList.add('save');
      this.btnSave.setAttribute('type', 'button');
      this.btnSave.innerHTML =
        '<span class="icon"><i class="fas fa-save"></i></span>';
      this.btnSave.addEventListener('click', this.save.bind(this), false);

      this.btnRight.append(this.btnSave);

      this.saveUrl = this.target.getAttribute('data-saveUrl');
    }

    this.editorPrevTitleButton = document.createElement('button');
    this.editorPrevTitleButton.setAttribute('type', 'button');
    this.editorPrevTitleButton.innerHTML =
      '<span class="icon"><i class="fas fa-eye"></i></span>';
    this.editorPrevTitleButton.classList.add(
      'liveEditor-btn',
      'button',
      'is-light',
    );
    this.editorPrevTitleButton.classList.add('save');
    this.btnRight.append(this.editorPrevTitleButton);

    this.editorInput.append(this.editorBody);
    this.target.append(this.editorInput);
    this.editorInput.append(this.editorPrev);

    // Load code mirror
    this.mirror = CodeMirror.fromTextArea(this.editorText, {
      mode: 'gfm',
      autoRefresh: true,
      viewportMargin: Infinity,
      lineWrapping: true,
    });

    this.mirror.setValue(initialValue);
    this.mirror.getInputField().spellcheck = true;

    // Load markdown-it for previews
    const mapping = {
      h1: 'title is-1',
      h2: 'title is-2',
      h3: 'title is-3',
      h4: 'title is-4',
      h5: 'title is-5',
      h6: 'title is-5',
      p: 'block',
      table: 'table',
    };

    const markdownIt = require('markdown-it')({
      html: true,
      breaks: false,
      linkify: true,
      typographer: true,
    });
    this.md = markdownIt.use(require('@toycode/markdown-it-class'), mapping);

    this.editorPrevTitleButton.addEventListener(
      'click',
      this.updateMirror.bind(this),
      false,
    );

    if (this.target.closest('.mdl')) {
      this.target
        .closest('.mdl')
        .addEventListener('mdl-open', this.updateMirror.bind(this));
    }
  }

  const k =
    document.requestAnimationFrame ||
    document.setImmediate ||
    function (b) {
      return setTimeout(b, 0);
    };

  Loader.prototype = {
    // Mirror even
    updateMirror() {
      const a = this;
      const md = this.md;
      const button = this.editorPrevTitleButton;
      k(function () {
        const icon = button.querySelector('i.fas');
        if (icon.classList.contains('fa-eye')) {
          a.editorPrevText.innerHTML = md.render(a.mirror.getValue());
          icon.classList.remove('fa-eye');
          icon.classList.add('fa-eye-slash');

          // Switch preview box and code box
          a.editorPrev.classList.remove('is-hidden');
          a.editorBody.classList.add('is-hidden');
          (a.btnLeft.querySelectorAll('button') || []).forEach((element) => {
            element.setAttribute('disabled', 'disabled');
          });
          document.dispatchEvent(new CustomEvent('code-highlight'));
          document.dispatchEvent(new CustomEvent('load-charts'));
        } else {
          icon.classList.remove('fa-eye-slash');
          icon.classList.add('fa-eye');
          a.editorPrev.classList.add('is-hidden');
          (a.btnLeft.querySelectorAll('button') || []).forEach((element) => {
            element.removeAttribute('disabled');
          });
          a.editorBody.classList.remove('is-hidden');
        }
      });
    },
    // Button events
    insertBold() {
      const a = this;

      k(function () {
        const word = a.mirror.findWordAt(a.mirror.getCursor());
        let selection = a.mirror.getSelection().trim();
        if (a.mirror.getSelection().length > 0) {
          a.mirror.replaceSelection(
            '**' + a.mirror.getSelection().trim() + '**',
          );
        } else {
          a.mirror.setSelection(word.anchor, word.head);

          if (selection === '') {
            selection = 'bold text';
          }

          a.mirror.replaceSelection('**' + selection + '**');
        }

        a.mirror.focus();
      });
    },
    insertItalics() {
      const a = this;
      k(function () {
        const word = a.mirror.findWordAt(a.mirror.getCursor());
        let selection = a.mirror.getSelection().trim();
        if (a.mirror.getSelection().length > 0) {
          a.mirror.replaceSelection('*' + a.mirror.getSelection().trim() + '*');
        } else {
          a.mirror.setSelection(word.anchor, word.head);

          if (selection === '') {
            selection = 'italic text';
          }

          a.mirror.replaceSelection('*' + selection + '*');
        }

        a.mirror.focus();
      });
    },
    insertHeading() {
      const a = this;
      k(function () {
        const line = a.mirror.getCursor().line;
        const currentHead = a.mirror.getLine(line);
        const start = currentHead.split(' ')[0];

        if (currentHead === '') {
          a.mirror.replaceRange('# Heading', {
            line,
            ch: 0,
          });
        } else if (start[0] === '#') {
          const level = (start.match(/#/g) || []).length;
          if (level >= 6) {
            a.mirror.replaceRange(
              currentHead.slice(Math.max(0, currentHead.indexOf(' ') + 1)),
              {
                line,
                ch: 0,
              },
              {
                line,
                ch: currentHead.length,
              },
            );
          } else {
            a.mirror.replaceRange('#', {
              line,
              ch: 0,
            });
          }
        } else {
          a.mirror.replaceRange('# ', {
            line,
            ch: 0,
          });
        }

        a.mirror.focus();
      });
    },
    insertQuote() {
      const a = this;
      k(function () {
        const word = a.mirror.findWordAt(a.mirror.getCursor());
        const selection = a.mirror.getSelection().trim();
        if (a.mirror.getSelection().length > 0) {
          // Expand selection to be full lines
          a.mirror.setSelection(
            {
              line: Math.min(
                a.mirror.listSelections()[0].head.line,
                a.mirror.listSelections()[0].anchor.line,
              ),
              ch: 0,
            },
            {
              line: Math.max(
                a.mirror.listSelections()[0].head.line,
                a.mirror.listSelections()[0].anchor.line,
              ),
              ch: 1,
            },
          );
          a.mirror.replaceSelection(
            '> ' +
              a.mirror.getSelection().replace(/\n/g, function () {
                return '\n> ';
              }),
          );
        } else {
          a.mirror.setSelection(
            {
              line: Math.min(word.head.line, word.anchor.line),
              ch: 0,
            },
            {
              line: Math.max(word.head.line, word.anchor.line),
              ch: 1,
            },
          );
          if (selection === '') {
            a.mirror.replaceSelection('> fancy blockquote');
          } else {
            a.mirror.replaceSelection('> ' + selection);
          }
        }

        a.mirror.focus();
      });
    },
    insertCode() {
      const a = this;
      k(function () {
        const word = a.mirror.findWordAt(a.mirror.getCursor());
        const selection = a.mirror.getSelection().trim();
        if (a.mirror.getSelection().length > 0) {
          a.mirror.replaceSelection(
            '\n```sql\n' + a.mirror.getSelection() + '\n```\n',
          );
        } else {
          a.mirror.setSelection(word.anchor, word.head);

          if (selection === '') {
            a.mirror.replaceSelection(
              '\n```sql\nselect smiles; -- :) --\n```\n',
            );
          } else {
            a.mirror.replaceSelection('`' + selection + '`');
          }
        }

        a.mirror.focus();
      });
    },
    insertUl() {
      const a = this;
      k(function () {
        const word = a.mirror.findWordAt(a.mirror.getCursor());
        const selection = a.mirror.getSelection().trim();
        if (a.mirror.getSelection().length > 0) {
          // Expand selection to be full lines
          a.mirror.setSelection(
            {
              line: Math.min(
                a.mirror.listSelections()[0].head.line,
                a.mirror.listSelections()[0].anchor.line,
              ),
              ch: 0,
            },
            {
              line: Math.max(
                a.mirror.listSelections()[0].head.line,
                a.mirror.listSelections()[0].anchor.line,
              ),
              ch: 1,
            },
          );
          a.mirror.replaceSelection(
            '- ' +
              a.mirror.getSelection().replace(/\n/g, function () {
                return '\n- ';
              }),
          );
        } else {
          a.mirror.setSelection(
            {
              line: Math.min(word.head.line, word.anchor.line),
              ch: 0,
            },
            {
              line: Math.max(word.head.line, word.anchor.line),
              ch: 1,
            },
          );

          if (selection === '') {
            a.mirror.replaceSelection('- item one\n- item two\n- item three');
          } else {
            a.mirror.replaceSelection('- ' + selection);
          }
        }

        a.mirror.focus();
      });
    },
    insertOl() {
      const a = this;
      k(function () {
        const word = a.mirror.findWordAt(a.mirror.getCursor());
        const selection = a.mirror.getSelection().trim();
        if (a.mirror.getSelection().length > 0) {
          // Expand selection to be full lines
          a.mirror.setSelection(
            {
              line: Math.min(
                a.mirror.listSelections()[0].head.line,
                a.mirror.listSelections()[0].anchor.line,
              ),
              ch: 0,
            },
            {
              line: Math.max(
                a.mirror.listSelections()[0].head.line,
                a.mirror.listSelections()[0].anchor.line,
              ),
              ch: 1,
            },
          );
          let number = 1;
          a.mirror.replaceSelection(
            number +
              '. ' +
              a.mirror.getSelection().replace(/\n/g, function () {
                return '\n' + ++number + '. ';
              }),
          );
        } else {
          a.mirror.setSelection(
            {
              line: Math.min(word.head.line, word.anchor.line),
              ch: 0,
            },
            {
              line: Math.max(word.head.line, word.anchor.line),
              ch: 1,
            },
          );

          if (selection === '') {
            a.mirror.replaceSelection(
              '1. item one\n2. item two\n3. item three',
            );
          } else {
            a.mirror.replaceSelection('1. ' + selection);
          }
        }

        a.mirror.focus();
      });
    },
    insertLink() {
      const a = this;
      k(function () {
        if (a.mirror.getSelection().length > 0) {
          a.mirror.replaceSelection(
            '[' + a.mirror.getSelection().trim() + '](https://atlas)',
          );
        } else {
          a.mirror.replaceSelection('[Link Title](https://atlas)');
        }

        a.mirror.focus();
      });
    },
    save() {
      const a = this;
      k(function () {
        const data = {};
        data.id = getUrlVars().id;
        data.description = a.mirror.getValue();
        const q = new XMLHttpRequest();
        q.open('post', a.saveUrl, true);
        q.setRequestHeader('Content-Type', 'text/plain;charset=UTF-8`');
        q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
        q.send(JSON.stringify(data));

        q.addEventListener('load', function () {
          document.querySelector('#editorMdl-titleSave').style.visibility =
            'visible';
          setTimeout(function () {
            document
              .querySelector('#editorMdl-titleSave')
              .style.removeProperty('visibility');
          }, 750);
        });
      });
    },
  };

  load();
  document.addEventListener('ajax', function () {
    load();
  });
})();
