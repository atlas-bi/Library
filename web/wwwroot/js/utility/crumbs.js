(function () {
  // add page location to breadcrumbs when leaving page

  // window.onbeforeunload = function () {
  //   setTimeout(function () {
  //     breadcrumbs();
  //   }, 0);
  // };

  var breadcrumbs = function () {
    var title =
        document.title.indexOf(' | ') != -1
          ? document.title.split(' | ')[0]
          : document.title,
      url = window.location.href,
      j = {},
      crumbs = sessionStorage.getItem('breadcrumbs');

    crumbs = crumbs !== null ? JSON.parse(crumbs) : [];

    if (
      crumbs.length === 0 ||
      !(
        crumbs[crumbs.length - 1].title == title &&
        crumbs[crumbs.length - 1].url == url
      )
    ) {
      j.title = title;
      j.url = url;

      if (
        crumbs.length > 0 &&
        crumbs[crumbs.length - 1].title.startsWith('Search') &&
        j.title.startsWith('Search')
      ) {
        crumbs.pop();
      }

      crumbs.push(j);
      sessionStorage.setItem('breadcrumbs', JSON.stringify(crumbs));
    }

    var el = document.getElementsByClassName('breadcrumb')[0];
    if (crumbs.length <= 1) return;
    el.innerHTML = buildcrumbs(crumbs).outerHTML;
    el.style.opacity = 1;
  };

  var buildcrumbs = function (crumbs) {
    var $ul = document.createElement('ul');

    crumbs = crumbs.slice(Math.max(crumbs.length - 7, 0));
    crumbs.reverse();

    for (var x = 0; x < crumbs.length; x++) {
      // if name is long, get a substring of it and add a tooltip.
      var $li = document.createElement('li');
      var $a = document.createElement('a');

      if (x == 0) {
        $a.setAttribute('href', '#');
        $li.classList.add('is-active');
      } else {
        $a.setAttribute('href', crumbs[x].url);
      }

      var $words = crumbs[x].title.split(' ');
      var $combined_words = '';

      for (var w = 0; w < $words.length; w++) {
        $combined_words += $words[w] + ' ';

        if ($combined_words.length > 15) {
          if (w < $words.length) {
            $combined_words += '…';
            $a.classList.add(
              'is-block',
              'has-tooltip-bottom',
              'has-tooltip-arrow',
            );
            $a.setAttribute('data-tooltip', crumbs[x].title);
          }
          break;
        }
      }

      $a.innerText = $combined_words;
      $li.appendChild($a);
      $ul.appendChild($li);
    }

    return $ul;
  };

  breadcrumbs();
})();
