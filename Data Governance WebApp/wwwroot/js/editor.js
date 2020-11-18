/*
    Atlas of Information Management business intelligence library and documentation database.
    Copyright (C) 2020  Riverside Healthcare, Kankakee, IL

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
(function() {


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
            setTimeout(function() {
                test++;
                load();
            }, 100);
        } else {
            test = 0;
            l();
        }
    }

    function l() {
        var editor = Array.prototype.slice.call(document.querySelectorAll(".editor:not(.loaded)")),
            x;

        for (x = 0; x < editor.length; x++) {
            new e(editor[x]);
        }
    }

    function e(b) {
        this.mdrequest=null;
        this.target = b;
        this.target.classList.add('loaded');
        // editor input
        this.editorInput = document.createElement('div');
        this.editorInput.classList.add('liveEditor');

        this.editorBody = document.createElement('div');
        this.editorBody.classList.add('liveEditor-body');
        this.editorText = document.createElement('textarea');

        var initialValue = "";
        if (this.target.getElementsByTagName('textarea').length > 0) {

            initialValue = this.target.getElementsByTagName('textarea')[0].value;
            this.target.removeChild(this.target.getElementsByTagName('textarea')[0]);
        }


        this.editorText.setAttribute('id', this.target.getAttribute('data-inputId'));
        this.editorText.setAttribute('name', this.target.getAttribute('data-inputName'));

        this.editorBody.appendChild(this.editorText);

        // editor output
        this.editorPrev = document.createElement('div');
        this.editorPrev.classList.add('editor-liveEditorPrev');
        this.editorPrevTitle = document.createElement('h4');
        this.editorPrevTitle.innerHTML = 'Preview';
        this.editorPrev.appendChild(this.editorPrevTitle);


        this.editorPrevText = document.createElement('div');
        this.editorPrev.appendChild(this.editorPrevText);

        // create buttons

        this.btn = document.createElement('div');
        this.btn.classList.add('liveEditor-btnGrp');

        this.btnBold = document.createElement('button');
        this.btnBold.classList.add('liveEditor-btn');
        this.btnBold.setAttribute('type', 'button');
        this.btnBold.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 384 512"><path d="M333.49 238a122 122 0 0 0 27-65.21C367.87 96.49 308 32 233.42 32H34a16 16 0 0 0-16 16v48a16 16 0 0 0 16 16h31.87v288H34a16 16 0 0 0-16 16v48a16 16 0 0 0 16 16h209.32c70.8 0 134.14-51.75 141-122.4 4.74-48.45-16.39-92.06-50.83-119.6zM145.66 112h87.76a48 48 0 0 1 0 96h-87.76zm87.76 288h-87.76V288h87.76a56 56 0 0 1 0 112z"/></svg>';

        this.btn.appendChild(this.btnBold);

        this.btnBold.addEventListener('click', this.insertBold.bind(this), false);

        this.btnItalic = document.createElement('button');
        this.btnItalic.classList.add('liveEditor-btn');
        this.btnItalic.setAttribute('type', 'button');
        this.btnItalic.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 320 512"><path d="M320 48v32a16 16 0 0 1-16 16h-62.76l-80 320H208a16 16 0 0 1 16 16v32a16 16 0 0 1-16 16H16a16 16 0 0 1-16-16v-32a16 16 0 0 1 16-16h62.76l80-320H112a16 16 0 0 1-16-16V48a16 16 0 0 1 16-16h192a16 16 0 0 1 16 16z"/></svg>';

        this.btn.appendChild(this.btnItalic);
        this.btnItalic.addEventListener('click', this.insertItalics.bind(this), false);

        this.btnHeading = document.createElement('button');
        this.btnHeading.classList.add('liveEditor-btn');
        this.btnHeading.setAttribute('type', 'button');
        this.btnHeading.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512"><path d="M448 96v320h32a16 16 0 0 1 16 16v32a16 16 0 0 1-16 16H320a16 16 0 0 1-16-16v-32a16 16 0 0 1 16-16h32V288H160v128h32a16 16 0 0 1 16 16v32a16 16 0 0 1-16 16H32a16 16 0 0 1-16-16v-32a16 16 0 0 1 16-16h32V96H32a16 16 0 0 1-16-16V48a16 16 0 0 1 16-16h160a16 16 0 0 1 16 16v32a16 16 0 0 1-16 16h-32v128h192V96h-32a16 16 0 0 1-16-16V48a16 16 0 0 1 16-16h160a16 16 0 0 1 16 16v32a16 16 0 0 1-16 16z"/></svg>';

        this.btn.appendChild(this.btnHeading);
        this.btnHeading.addEventListener('click', this.insertHeading.bind(this), false);

        this.btnHeadingSep = document.createElement('span');
        this.btnHeadingSep.classList.add('liveEditor-btnSep');
        this.btn.appendChild(this.btnHeadingSep);

        this.btnQuote = document.createElement('button');
        this.btnQuote.classList.add('liveEditor-btn');
        this.btnQuote.setAttribute('type', 'button');
        this.btnQuote.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512"><path d="M464 256h-80v-64c0-35.3 28.7-64 64-64h8c13.3 0 24-10.7 24-24V56c0-13.3-10.7-24-24-24h-8c-88.4 0-160 71.6-160 160v240c0 26.5 21.5 48 48 48h128c26.5 0 48-21.5 48-48V304c0-26.5-21.5-48-48-48zm-288 0H96v-64c0-35.3 28.7-64 64-64h8c13.3 0 24-10.7 24-24V56c0-13.3-10.7-24-24-24h-8C71.6 32 0 103.6 0 192v240c0 26.5 21.5 48 48 48h128c26.5 0 48-21.5 48-48V304c0-26.5-21.5-48-48-48z"/></svg>';

        this.btn.appendChild(this.btnQuote);
        this.btnQuote.addEventListener('click', this.insertQuote.bind(this), false);

        this.btnCode = document.createElement('button');
        this.btnCode.classList.add('liveEditor-btn');
        this.btnCode.setAttribute('type', 'button');
        this.btnCode.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 640 512"><path d="M278.9 511.5l-61-17.7c-6.4-1.8-10-8.5-8.2-14.9L346.2 8.7c1.8-6.4 8.5-10 14.9-8.2l61 17.7c6.4 1.8 10 8.5 8.2 14.9L293.8 503.3c-1.9 6.4-8.5 10.1-14.9 8.2zm-114-112.2l43.5-46.4c4.6-4.9 4.3-12.7-.8-17.2L117 256l90.6-79.7c5.1-4.5 5.5-12.3.8-17.2l-43.5-46.4c-4.5-4.8-12.1-5.1-17-.5L3.8 247.2c-5.1 4.7-5.1 12.8 0 17.5l144.1 135.1c4.9 4.6 12.5 4.4 17-.5zm327.2.6l144.1-135.1c5.1-4.7 5.1-12.8 0-17.5L492.1 112.1c-4.8-4.5-12.4-4.3-17 .5L431.6 159c-4.6 4.9-4.3 12.7.8 17.2L523 256l-90.6 79.7c-5.1 4.5-5.5 12.3-.8 17.2l43.5 46.4c4.5 4.9 12.1 5.1 17 .6z"/></svg>';

        this.btn.appendChild(this.btnCode);
        this.btnCode.addEventListener('click', this.insertCode.bind(this), false);

        this.btnCodeSep = document.createElement('span');
        this.btnCodeSep.classList.add('liveEditor-btnSep');
        this.btn.appendChild(this.btnCodeSep);

        this.btnUl = document.createElement('button');
        this.btnUl.classList.add('liveEditor-btn');
        this.btnUl.setAttribute('type', 'button');
        this.btnUl.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512"><path d="M48 48a48 48 0 1 0 48 48 48 48 0 0 0-48-48zm0 160a48 48 0 1 0 48 48 48 48 0 0 0-48-48zm0 160a48 48 0 1 0 48 48 48 48 0 0 0-48-48zm448 16H176a16 16 0 0 0-16 16v32a16 16 0 0 0 16 16h320a16 16 0 0 0 16-16v-32a16 16 0 0 0-16-16zm0-320H176a16 16 0 0 0-16 16v32a16 16 0 0 0 16 16h320a16 16 0 0 0 16-16V80a16 16 0 0 0-16-16zm0 160H176a16 16 0 0 0-16 16v32a16 16 0 0 0 16 16h320a16 16 0 0 0 16-16v-32a16 16 0 0 0-16-16z"/></svg>';

        this.btn.appendChild(this.btnUl);
        this.btnUl.addEventListener('click', this.insertUl.bind(this), false);

        this.btnOl = document.createElement('button');
        this.btnOl.classList.add('liveEditor-btn');
        this.btnOl.setAttribute('type', 'button');
        this.btnOl.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512"><path d="M61.77 401l17.5-20.15a19.92 19.92 0 0 0 5.07-14.19v-3.31C84.34 356 80.5 352 73 352H16a8 8 0 0 0-8 8v16a8 8 0 0 0 8 8h22.83a157.41 157.41 0 0 0-11 12.31l-5.61 7c-4 5.07-5.25 10.13-2.8 14.88l1.05 1.93c3 5.76 6.29 7.88 12.25 7.88h4.73c10.33 0 15.94 2.44 15.94 9.09 0 4.72-4.2 8.22-14.36 8.22a41.54 41.54 0 0 1-15.47-3.12c-6.49-3.88-11.74-3.5-15.6 3.12l-5.59 9.31c-3.72 6.13-3.19 11.72 2.63 15.94 7.71 4.69 20.38 9.44 37 9.44 34.16 0 48.5-22.75 48.5-44.12-.03-14.38-9.12-29.76-28.73-34.88zM496 224H176a16 16 0 0 0-16 16v32a16 16 0 0 0 16 16h320a16 16 0 0 0 16-16v-32a16 16 0 0 0-16-16zm0-160H176a16 16 0 0 0-16 16v32a16 16 0 0 0 16 16h320a16 16 0 0 0 16-16V80a16 16 0 0 0-16-16zm0 320H176a16 16 0 0 0-16 16v32a16 16 0 0 0 16 16h320a16 16 0 0 0 16-16v-32a16 16 0 0 0-16-16zM16 160h64a8 8 0 0 0 8-8v-16a8 8 0 0 0-8-8H64V40a8 8 0 0 0-8-8H32a8 8 0 0 0-7.14 4.42l-8 16A8 8 0 0 0 24 64h8v64H16a8 8 0 0 0-8 8v16a8 8 0 0 0 8 8zm-3.91 160H80a8 8 0 0 0 8-8v-16a8 8 0 0 0-8-8H41.32c3.29-10.29 48.34-18.68 48.34-56.44 0-29.06-25-39.56-44.47-39.56-21.36 0-33.8 10-40.46 18.75-4.37 5.59-3 10.84 2.8 15.37l8.58 6.88c5.61 4.56 11 2.47 16.12-2.44a13.44 13.44 0 0 1 9.46-3.84c3.33 0 9.28 1.56 9.28 8.75C51 248.19 0 257.31 0 304.59v4C0 316 5.08 320 12.09 320z"/></svg>';

        this.btn.appendChild(this.btnOl);
        this.btnOl.addEventListener('click', this.insertOl.bind(this), false);

        this.btnOlSep = document.createElement('span');
        this.btnOlSep.classList.add('liveEditor-btnSep');
        this.btn.appendChild(this.btnOlSep);

        this.btnLink = document.createElement('button');
        this.btnLink.classList.add('liveEditor-btn');
        this.btnLink.setAttribute('type', 'button');
        this.btnLink.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512"><path d="M326.612 185.391c59.747 59.809 58.927 155.698.36 214.59-.11.12-.24.25-.36.37l-67.2 67.2c-59.27 59.27-155.699 59.262-214.96 0-59.27-59.26-59.27-155.7 0-214.96l37.106-37.106c9.84-9.84 26.786-3.3 27.294 10.606.648 17.722 3.826 35.527 9.69 52.721 1.986 5.822.567 12.262-3.783 16.612l-13.087 13.087c-28.026 28.026-28.905 73.66-1.155 101.96 28.024 28.579 74.086 28.749 102.325.51l67.2-67.19c28.191-28.191 28.073-73.757 0-101.83-3.701-3.694-7.429-6.564-10.341-8.569a16.037 16.037 0 0 1-6.947-12.606c-.396-10.567 3.348-21.456 11.698-29.806l21.054-21.055c5.521-5.521 14.182-6.199 20.584-1.731a152.482 152.482 0 0 1 20.522 17.197zM467.547 44.449c-59.261-59.262-155.69-59.27-214.96 0l-67.2 67.2c-.12.12-.25.25-.36.37-58.566 58.892-59.387 154.781.36 214.59a152.454 152.454 0 0 0 20.521 17.196c6.402 4.468 15.064 3.789 20.584-1.731l21.054-21.055c8.35-8.35 12.094-19.239 11.698-29.806a16.037 16.037 0 0 0-6.947-12.606c-2.912-2.005-6.64-4.875-10.341-8.569-28.073-28.073-28.191-73.639 0-101.83l67.2-67.19c28.239-28.239 74.3-28.069 102.325.51 27.75 28.3 26.872 73.934-1.155 101.96l-13.087 13.087c-4.35 4.35-5.769 10.79-3.783 16.612 5.864 17.194 9.042 34.999 9.69 52.721.509 13.906 17.454 20.446 27.294 10.606l37.106-37.106c59.271-59.259 59.271-155.699.001-214.959z"/></svg>';

        this.btn.appendChild(this.btnLink);
        this.btnLink.addEventListener('click', this.insertLink.bind(this), false);

        this.editorInput.appendChild(this.btn);

        if (this.target.getAttribute('data-saveUrl')) {

            this.btnSave = document.createElement('button');
            this.btnSave.classList.add('liveEditor-btn');
            this.btnSave.classList.add('save');
            this.btnSave.setAttribute('type', 'button');
            this.btnSave.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512"><path d="M433.941 129.941l-83.882-83.882A48 48 0 0 0 316.118 32H48C21.49 32 0 53.49 0 80v352c0 26.51 21.49 48 48 48h352c26.51 0 48-21.49 48-48V163.882a48 48 0 0 0-14.059-33.941zM224 416c-35.346 0-64-28.654-64-64 0-35.346 28.654-64 64-64s64 28.654 64 64c0 35.346-28.654 64-64 64zm96-304.52V212c0 6.627-5.373 12-12 12H76c-6.627 0-12-5.373-12-12V108c0-6.627 5.373-12 12-12h228.52c3.183 0 6.235 1.264 8.485 3.515l3.48 3.48A11.996 11.996 0 0 1 320 111.48z"/></svg>';
            this.btnSave.addEventListener('click', this.save.bind(this), false);

            this.editorInput.appendChild(this.btnSave);

            this.saveUrl = this.target.getAttribute('data-saveUrl');
        }

        this.editorInput.appendChild(this.editorBody);
        this.target.appendChild(this.editorInput);
        this.target.appendChild(this.editorPrev);

        // load code mirror
        this.mirror = CodeMirror.fromTextArea(this.editorText, {
            mode: "gfm",
            autoRefresh: true,
            viewportMargin: Infinity,
            lineWrapping: true
        });

        this.mirror.setValue(initialValue);

        this.mirror.on('change', this.updateMirror.bind(this), false);

        if (this.target.closest('.mdl')) {
            this.target.closest('.mdl').addEventListener('mdl-open', this.updateMirror.bind(this));
        }
    }


    var k =
        document.requestAnimationFrame ||
        document.setImmediate ||
        function(b) {
            return setTimeout(b, 0);
        };

    e.prototype = {
        // mirror even
        updateMirror: function updateMirror(b) {
            var a = this;
           
            k(function() {
                var data = {
                        md: a.mirror.getValue()
                    };
                if (a.mdrequest) {
                    a.mdrequest.abort();
                }
                a.mdrequest = new XMLHttpRequest();
                a.mdrequest.open('post', "/API?handler=RenderMarkdown", true);
                a.mdrequest.setRequestHeader('Content-Type', 'text/plain;charset=UTF-8`');
                a.mdrequest.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
                a.mdrequest.send(JSON.stringify(data));
                a.mdrequest.onload = function() {
                    a.editorPrevText.innerHTML = a.mdrequest.responseText;
                    document.dispatchEvent(new CustomEvent('code-highlight'));
                    document.dispatchEvent(new CustomEvent('load-charts'));
                };
            });
        },
        // button events
        insertBold: function insertBold(b) {
            var a = this;

            k(function() {
                var word = a.mirror.findWordAt(a.mirror.getCursor());
                var selection = a.mirror.getSelection().trim();
                if (a.mirror.getSelection().length > 0) {
                    a.mirror.replaceSelection('**' + a.mirror.getSelection().trim() + '**');
                } else {
                    a.mirror.setSelection(word.anchor, word.head);

                    if (selection == "") {
                        selection = "bold text";
                    }

                    a.mirror.replaceSelection('**' + selection + '**');
                }
                a.mirror.focus();
            });
        },
        insertItalics: function insertItalics(b) {
            var a = this;
            k(function() {
                var word = a.mirror.findWordAt(a.mirror.getCursor());
                var selection = a.mirror.getSelection().trim();
                if (a.mirror.getSelection().length > 0) {
                    a.mirror.replaceSelection('*' + a.mirror.getSelection().trim() + '*');
                } else {
                    a.mirror.setSelection(word.anchor, word.head);

                    if (selection == "") {
                        selection = "italic text";
                    }
                    a.mirror.replaceSelection('*' + selection + '*');
                }
                a.mirror.focus();
            });
        },
        insertHeading: function insertHeading(b) {
            var a = this;
            k(function() {
                var line = a.mirror.getCursor().line,
                    currentHead = a.mirror.getLine(line),
                    start = currentHead.split(" ")[0];

                if (currentHead == "") {
                    a.mirror.replaceRange("# Heading", {
                        line: line,
                        ch: 0
                    });
                } else if (start[0] != '#') {
                    a.mirror.replaceRange("# ", {
                        line: line,
                        ch: 0
                    });
                } else {
                    var level = (start.match(/#/g) || []).length;
                    if (level >= 6) {
                        a.mirror.replaceRange(currentHead.substring(currentHead.indexOf(" ") + 1), {
                            line: line,
                            ch: 0
                        }, {
                            line: line,
                            ch: currentHead.length
                        });
                    } else {
                        a.mirror.replaceRange("#", {
                            line: line,
                            ch: 0
                        });
                    }
                }
                a.mirror.focus();
            });
        },
        insertQuote: function insertQuote(b) {
            var a = this;
            k(function() {
                var word = a.mirror.findWordAt(a.mirror.getCursor());
                var selection = a.mirror.getSelection().trim();
                if (a.mirror.getSelection().length > 0) {
                    // expand selection to be full lines
                    a.mirror.setSelection({
                        line: Math.min(a.mirror.listSelections()[0].head.line, a.mirror.listSelections()[0].anchor.line),
                        ch: 0
                    }, {
                        line: Math.max(a.mirror.listSelections()[0].head.line, a.mirror.listSelections()[0].anchor.line),
                        ch: 1
                    });
                    a.mirror.replaceSelection('> ' + a.mirror.getSelection().replace(/\n/g, function() {
                        return '\n> ';
                    }));
                } else {
                    a.mirror.setSelection({
                        line: Math.min(word.head.line, word.anchor.line),
                        ch: 0
                    }, {
                        line: Math.max(word.head.line, word.anchor.line),
                        ch: 1
                    });
                    if (selection == "") {
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
            k(function() {
                var word = a.mirror.findWordAt(a.mirror.getCursor());
                var selection = a.mirror.getSelection().trim();
                if (a.mirror.getSelection().length > 0) {
                    a.mirror.replaceSelection('\n```sql\n' + a.mirror.getSelection() + '\n```\n');
                } else {
                    a.mirror.setSelection(word.anchor, word.head);

                    if (selection == "") {
                        a.mirror.replaceSelection('\n```sql\nselect smiles; -- :) --\n```\n');
                    } else {
                        a.mirror.replaceSelection('`' + selection + '`');
                    }
                }
                a.mirror.focus();
            });
        },
        insertUl: function insertUl(b) {
            var a = this;
            k(function() {
                var word = a.mirror.findWordAt(a.mirror.getCursor());
                var selection = a.mirror.getSelection().trim();
                if (a.mirror.getSelection().length > 0) {
                    // expand selection to be full lines
                    a.mirror.setSelection({
                        line: Math.min(a.mirror.listSelections()[0].head.line, a.mirror.listSelections()[0].anchor.line),
                        ch: 0
                    }, {
                        line: Math.max(a.mirror.listSelections()[0].head.line, a.mirror.listSelections()[0].anchor.line),
                        ch: 1
                    });
                    a.mirror.replaceSelection('- ' + a.mirror.getSelection().replace(/\n/g, function() {
                        return '\n- ';
                    }));
                } else {
                    a.mirror.setSelection({
                        line: Math.min(word.head.line, word.anchor.line),
                        ch: 0
                    }, {
                        line: Math.max(word.head.line, word.anchor.line),
                        ch: 1
                    });

                    if (selection == "") {
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
            k(function() {
                var word = a.mirror.findWordAt(a.mirror.getCursor());
                var selection = a.mirror.getSelection().trim();
                if (a.mirror.getSelection().length > 0) {
                    // expand selection to be full lines
                    a.mirror.setSelection({
                        line: Math.min(a.mirror.listSelections()[0].head.line, a.mirror.listSelections()[0].anchor.line),
                        ch: 0
                    }, {
                        line: Math.max(a.mirror.listSelections()[0].head.line, a.mirror.listSelections()[0].anchor.line),
                        ch: 1
                    });
                    var number = 1;
                    a.mirror.replaceSelection(number + '. ' + a.mirror.getSelection().replace(/\n/g, function() {
                        return '\n' + (++number) + '. ';
                    }));
                } else {
                    a.mirror.setSelection({
                        line: Math.min(word.head.line, word.anchor.line),
                        ch: 0
                    }, {
                        line: Math.max(word.head.line, word.anchor.line),
                        ch: 1
                    });

                    if (selection == "") {
                        a.mirror.replaceSelection('1. item one\n2. item two\n3. item three');
                    } else {
                        a.mirror.replaceSelection('1. ' + selection);
                    }
                }
                a.mirror.focus();
            });
        },
        insertLink: function insertLink(b) {
            var a = this;
            k(function() {
                if (a.mirror.getSelection().length > 0) {
                    a.mirror.replaceSelection("[" + a.mirror.getSelection().trim() + "](https://atlas)");
                } else {
                    a.mirror.replaceSelection("[Link Title](https://atlas)");
                }
                a.mirror.focus();
            });
        },
        save: function save(b) {
            var a = this;
            k(function() {


                var data = {};
                data.id = getUrlVars().id;
                data.description = a.mirror.getValue();
                var q = new XMLHttpRequest();
                q.open('post', a.saveUrl, true);
                q.setRequestHeader('Content-Type', 'text/plain;charset=UTF-8`');
                q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
                q.send(JSON.stringify(data));

                q.onload = function(e) {
                    document.getElementById('editorMdl-titleSave').style.visibility = 'visible';
                    setTimeout(function() {
                        document.getElementById('editorMdl-titleSave').style.removeProperty("visibility");
                    }, 750);
                };
            });
        }

    };

    load();
    document.addEventListener("ajax", function() {
        load();
    });
    console.log('editor scripts loaded');
})();