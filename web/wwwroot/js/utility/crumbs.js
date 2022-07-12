(function () {
  // Add page location to breadcrumbs when leaving page

  // window.onbeforeunload = function () {
  //   setTimeout(function () {
  //     breadcrumbs();
  //   }, 0);
  // };

  const breadcrumbs = function () {
    const element = document.querySelector('.breadcrumb.site-breadcrumbs');

    if (element === null) {
      return -1;
    }

    const title = document.title.includes(' | ')
      ? document.title.split(' | ')[0]
      : document.title;
    const url = window.location.href;
    const j = {};
    let crumbs = sessionStorage.getItem('breadcrumbs');

    crumbs = crumbs === null ? [] : JSON.parse(crumbs);

    if (
      crumbs.length === 0 ||
      !(
        crumbs[crumbs.length - 1].title === title &&
        crumbs[crumbs.length - 1].url === url
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

    if (crumbs.length <= 1) return;
    element.innerHTML = DOMPurify.sanitize(buildcrumbs(crumbs).outerHTML);
    element.style.opacity = 1;
  };

  function buildcrumbs(crumbs) {
    const $ul = document.createElement('ul');
    $ul.classList.add('mt-4');

    crumbs = crumbs.slice(Math.max(crumbs.length - 7, 0));
    crumbs.reverse();

    for (let x = 0; x < crumbs.length; x++) {
      // If name is long, get a substring of it and add a tooltip.
      const $li = document.createElement('li');
      const $a = document.createElement('a');

      if (x === 0) {
        $a.setAttribute('href', '#');
        $li.classList.add('is-active');
      } else {
        $a.setAttribute('href', crumbs[x].url);
      }

      const $words = crumbs[x].title.split(' ');
      let $combinedWords = '';

      for (let w = 0; w < $words.length; w++) {
        $combinedWords += $words[w] + ' ';

        if ($combinedWords.length > 15) {
          if (w < $words.length) {
            $combinedWords += '…';
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

      $a.textContent = $combinedWords;
      $li.append($a);
      $ul.append($li);
    }

    return $ul;
  }

  breadcrumbs();
})();
