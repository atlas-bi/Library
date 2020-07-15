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

    var l = function() {

        // show editor button.
        if (document.getElementsByClassName('editor-buttonHidden').length > 0) {
            document.getElementsByClassName('editor-buttonHidden')[0].classList.remove('editor-buttonHidden');

            var d = document,
                devDec = document.getElementById('ReportObjectDoc_DeveloperDescription'),
                devDecMirror = CodeMirror.fromTextArea(devDec, {
                    mode: "gfm",
                    autoRefresh: true,
                    viewportMargin: Infinity,
                    lineWrapping: true
                }),
                keyAssump = document.getElementById('ReportObjectDoc_KeyAssumptions'),
                keyAssumpMirror = CodeMirror.fromTextArea(keyAssump, {
                    mode: "gfm",
                    autoRefresh: true,
                    viewportMargin: Infinity,
                    lineWrapping: true
                }),
                Id = getUrlVars().id,
                q,
                saveMessage = function() {
                    document.getElementById('editorMdl-titleSave').style.visibility = 'visible';
                    setTimeout(function() {
                        document.getElementById('editorMdl-titleSave').style.removeProperty("visibility");
                    }, 750);
                },
                saveReportDescription = function() {
                    var value = devDecMirror.getValue(),

                        data = {};
                    data.id = Id;
                    data.description = value;
                    q = new XMLHttpRequest();
                    q.open('post', "/Reports?handler=NewDescription", true);
                    q.setRequestHeader('Content-Type', 'text/plain;charset=UTF-8`');
                    q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
                    q.send(JSON.stringify(data));

                    q.onload = function(e) {
                        var data = q.responseText;
                        if (data.trim().length == 0) {
                            document.querySelector('.navLinks-link[href="description"]').parentElement.removeChild(document.querySelector('.navLinks-link[href="description"]'));
                            document.getElementById('report-description-container').innerHTML = '';
                        } else {
                            if (document.querySelector('.navLinks-link[href="#description"]') == null) {
                                var newHtml = '<a href="#description" class="navLinks-link">Description</a>';
                                if (document.querySelector('.navLinks-link[href="#images"]') == null) {
                                    document.querySelector('.navLinks-link[href="#images"]').insertAdjacentHTML('afterend', newHtml);
                                } else {
                                    document.getElementsByClassName('navLinks')[0].innerHTML = newHtml + document.getElementsByClassName('navLinks')[0].innerHTML;
                                }
                            }
                            document.getElementById('report-description-container').innerHTML = data;

                            document.dispatchEvent(new CustomEvent('code-highlight'));
                            document.dispatchEvent(new CustomEvent('load-charts'));

                            saveMessage();
                        }
                    };
                },
                saveReportKeyAssumptions = function() {
                    var value = keyAssumpMirror.getValue(),


                        data = {};
                    data.id = Id;
                    data.description = value;


                    q = new XMLHttpRequest();
                    q.open('post', "/Reports?handler=NewKeyAssumptions", true);
                    q.setRequestHeader('Content-Type', 'text/plain;charset=UTF-8`');
                    q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
                    q.send(JSON.stringify(data));

                    q.onload = function(e) {
                        var data = q.responseText;
                        if (data.trim().length == 0) {
                            document.querySelector('.navLinks-link[href="description"]').parentElement.removeChild(document.querySelector('.navLinks-link[href="description"]'));
                            document.getElementById('report-description-container').innerHTML = '';
                        } else {
                            if (document.querySelector('.navLinks-link[href="#description"]') == null) {
                                var newHtml = '<a href="#description" class="navLinks-link">Description</a>';
                                if (document.querySelector('.navLinks-link[href="#images"]') == null) {
                                    document.querySelector('.navLinks-link[href="#images"]').insertAdjacentHTML('afterend', newHtml);
                                } else {
                                    document.getElementsByClassName('navLinks')[0].innerHTML = newHtml + document.getElementsByClassName('navLinks')[0].innerHTML;
                                }
                            }
                            document.getElementById('report-description-container').innerHTML = data;

                            document.dispatchEvent(new CustomEvent('code-highlight'));
                            document.dispatchEvent(new CustomEvent('load-charts'));

                            saveMessage();
                        }
                    };
                };

            document.addEventListener('click', function(e) {
                if (e.target.closest('#report-edit-description-save-button')) {
                    saveReportDescription();
                } else if (e.target.closest('#report-edit-keyassumptions-save-button')) {
                    saveReportKeyAssumptions();
                }
            });


            devDecMirror.on('change', function(e) {

                var data = {
                    md: devDecMirror.getValue()
                };


                if (q != null) q.abort();

                q = new XMLHttpRequest();
                q.open('post', "/API?handler=RenderMarkdown", true);
                q.setRequestHeader('Content-Type', 'text/plain;charset=UTF-8`');
                q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
                q.send(JSON.stringify(data));
                q.onload = function() {
                    document.getElementById('dev-desc-editor-preview').innerHTML = q.responseText;
                    document.dispatchEvent(new CustomEvent('code-highlight'));
                    document.dispatchEvent(new CustomEvent('load-charts'));
                };
            });
            keyAssumpMirror.on('change', function(e) {

                var data = {
                        md: keyAssumpMirror.getValue()
                    },
                    url = Object.keys(data).map(function(k) {
                        return encodeURIComponent(k) + '=' + encodeURIComponent(data[k]);
                    }).join('&');

                if (q != null) q.abort();

                q = new XMLHttpRequest();
                q.open('post', "/API?handler=RenderMarkdown", true);
                q.setRequestHeader('Content-Type', 'text/plain;charset=UTF-8`');
                q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
                q.send(JSON.stringify(data));
                q.onload = function() {
                    document.getElementById('key-assump-editor-preview').innerHTML = q.responseText;
                    document.dispatchEvent(new CustomEvent('code-highlight'));
                    document.dispatchEvent(new CustomEvent('load-charts'));
                };
            });


            /* images */
            document.addEventListener('change', function(e) {
                if (e.target.matches('#report-add-image-input')) {

                    var form = e.target.closest('form'),
                        u, id = form.querySelector('[name="Id"]').value;

                    q = new XMLHttpRequest();
                    q.open('post', form.getAttribute('action'), true);
                    q.withCredentials = true;
                    q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
                    q.send(new FormData(form));
                    q.upload.addEventListener("progress", function(e) {
                        document.dispatchEvent(new CustomEvent('progress-start', {
                            cancelable: true,
                            detail: {
                                value: e.total / e.loaded
                            }
                        }));
                    });
                    q.onload = function() {
                        form.closest('.editorImg-img').insertAdjacentHTML('beforebegin', q.responseText);

                        if (document.querySelector('.navLinks-link[href="#images"]') == null) {
                            var newHtml = '<a href="#images" class="navLinks-link">Images</a>';
                            document.getElementsByClassName('navLinks')[0].innerHTML = newHtml + document.getElementsByClassName('navLinks')[0].innerHTML;
                        }

                        e.target.setAttribute('value', '');
                        e.target.value = null;
                        u = new XMLHttpRequest();
                        u.open('get', "reports?handler=LoadImages&id=" + id, true);
                        u.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded; charset=UTF-8');
                        u.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
                        u.send();

                        u.onload = function() {
                            document.getElementById('report-image-container').innerHTML = u.responseText;
                            document.dispatchEvent(new CustomEvent('load-carousel'));
                            document.dispatchEvent(new CustomEvent('scroll'));
                        };
                    };
                }
            });

            document.addEventListener('submit', function(e) {
                if (e.target.closest('.report-remove-image')) {
                    e.preventDefault();
                    var url, el = e.target.closest('.editorImg-img'),
                        u, id = el.querySelector('[name="Id"]').value,
                        imgId = el.querySelector('[name="ImgId"]').value,
                        data = {};
                    data.Id = id;
                    data.ImgId = imgId;

                    url = Object.keys(data).map(function(k) {
                        return encodeURIComponent(k) + '=' + encodeURIComponent(data[k]);
                    }).join('&');

                    q = new XMLHttpRequest();
                    q.open('post', "reports?handler=RemoveImage&" + url, true);
                    q.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded; charset=UTF-8');
                    q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
                    q.send();

                    q.onload = function() {
                        u = new XMLHttpRequest();
                        u.open('get', "reports?handler=LoadImages&id=" + id, true);
                        u.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded; charset=UTF-8');
                        u.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
                        u.send();

                        u.onload = function() {
                            document.getElementById('report-image-container').innerHTML = u.responseText;
                            document.dispatchEvent(new CustomEvent('load-carousel'));
                            document.dispatchEvent(new CustomEvent('scroll'));
                        };

                        // if there are 0 left, remove nav link
                        if (document.getElementsByClassName('editorImg-img').length <= 2) {
                            document.querySelector('.navLinks-link[href="#images"]').parentElement.removeChild(document.querySelector('.navLinks-link[href="#images"]'));
                        }
                        el.parentElement.removeChild(el);
                    };

                }

            });


            /* editor */

            document.addEventListener('click', function(e) {
                if (e.target.closest('.liveEditor-btnGrp')) {
                    var doc = e.target.closest('.liveEditor').getElementsByClassName('markdown-editor')[0].getAttribute('id') == 'ReportObjectDoc_DeveloperDescription' ? devDecMirror : keyAssumpMirror,
                        word = doc.findWordAt(doc.getCursor()),
                        selection = doc.getSelection().trim();
                    if (e.target.closest('.liveEditor-btnGrp > .insert-link')) {
                        if (doc.getSelection().length > 0) {
                            doc.replaceSelection("[" + doc.getSelection().trim() + "](https://atlas)");
                        } else {
                            doc.replaceSelection("[Link Title](https://atlas)");
                        }
                        doc.focus();
                    } else if (e.target.closest('.liveEditor-btnGrp > .insert-bold')) {
                        if (doc.getSelection().length > 0) {
                            doc.replaceSelection('**' + doc.getSelection().trim() + '**');
                        } else {
                            doc.setSelection(word.anchor, word.head);

                            if (selection == "") {
                                selection = "bold text";
                            }

                            doc.replaceSelection('**' + selection + '**');
                        }
                        doc.focus();
                    } else if (e.target.closest('.liveEditor-btnGrp > .insert-italic')) {
                        if (doc.getSelection().length > 0) {
                            doc.replaceSelection('*' + doc.getSelection().trim() + '*');
                        } else {
                            doc.setSelection(word.anchor, word.head);

                            if (selection == "") {
                                selection = "italic text";
                            }
                            doc.replaceSelection('*' + selection + '*');
                        }
                        doc.focus();
                    } else if (e.target.closest('.liveEditor-btnGrp > .insert-code')) {
                        if (doc.getSelection().length > 0) {
                            doc.replaceSelection('```sql\n' + doc.getSelection() + '\n```');
                        } else {
                            doc.setSelection(word.anchor, word.head);

                            if (selection == "") {
                                doc.replaceSelection('```sql\nselect smiles; -- :) --\n```');
                            } else {
                                doc.replaceSelection('`' + selection + '`');
                            }
                        }
                        doc.focus();
                    } else if (e.target.closest('.liveEditor-btnGrp > .insert-heading')) {
                        var line = doc.getCursor().line,
                            currentHead = doc.getLine(line),
                            start = currentHead.split(" ")[0];

                        if (currentHead == "") {
                            doc.replaceRange("# Heading", {
                                line: line,
                                ch: 0
                            });
                        } else if (start[0] != '#') {
                            doc.replaceRange("# ", {
                                line: line,
                                ch: 0
                            });
                        } else {
                            var level = (start.match(/#/g) || []).length;
                            if (level >= 6) {
                                doc.replaceRange(currentHead.substring(currentHead.indexOf(" ") + 1), {
                                    line: line,
                                    ch: 0
                                }, {
                                    line: line,
                                    ch: currentHead.length
                                });
                            } else {
                                doc.replaceRange("#", {
                                    line: line,
                                    ch: 0
                                });
                            }
                        }
                        doc.focus();
                    } else if (e.target.closest('.liveEditor-btnGrp > .insert-ol')) {
                        if (doc.getSelection().length > 0) {
                            // expand selection to be full lines
                            doc.setSelection({
                                line: Math.min(doc.listSelections()[0].head.line, doc.listSelections()[0].anchor.line),
                                ch: 0
                            }, {
                                line: Math.max(doc.listSelections()[0].head.line, doc.listSelections()[0].anchor.line),
                                ch: 1
                            });
                            var number = 1;
                            doc.replaceSelection(number + '. ' + doc.getSelection().replace(/\n/g, function() {
                                return '\n' + (++number) + '. ';
                            }));
                        } else {
                            doc.setSelection({
                                line: Math.min(word.head.line, word.anchor.line),
                                ch: 0
                            }, {
                                line: Math.max(word.head.line, word.anchor.line),
                                ch: 1
                            });

                            if (selection == "") {
                                doc.replaceSelection('1. item one\n2. item two\n3. item three');
                            } else {
                                doc.replaceSelection('1. ' + selection);
                            }
                        }
                        doc.focus();
                    } else if (e.target.closest('.liveEditor-btnGrp > .insert-ul')) {
                        if (doc.getSelection().length > 0) {
                            // expand selection to be full lines
                            doc.setSelection({
                                line: Math.min(doc.listSelections()[0].head.line, doc.listSelections()[0].anchor.line),
                                ch: 0
                            }, {
                                line: Math.max(doc.listSelections()[0].head.line, doc.listSelections()[0].anchor.line),
                                ch: 1
                            });
                            doc.replaceSelection('- ' + doc.getSelection().replace(/\n/g, function() {
                                return '\n- ';
                            }));
                        } else {
                            doc.setSelection({
                                line: Math.min(word.head.line, word.anchor.line),
                                ch: 0
                            }, {
                                line: Math.max(word.head.line, word.anchor.line),
                                ch: 1
                            });

                            if (selection == "") {
                                doc.replaceSelection('- item one\n- item two\n- item three');
                            } else {
                                doc.replaceSelection('- ' + selection);
                            }
                        }
                        doc.focus();
                    } else if (e.target.closest('.liveEditor-btnGrp > .insert-quote')) {
                        if (doc.getSelection().length > 0) {
                            // expand selection to be full lines
                            doc.setSelection({
                                line: Math.min(doc.listSelections()[0].head.line, doc.listSelections()[0].anchor.line),
                                ch: 0
                            }, {
                                line: Math.max(doc.listSelections()[0].head.line, doc.listSelections()[0].anchor.line),
                                ch: 1
                            });
                            doc.replaceSelection('> ' + doc.getSelection().replace(/\n/g, function() {
                                return '\n> ';
                            }));
                        } else {
                            doc.setSelection({
                                line: Math.min(word.head.line, word.anchor.line),
                                ch: 0
                            }, {
                                line: Math.max(word.head.line, word.anchor.line),
                                ch: 1
                            });
                            if (selection == "") {
                                doc.replaceSelection('> fancy blockquote');
                            } else {
                                doc.replaceSelection('> ' + selection);
                            }
                        }
                        doc.focus();
                    }
                }
            });

            /* terms */

            // show existing term detials when searched.
            document.addEventListener('change', function(e) {
                if (e.target.matches('#NewTermLink_TermId')) {


                    q = new XMLHttpRequest();
                    q.open('post', "/Reports?handler=CurrentTermDetails&TermId=" + e.target.value, true);
                    q.setRequestHeader('Content-Type', 'text/plain;charset=UTF-8`');
                    q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
                    q.send();
                    q.onload = function() {
                        if (q.responseText) {
                            data = JSON.parse(q.responseText);
                            document.getElementsByClassName('new-term-summary')[0].innerHTML = '<div class="markdown noleft">' + data.summary || 'n/a' + '</div>';
                            document.getElementsByClassName('new-term-def')[0].innerHTML = '<div class="markdown noleft">' + data.technicalDefinition || 'n/a' + '</div>';
                            // render markdown
                            document.dispatchEvent(new CustomEvent('code-highlight'));
                        }
                    };
                }
            });

            document.addEventListener('submit', function(e) {
                var q, data, u, form, url,
                    termsContainer = document.getElementById('report-terms-container');
                if (e.target.closest('#report-editor-remove-term-form') || e.target.closest('#report-editor-add-term-form') || e.target.closest('#report-editor-add-new-term-form')) {
                    e.preventDefault();

                    form = e.target.closest('#report-editor-remove-term-form') || e.target.closest('#report-editor-add-term-form') || e.target.closest('#report-editor-add-new-term-form');
                    url = form.getAttribute('action');

                    q = new XMLHttpRequest();
                    q.open('post', form.getAttribute('action') + '&' + serialize(form), true);
                    q.setRequestHeader('Content-Type', 'text/plain;charset=UTF-8`');
                    q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
                    q.send();
                    q.onload = function() {
                        data = q.responseText;

                        if (data.trim().length == 0) {
                            termsContainer.innerHTML = '';
                            document.querySelector('.navLinks a[href="#terms"]').parentElement.removeChild(document.querySelector('.navLinks a[href="#terms"]'));
                        } else {
                            termsContainer.innerHTML = data;
                            // render markdown
                            document.dispatchEvent(new CustomEvent('code-highlight'));

                            if (document.querySelector('.navLinks-link[href="#terms"]') == null) {
                                var newHtml = '<a href="#terms" class="navLinks-link">Terms</a>';
                                document.querySelector('.navLinks-link[href="#details"]').insertAdjacentHTML('beforebegin', newHtml);
                            }
                        }
                        saveMessage();
                        u = new XMLHttpRequest();
                        u.open('get', 'Reports?handler=TermLinks&Id=' + Id, true);
                        u.setRequestHeader('Content-Type', 'text/plain;charset=UTF-8`');
                        u.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
                        u.send();
                        u.onload = function() {
                            document.getElementById('report-editor-currentterms-container').innerHTML = u.responseText;
                        };
                    };
                    form.reset();
                    document.getElementsByClassName('new-term-summary')[0].innerHTML = '';
                    document.getElementsByClassName('new-term-def')[0].innerHTML = '';
                }
            });

            /* details */

            /* term approved */
            var approvedYnHiddenTextArea = d.getElementsByClassName('approvedYnHiddenTextArea')[0],
                approvedYn = d.getElementById('approvedYn');

            if (approvedYnHiddenTextArea && approvedYn) {
                approvedYn.checked = approvedYnHiddenTextArea.value == "Y" ? true : false;
                approvedYn.addEventListener('change', function(e) {
                    approvedYnHiddenTextArea.value = approvedYn.checked ? "Y" : "N";
                });
            }


            var executiveVisibilityYnHiddenTextArea = d.getElementsByClassName('ExecutiveVisibilityYnHiddenTextArea')[0],
                executiveVisibilityYn = d.getElementById('ExecutiveVisibilityYn');

            if (executiveVisibilityYnHiddenTextArea && executiveVisibilityYn) {
                executiveVisibilityYn.checked = executiveVisibilityYnHiddenTextArea.value == "Y" ? true : false;
                executiveVisibilityYn.addEventListener('change', function(e) {
                    executiveVisibilityYnHiddenTextArea.value = executiveVisibilityYn.checked ? "Y" : "N";
                });
            }


            var hiddenReportHiddenTextArea = d.getElementsByClassName('HiddenReportHiddenTextArea')[0],
                hidden = d.getElementById('Hidden');

            if (hiddenReportHiddenTextArea && hidden) {
                hidden.checked = hiddenReportHiddenTextArea.value == "Y" ? true : false;
                hidden.addEventListener('change', function(e) {
                    hiddenReportHiddenTextArea.value = hidden.checked ? "Y" : "N";
                });
            }

            /* ssrs only */
            var enabledForHyperspaceHiddenTextArea = d.getElementsByClassName('EnabledForHyperspaceHiddenTextArea')[0],
                enabledForHyperspace = d.getElementById('EnabledForHyperspace');

            if (enabledForHyperspaceHiddenTextArea && enabledForHyperspace) {
                enabledForHyperspace.checked = enabledForHyperspaceHiddenTextArea.value == "Y" ? true : false;
                enabledForHyperspace.addEventListener('change', function(e) {
                    enabledForHyperspaceHiddenTextArea.value = enabledForHyperspace.checked ? "Y" : "N";
                });
            }


            var doNotPurgeHiddenTextArea = d.getElementsByClassName('DoNotPurgeHiddenTextArea')[0],
                doNotPurge = d.getElementById('DoNotPurge');

            if (doNotPurgeHiddenTextArea && doNotPurge) {
                doNotPurge.checked = doNotPurgeHiddenTextArea.value == "Y" ? true : false;
                doNotPurge.addEventListener('change', function(e) {
                    doNotPurgeHiddenTextArea.value = doNotPurge.checked ? "Y" : "N";
                });
            }

            document.getElementById('report-edit-details-form').addEventListener('submit', function(e) {
                e.preventDefault();
                var form = e.target,
                    u,
                    q = new XMLHttpRequest();
                q.open('post', form.getAttribute('action') + '&' + serialize(form), true);
                q.setRequestHeader('Content-Type', 'text/plain;charset=UTF-8`');
                q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
                q.send();
                q.onload = function() {
                    d.getElementById('report-details-container').innerHTML = q.responseText;
                    saveMessage();
                };

            });


            /* me tickets */

            document.addEventListener('submit', function(e) {
                if (e.target.closest('#report-edit-new-meticket') || e.target.closest('#report-editor-remove-meticket-form')) {

                    e.preventDefault();
                    var form = e.target.closest('#report-edit-new-meticket') || e.target.closest('#report-editor-remove-meticket-form'),
                        u,
                        q = new XMLHttpRequest();
                    q.open('post', form.getAttribute('action') + '&' + serialize(form), true);
                    q.setRequestHeader('Content-Type', 'text/plain;charset=UTF-8`');
                    q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
                    q.send();
                    q.onload = function() {
                        d.getElementById('report-meticket-container').innerHTML = q.responseText;
                        saveMessage();
                        u = new XMLHttpRequest();
                        u.open('get', 'Reports?handler=GetMeTicket&Id=' + Id, true);
                        u.setRequestHeader('Content-Type', 'text/plain;charset=UTF-8`');
                        u.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
                        u.send();
                        u.onload = function() {
                            document.getElementById('report-editor-metickets-container').innerHTML = u.responseText;
                        };
                        form.reset();
                    };
                }
            });

            /* maint logs */

            document.getElementById('report-edit-new-maintenance').addEventListener('submit', function(e) {
                e.preventDefault();
                var form = e.target,
                    q = new XMLHttpRequest();
                q.open('post', form.getAttribute('action') + '&' + serialize(form), true);
                q.setRequestHeader('Content-Type', 'text/plain;charset=UTF-8`');
                q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
                q.send();
                q.onload = function() {
                    d.getElementById('report-maintenance-container').innerHTML = q.responseText;
                    saveMessage();

                    if (document.querySelector('.navLinks-link[href="#maintenance"]') == null) {
                        var newHtml = '<a href="#maintenance" class="navLinks-link">Maintenance</a>';
                        document.querySelector('.navLinks-link[href="#comments"]').insertAdjacentHTML('beforebegin', newHtml);
                    }
                };
                form.reset();
            });
        }
    };


    document.addEventListener('live-editor', function() { load(); }, false);
    load();

    var x = 0;

    function load() {
        // wait for codemirror to load
        if (x == 50) {
            return false;
        } else if (typeof CodeMirror == 'undefined') {
            setTimeout(function() {
                x++;
                load();
            }, 100);
        } else {
            x = 0;
            l();
        }
    }

    /* for data project */
    /*
        document.addEventListener('change', function(e) {
            if (e.target.matches('#dp-add-attachment-form')) {

                var form = e.target.closest('form'),
                    u, id = form.querySelector('[name="Id"]').value;

                form.submit();
            }
        });
    */

})();