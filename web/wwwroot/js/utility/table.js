(function () {
  const d = document;
  const tableSort = function () {
    setTimeout(function () {
      const t = d.querySelectorAll('table.sort');

      for (let x = 0; x < t.length; x++) {
        const element = t[x]; // Only add sort if it is not there already.

        if (element.querySelectorAll('th .icon-sm').length === 0) {
          const th = element.querySelectorAll('th');

          for (let q = 0; q < th.length; q++) {
            const h = th[q];
            h.style.cursor = 'pointer';
            h.innerHTML +=
              '<svg class="icon-sm" viewBox="0 0 320 512" xmlns="http://www.w3.org/2000/svg"><path d="M41 288h238c21.4 0 32.1 25.9 17 41L177 448c-9.4 9.4-24.6 9.4-33.9 0L24 329c-15.1-15.1-4.4-41 17-41zm255-105L177 64c-9.4-9.4-24.6-9.4-33.9 0L24 183c-15.1 15.1-4.4 41 17 41h238c21.4 0 32.1-25.9 17-41z"/></svg>';
            h.parentElement.style.whiteSpace = 'nowrap';
            (function (h) {
              h.addEventListener('click', function () {
                const table = this.closest('table.sort');
                let r = Array.from(table.querySelectorAll('tr'))
                  .slice(1)
                  .sort(comparer(index(h) - 1));
                this.asc = !this.asc;

                if (!this.asc) {
                  r = r.reverse();
                }

                for (let i = 0; i < r.length; i++) {
                  table.append(r[i]);
                }
              });
            })(h);
          }
        }
      }
    }, 0);
  };

  const comparer = function (index) {
    return function (a, b) {
      const valueA = getCellValue(a, index).replace('$', '');
      const valueB = getCellValue(b, index).replace('$', '');
      if (
        /^(?:\d{1,2}\/){2}\d{2,4}$/.test(valueA) &&
        /^(?:\d{1,2}\/){2}\d{2,4}$/.test(valueB)
      ) {
        return new Date(valueA) - new Date(valueB);
      }

      return isNumeric(valueA) && isNumeric(valueB)
        ? valueA - valueB
        : valueA.toString().localeCompare(valueB);
    };
  };

  const isNumeric = function (n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
  };

  const getCellValue = function (row, index) {
    return row.querySelectorAll('td')[index].textContent;
  };

  const index = function (element) {
    if (!element) return -1;
    let i = 0;
    while (element) {
      i++;
      element = element.previousElementSibling;
    }

    return i;
  };

  d.addEventListener('ajax', function () {
    tableSort();
  });
  tableSort();
})();
