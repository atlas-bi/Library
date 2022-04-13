// Document.addEventListener('DOMContentLoaded', function () {
//   var e = document,
//     t = null,
//     getActiveUsers = function () {
//       if (t !== null) {
//         t.abort();
//       }

//       t = new XMLHttpRequest();
//       t.open('get', '/analytics?handler=LiveUsers', !0);
//       t.setRequestHeader(
//         'Content-Type',
//         'application/x-www-form-urlencoded; charset=UTF-8',
//       );
//       t.send();
//       t.onload = function () {
//         if (e.getElementById('active-users')) {
//           e.getElementById('active-users').innerHTML = t.responseText;
//           e.dispatchEvent(new CustomEvent('ajax'));
//         }
//       };
//     };

//   getActiveUsers();
//   setInterval(function () {
//     if (e.getElementById('active-users')) {
//       getActiveUsers();
//     }
//   }, 1e4);
// });
