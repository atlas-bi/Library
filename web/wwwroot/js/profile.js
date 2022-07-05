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

  function formatThousandsWithRounding(n, dp) {
    // https://stackoverflow.com/a/20545587/10265880
    const w = Number(n).toFixed(Number(dp));
    const k = Math.trunc(w);
    const b = n < 0 ? 1 : 0;
    const u = Math.abs(w - k);
    const d = String(u.toFixed(Number(dp))).slice(2, 2 + Number(dp));
    const s = String(k);
    let i = s.length;
    let r = '';
    while ((i -= 3) > b) {
      r = ',' + s.slice(i, 3) + r;
    }

    return s.slice(0, Math.max(0, i + 3)) + r + (d ? '.' + d : '');
  }

  const isInViewport = function (element) {
    const bounding = element.getBoundingClientRect();
    const padding = 600;
    return (
      bounding.top >= 0 - padding &&
      bounding.left >= 0 &&
      bounding.bottom - element.clientHeight - padding <=
        (document.documentElement.clientHeight ||
          document.documentElement.clientHeight) &&
      bounding.right - padding - element.clientWidth <=
        (document.documentElement.clientWidth ||
          document.documentElement.clientWidth) &&
      // Is visible
      element.offsetParent !== null
    );
  };

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
      (
        document.querySelectorAll(
          '.profile.chart-wrapper > canvas:not(.loaded)',
        ) || []
      ).forEach((element) => {
        if (isInViewport(element.closest('.profile.chart-wrapper'))) {
          new ChartLoader(element);
        }
      });
    }
  }

  load();

  document.addEventListener('ajax', function () {
    setTimeout(function () {
      load();
    }, 0);
  });

  document.addEventListener('modal-open', function () {
    load();
  });

  document.addEventListener('tab-opened', function () {
    load();
  });

  document.addEventListener(
    'scroll',
    function () {
      debounce(load(), 100);
    },
    {
      passive: true,
    },
  );

  function ChartLoader(chart) {
    chart.classList.add('loaded');
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

    const ChartJsChart = new Chart(chart.getContext('2d'), config);

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
        `.dropdown.is-select[data-target="${chart.id}"] .dropdown-item`,
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
            case '1':
              // Yesterday
              dataset =
                '?start_at=' +
                differenceInSeconds(startOfDay(addHours(now, -24)), now) +
                '&end_at=' +
                differenceInSeconds(endOfDay(addHours(now, -24)), now);

              break;
            case '11':
            default:
              // Last 12 months
              dataset =
                '?start_at=' +
                differenceInSeconds(addYears(now, -1), now) +
                '&end_at=0';
              break;
          }

          loadChart(dataset);
          loadBoxes(dataset);
          loadAjax(dataset);
          loadFilters(dataset);
        }),
      );

    document.addEventListener('click', (event) => {
      if (event.target.closest('.profile-filter input[type=checkbox]')) {
        const element = event.target.closest(
          '.profile-filter input[type=checkbox]',
        );

        const parameters = element.checked
          ? chart.dataset.parameters + `&${element.dataset.filter}`
          : chart.dataset.parameters.replace(`&${element.dataset.filter}`, '');

        loadChart(parameters);
        loadFilters(parameters);
        loadBoxes(parameters);
      }
    });

    function loadFilters(parameters = '') {
      (document.querySelectorAll('.filters[data-url]') || []).forEach(
        ($element) => {
          $element.setAttribute('data-parameters', parameters);
          $element.dispatchEvent(new CustomEvent('reload'));
        },
      );
    }

    function buildDataTable(data) {
      if (data.length > 0) {
        const table = document.createElement('table');
        table.classList.add(
          'table',
          'is-no-border',
          'is-fullwidth',
          'bar',
          'sort',
        );
        const header = document.createElement('tr');
        const dateHeader =
          'date_title' in data[0] ? `<th>${data[0].date_title}</th>` : '';
        header.innerHTML = `<tr><th>${data[0].title_one}</th>${dateHeader}<th class="is-narrow">${data[0].title_two}</th></tr>`;
        table.append(header);

        // Check for date column

        data.forEach(($r) => {
          const row = document.createElement('tr');
          const $link = $r.href ? `<a href="${$r.href}">${$r.key}</a>` : $r.key;
          const dateValue =
            'date_title' in data[0]
              ? 'date' in $r
                ? `<td>${$r.date}</td>`
                : '<td></td>'
              : '';
          row.innerHTML = `<td class="is-middle">${$link}</td>${dateValue}<td class="is-narrow has-text-right"><strong class="mr-2">${bigNumber(
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
      (document.querySelectorAll('.profile[data-url]') || []).forEach(
        ($element) => {
          $element.setAttribute('data-parameters', parameters);
          $element.dispatchEvent(new CustomEvent('reload'));
        },
      );
    }

    function loadBoxes(parameters = undefined) {
      (
        document.querySelectorAll(
          `.bar-data-wrapper.profile[data-url][data-target="${chart.id}"]`,
        ) || []
      ).forEach(($element) => {
        $element.style.opacity = '.5';
        if (parameters !== undefined) {
          $element.setAttribute(
            'data-parameters',
            parameters.replace('?', '&'),
          );
        }

        const aj = new XMLHttpRequest();
        aj.open(
          'get',
          $element.dataset.url +
            ($element.dataset.parameters || '').replace('?', '&'),
          true,
        );
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
          document.dispatchEvent(new CustomEvent('ajax'));
        });
      });
    }

    function loadChart(parameters = undefined) {
      chart.style.opacity = '.5';
      const runs = document.querySelector(`#${chart.id}-runs`);
      const users = document.querySelector(`#${chart.id}-users`);
      const runTime = document.querySelector(`#${chart.id}-run-time`);

      runs.style.opacity = '.5';
      users.style.opacity = '.5';
      runTime.style.opacity = '.5';

      if (primaryChartAjax !== null) {
        primaryChartAjax.abort();
      }

      if (parameters === undefined) {
        parameters = '';
      } else {
        if (chart.dataset.url.includes('?')) {
          parameters = parameters.replace('?', '&');
        }

        chart.setAttribute('data-parameters', parameters);
      }

      primaryChartAjax = new XMLHttpRequest();
      primaryChartAjax.open(
        'get',
        chart.dataset.url + (chart.dataset.parameters || ''),
        true,
      );
      primaryChartAjax.setRequestHeader(
        'Content-Type',
        'application/x-www-form-urlencoded; charset=UTF-8',
      );
      primaryChartAjax.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
      primaryChartAjax.send();

      primaryChartAjax.addEventListener('load', function () {
        const rep = JSON.parse(primaryChartAjax.responseText);

        if (runs) {
          runs.textContent = bigNumber(rep.runs);
          runs.parentElement.setAttribute(
            'data-tooltip',
            `${formatThousandsWithRounding(rep.runs, 0)} runs`,
          );
        } else {
          runs.parentElement.removeAttribute('data-tooltip');
        }

        if (users) {
          users.textContent = bigNumber(rep.users);
          users.parentElement.setAttribute(
            'data-tooltip',
            `${formatThousandsWithRounding(rep.users, 0)} users`,
          );
        } else {
          users.parentElement.removeAttribute('data-tooltip');
        }

        if (runTime) {
          runTime.textContent = rep.run_time;
          runTime.parentElement.setAttribute(
            'data-tooltip',
            `${formatThousandsWithRounding(rep.run_time, 2)} seconds`,
          );
        } else {
          runTime.parentElement.removeAttribute('data-tooltip');
        }

        ChartJsChart.data = rep.data;
        ChartJsChart.update();
        chart.style.opacity = '1';
        runs.style.opacity = '1';
        users.style.opacity = '1';
        runTime.style.opacity = '1';
      });
    }
  }
})();
