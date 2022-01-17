function _instanceof(left, right) {
  if (
    right != null &&
    typeof Symbol !== 'undefined' &&
    right[Symbol.hasInstance]
  ) {
    return !!right[Symbol.hasInstance](left);
  } else {
    return left instanceof right;
  }
}

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

/*! (c) Andrea Giammarchi - ISC */
var self = this || {};
try {
  self.EventTarget = new EventTarget().constructor;
} catch (e) {
  !(function (e, o) {
    var t = e.create,
      r = e.defineProperty,
      n = i.prototype;

    function i() {
      'use strict';
      o.set(this, t(null));
    }

    function s(e, t, n) {
      r(e, t, {
        configurable: !0,
        writable: !0,
        value: n,
      });
    }

    function a(e) {
      var t = e.options;
      t && t.once && e.target.removeEventListener(this.type, e.listener),
        'function' == typeof e.listener
          ? e.listener.call(e.target, this)
          : e.listener.handleEvent(this);
    }
    s(n, 'addEventListener', function (e, t, n) {
      for (
        var r = o.get(this), i = r[e] || (r[e] = []), s = 0, a = i.length;
        s < a;
        s++
      )
        if (i[s].listener === t) return;
      i.push({
        target: this,
        listener: t,
        options: n,
      });
    }),
      s(n, 'dispatchEvent', function (e) {
        var t = o.get(this)[e.type];
        return (
          t &&
            (s(e, 'target', this),
            s(e, 'currentTarget', this),
            t.slice(0).forEach(a, e),
            delete e.currentTarget,
            delete e.target),
          !0
        );
      }),
      s(n, 'removeEventListener', function (e, t) {
        for (
          var n = o.get(this), r = n[e] || (n[e] = []), i = 0, s = r.length;
          i < s;
          i++
        )
          if (r[i].listener === t) return void r.splice(i, 1);
      }),
      (self.EventTarget = i);
  })(Object, new WeakMap());
}

(function () {
  var supportsPassive = false;
  document.createElement('div').addEventListener('test', function () {}, {
    get passive() {
      supportsPassive = true;
      return false;
    },
  });

  if (!supportsPassive) {
    var super_add_event_listener = EventTarget.prototype.addEventListener;
    var super_remove_event_listener = EventTarget.prototype.removeEventListener;
    var super_prevent_default = Event.prototype.preventDefault;

    function parseOptions(type, listener, options, action) {
      var needsWrapping = false;
      var useCapture = false;
      var passive = false;
      var fieldId;
      if (options) {
        if (typeof options === 'object') {
          passive = options.passive ? true : false;
          useCapture = options.useCapture ? true : false;
        } else {
          useCapture = options;
        }
      }
      if (passive) needsWrapping = true;
      if (needsWrapping) {
        fieldId = useCapture.toString();
        fieldId += passive.toString();
      }
      action(needsWrapping, fieldId, useCapture, passive);
    }

    Event.prototype.preventDefault = function () {
      if (this.__passive) {
        console.warn(
          'Ignored attempt to preventDefault an event from a passive listener',
        );
        return;
      }
      super_prevent_default.apply(this);
    };

    EventTarget.prototype.addEventListener = function (
      type,
      listener,
      options,
    ) {
      var super_this = this;
      parseOptions(
        type,
        listener,
        options,
        function (needsWrapping, fieldId, useCapture, passive) {
          if (needsWrapping) {
            var fieldId = useCapture.toString();
            fieldId += passive.toString();

            if (!this.__event_listeners_options)
              this.__event_listeners_options = {};
            if (!this.__event_listeners_options[type])
              this.__event_listeners_options[type] = {};
            if (!this.__event_listeners_options[type][listener])
              this.__event_listeners_options[type][listener] = [];
            if (this.__event_listeners_options[type][listener][fieldId]) return;
            var wrapped = {
              handleEvent: function (e) {
                e.__passive = passive;
                if (typeof listener === 'function') {
                  listener(e);
                } else {
                  listener.handleEvent(e);
                }
                e.__passive = false;
              },
            };
            this.__event_listeners_options[type][listener][fieldId] = wrapped;
            super_add_event_listener.call(
              super_this,
              type,
              wrapped,
              useCapture,
            );
          } else {
            super_add_event_listener.call(
              super_this,
              type,
              listener,
              useCapture,
            );
          }
        },
      );
    };

    EventTarget.prototype.removeEventListener = function (
      type,
      listener,
      options,
    ) {
      var super_this = this;
      parseOptions(
        type,
        listener,
        options,
        function (needsWrapping, fieldId, useCapture, passive) {
          if (
            needsWrapping &&
            this.__event_listeners_options &&
            this.__event_listeners_options[type] &&
            this.__event_listeners_options[type][listener] &&
            this.__event_listeners_options[type][listener][fieldId]
          ) {
            super_remove_event_listener.call(
              super_this,
              type,
              this.__event_listeners_options[type][listener][fieldId],
              false,
            );
            delete this.__event_listeners_options[type][listener][fieldId];
            if (this.__event_listeners_options[type][listener].length == 0)
              delete this.__event_listeners_options[type][listener];
          } else {
            super_remove_event_listener.call(
              super_this,
              type,
              listener,
              useCapture,
            );
          }
        },
      );
    };
  }
})();

