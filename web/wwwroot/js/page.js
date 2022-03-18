// related reports
(function () {
  function relatedReports() {
    var links = document.querySelectorAll('.body-main a[href]'),
      ids = [],
      re = /[R|r]eports\?id=(\d+)/,
      m;

    for (var x = 0; x < links.length; x++) {
      m = links[x].getAttribute('href').match(re);
      if (m) {
        ids.push(m[1]);
      }
    }
    m = window.location.href.match(re);
    if (m) {
      ids.push(m[1]);
    }

    if (ids.length > 0 && document.getElementById('AdColTwo')) {
      var div = document.getElementById('related-reports');
      if (!div) {
        div = document.createElement('div');
        div.setAttribute('id', 'related-reports');
        document.getElementById('AdColTwo').appendChild(div);
      }
      div.setAttribute(
        'data-url',
        '/Search/?handler=RelatedReports&id=' + ids.slice(0, 5).join(','),
      );
      div.setAttribute('data-ajax', 'yes');

      document.dispatchEvent(new CustomEvent('load-ajax-content'));
    }
  }
  relatedReports();

  document.addEventListener('related-reports', function () {
    relatedReports();
  });
})();
