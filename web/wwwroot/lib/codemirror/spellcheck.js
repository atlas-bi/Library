/**
 * codemirror-spell-checker v1.2.1
 * Copyright Next Step Webs, Inc.
 * @link https://github.com/NextStepWebs/codemirror-spell-checker
 * @license MIT
 */
!(function (t) {
  if ('object' == typeof exports && 'undefined' != typeof module)
    module.exports = t();
  else if ('function' == typeof define && define.amd) define([], t);
  else {
    var e;
    (e =
      'undefined' != typeof window
        ? window
        : 'undefined' != typeof global
        ? global
        : 'undefined' != typeof self
        ? self
        : this),
      (e.CodeMirrorSpellChecker = t());
  }
})(function () {
  return (function t(e, r, n) {
    function i(s, a) {
      if (!r[s]) {
        if (!e[s]) {
          var f = 'function' == typeof require && require;
          if (!a && f) return f(s, !0);
          if (o) return o(s, !0);
          var u = new Error("Cannot find module '" + s + "'");
          throw ((u.code = 'MODULE_NOT_FOUND'), u);
        }
        var h = (r[s] = { exports: {} });
        e[s][0].call(
          h.exports,
          function (t) {
            var r = e[s][1][t];
            return i(r ? r : t);
          },
          h,
          h.exports,
          t,
          e,
          r,
          n,
        );
      }
      return r[s].exports;
    }
    for (
      var o = 'function' == typeof require && require, s = 0;
      s < n.length;
      s++
    )
      i(n[s]);
    return i;
  })(
    {
      1: [
        function (t, e, r) {
          'use strict';
          function n() {
            for (
              var t =
                  'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/',
                e = 0,
                r = t.length;
              r > e;
              ++e
            )
              (f[e] = t[e]), (u[t.charCodeAt(e)] = e);
            (u['-'.charCodeAt(0)] = 62), (u['_'.charCodeAt(0)] = 63);
          }
          function i(t) {
            var e,
              r,
              n,
              i,
              o,
              s,
              a = t.length;
            if (a % 4 > 0)
              throw new Error('Invalid string. Length must be a multiple of 4');
            (o = '=' === t[a - 2] ? 2 : '=' === t[a - 1] ? 1 : 0),
              (s = new h((3 * a) / 4 - o)),
              (n = o > 0 ? a - 4 : a);
            var f = 0;
            for (e = 0, r = 0; n > e; e += 4, r += 3)
              (i =
                (u[t.charCodeAt(e)] << 18) |
                (u[t.charCodeAt(e + 1)] << 12) |
                (u[t.charCodeAt(e + 2)] << 6) |
                u[t.charCodeAt(e + 3)]),
                (s[f++] = (i >> 16) & 255),
                (s[f++] = (i >> 8) & 255),
                (s[f++] = 255 & i);
            return (
              2 === o
                ? ((i =
                    (u[t.charCodeAt(e)] << 2) | (u[t.charCodeAt(e + 1)] >> 4)),
                  (s[f++] = 255 & i))
                : 1 === o &&
                  ((i =
                    (u[t.charCodeAt(e)] << 10) |
                    (u[t.charCodeAt(e + 1)] << 4) |
                    (u[t.charCodeAt(e + 2)] >> 2)),
                  (s[f++] = (i >> 8) & 255),
                  (s[f++] = 255 & i)),
              s
            );
          }
          function o(t) {
            return (
              f[(t >> 18) & 63] +
              f[(t >> 12) & 63] +
              f[(t >> 6) & 63] +
              f[63 & t]
            );
          }
          function s(t, e, r) {
            for (var n, i = [], s = e; r > s; s += 3)
              (n = (t[s] << 16) + (t[s + 1] << 8) + t[s + 2]), i.push(o(n));
            return i.join('');
          }
          function a(t) {
            for (
              var e,
                r = t.length,
                n = r % 3,
                i = '',
                o = [],
                a = 16383,
                u = 0,
                h = r - n;
              h > u;
              u += a
            )
              o.push(s(t, u, u + a > h ? h : u + a));
            return (
              1 === n
                ? ((e = t[r - 1]),
                  (i += f[e >> 2]),
                  (i += f[(e << 4) & 63]),
                  (i += '=='))
                : 2 === n &&
                  ((e = (t[r - 2] << 8) + t[r - 1]),
                  (i += f[e >> 10]),
                  (i += f[(e >> 4) & 63]),
                  (i += f[(e << 2) & 63]),
                  (i += '=')),
              o.push(i),
              o.join('')
            );
          }
          (r.toByteArray = i), (r.fromByteArray = a);
          var f = [],
            u = [],
            h = 'undefined' != typeof Uint8Array ? Uint8Array : Array;
          n();
        },
        {},
      ],
      2: [function (t, e, r) {}, {}],
      3: [
        function (t, e, r) {
          (function (e) {
            'use strict';
            function n() {
              try {
                var t = new Uint8Array(1);
                return (
                  (t.foo = function () {
                    return 42;
                  }),
                  42 === t.foo() &&
                    'function' == typeof t.subarray &&
                    0 === t.subarray(1, 1).byteLength
                );
              } catch (e) {
                return !1;
              }
            }
            function i() {
              return s.TYPED_ARRAY_SUPPORT ? 2147483647 : 1073741823;
            }
            function o(t, e) {
              if (i() < e) throw new RangeError('Invalid typed array length');
              return (
                s.TYPED_ARRAY_SUPPORT
                  ? ((t = new Uint8Array(e)), (t.__proto__ = s.prototype))
                  : (null === t && (t = new s(e)), (t.length = e)),
                t
              );
            }
            function s(t, e, r) {
              if (!(s.TYPED_ARRAY_SUPPORT || this instanceof s))
                return new s(t, e, r);
              if ('number' == typeof t) {
                if ('string' == typeof e)
                  throw new Error(
                    'If encoding is specified then the first argument must be a string',
                  );
                return h(this, t);
              }
              return a(this, t, e, r);
            }
            function a(t, e, r, n) {
              if ('number' == typeof e)
                throw new TypeError('"value" argument must not be a number');
              return 'undefined' != typeof ArrayBuffer &&
                e instanceof ArrayBuffer
                ? p(t, e, r, n)
                : 'string' == typeof e
                ? l(t, e, r)
                : d(t, e);
            }
            function f(t) {
              if ('number' != typeof t)
                throw new TypeError('"size" argument must be a number');
            }
            function u(t, e, r, n) {
              return (
                f(e),
                0 >= e
                  ? o(t, e)
                  : void 0 !== r
                  ? 'string' == typeof n
                    ? o(t, e).fill(r, n)
                    : o(t, e).fill(r)
                  : o(t, e)
              );
            }
            function h(t, e) {
              if (
                (f(e), (t = o(t, 0 > e ? 0 : 0 | g(e))), !s.TYPED_ARRAY_SUPPORT)
              )
                for (var r = 0; e > r; r++) t[r] = 0;
              return t;
            }
            function l(t, e, r) {
              if (
                (('string' == typeof r && '' !== r) || (r = 'utf8'),
                !s.isEncoding(r))
              )
                throw new TypeError(
                  '"encoding" must be a valid string encoding',
                );
              var n = 0 | v(e, r);
              return (t = o(t, n)), t.write(e, r), t;
            }
            function c(t, e) {
              var r = 0 | g(e.length);
              t = o(t, r);
              for (var n = 0; r > n; n += 1) t[n] = 255 & e[n];
              return t;
            }
            function p(t, e, r, n) {
              if ((e.byteLength, 0 > r || e.byteLength < r))
                throw new RangeError("'offset' is out of bounds");
              if (e.byteLength < r + (n || 0))
                throw new RangeError("'length' is out of bounds");
              return (
                (e =
                  void 0 === n
                    ? new Uint8Array(e, r)
                    : new Uint8Array(e, r, n)),
                s.TYPED_ARRAY_SUPPORT
                  ? ((t = e), (t.__proto__ = s.prototype))
                  : (t = c(t, e)),
                t
              );
            }
            function d(t, e) {
              if (s.isBuffer(e)) {
                var r = 0 | g(e.length);
                return (
                  (t = o(t, r)), 0 === t.length ? t : (e.copy(t, 0, 0, r), t)
                );
              }
              if (e) {
                if (
                  ('undefined' != typeof ArrayBuffer &&
                    e.buffer instanceof ArrayBuffer) ||
                  'length' in e
                )
                  return 'number' != typeof e.length || V(e.length)
                    ? o(t, 0)
                    : c(t, e);
                if ('Buffer' === e.type && Q(e.data)) return c(t, e.data);
              }
              throw new TypeError(
                'First argument must be a string, Buffer, ArrayBuffer, Array, or array-like object.',
              );
            }
            function g(t) {
              if (t >= i())
                throw new RangeError(
                  'Attempt to allocate Buffer larger than maximum size: 0x' +
                    i().toString(16) +
                    ' bytes',
                );
              return 0 | t;
            }
            function y(t) {
              return +t != t && (t = 0), s.alloc(+t);
            }
            function v(t, e) {
              if (s.isBuffer(t)) return t.length;
              if (
                'undefined' != typeof ArrayBuffer &&
                'function' == typeof ArrayBuffer.isView &&
                (ArrayBuffer.isView(t) || t instanceof ArrayBuffer)
              )
                return t.byteLength;
              'string' != typeof t && (t = '' + t);
              var r = t.length;
              if (0 === r) return 0;
              for (var n = !1; ; )
                switch (e) {
                  case 'ascii':
                  case 'binary':
                  case 'raw':
                  case 'raws':
                    return r;
                  case 'utf8':
                  case 'utf-8':
                  case void 0:
                    return z(t).length;
                  case 'ucs2':
                  case 'ucs-2':
                  case 'utf16le':
                  case 'utf-16le':
                    return 2 * r;
                  case 'hex':
                    return r >>> 1;
                  case 'base64':
                    return H(t).length;
                  default:
                    if (n) return z(t).length;
                    (e = ('' + e).toLowerCase()), (n = !0);
                }
            }
            function w(t, e, r) {
              var n = !1;
              if (((void 0 === e || 0 > e) && (e = 0), e > this.length))
                return '';
              if (
                ((void 0 === r || r > this.length) && (r = this.length), 0 >= r)
              )
                return '';
              if (((r >>>= 0), (e >>>= 0), e >= r)) return '';
              for (t || (t = 'utf8'); ; )
                switch (t) {
                  case 'hex':
                    return x(this, e, r);
                  case 'utf8':
                  case 'utf-8':
                    return C(this, e, r);
                  case 'ascii':
                    return B(this, e, r);
                  case 'binary':
                    return M(this, e, r);
                  case 'base64':
                    return S(this, e, r);
                  case 'ucs2':
                  case 'ucs-2':
                  case 'utf16le':
                  case 'utf-16le':
                    return I(this, e, r);
                  default:
                    if (n) throw new TypeError('Unknown encoding: ' + t);
                    (t = (t + '').toLowerCase()), (n = !0);
                }
            }
            function m(t, e, r) {
              var n = t[e];
              (t[e] = t[r]), (t[r] = n);
            }
            function E(t, e, r, n) {
              function i(t, e) {
                return 1 === o ? t[e] : t.readUInt16BE(e * o);
              }
              var o = 1,
                s = t.length,
                a = e.length;
              if (
                void 0 !== n &&
                ((n = String(n).toLowerCase()),
                'ucs2' === n ||
                  'ucs-2' === n ||
                  'utf16le' === n ||
                  'utf-16le' === n)
              ) {
                if (t.length < 2 || e.length < 2) return -1;
                (o = 2), (s /= 2), (a /= 2), (r /= 2);
              }
              for (var f = -1, u = 0; s > r + u; u++)
                if (i(t, r + u) === i(e, -1 === f ? 0 : u - f)) {
                  if ((-1 === f && (f = u), u - f + 1 === a))
                    return (r + f) * o;
                } else -1 !== f && (u -= u - f), (f = -1);
              return -1;
            }
            function b(t, e, r, n) {
              r = Number(r) || 0;
              var i = t.length - r;
              n ? ((n = Number(n)), n > i && (n = i)) : (n = i);
              var o = e.length;
              if (o % 2 !== 0) throw new Error('Invalid hex string');
              n > o / 2 && (n = o / 2);
              for (var s = 0; n > s; s++) {
                var a = parseInt(e.substr(2 * s, 2), 16);
                if (isNaN(a)) return s;
                t[r + s] = a;
              }
              return s;
            }
            function R(t, e, r, n) {
              return K(z(e, t.length - r), t, r, n);
            }
            function _(t, e, r, n) {
              return K(G(e), t, r, n);
            }
            function A(t, e, r, n) {
              return _(t, e, r, n);
            }
            function P(t, e, r, n) {
              return K(H(e), t, r, n);
            }
            function T(t, e, r, n) {
              return K($(e, t.length - r), t, r, n);
            }
            function S(t, e, r) {
              return 0 === e && r === t.length
                ? J.fromByteArray(t)
                : J.fromByteArray(t.slice(e, r));
            }
            function C(t, e, r) {
              r = Math.min(t.length, r);
              for (var n = [], i = e; r > i; ) {
                var o = t[i],
                  s = null,
                  a = o > 239 ? 4 : o > 223 ? 3 : o > 191 ? 2 : 1;
                if (r >= i + a) {
                  var f, u, h, l;
                  switch (a) {
                    case 1:
                      128 > o && (s = o);
                      break;
                    case 2:
                      (f = t[i + 1]),
                        128 === (192 & f) &&
                          ((l = ((31 & o) << 6) | (63 & f)),
                          l > 127 && (s = l));
                      break;
                    case 3:
                      (f = t[i + 1]),
                        (u = t[i + 2]),
                        128 === (192 & f) &&
                          128 === (192 & u) &&
                          ((l = ((15 & o) << 12) | ((63 & f) << 6) | (63 & u)),
                          l > 2047 && (55296 > l || l > 57343) && (s = l));
                      break;
                    case 4:
                      (f = t[i + 1]),
                        (u = t[i + 2]),
                        (h = t[i + 3]),
                        128 === (192 & f) &&
                          128 === (192 & u) &&
                          128 === (192 & h) &&
                          ((l =
                            ((15 & o) << 18) |
                            ((63 & f) << 12) |
                            ((63 & u) << 6) |
                            (63 & h)),
                          l > 65535 && 1114112 > l && (s = l));
                  }
                }
                null === s
                  ? ((s = 65533), (a = 1))
                  : s > 65535 &&
                    ((s -= 65536),
                    n.push(((s >>> 10) & 1023) | 55296),
                    (s = 56320 | (1023 & s))),
                  n.push(s),
                  (i += a);
              }
              return U(n);
            }
            function U(t) {
              var e = t.length;
              if (W >= e) return String.fromCharCode.apply(String, t);
              for (var r = '', n = 0; e > n; )
                r += String.fromCharCode.apply(String, t.slice(n, (n += W)));
              return r;
            }
            function B(t, e, r) {
              var n = '';
              r = Math.min(t.length, r);
              for (var i = e; r > i; i++) n += String.fromCharCode(127 & t[i]);
              return n;
            }
            function M(t, e, r) {
              var n = '';
              r = Math.min(t.length, r);
              for (var i = e; r > i; i++) n += String.fromCharCode(t[i]);
              return n;
            }
            function x(t, e, r) {
              var n = t.length;
              (!e || 0 > e) && (e = 0), (!r || 0 > r || r > n) && (r = n);
              for (var i = '', o = e; r > o; o++) i += q(t[o]);
              return i;
            }
            function I(t, e, r) {
              for (var n = t.slice(e, r), i = '', o = 0; o < n.length; o += 2)
                i += String.fromCharCode(n[o] + 256 * n[o + 1]);
              return i;
            }
            function O(t, e, r) {
              if (t % 1 !== 0 || 0 > t)
                throw new RangeError('offset is not uint');
              if (t + e > r)
                throw new RangeError('Trying to access beyond buffer length');
            }
            function Y(t, e, r, n, i, o) {
              if (!s.isBuffer(t))
                throw new TypeError(
                  '"buffer" argument must be a Buffer instance',
                );
              if (e > i || o > e)
                throw new RangeError('"value" argument is out of bounds');
              if (r + n > t.length) throw new RangeError('Index out of range');
            }
            function L(t, e, r, n) {
              0 > e && (e = 65535 + e + 1);
              for (var i = 0, o = Math.min(t.length - r, 2); o > i; i++)
                t[r + i] =
                  (e & (255 << (8 * (n ? i : 1 - i)))) >>>
                  (8 * (n ? i : 1 - i));
            }
            function D(t, e, r, n) {
              0 > e && (e = 4294967295 + e + 1);
              for (var i = 0, o = Math.min(t.length - r, 4); o > i; i++)
                t[r + i] = (e >>> (8 * (n ? i : 3 - i))) & 255;
            }
            function F(t, e, r, n, i, o) {
              if (r + n > t.length) throw new RangeError('Index out of range');
              if (0 > r) throw new RangeError('Index out of range');
            }
            function k(t, e, r, n, i) {
              return (
                i ||
                  F(t, e, r, 4, 3.4028234663852886e38, -3.4028234663852886e38),
                Z.write(t, e, r, n, 23, 4),
                r + 4
              );
            }
            function N(t, e, r, n, i) {
              return (
                i ||
                  F(
                    t,
                    e,
                    r,
                    8,
                    1.7976931348623157e308,
                    -1.7976931348623157e308,
                  ),
                Z.write(t, e, r, n, 52, 8),
                r + 8
              );
            }
            function j(t) {
              if (((t = X(t).replace(tt, '')), t.length < 2)) return '';
              for (; t.length % 4 !== 0; ) t += '=';
              return t;
            }
            function X(t) {
              return t.trim ? t.trim() : t.replace(/^\s+|\s+$/g, '');
            }
            function q(t) {
              return 16 > t ? '0' + t.toString(16) : t.toString(16);
            }
            function z(t, e) {
              e = e || 1 / 0;
              for (var r, n = t.length, i = null, o = [], s = 0; n > s; s++) {
                if (((r = t.charCodeAt(s)), r > 55295 && 57344 > r)) {
                  if (!i) {
                    if (r > 56319) {
                      (e -= 3) > -1 && o.push(239, 191, 189);
                      continue;
                    }
                    if (s + 1 === n) {
                      (e -= 3) > -1 && o.push(239, 191, 189);
                      continue;
                    }
                    i = r;
                    continue;
                  }
                  if (56320 > r) {
                    (e -= 3) > -1 && o.push(239, 191, 189), (i = r);
                    continue;
                  }
                  r = (((i - 55296) << 10) | (r - 56320)) + 65536;
                } else i && (e -= 3) > -1 && o.push(239, 191, 189);
                if (((i = null), 128 > r)) {
                  if ((e -= 1) < 0) break;
                  o.push(r);
                } else if (2048 > r) {
                  if ((e -= 2) < 0) break;
                  o.push((r >> 6) | 192, (63 & r) | 128);
                } else if (65536 > r) {
                  if ((e -= 3) < 0) break;
                  o.push(
                    (r >> 12) | 224,
                    ((r >> 6) & 63) | 128,
                    (63 & r) | 128,
                  );
                } else {
                  if (!(1114112 > r)) throw new Error('Invalid code point');
                  if ((e -= 4) < 0) break;
                  o.push(
                    (r >> 18) | 240,
                    ((r >> 12) & 63) | 128,
                    ((r >> 6) & 63) | 128,
                    (63 & r) | 128,
                  );
                }
              }
              return o;
            }
            function G(t) {
              for (var e = [], r = 0; r < t.length; r++)
                e.push(255 & t.charCodeAt(r));
              return e;
            }
            function $(t, e) {
              for (
                var r, n, i, o = [], s = 0;
                s < t.length && !((e -= 2) < 0);
                s++
              )
                (r = t.charCodeAt(s)),
                  (n = r >> 8),
                  (i = r % 256),
                  o.push(i),
                  o.push(n);
              return o;
            }
            function H(t) {
              return J.toByteArray(j(t));
            }
            function K(t, e, r, n) {
              for (
                var i = 0;
                n > i && !(i + r >= e.length || i >= t.length);
                i++
              )
                e[i + r] = t[i];
              return i;
            }
            function V(t) {
              return t !== t;
            }
            var J = t('base64-js'),
              Z = t('ieee754'),
              Q = t('isarray');
            (r.Buffer = s),
              (r.SlowBuffer = y),
              (r.INSPECT_MAX_BYTES = 50),
              (s.TYPED_ARRAY_SUPPORT =
                void 0 !== e.TYPED_ARRAY_SUPPORT ? e.TYPED_ARRAY_SUPPORT : n()),
              (r.kMaxLength = i()),
              (s.poolSize = 8192),
              (s._augment = function (t) {
                return (t.__proto__ = s.prototype), t;
              }),
              (s.from = function (t, e, r) {
                return a(null, t, e, r);
              }),
              s.TYPED_ARRAY_SUPPORT &&
                ((s.prototype.__proto__ = Uint8Array.prototype),
                (s.__proto__ = Uint8Array),
                'undefined' != typeof Symbol &&
                  Symbol.species &&
                  s[Symbol.species] === s &&
                  Object.defineProperty(s, Symbol.species, {
                    value: null,
                    configurable: !0,
                  })),
              (s.alloc = function (t, e, r) {
                return u(null, t, e, r);
              }),
              (s.allocUnsafe = function (t) {
                return h(null, t);
              }),
              (s.allocUnsafeSlow = function (t) {
                return h(null, t);
              }),
              (s.isBuffer = function (t) {
                return !(null == t || !t._isBuffer);
              }),
              (s.compare = function (t, e) {
                if (!s.isBuffer(t) || !s.isBuffer(e))
                  throw new TypeError('Arguments must be Buffers');
                if (t === e) return 0;
                for (
                  var r = t.length, n = e.length, i = 0, o = Math.min(r, n);
                  o > i;
                  ++i
                )
                  if (t[i] !== e[i]) {
                    (r = t[i]), (n = e[i]);
                    break;
                  }
                return n > r ? -1 : r > n ? 1 : 0;
              }),
              (s.isEncoding = function (t) {
                switch (String(t).toLowerCase()) {
                  case 'hex':
                  case 'utf8':
                  case 'utf-8':
                  case 'ascii':
                  case 'binary':
                  case 'base64':
                  case 'raw':
                  case 'ucs2':
                  case 'ucs-2':
                  case 'utf16le':
                  case 'utf-16le':
                    return !0;
                  default:
                    return !1;
                }
              }),
              (s.concat = function (t, e) {
                if (!Q(t))
                  throw new TypeError(
                    '"list" argument must be an Array of Buffers',
                  );
                if (0 === t.length) return s.alloc(0);
                var r;
                if (void 0 === e)
                  for (e = 0, r = 0; r < t.length; r++) e += t[r].length;
                var n = s.allocUnsafe(e),
                  i = 0;
                for (r = 0; r < t.length; r++) {
                  var o = t[r];
                  if (!s.isBuffer(o))
                    throw new TypeError(
                      '"list" argument must be an Array of Buffers',
                    );
                  o.copy(n, i), (i += o.length);
                }
                return n;
              }),
              (s.byteLength = v),
              (s.prototype._isBuffer = !0),
              (s.prototype.swap16 = function () {
                var t = this.length;
                if (t % 2 !== 0)
                  throw new RangeError(
                    'Buffer size must be a multiple of 16-bits',
                  );
                for (var e = 0; t > e; e += 2) m(this, e, e + 1);
                return this;
              }),
              (s.prototype.swap32 = function () {
                var t = this.length;
                if (t % 4 !== 0)
                  throw new RangeError(
                    'Buffer size must be a multiple of 32-bits',
                  );
                for (var e = 0; t > e; e += 4)
                  m(this, e, e + 3), m(this, e + 1, e + 2);
                return this;
              }),
              (s.prototype.toString = function () {
                var t = 0 | this.length;
                return 0 === t
                  ? ''
                  : 0 === arguments.length
                  ? C(this, 0, t)
                  : w.apply(this, arguments);
              }),
              (s.prototype.equals = function (t) {
                if (!s.isBuffer(t))
                  throw new TypeError('Argument must be a Buffer');
                return this === t ? !0 : 0 === s.compare(this, t);
              }),
              (s.prototype.inspect = function () {
                var t = '',
                  e = r.INSPECT_MAX_BYTES;
                return (
                  this.length > 0 &&
                    ((t = this.toString('hex', 0, e).match(/.{2}/g).join(' ')),
                    this.length > e && (t += ' ... ')),
                  '<Buffer ' + t + '>'
                );
              }),
              (s.prototype.compare = function (t, e, r, n, i) {
                if (!s.isBuffer(t))
                  throw new TypeError('Argument must be a Buffer');
                if (
                  (void 0 === e && (e = 0),
                  void 0 === r && (r = t ? t.length : 0),
                  void 0 === n && (n = 0),
                  void 0 === i && (i = this.length),
                  0 > e || r > t.length || 0 > n || i > this.length)
                )
                  throw new RangeError('out of range index');
                if (n >= i && e >= r) return 0;
                if (n >= i) return -1;
                if (e >= r) return 1;
                if (
                  ((e >>>= 0), (r >>>= 0), (n >>>= 0), (i >>>= 0), this === t)
                )
                  return 0;
                for (
                  var o = i - n,
                    a = r - e,
                    f = Math.min(o, a),
                    u = this.slice(n, i),
                    h = t.slice(e, r),
                    l = 0;
                  f > l;
                  ++l
                )
                  if (u[l] !== h[l]) {
                    (o = u[l]), (a = h[l]);
                    break;
                  }
                return a > o ? -1 : o > a ? 1 : 0;
              }),
              (s.prototype.indexOf = function (t, e, r) {
                if (
                  ('string' == typeof e
                    ? ((r = e), (e = 0))
                    : e > 2147483647
                    ? (e = 2147483647)
                    : -2147483648 > e && (e = -2147483648),
                  (e >>= 0),
                  0 === this.length)
                )
                  return -1;
                if (e >= this.length) return -1;
                if (
                  (0 > e && (e = Math.max(this.length + e, 0)),
                  'string' == typeof t && (t = s.from(t, r)),
                  s.isBuffer(t))
                )
                  return 0 === t.length ? -1 : E(this, t, e, r);
                if ('number' == typeof t)
                  return s.TYPED_ARRAY_SUPPORT &&
                    'function' === Uint8Array.prototype.indexOf
                    ? Uint8Array.prototype.indexOf.call(this, t, e)
                    : E(this, [t], e, r);
                throw new TypeError('val must be string, number or Buffer');
              }),
              (s.prototype.includes = function (t, e, r) {
                return -1 !== this.indexOf(t, e, r);
              }),
              (s.prototype.write = function (t, e, r, n) {
                if (void 0 === e) (n = 'utf8'), (r = this.length), (e = 0);
                else if (void 0 === r && 'string' == typeof e)
                  (n = e), (r = this.length), (e = 0);
                else {
                  if (!isFinite(e))
                    throw new Error(
                      'Buffer.write(string, encoding, offset[, length]) is no longer supported',
                    );
                  (e = 0 | e),
                    isFinite(r)
                      ? ((r = 0 | r), void 0 === n && (n = 'utf8'))
                      : ((n = r), (r = void 0));
                }
                var i = this.length - e;
                if (
                  ((void 0 === r || r > i) && (r = i),
                  (t.length > 0 && (0 > r || 0 > e)) || e > this.length)
                )
                  throw new RangeError(
                    'Attempt to write outside buffer bounds',
                  );
                n || (n = 'utf8');
                for (var o = !1; ; )
                  switch (n) {
                    case 'hex':
                      return b(this, t, e, r);
                    case 'utf8':
                    case 'utf-8':
                      return R(this, t, e, r);
                    case 'ascii':
                      return _(this, t, e, r);
                    case 'binary':
                      return A(this, t, e, r);
                    case 'base64':
                      return P(this, t, e, r);
                    case 'ucs2':
                    case 'ucs-2':
                    case 'utf16le':
                    case 'utf-16le':
                      return T(this, t, e, r);
                    default:
                      if (o) throw new TypeError('Unknown encoding: ' + n);
                      (n = ('' + n).toLowerCase()), (o = !0);
                  }
              }),
              (s.prototype.toJSON = function () {
                return {
                  type: 'Buffer',
                  data: Array.prototype.slice.call(this._arr || this, 0),
                };
              });
            var W = 4096;
            (s.prototype.slice = function (t, e) {
              var r = this.length;
              (t = ~~t),
                (e = void 0 === e ? r : ~~e),
                0 > t ? ((t += r), 0 > t && (t = 0)) : t > r && (t = r),
                0 > e ? ((e += r), 0 > e && (e = 0)) : e > r && (e = r),
                t > e && (e = t);
              var n;
              if (s.TYPED_ARRAY_SUPPORT)
                (n = this.subarray(t, e)), (n.__proto__ = s.prototype);
              else {
                var i = e - t;
                n = new s(i, void 0);
                for (var o = 0; i > o; o++) n[o] = this[o + t];
              }
              return n;
            }),
              (s.prototype.readUIntLE = function (t, e, r) {
                (t = 0 | t), (e = 0 | e), r || O(t, e, this.length);
                for (var n = this[t], i = 1, o = 0; ++o < e && (i *= 256); )
                  n += this[t + o] * i;
                return n;
              }),
              (s.prototype.readUIntBE = function (t, e, r) {
                (t = 0 | t), (e = 0 | e), r || O(t, e, this.length);
                for (var n = this[t + --e], i = 1; e > 0 && (i *= 256); )
                  n += this[t + --e] * i;
                return n;
              }),
              (s.prototype.readUInt8 = function (t, e) {
                return e || O(t, 1, this.length), this[t];
              }),
              (s.prototype.readUInt16LE = function (t, e) {
                return e || O(t, 2, this.length), this[t] | (this[t + 1] << 8);
              }),
              (s.prototype.readUInt16BE = function (t, e) {
                return e || O(t, 2, this.length), (this[t] << 8) | this[t + 1];
              }),
              (s.prototype.readUInt32LE = function (t, e) {
                return (
                  e || O(t, 4, this.length),
                  (this[t] | (this[t + 1] << 8) | (this[t + 2] << 16)) +
                    16777216 * this[t + 3]
                );
              }),
              (s.prototype.readUInt32BE = function (t, e) {
                return (
                  e || O(t, 4, this.length),
                  16777216 * this[t] +
                    ((this[t + 1] << 16) | (this[t + 2] << 8) | this[t + 3])
                );
              }),
              (s.prototype.readIntLE = function (t, e, r) {
                (t = 0 | t), (e = 0 | e), r || O(t, e, this.length);
                for (var n = this[t], i = 1, o = 0; ++o < e && (i *= 256); )
                  n += this[t + o] * i;
                return (i *= 128), n >= i && (n -= Math.pow(2, 8 * e)), n;
              }),
              (s.prototype.readIntBE = function (t, e, r) {
                (t = 0 | t), (e = 0 | e), r || O(t, e, this.length);
                for (var n = e, i = 1, o = this[t + --n]; n > 0 && (i *= 256); )
                  o += this[t + --n] * i;
                return (i *= 128), o >= i && (o -= Math.pow(2, 8 * e)), o;
              }),
              (s.prototype.readInt8 = function (t, e) {
                return (
                  e || O(t, 1, this.length),
                  128 & this[t] ? -1 * (255 - this[t] + 1) : this[t]
                );
              }),
              (s.prototype.readInt16LE = function (t, e) {
                e || O(t, 2, this.length);
                var r = this[t] | (this[t + 1] << 8);
                return 32768 & r ? 4294901760 | r : r;
              }),
              (s.prototype.readInt16BE = function (t, e) {
                e || O(t, 2, this.length);
                var r = this[t + 1] | (this[t] << 8);
                return 32768 & r ? 4294901760 | r : r;
              }),
              (s.prototype.readInt32LE = function (t, e) {
                return (
                  e || O(t, 4, this.length),
                  this[t] |
                    (this[t + 1] << 8) |
                    (this[t + 2] << 16) |
                    (this[t + 3] << 24)
                );
              }),
              (s.prototype.readInt32BE = function (t, e) {
                return (
                  e || O(t, 4, this.length),
                  (this[t] << 24) |
                    (this[t + 1] << 16) |
                    (this[t + 2] << 8) |
                    this[t + 3]
                );
              }),
              (s.prototype.readFloatLE = function (t, e) {
                return e || O(t, 4, this.length), Z.read(this, t, !0, 23, 4);
              }),
              (s.prototype.readFloatBE = function (t, e) {
                return e || O(t, 4, this.length), Z.read(this, t, !1, 23, 4);
              }),
              (s.prototype.readDoubleLE = function (t, e) {
                return e || O(t, 8, this.length), Z.read(this, t, !0, 52, 8);
              }),
              (s.prototype.readDoubleBE = function (t, e) {
                return e || O(t, 8, this.length), Z.read(this, t, !1, 52, 8);
              }),
              (s.prototype.writeUIntLE = function (t, e, r, n) {
                if (((t = +t), (e = 0 | e), (r = 0 | r), !n)) {
                  var i = Math.pow(2, 8 * r) - 1;
                  Y(this, t, e, r, i, 0);
                }
                var o = 1,
                  s = 0;
                for (this[e] = 255 & t; ++s < r && (o *= 256); )
                  this[e + s] = (t / o) & 255;
                return e + r;
              }),
              (s.prototype.writeUIntBE = function (t, e, r, n) {
                if (((t = +t), (e = 0 | e), (r = 0 | r), !n)) {
                  var i = Math.pow(2, 8 * r) - 1;
                  Y(this, t, e, r, i, 0);
                }
                var o = r - 1,
                  s = 1;
                for (this[e + o] = 255 & t; --o >= 0 && (s *= 256); )
                  this[e + o] = (t / s) & 255;
                return e + r;
              }),
              (s.prototype.writeUInt8 = function (t, e, r) {
                return (
                  (t = +t),
                  (e = 0 | e),
                  r || Y(this, t, e, 1, 255, 0),
                  s.TYPED_ARRAY_SUPPORT || (t = Math.floor(t)),
                  (this[e] = 255 & t),
                  e + 1
                );
              }),
              (s.prototype.writeUInt16LE = function (t, e, r) {
                return (
                  (t = +t),
                  (e = 0 | e),
                  r || Y(this, t, e, 2, 65535, 0),
                  s.TYPED_ARRAY_SUPPORT
                    ? ((this[e] = 255 & t), (this[e + 1] = t >>> 8))
                    : L(this, t, e, !0),
                  e + 2
                );
              }),
              (s.prototype.writeUInt16BE = function (t, e, r) {
                return (
                  (t = +t),
                  (e = 0 | e),
                  r || Y(this, t, e, 2, 65535, 0),
                  s.TYPED_ARRAY_SUPPORT
                    ? ((this[e] = t >>> 8), (this[e + 1] = 255 & t))
                    : L(this, t, e, !1),
                  e + 2
                );
              }),
              (s.prototype.writeUInt32LE = function (t, e, r) {
                return (
                  (t = +t),
                  (e = 0 | e),
                  r || Y(this, t, e, 4, 4294967295, 0),
                  s.TYPED_ARRAY_SUPPORT
                    ? ((this[e + 3] = t >>> 24),
                      (this[e + 2] = t >>> 16),
                      (this[e + 1] = t >>> 8),
                      (this[e] = 255 & t))
                    : D(this, t, e, !0),
                  e + 4
                );
              }),
              (s.prototype.writeUInt32BE = function (t, e, r) {
                return (
                  (t = +t),
                  (e = 0 | e),
                  r || Y(this, t, e, 4, 4294967295, 0),
                  s.TYPED_ARRAY_SUPPORT
                    ? ((this[e] = t >>> 24),
                      (this[e + 1] = t >>> 16),
                      (this[e + 2] = t >>> 8),
                      (this[e + 3] = 255 & t))
                    : D(this, t, e, !1),
                  e + 4
                );
              }),
              (s.prototype.writeIntLE = function (t, e, r, n) {
                if (((t = +t), (e = 0 | e), !n)) {
                  var i = Math.pow(2, 8 * r - 1);
                  Y(this, t, e, r, i - 1, -i);
                }
                var o = 0,
                  s = 1,
                  a = 0;
                for (this[e] = 255 & t; ++o < r && (s *= 256); )
                  0 > t && 0 === a && 0 !== this[e + o - 1] && (a = 1),
                    (this[e + o] = (((t / s) >> 0) - a) & 255);
                return e + r;
              }),
              (s.prototype.writeIntBE = function (t, e, r, n) {
                if (((t = +t), (e = 0 | e), !n)) {
                  var i = Math.pow(2, 8 * r - 1);
                  Y(this, t, e, r, i - 1, -i);
                }
                var o = r - 1,
                  s = 1,
                  a = 0;
                for (this[e + o] = 255 & t; --o >= 0 && (s *= 256); )
                  0 > t && 0 === a && 0 !== this[e + o + 1] && (a = 1),
                    (this[e + o] = (((t / s) >> 0) - a) & 255);
                return e + r;
              }),
              (s.prototype.writeInt8 = function (t, e, r) {
                return (
                  (t = +t),
                  (e = 0 | e),
                  r || Y(this, t, e, 1, 127, -128),
                  s.TYPED_ARRAY_SUPPORT || (t = Math.floor(t)),
                  0 > t && (t = 255 + t + 1),
                  (this[e] = 255 & t),
                  e + 1
                );
              }),
              (s.prototype.writeInt16LE = function (t, e, r) {
                return (
                  (t = +t),
                  (e = 0 | e),
                  r || Y(this, t, e, 2, 32767, -32768),
                  s.TYPED_ARRAY_SUPPORT
                    ? ((this[e] = 255 & t), (this[e + 1] = t >>> 8))
                    : L(this, t, e, !0),
                  e + 2
                );
              }),
              (s.prototype.writeInt16BE = function (t, e, r) {
                return (
                  (t = +t),
                  (e = 0 | e),
                  r || Y(this, t, e, 2, 32767, -32768),
                  s.TYPED_ARRAY_SUPPORT
                    ? ((this[e] = t >>> 8), (this[e + 1] = 255 & t))
                    : L(this, t, e, !1),
                  e + 2
                );
              }),
              (s.prototype.writeInt32LE = function (t, e, r) {
                return (
                  (t = +t),
                  (e = 0 | e),
                  r || Y(this, t, e, 4, 2147483647, -2147483648),
                  s.TYPED_ARRAY_SUPPORT
                    ? ((this[e] = 255 & t),
                      (this[e + 1] = t >>> 8),
                      (this[e + 2] = t >>> 16),
                      (this[e + 3] = t >>> 24))
                    : D(this, t, e, !0),
                  e + 4
                );
              }),
              (s.prototype.writeInt32BE = function (t, e, r) {
                return (
                  (t = +t),
                  (e = 0 | e),
                  r || Y(this, t, e, 4, 2147483647, -2147483648),
                  0 > t && (t = 4294967295 + t + 1),
                  s.TYPED_ARRAY_SUPPORT
                    ? ((this[e] = t >>> 24),
                      (this[e + 1] = t >>> 16),
                      (this[e + 2] = t >>> 8),
                      (this[e + 3] = 255 & t))
                    : D(this, t, e, !1),
                  e + 4
                );
              }),
              (s.prototype.writeFloatLE = function (t, e, r) {
                return k(this, t, e, !0, r);
              }),
              (s.prototype.writeFloatBE = function (t, e, r) {
                return k(this, t, e, !1, r);
              }),
              (s.prototype.writeDoubleLE = function (t, e, r) {
                return N(this, t, e, !0, r);
              }),
              (s.prototype.writeDoubleBE = function (t, e, r) {
                return N(this, t, e, !1, r);
              }),
              (s.prototype.copy = function (t, e, r, n) {
                if (
                  (r || (r = 0),
                  n || 0 === n || (n = this.length),
                  e >= t.length && (e = t.length),
                  e || (e = 0),
                  n > 0 && r > n && (n = r),
                  n === r)
                )
                  return 0;
                if (0 === t.length || 0 === this.length) return 0;
                if (0 > e) throw new RangeError('targetStart out of bounds');
                if (0 > r || r >= this.length)
                  throw new RangeError('sourceStart out of bounds');
                if (0 > n) throw new RangeError('sourceEnd out of bounds');
                n > this.length && (n = this.length),
                  t.length - e < n - r && (n = t.length - e + r);
                var i,
                  o = n - r;
                if (this === t && e > r && n > e)
                  for (i = o - 1; i >= 0; i--) t[i + e] = this[i + r];
                else if (1e3 > o || !s.TYPED_ARRAY_SUPPORT)
                  for (i = 0; o > i; i++) t[i + e] = this[i + r];
                else
                  Uint8Array.prototype.set.call(t, this.subarray(r, r + o), e);
                return o;
              }),
              (s.prototype.fill = function (t, e, r, n) {
                if ('string' == typeof t) {
                  if (
                    ('string' == typeof e
                      ? ((n = e), (e = 0), (r = this.length))
                      : 'string' == typeof r && ((n = r), (r = this.length)),
                    1 === t.length)
                  ) {
                    var i = t.charCodeAt(0);
                    256 > i && (t = i);
                  }
                  if (void 0 !== n && 'string' != typeof n)
                    throw new TypeError('encoding must be a string');
                  if ('string' == typeof n && !s.isEncoding(n))
                    throw new TypeError('Unknown encoding: ' + n);
                } else 'number' == typeof t && (t = 255 & t);
                if (0 > e || this.length < e || this.length < r)
                  throw new RangeError('Out of range index');
                if (e >= r) return this;
                (e >>>= 0),
                  (r = void 0 === r ? this.length : r >>> 0),
                  t || (t = 0);
                var o;
                if ('number' == typeof t) for (o = e; r > o; o++) this[o] = t;
                else {
                  var a = s.isBuffer(t) ? t : z(new s(t, n).toString()),
                    f = a.length;
                  for (o = 0; r - e > o; o++) this[o + e] = a[o % f];
                }
                return this;
              });
            var tt = /[^+\/0-9A-Za-z-_]/g;
          }.call(
            this,
            'undefined' != typeof global
              ? global
              : 'undefined' != typeof self
              ? self
              : 'undefined' != typeof window
              ? window
              : {},
          ));
        },
        { 'base64-js': 1, ieee754: 4, isarray: 5 },
      ],
      4: [
        function (t, e, r) {
          (r.read = function (t, e, r, n, i) {
            var o,
              s,
              a = 8 * i - n - 1,
              f = (1 << a) - 1,
              u = f >> 1,
              h = -7,
              l = r ? i - 1 : 0,
              c = r ? -1 : 1,
              p = t[e + l];
            for (
              l += c, o = p & ((1 << -h) - 1), p >>= -h, h += a;
              h > 0;
              o = 256 * o + t[e + l], l += c, h -= 8
            );
            for (
              s = o & ((1 << -h) - 1), o >>= -h, h += n;
              h > 0;
              s = 256 * s + t[e + l], l += c, h -= 8
            );
            if (0 === o) o = 1 - u;
            else {
              if (o === f) return s ? NaN : (p ? -1 : 1) * (1 / 0);
              (s += Math.pow(2, n)), (o -= u);
            }
            return (p ? -1 : 1) * s * Math.pow(2, o - n);
          }),
            (r.write = function (t, e, r, n, i, o) {
              var s,
                a,
                f,
                u = 8 * o - i - 1,
                h = (1 << u) - 1,
                l = h >> 1,
                c = 23 === i ? Math.pow(2, -24) - Math.pow(2, -77) : 0,
                p = n ? 0 : o - 1,
                d = n ? 1 : -1,
                g = 0 > e || (0 === e && 0 > 1 / e) ? 1 : 0;
              for (
                e = Math.abs(e),
                  isNaN(e) || e === 1 / 0
                    ? ((a = isNaN(e) ? 1 : 0), (s = h))
                    : ((s = Math.floor(Math.log(e) / Math.LN2)),
                      e * (f = Math.pow(2, -s)) < 1 && (s--, (f *= 2)),
                      (e += s + l >= 1 ? c / f : c * Math.pow(2, 1 - l)),
                      e * f >= 2 && (s++, (f /= 2)),
                      s + l >= h
                        ? ((a = 0), (s = h))
                        : s + l >= 1
                        ? ((a = (e * f - 1) * Math.pow(2, i)), (s += l))
                        : ((a = e * Math.pow(2, l - 1) * Math.pow(2, i)),
                          (s = 0)));
                i >= 8;
                t[r + p] = 255 & a, p += d, a /= 256, i -= 8
              );
              for (
                s = (s << i) | a, u += i;
                u > 0;
                t[r + p] = 255 & s, p += d, s /= 256, u -= 8
              );
              t[r + p - d] |= 128 * g;
            });
        },
        {},
      ],
      5: [
        function (t, e, r) {
          var n = {}.toString;
          e.exports =
            Array.isArray ||
            function (t) {
              return '[object Array]' == n.call(t);
            };
        },
        {},
      ],
      6: [
        function (t, e, r) {
          (function (r, n) {
            'use strict';
            var i = function (t, e, r, i) {
              if (
                ((i = i || {}),
                (this.dictionary = null),
                (this.rules = {}),
                (this.dictionaryTable = {}),
                (this.compoundRules = []),
                (this.compoundRuleCodes = {}),
                (this.replacementTable = []),
                (this.flags = i.flags || {}),
                t)
              ) {
                if (
                  ((this.dictionary = t),
                  'undefined' != typeof window &&
                    'chrome' in window &&
                    'extension' in window.chrome &&
                    'getURL' in window.chrome.extension)
                )
                  e ||
                    (e = this._readFile(
                      chrome.extension.getURL(
                        'lib/typo/dictionaries/' + t + '/' + t + '.aff',
                      ),
                    )),
                    r ||
                      (r = this._readFile(
                        chrome.extension.getURL(
                          'lib/typo/dictionaries/' + t + '/' + t + '.dic',
                        ),
                      ));
                else {
                  if (i.dictionaryPath) var o = i.dictionaryPath;
                  else if ('undefined' != typeof n) var o = n + '/dictionaries';
                  else var o = './dictionaries';
                  e || (e = this._readFile(o + '/' + t + '/' + t + '.aff')),
                    r || (r = this._readFile(o + '/' + t + '/' + t + '.dic'));
                }
                (this.rules = this._parseAFF(e)), (this.compoundRuleCodes = {});
                for (var s = 0, a = this.compoundRules.length; a > s; s++)
                  for (
                    var f = this.compoundRules[s], u = 0, h = f.length;
                    h > u;
                    u++
                  )
                    this.compoundRuleCodes[f[u]] = [];
                'ONLYINCOMPOUND' in this.flags &&
                  (this.compoundRuleCodes[this.flags.ONLYINCOMPOUND] = []),
                  (this.dictionaryTable = this._parseDIC(r));
                for (var s in this.compoundRuleCodes)
                  0 == this.compoundRuleCodes[s].length &&
                    delete this.compoundRuleCodes[s];
                for (var s = 0, a = this.compoundRules.length; a > s; s++) {
                  for (
                    var l = this.compoundRules[s], c = '', u = 0, h = l.length;
                    h > u;
                    u++
                  ) {
                    var p = l[u];
                    c +=
                      p in this.compoundRuleCodes
                        ? '(' + this.compoundRuleCodes[p].join('|') + ')'
                        : p;
                  }
                  this.compoundRules[s] = new RegExp(c, 'i');
                }
              }
              return this;
            };
            (i.prototype = {
              load: function (t) {
                for (var e in t) this[e] = t[e];
                return this;
              },
              _readFile: function (e, n) {
                if ((n || (n = 'utf8'), 'undefined' != typeof XMLHttpRequest)) {
                  var i = new XMLHttpRequest();
                  return (
                    i.open('GET', e, !1),
                    i.overrideMimeType &&
                      i.overrideMimeType('text/plain; charset=' + n),
                    i.send(null),
                    i.responseText
                  );
                }
                if ('undefined' != typeof t) {
                  var o = t('fs');
                  try {
                    if (o.existsSync(e)) {
                      var s = o.statSync(e),
                        a = o.openSync(e, 'r'),
                        f = new r(s.size);
                      return (
                        o.readSync(a, f, 0, f.length, null),
                        f.toString(n, 0, f.length)
                      );
                    }
                    console.log('Path ' + e + ' does not exist.');
                  } catch (u) {
                    return console.log(u), '';
                  }
                }
              },
              _parseAFF: function (t) {
                var e = {};
                t = this._removeAffixComments(t);
                for (var r = t.split('\n'), n = 0, i = r.length; i > n; n++) {
                  var o = r[n],
                    s = o.split(/\s+/),
                    a = s[0];
                  if ('PFX' == a || 'SFX' == a) {
                    for (
                      var f = s[1],
                        u = s[2],
                        h = parseInt(s[3], 10),
                        l = [],
                        c = n + 1,
                        p = n + 1 + h;
                      p > c;
                      c++
                    ) {
                      var o = r[c],
                        d = o.split(/\s+/),
                        g = d[2],
                        y = d[3].split('/'),
                        v = y[0];
                      '0' === v && (v = '');
                      var w = this.parseRuleCodes(y[1]),
                        m = d[4],
                        E = {};
                      (E.add = v),
                        w.length > 0 && (E.continuationClasses = w),
                        '.' !== m &&
                          ('SFX' === a
                            ? (E.match = new RegExp(m + '$'))
                            : (E.match = new RegExp('^' + m))),
                        '0' != g &&
                          ('SFX' === a
                            ? (E.remove = new RegExp(g + '$'))
                            : (E.remove = g)),
                        l.push(E);
                    }
                    (e[f] = { type: a, combineable: 'Y' == u, entries: l }),
                      (n += h);
                  } else if ('COMPOUNDRULE' === a) {
                    for (
                      var h = parseInt(s[1], 10), c = n + 1, p = n + 1 + h;
                      p > c;
                      c++
                    ) {
                      var o = r[c],
                        d = o.split(/\s+/);
                      this.compoundRules.push(d[1]);
                    }
                    n += h;
                  } else if ('REP' === a) {
                    var d = o.split(/\s+/);
                    3 === d.length && this.replacementTable.push([d[1], d[2]]);
                  } else this.flags[a] = s[1];
                }
                return e;
              },
              _removeAffixComments: function (t) {
                return (
                  (t = t.replace(/#.*$/gm, '')),
                  (t = t.replace(/^\s\s*/m, '').replace(/\s\s*$/m, '')),
                  (t = t.replace(/\n{2,}/g, '\n')),
                  (t = t.replace(/^\s\s*/, '').replace(/\s\s*$/, ''))
                );
              },
              _parseDIC: function (t) {
                function e(t, e) {
                  (t in n && 'object' == typeof n[t]) || (n[t] = []),
                    n[t].push(e);
                }
                t = this._removeDicComments(t);
                for (
                  var r = t.split('\n'), n = {}, i = 1, o = r.length;
                  o > i;
                  i++
                ) {
                  var s = r[i],
                    a = s.split('/', 2),
                    f = a[0];
                  if (a.length > 1) {
                    var u = this.parseRuleCodes(a[1]);
                    ('NEEDAFFIX' in this.flags &&
                      -1 != u.indexOf(this.flags.NEEDAFFIX)) ||
                      e(f, u);
                    for (var h = 0, l = u.length; l > h; h++) {
                      var c = u[h],
                        p = this.rules[c];
                      if (p)
                        for (
                          var d = this._applyRule(f, p), g = 0, y = d.length;
                          y > g;
                          g++
                        ) {
                          var v = d[g];
                          if ((e(v, []), p.combineable))
                            for (var w = h + 1; l > w; w++) {
                              var m = u[w],
                                E = this.rules[m];
                              if (E && E.combineable && p.type != E.type)
                                for (
                                  var b = this._applyRule(v, E),
                                    R = 0,
                                    _ = b.length;
                                  _ > R;
                                  R++
                                ) {
                                  var A = b[R];
                                  e(A, []);
                                }
                            }
                        }
                      c in this.compoundRuleCodes &&
                        this.compoundRuleCodes[c].push(f);
                    }
                  } else e(f.trim(), []);
                }
                return n;
              },
              _removeDicComments: function (t) {
                return (t = t.replace(/^\t.*$/gm, ''));
              },
              parseRuleCodes: function (t) {
                if (!t) return [];
                if (!('FLAG' in this.flags)) return t.split('');
                if ('long' === this.flags.FLAG) {
                  for (var e = [], r = 0, n = t.length; n > r; r += 2)
                    e.push(t.substr(r, 2));
                  return e;
                }
                return 'num' === this.flags.FLAG ? textCode.split(',') : void 0;
              },
              _applyRule: function (t, e) {
                for (
                  var r = e.entries, n = [], i = 0, o = r.length;
                  o > i;
                  i++
                ) {
                  var s = r[i];
                  if (!s.match || t.match(s.match)) {
                    var a = t;
                    if (
                      (s.remove && (a = a.replace(s.remove, '')),
                      'SFX' === e.type ? (a += s.add) : (a = s.add + a),
                      n.push(a),
                      'continuationClasses' in s)
                    )
                      for (
                        var f = 0, u = s.continuationClasses.length;
                        u > f;
                        f++
                      ) {
                        var h = this.rules[s.continuationClasses[f]];
                        h && (n = n.concat(this._applyRule(a, h)));
                      }
                  }
                }
                return n;
              },
              check: function (t) {
                var e = t.replace(/^\s\s*/, '').replace(/\s\s*$/, '');
                if (this.checkExact(e)) return !0;
                if (e.toUpperCase() === e) {
                  var r = e[0] + e.substring(1).toLowerCase();
                  if (this.hasFlag(r, 'KEEPCASE')) return !1;
                  if (this.checkExact(r)) return !0;
                }
                var n = e.toLowerCase();
                if (n !== e) {
                  if (this.hasFlag(n, 'KEEPCASE')) return !1;
                  if (this.checkExact(n)) return !0;
                }
                return !1;
              },
              checkExact: function (t) {
                var e = this.dictionaryTable[t];
                if ('undefined' == typeof e) {
                  if (
                    'COMPOUNDMIN' in this.flags &&
                    t.length >= this.flags.COMPOUNDMIN
                  )
                    for (var r = 0, n = this.compoundRules.length; n > r; r++)
                      if (t.match(this.compoundRules[r])) return !0;
                  return !1;
                }
                if ('object' == typeof e) {
                  for (var r = 0, n = e.length; n > r; r++)
                    if (!this.hasFlag(t, 'ONLYINCOMPOUND', e[r])) return !0;
                  return !1;
                }
              },
              hasFlag: function (t, e, r) {
                if (e in this.flags) {
                  if ('undefined' == typeof r)
                    var r = Array.prototype.concat.apply(
                      [],
                      this.dictionaryTable[t],
                    );
                  if (r && -1 !== r.indexOf(this.flags[e])) return !0;
                }
                return !1;
              },
              alphabet: '',
              suggest: function (t, e) {
                function r(t) {
                  for (var e = [], r = 0, n = t.length; n > r; r++) {
                    for (
                      var i = t[r], o = [], s = 0, a = i.length + 1;
                      a > s;
                      s++
                    )
                      o.push([i.substring(0, s), i.substring(s, i.length)]);
                    for (var f = [], s = 0, a = o.length; a > s; s++) {
                      var h = o[s];
                      h[1] && f.push(h[0] + h[1].substring(1));
                    }
                    for (var l = [], s = 0, a = o.length; a > s; s++) {
                      var h = o[s];
                      h[1].length > 1 &&
                        l.push(h[0] + h[1][1] + h[1][0] + h[1].substring(2));
                    }
                    for (var c = [], s = 0, a = o.length; a > s; s++) {
                      var h = o[s];
                      if (h[1])
                        for (var p = 0, d = u.alphabet.length; d > p; p++)
                          c.push(h[0] + u.alphabet[p] + h[1].substring(1));
                    }
                    for (var g = [], s = 0, a = o.length; a > s; s++) {
                      var h = o[s];
                      if (h[1])
                        for (var p = 0, d = u.alphabet.length; d > p; p++)
                          c.push(h[0] + u.alphabet[p] + h[1]);
                    }
                    (e = e.concat(f)),
                      (e = e.concat(l)),
                      (e = e.concat(c)),
                      (e = e.concat(g));
                  }
                  return e;
                }
                function n(t) {
                  for (var e = [], r = 0; r < t.length; r++)
                    u.check(t[r]) && e.push(t[r]);
                  return e;
                }
                function i(t) {
                  function i(t, e) {
                    return t[1] < e[1] ? -1 : 1;
                  }
                  for (
                    var o = r([t]),
                      s = r(o),
                      a = n(o).concat(n(s)),
                      f = {},
                      h = 0,
                      l = a.length;
                    l > h;
                    h++
                  )
                    a[h] in f ? (f[a[h]] += 1) : (f[a[h]] = 1);
                  var c = [];
                  for (var h in f) c.push([h, f[h]]);
                  c.sort(i).reverse();
                  for (var p = [], h = 0, l = Math.min(e, c.length); l > h; h++)
                    u.hasFlag(c[h][0], 'NOSUGGEST') || p.push(c[h][0]);
                  return p;
                }
                if ((e || (e = 5), this.check(t))) return [];
                for (var o = 0, s = this.replacementTable.length; s > o; o++) {
                  var a = this.replacementTable[o];
                  if (-1 !== t.indexOf(a[0])) {
                    var f = t.replace(a[0], a[1]);
                    if (this.check(f)) return [f];
                  }
                }
                var u = this;
                return (u.alphabet = 'abcdefghijklmnopqrstuvwxyz'), i(t);
              },
            }),
              'undefined' != typeof e && (e.exports = i);
          }.call(this, t('buffer').Buffer, '/node_modules/typo-js'));
        },
        { buffer: 3, fs: 2 },
      ],
      7: [
        function (t, e, r) {
          'use strict';
          function n(t) {
            return (
              (t = t || {}),
              'function' != typeof t.codeMirrorInstance ||
              'function' != typeof t.codeMirrorInstance.defineMode
                ? void console.log(
                    'CodeMirror Spell Checker: You must provide an instance of CodeMirror via the option `codeMirrorInstance`',
                  )
                : (String.prototype.includes ||
                    (String.prototype.includes = function () {
                      return (
                        -1 !== String.prototype.indexOf.apply(this, arguments)
                      );
                    }),
                  void t.codeMirrorInstance.defineMode(
                    'spell-checker',
                    function (e) {
                      if (!n.aff_loading) {
                        n.aff_loading = !0;
                        var r = new XMLHttpRequest();
                        r.open(
                          'GET',
                          'https://cdn.jsdelivr.net/codemirror.spell-checker/latest/en_US.aff',
                          !0,
                        ),
                          (r.onload = function () {
                            4 === r.readyState &&
                              200 === r.status &&
                              ((n.aff_data = r.responseText),
                              n.num_loaded++,
                              2 == n.num_loaded &&
                                (n.typo = new i(
                                  'en_US',
                                  n.aff_data,
                                  n.dic_data,
                                  { platform: 'any' },
                                )));
                          }),
                          r.send(null);
                      }
                      if (!n.dic_loading) {
                        n.dic_loading = !0;
                        var o = new XMLHttpRequest();
                        o.open(
                          'GET',
                          'https://cdn.jsdelivr.net/codemirror.spell-checker/latest/en_US.dic',
                          !0,
                        ),
                          (o.onload = function () {
                            4 === o.readyState &&
                              200 === o.status &&
                              ((n.dic_data = o.responseText),
                              n.num_loaded++,
                              2 == n.num_loaded &&
                                (n.typo = new i(
                                  'en_US',
                                  n.aff_data,
                                  n.dic_data,
                                  { platform: 'any' },
                                )));
                          }),
                          o.send(null);
                      }
                      var s = '!"#$%&()*+,-./:;<=>?@[\\]^_`{|}~ ',
                        a = {
                          token: function (t) {
                            var e = t.peek(),
                              r = '';
                            if (s.includes(e)) return t.next(), null;
                            for (; null != (e = t.peek()) && !s.includes(e); )
                              (r += e), t.next();
                            return n.typo && !n.typo.check(r)
                              ? 'spell-error'
                              : null;
                          },
                        },
                        f = t.codeMirrorInstance.getMode(
                          e,
                          e.backdrop || 'text/plain',
                        );
                      return t.codeMirrorInstance.overlayMode(f, a, !0);
                    },
                  ))
            );
          }
          var i = t('typo-js');
          (n.num_loaded = 0),
            (n.aff_loading = !1),
            (n.dic_loading = !1),
            (n.aff_data = ''),
            (n.dic_data = ''),
            n.typo,
            (e.exports = n);
        },
        { 'typo-js': 6 },
      ],
    },
    {},
    [7],
  )(7);
});
