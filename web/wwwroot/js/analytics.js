(function () {
  function bigNumber(text) {
    let n = parseInt(text, 10);
    const d = 10 ** 0;
    let i = 7;
    let s;
    while (i)
      // eslint-disable-next-line no-unused-expressions
      (s = 10 ** (i-- * 3)) <= n &&
        (n = Math.round((n * d) / s) / d + 'kMGTPE'[i]);
    return n;
  }

  const primaryChart = document.querySelector('#visits-chart');

  let primaryChartAjax = null;
  const config = {
    type: 'bar',
    options: {
      tooltips: {
        position: 'nearest',
        mode: 'label',
      },
      animation: {
        duration: 300,
      },
      hover: {
        animationDuration: 0,
      },
      responsive: true,
      responsiveAnimationDuration: 0,
      maintainAspectRatio: false,
      legend: {
        display: true,
        position: 'bottom',
        userPointStyle: true,
      },
      title: {
        display: false,
      },
      scales: {
        yAxes: [
          {
            id: '1',
            ticks: {
              beginAtZero: true,
              callback(value) {
                return bigNumber(value);
              },
            },
            stacked: false,
            type: 'linear',
            position: 'left',
          },
          {
            id: '2',
            ticks: {
              beginAtZero: true,
              callback(value) {
                return Math.round(value * 10) / 10 + 's';
              },
            },
            stacked: false,
            type: 'linear',
            position: 'right',
          },
        ],
        xAxes: [
          {
            stacked: true,
            gridLines: {
              display: false,
            },
          },
        ],
      },
    },
  };
  const ctx = document.querySelector('#visits-chart').getContext('2d');

  const activate = () => {
    // /* analytics page js for charts */

    window.visitsChart = new Chart(ctx, config);

    loadChart();
    loadBoxes();

    const {
      addHours,
      addDays,
      addYears,
      startOfDay,
      startOfWeek,
      startOfMonth,
      startOfYear,
      endOfDay,
      endOfWeek,
      endOfMonth,
      endOfYear,
      differenceInSeconds,
    } = require('date-fns');

    document
      .querySelectorAll(
        '.dropdown.is-select[data-target="views-chart"] .dropdown-item',
      )
      .forEach(($x) =>
        $x.addEventListener('click', (event) => {
          (
            event.target
              .closest('.dropdown')
              .querySelectorAll('.dropdown-item.is-active') || []
          ).forEach(($element) => {
            $element.classList.remove('is-active');
          });
          event.target.classList.add('is-active');
          const $target = event.target.closest(
            '.dropdown.is-select .dropdown-item',
          );

          $target
            .closest('.dropdown')
            .querySelector('.select-value').textContent = $target.textContent;

          /* Build date parameters */
          const now = new Date();

          let dataset;
          switch ($target.dataset.range) {
            case '1':
              // Today

              dataset =
                '?start_at=' +
                differenceInSeconds(startOfDay(now), now) +
                '&end_at=' +
                differenceInSeconds(endOfDay(now), now);

              break;

            case '3':
              // This week

              dataset =
                '?start_at=' +
                differenceInSeconds(startOfWeek(now, -24), now) +
                '&end_at=' +
                differenceInSeconds(endOfWeek(now), now);
              break;
            case '4':
              // Last 7 days

              dataset =
                '?start_at=' +
                differenceInSeconds(startOfDay(addDays(now, -7)), now) +
                '&end_at=0';
              break;
            case '5':
              // This month

              dataset =
                '?start_at=' +
                differenceInSeconds(startOfMonth(now), now) +
                '&end_at=' +
                differenceInSeconds(endOfMonth(now), now);
              break;
            case '6':
              // Last 30 days

              dataset =
                '?start_at=' +
                differenceInSeconds(startOfDay(addDays(now, -30)), now) +
                '&end_at=0';
              break;
            case '7':
              // Last 90 days

              dataset =
                '?start_at=' +
                differenceInSeconds(startOfDay(addDays(now, -90)), now) +
                '&end_at=0';
              break;
            case '8':
              // This year

              dataset =
                '?start_at=' +
                differenceInSeconds(startOfYear(now), now) +
                '&end_at=' +
                differenceInSeconds(endOfYear(now), now);
              break;
            case '9':
              // All time

              dataset =
                '?start_at=' +
                differenceInSeconds(addYears(now, -10), now) +
                '&end_at=0';
              break;
            case '10':
              // Custom range

              break;
            case '2':
            default:
              // Last 24 hours

              dataset =
                '?start_at=' +
                differenceInSeconds(addHours(now, -24), now) +
                '&end_at=0';

              break;
          }

          loadChart(dataset);
          loadBoxes(dataset);
          loadAjax(dataset);
        }),
      );
  };

  function buildDataTable(data) {
    if (data.length > 0) {
      const table = document.createElement('table');
      table.classList.add('table', 'is-no-border', 'is-fullwidth', 'bar');
      const header = document.createElement('tr');
      header.innerHTML = `<tr><th>${data[0].title_one}</th><th class="is-narrow">${data[0].title_two}</th></tr>`;
      table.append(header);
      data.forEach(($r) => {
        const row = document.createElement('tr');
        const $link = $r.href ? `<a href="${$r.href}">${$r.key}</a>` : $r.key;
        row.innerHTML = `<td class="is-middle">${$link}</td><td class="is-narrow has-text-right"><strong class="mr-2">${bigNumber(
          $r.count,
        )}</strong>`;
        if ($r.percent) {
          row.innerHTML += `<div class="bar-percent py-2 is-relative has-text-centered"><div class="bar-percent-fill" style="width:${
            $r.percent * 100
          }%"></div><span><strong class="has-text-grey-light">${Math.round(
            $r.percent * 100,
          )}%</strong></span>`;
        }

        row.innerHTML += `</td>`;
        table.append(row);
      });
      return table;
    }

    return document.createElement('span');
  }

  function loadAjax(parameters = '') {
    (document.querySelectorAll('.analytics[data-url]') || []).forEach(
      ($element) => {
        $element.setAttribute('data-parameters', parameters);
        $element.dispatchEvent(new CustomEvent('reload'));
      },
    );
  }

  function loadBoxes(parameters = '') {
    (
      document.querySelectorAll('.bar-data-wrapper.analytics[data-url]') || []
    ).forEach(($element) => {
      $element.style.opacity = '.5';
      const aj = new XMLHttpRequest();
      aj.open('get', $element.dataset.url + parameters.replace('?', '&'), true);
      aj.setRequestHeader(
        'Content-Type',
        'application/x-www-form-urlencoded; charset=UTF-8',
      );
      aj.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
      aj.send();

      aj.addEventListener('load', function () {
        $element.innerHTML = '';
        $element.append(buildDataTable(JSON.parse(aj.responseText)));
        $element.style.visibility = 'visible';
        $element.style.opacity = '1';
      });
    });
  }

  function loadChart(parameters = '') {
    primaryChart.style.opacity = '.5';

    const views = document.querySelector('#analytics-views');
    const visitors = document.querySelector('#analytics-visitors');
    const loadTime = document.querySelector('#analytics-load-time');

    views.style.opacity = '.5';
    visitors.style.opacity = '.5';
    loadTime.style.opacity = '.5';

    if (primaryChartAjax !== null) {
      primaryChartAjax.abort();
    }

    if (primaryChart.dataset.url.includes('?')) {
      parameters = parameters.replace('?', '&');
    }

    primaryChartAjax = new XMLHttpRequest();
    primaryChartAjax.open('get', primaryChart.dataset.url + parameters, true);
    primaryChartAjax.setRequestHeader(
      'Content-Type',
      'application/x-www-form-urlencoded; charset=UTF-8',
    );
    primaryChartAjax.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
    primaryChartAjax.send();

    primaryChartAjax.addEventListener('load', function () {
      const rep = JSON.parse(primaryChartAjax.responseText);

      if (views) {
        views.textContent = bigNumber(rep.views);
      }

      if (visitors) {
        visitors.textContent = bigNumber(rep.visitors);
      }

      if (loadTime) {
        loadTime.textContent = rep.load_time;
      }

      window.visitsChart.data = rep.data;
      window.visitsChart.update();
      primaryChart.style.opacity = '1';
      views.style.opacity = '1';
      visitors.style.opacity = '1';
      loadTime.style.opacity = '1';
    });
  }

  document.addEventListener('click', (event) => {
    if (
      (event.target.matches('.trace-resolved[type="checkbox"]') ||
        event.target.matches('.error-resolved[type="checkbox"]')) &&
      event.target.tagName === 'INPUT'
    ) {
      const i = event.target;
      let type = 1;

      if (i.hasAttribute('checked')) {
        i.removeAttribute('checked');
        type = 2;
        i.closest('tr.has-text-grey-light').classList.remove(
          'has-text-grey-light',
        );
        (
          i.closest('tr').querySelectorAll('a.has-text-grey-light') || []
        ).forEach(($i) => {
          $i.classList.remove('has-text-grey-light');
        });
        (i.closest('tr').querySelectorAll('div.notification') || []).forEach(
          ($i) => {
            $i.classList.add('is-danger');
          },
        );
      } else {
        i.setAttribute('checked', 'checked');
        i.closest('tr').classList.add('has-text-grey-light');
        (i.closest('tr').querySelectorAll('a') || []).forEach(($i) => {
          $i.classList.add('has-text-grey-light');
        });
        (
          i.closest('tr').querySelectorAll('div.notification.is-danger') || []
        ).forEach(($i) => {
          $i.classList.remove('is-danger');
        });
      }

      const data = {
        Id: i.dataset.id,
        Type: type,
      };
      const url = Object.keys(data)
        .map(function (k) {
          return encodeURIComponent(k) + '=' + encodeURIComponent(data[k]);
        })
        .join('&');
      const q = new XMLHttpRequest();
      q.open('post', i.dataset.url + '?handler=Resolved&' + url, true);
      q.setRequestHeader('Content-Type', 'text/html;charset=UTF-8`');
      q.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
      q.send();
    }
  });
  let test = 0;
  function load() {
    // Wait for chartjs to load.. basically 15 seconds max
    if (test === 300) {
      return false;
    }

    if (typeof Chart === 'undefined') {
      setTimeout(function () {
        test++;
        load();
      }, 100);
    } else {
      test = 0;
      activate();
    }
  }

  load();
})();
