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
    var d = document,
        w = window,
        grp = d.getElementsByClassName('sr-grp')[0],
        m = d.getElementsByClassName('body-mainCtn')[0],
        hst = d.getElementsByClassName('sr-hst')[0],
        i = grp.querySelector('.sr-grp input'),
        cls = d.getElementById('sr-cls'),
        scls = d.getElementById('open-search'),
        sAjx = null,
        hAjx = null,
        atmr,
        loader = '<img src="/img/loader.gif" style="margin-left:calc(50% - 75px / 2);width:75px; padding-top:50px" class="search"/>',
        a = document.createElement('a');
    var oldHref = window.location.pathname.toLowerCase().startsWith('/search') ? '/' : window.location.href;
    oldPath = window.location.pathname;
    lastTitle = document.title;

    function close(url) {
        a.href = url || oldHref;

        if (a.pathname == '/') {
            cls.classList.add('clps-o');
        } else {
            cls.classList.remove('clps-o');
        }

        i.value = '';
        d.title = lastTitle;
        history.pushState({
            state: 'ajax'
        }, lastTitle, oldHref);
        currentPathname = oldPath;
    }

    function AjaxSearch(value, url, string) {
        if (!window.location.pathname.toLowerCase().startsWith('/search')) {
            oldHref = window.location.href;
        }

        var s = '/Search?' + (url || 's=' + value),
            u = s.replace('/Search?s=', '');
        start = new Date();

        if (typeof value !== 'undefined' && value !== null && value.length > 0 || typeof url !== 'undefined') {
            document.documentElement.scrollTop = document.body.scrollTop = 0;
            hst.style.display = "none"; // stop any analytics posts if the user continues to type

            if (typeof atmr !== "undefined") clearTimeout(atmr);

            a.href = url || oldHref;

            document.dispatchEvent(new CustomEvent('progress-start', {
                cancelable: true,
                detail: {
                    value: 90
                }
            }));

            history.pushState({
                state: 'ajax',
                search: string
            }, document.title, window.location.origin + '/Search?s=' + encodeURI(u));

            if (cache.exists(s)) {
                l(cache.get(s), hst, a, m, d, atmr, s, u, value);
            } else {
                if (sAjx !== null) {
                    sAjx.abort();
                }

                sAjx = new XMLHttpRequest();
                sAjx.open('get', s, true);
                sAjx.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded; charset=UTF-8');
                sAjx.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
                sAjx.send();

                sAjx.onload = function() {
                    l(sAjx.responseText, hst, a, m, d, atmr, s, u, string);
                    var ccHeader = sAjx.getResponseHeader('Cache-Control') != null ? (sAjx.getResponseHeader('Cache-Control').match(/\d+/) || [null])[0] : null;

                    if (ccHeader) {
                        cache.set(s, sAjx.responseText, ccHeader);
                    }
                };
            }
            window.oldPopState = window.newPopStage;
        } else {
            d.dispatchEvent(new CustomEvent('load-ajax', {
                detail: oldHref
            }));
        }
    }

    var l = function l(t, hst, a, m, d, atmr, s, u, value) {
        hst.style.display = "none";
        document.dispatchEvent(new CustomEvent('progress-finish'));
        m.style.visibility = 'visible';
        m.innerHTML = t;
        sc = Array.prototype.slice.call(m.querySelectorAll('script:not([type="application/json"])'));

        for (var x = 0; x < sc.length; x++) {
            var q = document.createElement("script");
            q.innerHTML = sc[x].innerHTML;
            q.type = "text/javascript";
            q.setAttribute('async', 'true');
            m.appendChild(q);
            sc[x].parentElement.removeChild(sc[x]);
        } // get page title
        d.title = 'Search: ' + value + ' - Atlas of Information Management';
        history.replaceState({
            state: 'ajax',
            search: value
        }, document.title, window.location.origin + '/Search?s=' + encodeURI(u));
        currentPathname = document.location.pathname; // update icon

        atmr = setTimeout(function() {
            document.dispatchEvent(new CustomEvent('analytics-post', {
                cancelable: true,
                detail: {
                    value: new Date().getTime() - start.getTime(),
                    type: 'newpage'
                }
            }));
        }, 3000);
        document.dispatchEvent(new CustomEvent("ajax"));
        document.dispatchEvent(new CustomEvent('ss-load'));
        document.dispatchEvent(new CustomEvent("code-highlight"));
    };

    grp.addEventListener('click', function(e) {
        e.stopPropagation();
    });

    hst.addEventListener('click', function(e) {
        if (e.target.matches('.sr-hst ul li a')) {
            e.preventDefault();
            e.stopPropagation();
            var q = e.target,
                str = q.getElementsByClassName('searchString')[0].textContent.trim();
            i.value = str;
            AjaxSearch(str, q.getAttribute('search'), str);
        }
    });
    d.addEventListener('scroll', function() {
        i.blur();
        hst.style.display = 'none';
    }, { passive: true });
    i.addEventListener('focus', function(e) {
        grp.classList.add('sr-grp-f-win');
    });
    i.addEventListener('blur', function(e) {
        grp.classList.remove('sr-grp-f-win');
    });
    i.addEventListener('click', function(e) {
        hst.style.display = 'none';

        if (hAjx !== null) {
            hAjx.abort();
        }

        hAjx = new XMLHttpRequest();
        hAjx.open('get', '/Users?Handler=SearchHistory', true);
        hAjx.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded; charset=UTF-8');
        hAjx.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
        hAjx.send();

        hAjx.onreadystatechange = function(e) {
            if (this.readyState == 4 && this.status == 200) {
                hst.innerHTML = this.responseText;
                hst.style.removeProperty('display');
            }
        };
    });
    i.addEventListener('input', function(e) {
        if (i.value.trim() !== '') AjaxSearch(i.value, null, i.value);
    });
    d.addEventListener('click', function(e) {
        hst.style.display = 'none';
        var g, c, f, i;
        if (e.target.matches('.checkbox-container.search-filter input')) {
            e.preventDefault();
            f = e.target.closest('form');
            i = f.querySelector('input[name="f"]');
            i.value = e.target.getAttribute('value');
            submit(f);
            return !1;
        } else if (e.target.matches('.page-link')) {
            e.preventDefault();
            var h = e.target.closest('form'),
                j = document.createElement('input');
            j.type = 'hidden';
            j.value = e.target.getAttribute('value');
            j.name = e.target.getAttribute('name');
            h.appendChild(j);
            submit(h);
            return !1;
        } else if (e.target.matches('.checkbox-container.search-advanced-filter input')) {
            e.preventDefault(); // if "on" set to 1, if "off" set to 0

            g = e.target;
            c = g.getAttribute("checked");

            if (c) {
                g.removeAttribute('checked');
                g.value = 0;
            } else {
                g.setAttribute('checked', '');
                g.value = 1;
            }

            submit(g.closest('form'));
            return !1;
        } else if (e.target.matches('.checkbox-container.search-field-filter input')) {
            e.preventDefault();
            g = e.target;
            c = g.hasAttribute("checked");

            if (c) {
                g.removeAttribute('checked');
                g.value = 0;
            } else {
                g.setAttribute('checked', '');
                g.value = 1;
            }

            var tg = document.querySelectorAll('.checkbox-container.search-field-filter input[checked]');

            var st = "";
            for (var x = 0; x < tg.length; x++) {
                st += tg[x].getAttribute('mvalue') + ",";
            }
            st = st.slice(0, -1);
            f = e.target.closest('form');
            i = f.querySelector('input[name="sf"]');
            i.value = st;
            submit(f);
            return !1;
            // get all checked, concat and submit form.
        }
    });

    function submit(l) {
        // change all checkboxes to 1
        var c = l.querySelectorAll('input[type="checkbox"][checked');
        [].forEach.call(c, function(i) {
            i.value = 1;
        });
        var p = serialize(l);
        AjaxSearch(null, p, document.querySelector('.sr-grp input').value);
    }

    scls.addEventListener('click', function(e) {
        if (cls.classList.contains('clps-o')) {
            close();
        } else {
            i.focus();
            var val = i.value; //store the value of the element

            i.value = ''; //clear the value of the element

            i.value = val;
        }
    });
    return {
        close: close,
        ajax: AjaxSearch
    };
})();
