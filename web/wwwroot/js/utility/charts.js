(function () {
  /* A = {
      axis: {
        0: {
          title: 'hits'
        },
        1: {
          title: 'seconds'
        },
        2: {
          title: 'clicks'
        }
      },
      type: 'bar',
      height: 400,
      title: "Cool Chart",
      data: [{
        title: "Run Time (s)",
        axis: 0,
        type: "bar",
        units: "(s)",
        data: [{
          title: "Stack 1",
          data: 12
        }]
      }, {
        title: "Runs",
        type: "bar",
        axis: 1,
        data: [{
          title: "Stack 1",
          data: 641
        }],
      }, {
        title: "Clicks",
        type: "line",
        axis: 2,
        data: [{
          title: "Stack 1",
          data: 61
        }],
      }],
      options: {
        legend: false
      }
    };
    */

  const d = document;
  const l = debounce(function () {
    setTimeout(function () {
      const dest = document.querySelectorAll('.atlas-chart');
      for (let element = 0; element < dest.length; element++) {
        const a = dest[element].querySelector('script').innerHTML;
        if (dest[element].querySelectorAll('chart').length > 0) {
          dest[element].querySelector('chart').remove();
        }

        if (dest[element].querySelectorAll('ajaxLoader').length > 0) {
          dest[element].querySelector('ajaxLoader').remove();
        }

        const w = dest[element].clientWidth;

        if (w > 0) dest[element].append(new Build(a, w));
      }
    }, 0);
  }, 250);
  const k =
    d.requestAnimationFrame ||
    d.setImmediate ||
    function (b) {
      return setTimeout(b, 0);
    };

  const Build = function (json, w) {
    let chart = '';
    try {
      chart = JSON.parse(json);
    } catch {}

    const height = chart.height || 400;
    const options = chart.options;
    const charts = chart.data;
    const axis = chart.axis;
    let html = '';
    const cWidth = w || 800;
    let c;
    let q;
    let thisChart;
    let style;
    let side;
    const axii = [];
    let cols = 0;
    const range = {};
    let a;
    let bars = 0;
    const numbers = ['one', 'two', 'three', 'four', 'five'];

    this.table = document.createElement('table');

    for (c in charts) {
      if (!axii.includes(charts[c].axis)) {
        axii.push(charts[c].axis);
      }
    }

    const // Width = cWidth - 75 * axii.length, // enough padding for chart axis
      marLeft = (Math.floor(axii.length / 2) + (axii.length % 2)) * 70;
    const marRight = axii.length > 1 ? Math.floor(axii.length / 2) * 70 : 40;
    const width = cWidth - marLeft - marRight;
    this.table.classList.add('chart');
    this.table.style.height = height + 'px';
    this.table.style.marginRight = marRight + 'px';
    this.table.style.marginLeft = marLeft + 'px';
    this.table.style.width = width + 'px';

    this.table.innerHTML +=
      '<caption class="chart-caption">' +
      DOMPurify.sanitize(chart.title) +
      '</caption>';

    if (
      (Boolean(options) &&
        options.hasOwnproperty('legend') &&
        options.legend !== 'false') ||
      !options
    ) {
      this.thead = document.createElement('thead');
      this.thead.classList.add('chart-head');

      let th;
      for (c in charts) {
        th = document.createElement('th');
        th.innerHTML +=
          '<span class="chart-headClr ' +
          DOMPurify.sanitize(numbers[c]) +
          '"></span>' +
          DOMPurify.sanitize(charts[c].title);
        this.thead.append(th);
      }

      this.table.append(this.thead);
    }

    this.table.addEventListener('mouseover', this.mouseover.bind(this));
    this.table.addEventListener('mouseout', this.mouseout.bind(this));

    /* End of table head */

    for (c in charts) {
      cols = Math.max(cols, charts[c].data.length);
    }

    const colWidth = (width / cols) * 100; // As %

    // create range for axis
    for (a in axii) {
      let maxValue = 0;
      let minValue = 0;

      for (c in charts) {
        if (charts[c].axis !== axii[a]) continue;

        thisChart = charts[c];

        for (q in thisChart.data) {
          maxValue = Math.max(maxValue, parseFloat(thisChart.data[q].data));
          minValue = Math.min(minValue, parseFloat(thisChart.data[q].data));
        }
      }

      // Update charts w/ numbers for later use
      for (c in charts) {
        if (charts[c].axis === axii[a]) {
          charts[c].range = maxValue - minValue;
          charts[c].max = maxValue;
          charts[c].min = minValue;
        }
      }

      range[a] = maxValue - minValue;
    }

    // # of bar charts
    for (c in charts) {
      chart.lastlinePointY = null;
      chart.lastlinePointX = null;
      bars += charts[c].type === 'bar' ? 1 : 0;
    }

    /* Table body */
    html = '<tbody>';
    for (let d = 0; d < cols; d++) {
      // Tooltip
      let tooltip = charts[0].data[d].title + ': ';
      for (c in charts) {
        tooltip +=
          charts[c].data[d].data + ' ' + axis[charts[c].axis].title + '; ';
      }

      // Create point element
      html +=
        '<tr class="chart-col" style="left:' +
        colWidth * d +
        '%;width:calc(' +
        colWidth +
        '% - 10px);">';
      let barnum = 0;
      let maxHeight = 0;
      for (c in charts) {
        thisChart = charts[c];

        const lheight =
          parseFloat(thisChart.data[d].data) === 0
            ? 0
            : (parseFloat(thisChart.data[d].data) / thisChart.range) * 100;
        maxHeight = Math.max(maxHeight, lheight);
        const lwidth = 'calc(' + 100 / bars + '% - 5px)';

        if (Number(c) === 0)
          html +=
            '<th class="chart-xAxisTitle" scope="row" style="left:' +
            (-26 + ((colWidth / 100) * width) / 2 - 15) +
            'px">' +
            thisChart.data[d].title;

        const p =
          (lheight / 100) * height < 30 ? 'style="margin-top:-22px"' : '';
        if (thisChart.type === 'bar') {
          html +=
            '<td class="chart-' +
            thisChart.type +
            ' ' +
            numbers[c] +
            '" style="height:' +
            lheight +
            '%;width:' +
            lwidth +
            ';left:' +
            (barnum / bars) * 100 +
            '%"><p ' +
            p +
            '></p></td>';
        } else if (thisChart.type === 'line') {
          html +=
            '<td class="chart-' +
            thisChart.type +
            ' ' +
            numbers[c] +
            '" style="height:' +
            lheight +
            '%">';
          html += '<p ' + p + '></p>';
          if (
            Boolean(thisChart.lastlinePointY) ||
            thisChart.lastlinePointY === 0
          ) {
            w = (colWidth / 100) * width;
            const angle =
              (Math.atan2(
                (lheight / 100) * height - thisChart.lastlinePointY,
                -w,
              ) *
                180) /
              Math.PI;
            const distance = Math.sqrt(
              w ** 2 +
                ((lheight / 100) * height - thisChart.lastlinePointY) ** 2,
            );
            html +=
              '<div style="width:' +
              distance +
              'px;transform: rotate(' +
              angle +
              'deg);"></div>';
          }

          thisChart.lastlinePointY = (lheight / 100) * height;
        }

        if (thisChart.type === 'bar') barnum++;
        html += '</td>';
      }

      html +=
        '<td class="chart-tooltip" data-tooltip="' +
        tooltip +
        '" style="height:calc(' +
        maxHeight +
        '% + 20px)"></td>';
      html += '</tr>';
    }

    this.table.innerHTML += DOMPurify.sanitize(html) + '</tbody>';
    // Build grid lines. scale is every 50 px

    // titles
    html = '<caption class="chart-ticks">';
    for (c in axii) {
      style = (Math.floor(c / 2) + 1) * 55;
      side = c % 2 === 1 ? 'right' : 'left';
      if (typeof axis[a] !== 'undefined') {
        html +=
          '<div class="chart-yAxisTitle ' +
          side +
          '" style="' +
          side +
          ':-' +
          style +
          'px"><p>' +
          axis[c].title +
          '</p></div>';
      }
    }

    // Lines
    const ticks = parseInt(height / 50, 10);
    for (c = 1; c <= ticks; c++) {
      html +=
        '<div class="chart-ticksTick' +
        (c === 1 ? ' first ' : '') +
        (c === ticks ? ' last ' : '') +
        '">';
      for (q in range) {
        style = (Math.floor(q / 2) + 1) * 30;
        side = q % 2 === 1 ? 'right' : 'left';
        if (range[q] > 0) {
          let value = range[q] - (range[q] / ticks) * (c - 1);
          value = value < 10 ? value.toFixed(2) : Math.round(value);

          if (side === 'left') {
            style += parseFloat((value.toString().length - 1) * 3);
          }

          html += '<p style="' + side + ':-' + style + 'px">' + value + '</p>';
        }
      }

      html += '</div>';
    }

    html += '</table>';

    this.table.innerHTML += '</tbody>' + DOMPurify.sanitize(html);

    return this.table;
  };

  Build.prototype = {
    mouseover() {
      // eslint-disable-next-line no-restricted-globals
      const t = event.target;

      k(function () {
        // Remove all hover class
        const hover = Array.prototype.slice.call(
          t.closest('table').querySelectorAll('.hover'),
        );
        for (let x = 0; x < hover.length; x++)
          hover[x].classList.remove('hover');

        // Ie 11 does not support classlist.

        if (
          t.closest('.chart-head th') &&
          t.closest('.chart-head th').querySelectorAll('span') &&
          t.closest('.chart-head th').querySelectorAll('span')[0].classList
            .value !== undefined
        ) {
          const lines = Array.prototype.slice.call(
            t
              .closest('table')
              .getElementsByClassName(
                'chart-line ' +
                  t
                    .closest('.chart-head th')
                    .querySelectorAll('span')[0]
                    .classList.value.replace('chart-headClr', ''),
              ),
          );
          for (let xOne = 0; xOne < lines.length; xOne++)
            lines[xOne].classList.add('hover');
          const bars = Array.prototype.slice.call(
            t
              .closest('table')
              .getElementsByClassName(
                'chart-bar ' +
                  t
                    .closest('.chart-head th')
                    .querySelectorAll('span')[0]
                    .classList.value.replace('chart-headClr', ''),
              ),
          );
          for (let xTtwo = 0; xTtwo < bars.length; xTtwo++)
            bars[xTtwo].classList.add('hover');
        }
      });
    },
    mouseout() {
      // eslint-disable-next-line no-restricted-globals
      const t = event.target;

      k(function () {
        const hover = Array.prototype.slice.call(
          t.closest('table').querySelectorAll('.hover'),
        );
        for (let x = 0; x < hover.length; x++)
          hover[x].classList.remove('hover');
      });
    },
  };
  l();

  d.addEventListener('load-charts', function () {
    l();
  });
  d.addEventListener('mdl-open', function () {
    l();
  });
  d.addEventListener('tab-opened', function () {
    l();
  });
  d.addEventListener('collapse-opened', function () {
    l();
  });
  window.addEventListener('resize', function () {
    l();
  });
})();
