(function () {
  var d = document,
    tableSort = function tableSort() {
    setTimeout(function () {
      var t = d.querySelectorAll('table.sort');

      for (var x = 0; x < t.length; x++) {
        var e = t[x]; // only add sort if it is not there already.

        if (!e.querySelectorAll('th .icon-sm').length) {
          var th = e.getElementsByTagName('th');

          for (var q = 0; q < th.length; q++) {
            var h = th[q];
            h.style.cursor = 'pointer';
            h.innerHTML += '<svg class="icon-sm" viewBox="0 0 320 512" xmlns="http://www.w3.org/2000/svg"><path d="M41 288h238c21.4 0 32.1 25.9 17 41L177 448c-9.4 9.4-24.6 9.4-33.9 0L24 329c-15.1-15.1-4.4-41 17-41zm255-105L177 64c-9.4-9.4-24.6-9.4-33.9 0L24 183c-15.1 15.1-4.4 41 17 41h238c21.4 0 32.1-25.9 17-41z"/></svg>';
            h.parentElement.style.whiteSpace = 'nowrap';
            (function(h) {
            h.addEventListener('click', function () {

              var table = this.closest('table.sort'),
                r = Array.from(table.querySelectorAll('tr')).slice(1).sort(comparer(index(h)-1));
              this.asc = !this.asc;

              if (!this.asc) {
                r = r.reverse();
              }

              for (var i = 0; i < r.length; i++) {
                table.append(r[i]);
              }
            });
          })(h);
          }
        }
      }
    }, 0);
  },
      comparer = function comparer(index) {
    return function (a, b) {
      var valA = getCellValue(a, index).replace('$', '');
      var valB = getCellValue(b, index).replace('$', '');
      if (/^\d{1,2}\/\d{1,2}\/\d{2,4}$/.test(valA) && /^\d{1,2}\/\d{1,2}\/\d{2,4}$/.test(valB)) {
        return new Date(valA) - new Date(valB);
      } else {
        return isNumeric(valA) && isNumeric(valB) ? valA - valB : valA.toString().localeCompare(valB);
      }
    };
  },
      isNumeric = function isNumeric(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
  },
      getCellValue = function getCellValue(row, index) {
    return row.getElementsByTagName('td')[index].textContent;
  },
      index = function index(el) {
    if (!el) return -1;
    var i = 0;
    while(el) {
      i++;
      el = el.previousElementSibling;
    }
    return i;
  };

  document.addEventListener('ajax', function () {
    tableSort();
  });
  tableSort();
})();