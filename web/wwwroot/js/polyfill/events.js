// (function () {
// /*! (c) Andrea Giammarchi - ISC */
// var self = this || {};
// try {
//   self.EventTarget = new EventTarget().constructor;
// } catch (e) {
//   !(function (e, o) {
//     var t = e.create,
//       r = e.defineProperty,
//       n = i.prototype;

//     function i() {
//       'use strict';
//       o.set(this, t(null));
//     }

//     function s(e, t, n) {
//       r(e, t, {
//         configurable: !0,
//         writable: !0,
//         value: n,
//       });
//     }

//     function a(e) {
//       var t = e.options;
//       t && t.once && e.target.removeEventListener(this.type, e.listener),
//         'function' == typeof e.listener
//           ? e.listener.call(e.target, this)
//           : e.listener.handleEvent(this);
//     }
//     console.info('eventlistener polyfill for IE11');
//     s(n, 'addEventListener', function (e, t, n) {
//       for (
//         var r = o.get(this), i = r[e] || (r[e] = []), s = 0, a = i.length;
//         s < a;
//         s++
//       )
//         if (i[s].listener === t) return;
//       i.push({
//         target: this,
//         listener: t,
//         options: n,
//       });
//     }),
//       s(n, 'dispatchEvent', function (e) {
//         var t = o.get(this)[e.type];
//         return (
//           t &&
//             (s(e, 'target', this),
//             s(e, 'currentTarget', this),
//             t.slice(0).forEach(a, e),
//             delete e.currentTarget,
//             delete e.target),
//           !0
//         );
//       }),
//       s(n, 'removeEventListener', function (e, t) {
//         for (
//           var n = o.get(this), r = n[e] || (n[e] = []), i = 0, s = r.length;
//           i < s;
//           i++
//         )
//           if (r[i].listener === t) return void r.splice(i, 1);
//       }),
//       (self.EventTarget = i);
//   })(Object, new WeakMap());
// }

//   var supportsPassive = false;
//   document.createElement('div').addEventListener('test', function () {}, {
//     get passive() {
//       supportsPassive = true;
//       return false;
//     },
//   });

//   if (!supportsPassive) {
//     console.info('passive events polyfill for IE11');
//     var super_add_event_listener = EventTarget.prototype.addEventListener;
//     var super_remove_event_listener = EventTarget.prototype.removeEventListener;
//     var super_prevent_default = Event.prototype.preventDefault;

//     function parseOptions(type, listener, options, action) {
//       var needsWrapping = false;
//       var useCapture = false;
//       var passive = false;
//       var fieldId;
//       if (options) {
//         if (typeof options === 'object') {
//           passive = options.passive ? true : false;
//           useCapture = options.useCapture ? true : false;
//         } else {
//           useCapture = options;
//         }
//       }
//       if (passive) needsWrapping = true;
//       if (needsWrapping) {
//         fieldId = useCapture.toString();
//         fieldId += passive.toString();
//       }
//       action(needsWrapping, fieldId, useCapture, passive);
//     }

//     Event.prototype.preventDefault = function () {
//       if (this.__passive) {
//         console.warn(
//           'Ignored attempt to preventDefault an event from a passive listener',
//         );
//         return;
//       }
//       super_prevent_default.apply(this);
//     };

//     EventTarget.prototype.addEventListener = function (
//       type,
//       listener,
//       options,
//     ) {
//       var super_this = this;
//       parseOptions(
//         type,
//         listener,
//         options,
//         function (needsWrapping, fieldId, useCapture, passive) {
//           if (needsWrapping) {
//             var fieldId = useCapture.toString();
//             fieldId += passive.toString();

//             if (!this.__event_listeners_options)
//               this.__event_listeners_options = {};
//             if (!this.__event_listeners_options[type])
//               this.__event_listeners_options[type] = {};
//             if (!this.__event_listeners_options[type][listener])
//               this.__event_listeners_options[type][listener] = [];
//             if (this.__event_listeners_options[type][listener][fieldId]) return;
//             var wrapped = {
//               handleEvent: function (e) {
//                 e.__passive = passive;
//                 if (typeof listener === 'function') {
//                   listener(e);
//                 } else {
//                   listener.handleEvent(e);
//                 }
//                 e.__passive = false;
//               },
//             };
//             this.__event_listeners_options[type][listener][fieldId] = wrapped;
//             super_add_event_listener.call(
//               super_this,
//               type,
//               wrapped,
//               useCapture,
//             );
//           } else {
//             super_add_event_listener.call(
//               super_this,
//               type,
//               listener,
//               useCapture,
//             );
//           }
//         },
//       );
//     };

//     EventTarget.prototype.removeEventListener = function (
//       type,
//       listener,
//       options,
//     ) {
//       var super_this = this;
//       parseOptions(
//         type,
//         listener,
//         options,
//         function (needsWrapping, fieldId, useCapture, passive) {
//           if (
//             needsWrapping &&
//             this.__event_listeners_options &&
//             this.__event_listeners_options[type] &&
//             this.__event_listeners_options[type][listener] &&
//             this.__event_listeners_options[type][listener][fieldId]
//           ) {
//             super_remove_event_listener.call(
//               super_this,
//               type,
//               this.__event_listeners_options[type][listener][fieldId],
//               false,
//             );
//             delete this.__event_listeners_options[type][listener][fieldId];
//             if (this.__event_listeners_options[type][listener].length == 0)
//               delete this.__event_listeners_options[type][listener];
//           } else {
//             super_remove_event_listener.call(
//               super_this,
//               type,
//               listener,
//               useCapture,
//             );
//           }
//         },
//       );
//     };
//   }
// })();
