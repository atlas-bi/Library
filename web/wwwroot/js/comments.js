(function () {
  var d = document;
  d.addEventListener(
    'submit',
    function (e) {
      if (e.target.matches('.comments-form')) {
        e.preventDefault();
        var t = e.target,
          c = d.getElementsByClassName('comments')[0],
          url,
          q;
        url = serialize(t);
        c.style.opacity = 0;
        c.style.transition = 'opacity 0.1s ease-in-out';
        q = new XMLHttpRequest();
        q.open('post', t.getAttribute('action') + '&' + url, true);
        q.setRequestHeader(
          'Content-Type',
          'application/x-www-form-urlencoded; charset=UTF-8',
        );
        q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
        q.send();

        q.onload = function () {
          c.innerHTML = q.responseText;
          c.style.opacity = 1;
          d.dispatchEvent(new CustomEvent('build-inputs'));
        };
      }
    },
    false,
  );
})();