function insertAfter(newNode, referenceNode) {
  referenceNode.parentNode.insertBefore(newNode, referenceNode.nextSibling);
}

/*!
 * Stickyfill – `position: sticky` polyfill
 * v. 2.1.0 | https://github.com/wilddeer/stickyfill
 * MIT License
 */
!(function (a, b) {
  'use strict';
  function c(a, b) {
    if (!(a instanceof b))
      throw new TypeError('Cannot call a class as a function');
  }
  function d(a, b) {
    for (var c in b) b.hasOwnProperty(c) && (a[c] = b[c]);
  }
  function e(a) {
    return parseFloat(a) || 0;
  }
  function f(a) {
    for (var b = 0; a; ) (b += a.offsetTop), (a = a.offsetParent);
    return b;
  }
  function g() {
    function c() {
      a.pageXOffset != m.left
        ? ((m.top = a.pageYOffset), (m.left = a.pageXOffset), p.refreshAll())
        : a.pageYOffset != m.top &&
          ((m.top = a.pageYOffset),
          (m.left = a.pageXOffset),
          n.forEach(function (a) {
            return a._recalcPosition();
          }));
    }
    function d() {
      f = setInterval(function () {
        n.forEach(function (a) {
          return a._fastCheck();
        });
      }, 500);
    }
    function e() {
      clearInterval(f);
    }
    if (!k) {
      (k = !0),
        c(),
        a.addEventListener('scroll', c),
        a.addEventListener('resize', p.refreshAll),
        a.addEventListener('orientationchange', p.refreshAll);
      var f = void 0,
        g = void 0,
        h = void 0;
      'hidden' in b
        ? ((g = 'hidden'), (h = 'visibilitychange'))
        : 'webkitHidden' in b &&
          ((g = 'webkitHidden'), (h = 'webkitvisibilitychange')),
        h
          ? (b[g] || d(),
            b.addEventListener(h, function () {
              b[g] ? e() : d();
            }))
          : d();
    }
  }
  var h = (function () {
      function a(a, b) {
        for (var c = 0; c < b.length; c++) {
          var d = b[c];
          (d.enumerable = d.enumerable || !1),
            (d.configurable = !0),
            'value' in d && (d.writable = !0),
            Object.defineProperty(a, d.key, d);
        }
      }
      return function (b, c, d) {
        return c && a(b.prototype, c), d && a(b, d), b;
      };
    })(),
    i = !1,
    j = 'undefined' != typeof a;
  j && a.getComputedStyle
    ? !(function () {
        var a = b.createElement('div');
        ['', '-webkit-', '-moz-', '-ms-'].some(function (b) {
          try {
            a.style.position = b + 'sticky';
          } catch (a) {}
          return '' != a.style.position;
        }) && (i = !0);
      })()
    : (i = !0);
  var k = !1,
    l = 'undefined' != typeof ShadowRoot,
    m = { top: null, left: null },
    n = [],
    o = (function () {
      function g(a) {
        if ((c(this, g), !(a instanceof HTMLElement)))
          throw new Error('First argument must be HTMLElement');
        if (
          n.some(function (b) {
            return b._node === a;
          })
        )
          throw new Error('Stickyfill is already applied to this node');
        (this._node = a),
          (this._stickyMode = null),
          (this._active = !1),
          n.push(this),
          this.refresh();
      }
      return (
        h(g, [
          {
            key: 'refresh',
            value: function () {
              if (!i && !this._removed) {
                this._active && this._deactivate();
                var c = this._node,
                  g = getComputedStyle(c),
                  h = {
                    position: g.position,
                    top: g.top,
                    display: g.display,
                    marginTop: g.marginTop,
                    marginBottom: g.marginBottom,
                    marginLeft: g.marginLeft,
                    marginRight: g.marginRight,
                    cssFloat: g.cssFloat,
                  };
                if (
                  !isNaN(parseFloat(h.top)) &&
                  'table-cell' != h.display &&
                  'none' != h.display
                ) {
                  this._active = !0;
                  var j = c.style.position;
                  ('sticky' != g.position && '-webkit-sticky' != g.position) ||
                    (c.style.position = 'static');
                  var k = c.parentNode,
                    m = l && k instanceof ShadowRoot ? k.host : k,
                    n = c.getBoundingClientRect(),
                    o = m.getBoundingClientRect(),
                    p = getComputedStyle(m);
                  (this._parent = {
                    node: m,
                    styles: { position: m.style.position },
                    offsetHeight: m.offsetHeight,
                  }),
                    (this._offsetToWindow = {
                      left: n.left,
                      right: b.documentElement.clientWidth - n.right,
                    }),
                    (this._offsetToParent = {
                      top: n.top - o.top - e(p.borderTopWidth),
                      left: n.left - o.left - e(p.borderLeftWidth),
                      right: -n.right + o.right - e(p.borderRightWidth),
                    }),
                    (this._styles = {
                      position: j,
                      top: c.style.top,
                      bottom: c.style.bottom,
                      left: c.style.left,
                      right: c.style.right,
                      width: c.style.width,
                      marginTop: c.style.marginTop,
                      marginLeft: c.style.marginLeft,
                      marginRight: c.style.marginRight,
                    });
                  var q = e(h.top);
                  this._limits = {
                    start: n.top + a.pageYOffset - q,
                    end:
                      o.top +
                      a.pageYOffset +
                      m.offsetHeight -
                      e(p.borderBottomWidth) -
                      c.offsetHeight -
                      q -
                      e(h.marginBottom),
                  };
                  var r = p.position;
                  'absolute' != r &&
                    'relative' != r &&
                    (m.style.position = 'relative'),
                    this._recalcPosition();
                  var s = (this._clone = {});
                  (s.node = b.createElement('div')),
                    d(s.node.style, {
                      width: n.right - n.left + 'px',
                      height: n.bottom - n.top + 'px',
                      marginTop: h.marginTop,
                      marginBottom: h.marginBottom,
                      marginLeft: h.marginLeft,
                      marginRight: h.marginRight,
                      cssFloat: h.cssFloat,
                      padding: 0,
                      border: 0,
                      borderSpacing: 0,
                      fontSize: '1em',
                      position: 'static',
                    }),
                    k.insertBefore(s.node, c),
                    (s.docOffsetTop = f(s.node));
                }
              }
            },
          },
          {
            key: '_recalcPosition',
            value: function () {
              if (this._active && !this._removed) {
                var a =
                  m.top <= this._limits.start
                    ? 'start'
                    : m.top >= this._limits.end
                    ? 'end'
                    : 'middle';
                if (this._stickyMode != a) {
                  switch (a) {
                    case 'start':
                      d(this._node.style, {
                        position: 'absolute',
                        left: this._offsetToParent.left + 'px',
                        right: this._offsetToParent.right + 'px',
                        top: this._offsetToParent.top + 'px',
                        bottom: 'auto',
                        width: 'auto',
                        marginLeft: 0,
                        marginRight: 0,
                        marginTop: 0,
                      });
                      break;
                    case 'middle':
                      d(this._node.style, {
                        position: 'fixed',
                        left: this._offsetToWindow.left + 'px',
                        right: this._offsetToWindow.right + 'px',
                        top: this._styles.top,
                        bottom: 'auto',
                        width: 'auto',
                        marginLeft: 0,
                        marginRight: 0,
                        marginTop: 0,
                      });
                      break;
                    case 'end':
                      d(this._node.style, {
                        position: 'absolute',
                        left: this._offsetToParent.left + 'px',
                        right: this._offsetToParent.right + 'px',
                        top: 'auto',
                        bottom: 0,
                        width: 'auto',
                        marginLeft: 0,
                        marginRight: 0,
                      });
                  }
                  this._stickyMode = a;
                }
              }
            },
          },
          {
            key: '_fastCheck',
            value: function () {
              this._active &&
                !this._removed &&
                (Math.abs(f(this._clone.node) - this._clone.docOffsetTop) > 1 ||
                  Math.abs(
                    this._parent.node.offsetHeight - this._parent.offsetHeight,
                  ) > 1) &&
                this.refresh();
            },
          },
          {
            key: '_deactivate',
            value: function () {
              var a = this;
              this._active &&
                !this._removed &&
                (this._clone.node.parentNode.removeChild(this._clone.node),
                delete this._clone,
                d(this._node.style, this._styles),
                delete this._styles,
                n.some(function (b) {
                  return (
                    b !== a && b._parent && b._parent.node === a._parent.node
                  );
                }) || d(this._parent.node.style, this._parent.styles),
                delete this._parent,
                (this._stickyMode = null),
                (this._active = !1),
                delete this._offsetToWindow,
                delete this._offsetToParent,
                delete this._limits);
            },
          },
          {
            key: 'remove',
            value: function () {
              var a = this;
              this._deactivate(),
                n.some(function (b, c) {
                  if (b._node === a._node) return n.splice(c, 1), !0;
                }),
                (this._removed = !0);
            },
          },
        ]),
        g
      );
    })(),
    p = {
      stickies: n,
      Sticky: o,
      forceSticky: function () {
        (i = !1), g(), this.refreshAll();
      },
      addOne: function (a) {
        if (!(a instanceof HTMLElement)) {
          if (!a.length || !a[0]) return;
          a = a[0];
        }
        for (var b = 0; b < n.length; b++) if (n[b]._node === a) return n[b];
        return new o(a);
      },
      add: function (a) {
        if ((a instanceof HTMLElement && (a = [a]), a.length)) {
          for (
            var b = [],
              c = function (c) {
                var d = a[c];
                return d instanceof HTMLElement
                  ? n.some(function (a) {
                      if (a._node === d) return b.push(a), !0;
                    })
                    ? 'continue'
                    : void b.push(new o(d))
                  : (b.push(void 0), 'continue');
              },
              d = 0;
            d < a.length;
            d++
          ) {
            c(d);
          }
          return b;
        }
      },
      refreshAll: function () {
        n.forEach(function (a) {
          return a.refresh();
        });
      },
      removeOne: function (a) {
        if (!(a instanceof HTMLElement)) {
          if (!a.length || !a[0]) return;
          a = a[0];
        }
        n.some(function (b) {
          if (b._node === a) return b.remove(), !0;
        });
      },
      remove: function (a) {
        if ((a instanceof HTMLElement && (a = [a]), a.length))
          for (
            var b = function (b) {
                var c = a[b];
                n.some(function (a) {
                  if (a._node === c) return a.remove(), !0;
                });
              },
              c = 0;
            c < a.length;
            c++
          )
            b(c);
      },
      removeAll: function () {
        for (; n.length; ) n[0].remove();
      },
    };
  i || g(),
    'undefined' != typeof module && module.exports
      ? (module.exports = p)
      : j && (a.Stickyfill = p);
})(window, document);
var elements = document.querySelectorAll('.sticky');
Stickyfill.add(elements);
