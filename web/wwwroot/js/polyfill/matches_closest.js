Element.prototype.matches ||
  (Element.prototype.matches =
    Element.prototype.msMatchesSelector ||
    Element.prototype.webkitMatchesSelector),
  Element.prototype.closest ||
    (Element.prototype.closest = function (t) {
      var e = this;

      do {
        if (e.matches(t)) return e;
        e = e.parentElement || e.parentNode;
      } while (null !== e && 1 === e.nodeType);

      return null;
    }),
  (function () {
    function t(t, e) {
      e = e || {
        bubbles: !1,
        cancelable: !1,
        detail: void 0,
      };
      var n = document.createEvent('CustomEvent');
      return n.initCustomEvent(t, e.bubbles, e.cancelable, e.detail), n;
    }

    'function' != typeof window.CustomEvent &&
      ((t.prototype = window.Event.prototype), (window.CustomEvent = t));
  })(),
  String.prototype.startsWith ||
    (Object.defineProperty(String.prototype, 'startsWith', {
      value: function value(t, e) {
        var n = e > 0 ? 0 | e : 0;
        return this.substring(n, n + t.length) === t;
      },
    }),
    Array.from ||
      (Array.from = (function () {
        var t = Object.prototype.toString,
          e = function e(_e) {
            return (
              'function' == typeof _e || '[object Function]' === t.call(_e)
            );
          },
          n = Math.pow(2, 53) - 1,
          r = function r(t) {
            var e = (function (t) {
              var e = Number(t);
              return isNaN(e)
                ? 0
                : 0 !== e && isFinite(e)
                ? (e > 0 ? 1 : -1) * Math.floor(Math.abs(e))
                : e;
            })(t);

            return Math.min(Math.max(e, 0), n);
          };

        return function (t) {
          var n = Object(t);
          if (null == t)
            throw new TypeError(
              'Array.from requires an array-like object - not null or undefined',
            );
          var o,
            a = arguments.length > 1 ? arguments[1] : void 0;

          if (void 0 !== a) {
            if (!e(a))
              throw new TypeError(
                'Array.from: when provided, the second argument must be a function',
              );
            arguments.length > 2 && (o = arguments[2]);
          }

          for (
            var i,
              c = r(n.length),
              u = e(this) ? Object(new this(c)) : new Array(c),
              l = 0;
            l < c;

          ) {
            (i = n[l]),
              (u[l] = a ? (void 0 === o ? a(i, l) : a.call(o, i, l)) : i),
              (l += 1);
          }

          return (u.length = c), u;
        };
      })()),
    [Element.prototype, Document.prototype, DocumentFragment.prototype].forEach(
      function (t) {
        t.hasOwnProperty('append') ||
          Object.defineProperty(t, 'append', {
            configurable: !0,
            enumerable: !0,
            writable: !0,
            value: function value() {
              var t = Array.prototype.slice.call(arguments),
                e = document.createDocumentFragment();
              t.forEach(function (t) {
                var n = _instanceof(t, Node);

                e.appendChild(n ? t : document.createTextNode(String(t)));
              }),
                this.appendChild(e);
            },
          });
      },
    ));
