(function () {

  // add page location to breadcrumbs when leaving page

  window.onbeforeunload = function(){
    setTimeout(function () {
      breadcrumbs();
    }, 0);
  }

  var breadcrumbs = function() {
    var title = document.title.indexOf(" | ") != -1 ? document.title.split(" | ")[0] : document.title,
      url = window.location.href,
      j = {},
      crumbs = sessionStorage.getItem("breadcrumbs");

    crumbs = crumbs !== null ? JSON.parse(crumbs) : [];

    if (crumbs.length === 0 || !(crumbs[crumbs.length - 1].title == title && crumbs[crumbs.length - 1].url == url)) {
      j.title = title;
      j.url = url;

      if (
        crumbs.length > 0 &&
        crumbs[crumbs.length - 1].title.startsWith("Search") &&
        j.title.startsWith("Search")
      ) {
        crumbs.pop();
      }

      crumbs.push(j);
      sessionStorage.setItem("breadcrumbs", JSON.stringify(crumbs));
    }

    var el = document.getElementsByClassName("breadcrumb")[0];
    if (crumbs.length <= 1) return
    el.innerHTML = '<ul>' + buildcrumbs(crumbs) + '</ul>';
    el.style.opacity = 1;
  };

  var buildcrumbs = function (crumbs) {
    var x = 0,
      l = "";

    crumbs = crumbs.slice(Math.max(crumbs.length - 7, 0));
    crumbs.reverse();

    l = "<li class='is-active'><a href='#'>" + crumbs[0].title + "</a></li>";
    for (x = 1; x < crumbs.length; x++) {
      l += '<li><a href="' + crumbs[x].url + '" class="">' + crumbs[x].title + '</a></li>';
    }

    return l;
  };

  breadcrumbs();
})();
