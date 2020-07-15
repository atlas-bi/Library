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
    function closeModals(el) {
        if (typeof el == 'undefined') {
            // hide all other modals
            [].forEach.call(document.getElementsByClassName('mdl-o'), function(i) {
                i.classList.remove('mdl-o');
            });
        } else {
            el.classList.remove('mdl-o');
        }

        if (!(document.getElementsByClassName('mdl-o').length > 0)) {
            document.getElementsByTagName('body')[0].classList.remove('b-mdl');
        }
    }

    function showModal(el, e) {
        var d = document;
        d.getElementsByTagName('body')[0].classList.toggle('b-mdl');
        el.classList.toggle('mdl-o');
        el.dispatchEvent(new CustomEvent('mdl-open', {
            cancelable: true,
            detail: {
                e: e
            }
        }));
    }

    document.addEventListener('click', function(e) {
        var el,
            q,
            data,
            url,
            d = document; // open modal

        if (e.target.getAttribute('data-toggle') == 'mdl') {
            el = d.getElementById(e.target.getAttribute('data-target'));
            showModal(el, e.target);
        } // close modal
        else if (e.target.getAttribute('data-dismiss') == 'mdl' || e.target.matches('.mdl')) {
            // close closest modal
            closeModals(e.target.closest('.mdl'));
        } // request access
        else if (e.target.matches('.request-access')) {
            data = {
                reportName: e.target.getAttribute('report-name'),
                directorName: d.getElementById('director-name').value,
                reportUrl: window.location.href
            };
            url = Object.keys(data).map(function(k) {
                return encodeURIComponent(k) + '=' + encodeURIComponent(data[k]);
            }).join('&');
            q = new XMLHttpRequest();
            q.open('post', '/Requests?handler=AccessRequest&' + url, true);
            q.setRequestHeader('Content-Type', 'text/html;charset=UTF-8`');
            q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
            q.send();
            ShowMessageBox("Your request has been submitted.");
            closeModals();
        } // report a problem
        else if (e.target.matches('.request-acess')) {
            data = {
                reportName: e.target.getAttribute('report-name'),
                description: e.target.parentNode.previousElementSibling.getElementsByTagName('textarea')[0].value,
                reportUrl: window.location.href
            };
            q = new XMLHttpRequest();
            q.open('post', '/Requests?handler=ReportProblem&' + url, true);
            q.setRequestHeader('Content-Type', 'text/html;charset=UTF-8`');
            q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
            q.send();
            ShowMessageBox("Your request has been submitted.");
            closeModals();
        } else if (e.target.closest('.pop')) {
            d.getElementsByClassName('imagepreview')[0].setAttribute('src', e.target.closest('.pop').children[0].getAttribute('src'));
            showModal(d.getElementById('image-modal'), null);
        }
    }, true);
    document.getElementById('shareModal').addEventListener('mdl-open', function(e) {
        var button = e.detail.e,
            name = button.getAttribute('data-name'),
            url = window.location.origin;

        if (!button.hasAttribute('data-url') || button.getAttribute('data-url') == null || button.getAttribute('data-url') == '') {
            url = window.location.href;
        } else {
            url += button.getAttribute('data-url');
        }

        var modal = this;
        modal.querySelector('.mlbx-newMsgSubjIpt').innerHTML = "Share: " + name;
        modal.querySelector('.mlbx-newMsgMsg').innerHTML = "<div>Hi!<br>I would like to share this report with you.</div><br><a href='" + url + "' class='atlas-blue ajax'>" + name + "</a><br><br><div>Check it out sometime!</div><br><div>Regards!";

        // remove an "to" recips.
        var toName = modal.querySelectorAll('.dd-itm');
        for (var x = 0; x < toName.length; x++) {
            toName[x].parentElement.removeChild(toName[x]);
        }
        var toId = modal.querySelectorAll('.dd-hdn option');
        for (var x = 0; x < toId.length; x++) {
            toId[x].parentElement.removeChild(toId[x]);
        }
        document.dispatchEvent(new CustomEvent('dropdown', {
            cancelable: true,
            detail: {
                el: modal.querySelector('#new_msg_dynamic-dropdown:not(.dd-hdn)')
            }
        }));

    });
    document.getElementById('reportProblem').addEventListener('mdl-open', function(e) {
        var button = e.detail.e; // Button that triggered the modal

        var name = button.getAttribute('data-name'); // Extract info from data-* attributes

        var url = window.location.origin;

        if (!button.hasAttribute('data-url') || button.getAttribute('data-url') == null || button.getAttribute('data-url') == '') {
            url = window.location.href;
        } else {
            url += button.getAttribute('data-url');
        }

        var modal = this;
        modal.querySelector('.report-problem').setAttribute('report-name', name);

    });
    document.addEventListener('modal-close', function() {
        closeModals();
    });
})();