(function () {
  /*
    get all "editors"

    <div class="editor"><textarea>initial value</textarea></div>

    Editor should have a few props:
    	* class of "liveEditor"
    	* data-saveUrl (optional save url)
    	* data-inputName (optional name to assign to input when parent form is submitted for other uses)
    	* data-inputId (optional id to assign to input when parent form is submitted for other uses)

    */

  var test = 0;

  function load() {
    // wait for codemirror to load
    if (test == 50) {
      return false;
    } else if (typeof CodeMirror == 'undefined') {
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
    var editor = Array.prototype.slice.call(
        document.querySelectorAll('.editor:not(.loaded)'),
      ),
      x;

    for (x = 0; x < editor.length; x++) {
      new e(editor[x]);
    }
  }

  function e(b) {
    this.mdrequest = null;
    this.target = b;
    this.target.classList.add('loaded');
    // editor input
    this.editorInput = document.createElement('div');
    this.editorInput.classList.add('liveEditor', 'box', 'p-0', 'pb-2');

    this.editorBody = document.createElement('div');
    this.editorBody.classList.add('liveEditor-body', 'p-2');
    this.editorText = document.createElement('textarea');
    this.editorText.classList.add('textarea');
    var initialValue = '';
    if (this.target.getElementsByTagName('textarea').length > 0) {
      initialValue = this.target.getElementsByTagName('textarea')[0].value;
      this.target.removeChild(this.target.getElementsByTagName('textarea')[0]);
    }

    this.editorText.setAttribute(
      'id',
      this.target.getAttribute('data-inputId'),
    );
    this.editorText.setAttribute(
      'name',
      this.target.getAttribute('data-inputName'),
    );

    this.editorBody.appendChild(this.editorText);

    // editor output
    this.editorPrev = document.createElement('div');
    this.editorPrev.classList.add('editor-liveEditorPrev', 'is-hidden', 'p-2');

    this.editorPrevText = document.createElement('div');
    this.editorPrev.appendChild(this.editorPrevText);

    // create buttons

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

    this.btn.appendChild(this.btnLeft);
    this.btn.appendChild(this.btnRight);

    this.btnBold = document.createElement('button');
    this.btnBold.classList.add('liveEditor-btn', 'button', 'is-light');
    this.btnBold.setAttribute('type', 'button');
    this.btnBold.innerHTML =
      '<span class="icon"><i class="fas fa-bold"></i></span>';

    this.btnLeft.appendChild(this.btnBold);

    this.btnBold.addEventListener('click', this.insertBold.bind(this), false);

    this.btnItalic = document.createElement('button');
    this.btnItalic.classList.add('liveEditor-btn', 'button', 'is-light');
    this.btnItalic.setAttribute('type', 'button');
    this.btnItalic.innerHTML =
      '<span class="icon"><i class="fas fa-italic"></i></span>';

    this.btnLeft.appendChild(this.btnItalic);
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

    this.btnLeft.appendChild(this.btnHeading);
    this.btnHeading.addEventListener(
      'click',
      this.insertHeading.bind(this),
      false,
    );

    this.btnHeadingSep = document.createElement('span');
    this.btnHeadingSep.classList.add('liveEditor-btnSep');
    this.btnLeft.appendChild(this.btnHeadingSep);

    this.btnQuote = document.createElement('button');
    this.btnQuote.classList.add('liveEditor-btn', 'button', 'is-light');
    this.btnQuote.setAttribute('type', 'button');
    this.btnQuote.innerHTML =
      '<span class="icon"><i class="fas fa-quote-left"></i></span>';

    this.btnLeft.appendChild(this.btnQuote);
    this.btnQuote.addEventListener('click', this.insertQuote.bind(this), false);

    this.btnCode = document.createElement('button');
    this.btnCode.classList.add('liveEditor-btn', 'button', 'is-light');
    this.btnCode.setAttribute('type', 'button');
    this.btnCode.innerHTML =
      '<span class="icon"><i class="fas fa-code"></i></span>';

    this.btnLeft.appendChild(this.btnCode);
    this.btnCode.addEventListener('click', this.insertCode.bind(this), false);

    this.btnCodeSep = document.createElement('span');
    this.btnCodeSep.classList.add('liveEditor-btnSep');
    this.btnLeft.appendChild(this.btnCodeSep);

    this.btnUl = document.createElement('button');
    this.btnUl.classList.add('liveEditor-btn', 'button', 'is-light');
    this.btnUl.setAttribute('type', 'button');
    this.btnUl.innerHTML =
      '<span class="icon"><i class="fas fa-list-ul"></i></span>';

    this.btnLeft.appendChild(this.btnUl);
    this.btnUl.addEventListener('click', this.insertUl.bind(this), false);

    this.btnOl = document.createElement('button');
    this.btnOl.classList.add('liveEditor-btn', 'button', 'is-light');
    this.btnOl.setAttribute('type', 'button');
    this.btnOl.innerHTML =
      '<span class="icon"><i class="fas fa-list-ol"></i></span>';

    this.btnLeft.appendChild(this.btnOl);
    this.btnOl.addEventListener('click', this.insertOl.bind(this), false);

    this.btnOlSep = document.createElement('span');
    this.btnOlSep.classList.add('liveEditor-btnSep');
    this.btnLeft.appendChild(this.btnOlSep);

    this.btnLink = document.createElement('button');
    this.btnLink.classList.add('liveEditor-btn', 'button', 'is-light');
    this.btnLink.setAttribute('type', 'button');
    this.btnLink.innerHTML =
      '<span class="icon"><i class="fas fa-link"></i></span>';

    this.btnLeft.appendChild(this.btnLink);
    this.btnLink.addEventListener('click', this.insertLink.bind(this), false);

    this.editorInput.appendChild(this.btn);

    if (this.target.getAttribute('data-saveUrl')) {
      this.btnSave = document.createElement('button');
      this.btnSave.classList.add('liveEditor-btn', 'button', 'is-light');
      this.btnSave.classList.add('save');
      this.btnSave.setAttribute('type', 'button');
      this.btnSave.innerHTML =
        '<span class="icon"><i class="fas fa-save"></i></span>';
      this.btnSave.addEventListener('click', this.save.bind(this), false);

      this.btnRight.appendChild(this.btnSave);

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
    this.btnRight.appendChild(this.editorPrevTitleButton);

    this.editorInput.appendChild(this.editorBody);
    this.target.appendChild(this.editorInput);
    this.editorInput.appendChild(this.editorPrev);

    // load code mirror
    this.mirror = CodeMirror.fromTextArea(this.editorText, {
      mode: 'gfm',
      autoRefresh: true,
      viewportMargin: Infinity,
      lineWrapping: true,
    });

    this.mirror.setValue(initialValue);
    this.mirror.getInputField().spellcheck = true;

    // this.mirror.on("change", this.updateMirror.bind(this), false);

    // load markdown-it for previews
    this.md = window.markdownit({
      html: true,
      linkify: true,
      typographer: true,
    });

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

  var k =
    document.requestAnimationFrame ||
    document.setImmediate ||
    function (b) {
      return setTimeout(b, 0);
    };

  e.prototype = {
    // mirror even
    updateMirror: function updateMirror(b) {
      var a = this,
        md = this.md,
        button = this.editorPrevTitleButton;
      k(function () {
        var icon = button.querySelector('i.fas');
        if (icon.classList.contains('fa-eye')) {
          a.editorPrevText.innerHTML = md.render(a.mirror.getValue());
          icon.classList.remove('fa-eye');
          icon.classList.add('fa-eye-slash');

          // switch preview box and code box
          a.editorPrev.classList.remove('is-hidden');
          a.editorBody.classList.add('is-hidden');
          (a.btnLeft.querySelectorAll('button') || []).forEach((e) => {
            e.setAttribute('disabled', 'disabled');
          });
          document.dispatchEvent(new CustomEvent('code-highlight'));
          document.dispatchEvent(new CustomEvent('load-charts'));
        } else {
          icon.classList.remove('fa-eye-slash');
          icon.classList.add('fa-eye');
          a.editorPrev.classList.add('is-hidden');
          (a.btnLeft.querySelectorAll('button') || []).forEach((e) => {
            e.removeAttribute('disabled');
          });
          a.editorBody.classList.remove('is-hidden');
        }
      });
    },
    // button events
    insertBold: function insertBold(b) {
      var a = this;

      k(function () {
        var word = a.mirror.findWordAt(a.mirror.getCursor());
        var selection = a.mirror.getSelection().trim();
        if (a.mirror.getSelection().length > 0) {
          a.mirror.replaceSelection(
            '**' + a.mirror.getSelection().trim() + '**',
          );
        } else {
          a.mirror.setSelection(word.anchor, word.head);

          if (selection == '') {
            selection = 'bold text';
          }

          a.mirror.replaceSelection('**' + selection + '**');
        }
        a.mirror.focus();
      });
    },
    insertItalics: function insertItalics(b) {
      var a = this;
      k(function () {
        var word = a.mirror.findWordAt(a.mirror.getCursor());
        var selection = a.mirror.getSelection().trim();
        if (a.mirror.getSelection().length > 0) {
          a.mirror.replaceSelection('*' + a.mirror.getSelection().trim() + '*');
        } else {
          a.mirror.setSelection(word.anchor, word.head);

          if (selection == '') {
            selection = 'italic text';
          }
          a.mirror.replaceSelection('*' + selection + '*');
        }
        a.mirror.focus();
      });
    },
    insertHeading: function insertHeading(b) {
      var a = this;
      k(function () {
        var line = a.mirror.getCursor().line,
          currentHead = a.mirror.getLine(line),
          start = currentHead.split(' ')[0];

        if (currentHead == '') {
          a.mirror.replaceRange('# Heading', {
            line: line,
            ch: 0,
          });
        } else if (start[0] != '#') {
          a.mirror.replaceRange('# ', {
            line: line,
            ch: 0,
          });
        } else {
          var level = (start.match(/#/g) || []).length;
          if (level >= 6) {
            a.mirror.replaceRange(
              currentHead.substring(currentHead.indexOf(' ') + 1),
              {
                line: line,
                ch: 0,
              },
              {
                line: line,
                ch: currentHead.length,
              },
            );
          } else {
            a.mirror.replaceRange('#', {
              line: line,
              ch: 0,
            });
          }
        }
        a.mirror.focus();
      });
    },
    insertQuote: function insertQuote(b) {
      var a = this;
      k(function () {
        var word = a.mirror.findWordAt(a.mirror.getCursor());
        var selection = a.mirror.getSelection().trim();
        if (a.mirror.getSelection().length > 0) {
          // expand selection to be full lines
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
          if (selection == '') {
            a.mirror.replaceSelection('> fancy blockquote');
          } else {
            a.mirror.replaceSelection('> ' + selection);
          }
        }
        a.mirror.focus();
      });
    },
    insertCode: function insertCode(b) {
      var a = this;
      k(function () {
        var word = a.mirror.findWordAt(a.mirror.getCursor());
        var selection = a.mirror.getSelection().trim();
        if (a.mirror.getSelection().length > 0) {
          a.mirror.replaceSelection(
            '\n```sql\n' + a.mirror.getSelection() + '\n```\n',
          );
        } else {
          a.mirror.setSelection(word.anchor, word.head);

          if (selection == '') {
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
    insertUl: function insertUl(b) {
      var a = this;
      k(function () {
        var word = a.mirror.findWordAt(a.mirror.getCursor());
        var selection = a.mirror.getSelection().trim();
        if (a.mirror.getSelection().length > 0) {
          // expand selection to be full lines
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

          if (selection == '') {
            a.mirror.replaceSelection('- item one\n- item two\n- item three');
          } else {
            a.mirror.replaceSelection('- ' + selection);
          }
        }
        a.mirror.focus();
      });
    },
    insertOl: function insertOl(b) {
      var a = this;
      k(function () {
        var word = a.mirror.findWordAt(a.mirror.getCursor());
        var selection = a.mirror.getSelection().trim();
        if (a.mirror.getSelection().length > 0) {
          // expand selection to be full lines
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
          var number = 1;
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

          if (selection == '') {
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
    insertLink: function insertLink(b) {
      var a = this;
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
    save: function save(b) {
      var a = this;
      k(function () {
        var data = {};
        data.id = getUrlVars().id;
        data.description = a.mirror.getValue();
        var q = new XMLHttpRequest();
        q.open('post', a.saveUrl, true);
        q.setRequestHeader('Content-Type', 'text/plain;charset=UTF-8`');
        q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
        q.send(JSON.stringify(data));

        q.onload = function (e) {
          document.getElementById('editorMdl-titleSave').style.visibility =
            'visible';
          setTimeout(function () {
            document
              .getElementById('editorMdl-titleSave')
              .style.removeProperty('visibility');
          }, 750);
        };
      });
    },
  };

  load();
  document.addEventListener('ajax', function () {
    load();
  });
})();
