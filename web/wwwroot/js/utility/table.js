(function () {
  const d = document;
  const tableSort = function () {
    setTimeout(function () {
      const t = d.querySelectorAll('table.sort');

      for (let x = 0; x < t.length; x++) {
        const element = t[x]; // Only add sort if it is not there already.

        if (element.querySelectorAll('th .icon').length === 0) {
          const th = element.querySelectorAll('th');

          for (let q = 0; q < th.length; q++) {
            const h = th[q];
            h.style.cursor = 'pointer';
            h.innerHTML +=
              '<span class="icon is-small"><i class="fas fa-sort"></i></span>';
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
