const { ja, tr } = require('date-fns/locale');

(function () {
  function bigNumber(text) {
    var n = parseInt(text),
      p = Math.pow,
      d = p(10, 0),
      i = 7,
      s;
    while (i)
      (s = p(10, i-- * 3)) <= n &&
        (n = Math.round((n * d) / s) / d + 'kMGTPE'[i]);
    return n;
  }

  var primaryChart = document.getElementById('visits-chart');
  document.querySelector('null').addEventListener('click', function () {
    console.log('error');
  });
  var primaryChartAjax = null;
  var config = {
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
              callback: function (value, index, values) {
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
              callback: function (value, index, values) {
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
  var ctx = document.getElementById('visits-chart').getContext('2d');

  var activate = () => {
    // /* analytics page js for charts */

    window.visits_chart = new Chart(ctx, config);

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
        $x.addEventListener('click', ($e) => {
          (
            $e.target
              .closest('.dropdown')
              .querySelectorAll('.dropdown-item.is-active') || []
          ).forEach(($el) => {
            $el.classList.remove('is-active');
          });
          $e.target.classList.add('is-active');
          var $target = $e.target.closest('.dropdown.is-select .dropdown-item');

          $target
            .closest('.dropdown')
            .querySelector('.select-value').innerText = $target.innerText;

          /* build date parameters */
          let now = new Date();

          var dataset;
          switch ($target.dataset.range) {
            case '1':
              // today

              dataset =
                '?start_at=' +
                differenceInSeconds(startOfDay(now), now) +
                '&end_at=' +
                differenceInSeconds(endOfDay(now), now);

              break;
            default:
            case '2':
              // last 24 hours

              dataset =
                '?start_at=' +
                differenceInSeconds(addHours(now, -24), now) +
                '&end_at=0';

              break;
            case '3':
              // this week

              dataset =
                '?start_at=' +
                differenceInSeconds(startOfWeek(now, -24), now) +
                '&end_at=' +
                differenceInSeconds(endOfWeek(now), now);
              break;
            case '4':
              // last 7 days

              dataset =
                '?start_at=' +
                differenceInSeconds(startOfDay(addDays(now, -7)), now) +
                '&end_at=0';
              break;
            case '5':
              // this month

              dataset =
                '?start_at=' +
                differenceInSeconds(startOfMonth(now), now) +
                '&end_at=' +
                differenceInSeconds(endOfMonth(now), now);
              break;
            case '6':
              //last 30 days

              dataset =
                '?start_at=' +
                differenceInSeconds(startOfDay(addDays(now, -30)), now) +
                '&end_at=0';
              break;
            case '7':
              //last 90 days

              dataset =
                '?start_at=' +
                differenceInSeconds(startOfDay(addDays(now, -90)), now) +
                '&end_at=0';
              break;
            case '8':
              // this year

              dataset =
                '?start_at=' +
                differenceInSeconds(startOfYear(now), now) +
                '&end_at=' +
                differenceInSeconds(endOfYear(now), now);
              break;
            case '9':
              // all time

              dataset =
                '?start_at=' +
                differenceInSeconds(addYears(now, -10), now) +
                '&end_at=0';
              break;
            case '10':
              // custom range

              break;
          }

          loadChart(dataset);
          loadBoxes(dataset);
        }),
      );
  };
  function buildDataTable(data) {
    if (data.length) {
      var table = document.createElement('table');
      table.classList.add('table', 'is-no-border', 'is-fullwidth', 'bar');
      var header = document.createElement('tr');
      header.innerHTML = `<tr><th>${data[0].title_one}</th><th class="is-narrow">${data[0].title_two}</th></tr>`;
      table.appendChild(header);
      data.forEach(($r) => {
        var row = document.createElement('tr');
        var $link = $r.href ? `<a href="${$r.href}">${$r.key}</a>` : $r.key;
        row.innerHTML = `<td class="is-middle">${$link}</td><td class="is-narrow has-text-right"><strong class="mr-2">${bigNumber(
          $r.count,
        )}</strong>`;
        if ($r.percent) {
          row.innerHTML += `<div class="bar-percent py-2 is-relative has-text-centered"><div class="bar-percent-fill" style="width:${
            $r.percent * 100
          }%"></div><span><strong>${Math.round(
            $r.percent * 100,
          )}%</strong></span>`;
        }
        row.innerHTML += `</td>`;
        table.appendChild(row);
      });
      return table;
    } else {
      return document.createElement('span');
    }
  }
  function loadBoxes(params = '') {
    (document.querySelectorAll('.bar-data-wrapper[data-url]') || []).forEach(
      ($el) => {
        $el.style.opacity = '.5';
        var aj = new XMLHttpRequest();
        aj.open('get', $el.dataset.url + params.replace('?', '&'), true);
        aj.setRequestHeader(
          'Content-Type',
          'application/x-www-form-urlencoded; charset=UTF-8',
        );
        aj.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
        aj.send();

        aj.onload = function () {
          $el.innerHTML = '';
          $el.appendChild(buildDataTable(JSON.parse(aj.responseText)));
          $el.style.visibility = 'visible';
          $el.style.opacity = '1';
        };
      },
    );
  }

  function loadChart(params = '') {
    if (primaryChartAjax !== null) {
      primaryChartAjax.abort();
    }

    primaryChartAjax = new XMLHttpRequest();
    primaryChartAjax.open('get', primaryChart.dataset.url + params, true);
    primaryChartAjax.setRequestHeader(
      'Content-Type',
      'application/x-www-form-urlencoded; charset=UTF-8',
    );
    primaryChartAjax.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
    primaryChartAjax.send();

    primaryChartAjax.onload = function () {
      var rep = JSON.parse(primaryChartAjax.responseText);

      var views = document.getElementById('analytics-views');
      if (views) {
        views.innerText = bigNumber(rep.views);
      }
      var visitors = document.getElementById('analytics-visitors');
      if (visitors) {
        visitors.innerText = bigNumber(rep.visitors);
      }
      var load_time = document.getElementById('analytics-load-time');
      if (load_time) {
        load_time.innerText = rep.load_time;
      }
      window.visits_chart.data = rep.data;
      window.visits_chart.update();
    };
  }

  var test = 0;
  function load() {
    // wait for chartjs to load.. basically 15 seconds max
    if (test == 300) {
      return false;
    } else if (typeof Chart == 'undefined') {
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
