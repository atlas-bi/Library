// Related reports
(function () {
  function relatedReports() {
    const links = document.querySelectorAll('.body-main a[href]');
    const ids = [];
    const re = /[R|r]eports\?id=(\d+)/;
    let m;

    for (let x = 0; x < links.length; x++) {
      m = links[x].getAttribute('href').match(re);
      if (m) {
        ids.push(m[1]);
      }
    }

    m = window.location.href.match(re);
    if (m) {
      ids.push(m[1]);
    }

    if (ids.length > 0 && document.querySelector('#AdColTwo')) {
      let div = document.querySelector('#related-reports');
      if (!div) {
        div = document.createElement('div');
        div.setAttribute('id', 'related-reports');
        document.querySelector('#AdColTwo').append(div);
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
