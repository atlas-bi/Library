(function () {
  var d = document,
      q;
  d.addEventListener('click', function (e) {
    var t = e.target,
        v = d.getElementsByTagName("video")[0],
        w = d.getElementsByClassName('video')[0];

    function on() {
      var s = w.getElementsByTagName('source')[0];

      if (s.hasAttribute('data-src')) {
        s.setAttribute('src', s.getAttribute('data-src'));
        s.removeAttribute('data-src');
        v.appendChild(s);
      }

      v.play();
      q = new XMLHttpRequest();
      q.open('post', '/Users?handler=WelcomeToAtlasState&State=1', true);
      q.setRequestHeader('Content-Type', 'text/html;charset=UTF-8`');
      q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
      q.send();
    }

    function off() {
      w.classList.remove('video-large');
      w.classList.add('video-closed');
      v.pause();
      q = new XMLHttpRequest();
      q.open('post', '/Users?handler=WelcomeToAtlasState&State=0', true);
      q.setRequestHeader('Content-Type', 'text/html;charset=UTF-8`');
      q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
      q.send();
    }

    if (t.closest('.video-open')) {
      w.classList.remove('video-closed');
      w.classList.add('video-large');
      on();
    } else if (t.closest('.video-min')) {
      w.classList.remove('video-large');
      w.classList.remove('video-closed');
      on();
    } else if (t.closest('.video-close')) {
      off();
    } else if (t.closest('.video')) {
      w.classList.remove('video-closed');
      on();
    } else {
      if (w) {
        w.classList.remove('video-large');
      }
    }
  });
})();