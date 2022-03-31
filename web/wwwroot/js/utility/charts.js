(function () {
  /*a = {
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

  var d = document,
    l = debounce(function () {
      setTimeout(function () {
        var dest = document.getElementsByClassName('atlas-chart');
        for (var el = 0; el < dest.length; el++) {
          var a = dest[el].getElementsByTagName('script')[0].innerHTML;
          if (dest[el].getElementsByClassName('chart').length > 0) {
            dest[el].removeChild(dest[el].getElementsByClassName('chart')[0]);
          }
          if (dest[el].getElementsByClassName('ajaxLoader').length > 0) {
            dest[el].removeChild(
              dest[el].getElementsByClassName('ajaxLoader')[0],
            );
          }

          var w = dest[el].clientWidth;

          if (w > 0) dest[el].appendChild(new build(a, w));
        }
      }, 0);
    }, 250),
    k =
      d.requestAnimationFrame ||
      d.setImmediate ||
      function (b) {
        return setTimeout(b, 0);
      },
    build = function (json, w) {
      var chart = '';
      try {
        chart = JSON.parse(json);
      } catch (e) {}
      var height = chart.height || 400,
        options = chart.options,
        charts = chart.data,
        axis = chart.axis,
        html = '',
        cWidth = w || 800,
        c,
        q,
        thisChart,
        style,
        side,
        axii = [],
        cols = 0,
        range = {},
        a,
        bars = 0,
        colWidth,
        numbers = ['one', 'two', 'three', 'four', 'five'];

      this.table = document.createElement('table');

      for (c in charts) {
        if (axii.indexOf(charts[c].axis) == -1) {
          axii.push(charts[c].axis);
        }
      }

      var //width = cWidth - 75 * axii.length, // enough padding for chart axis
        marLeft = (Math.floor(axii.length / 2) + (axii.length % 2)) * 70,
        marRight = axii.length > 1 ? Math.floor(axii.length / 2) * 70 : 40,
        width = cWidth - marLeft - marRight;
      this.table.classList.add('chart');
      this.table.style.height = height + 'px';
      this.table.style.marginRight = marRight + 'px';
      this.table.style.marginLeft = marLeft + 'px';
      this.table.style.width = width + 'px';

      this.table.innerHTML +=
        '<caption class="chart-caption">' + chart.title + '</caption>';

      if (
        (!!options &&
          options.hasOwnproperty('legend') &&
          options.legend !== 'false') ||
        !options
      ) {
        this.thead = document.createElement('thead');
        this.thead.classList.add('chart-head');

        var th;
        for (c in charts) {
          th = document.createElement('th');
          th.innerHTML +=
            '<span class="chart-headClr ' +
            numbers[c] +
            '"></span>' +
            charts[c].title;
          this.thead.appendChild(th);
        }
        this.table.appendChild(this.thead);
      }

      this.table.addEventListener('mouseover', this.mouseover.bind(this));
      this.table.addEventListener('mouseout', this.mouseout.bind(this));

      /* end of table head */

      for (c in charts) {
        cols = Math.max(cols, charts[c].data.length);
      }
      colWidth = (width / width / cols) * 100; // as %

      // create range for axis
      for (a in axii) {
        var maxValue = 0,
          minValue = 0;

        for (c in charts) {
          if (charts[c].axis !== axii[a]) continue;

          thisChart = charts[c];

          for (q in thisChart.data) {
            maxValue = Math.max(maxValue, parseFloat(thisChart.data[q].data));
            minValue = Math.min(minValue, parseFloat(thisChart.data[q].data));
          }
        }

        // update charts w/ numbers for later use
        for (c in charts) {
          if (charts[c].axis == axii[a]) {
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
        bars += charts[c].type == 'bar' ? 1 : 0;
      }

      /* table body */
      html = '<tbody>';
      for (var d = 0; d < cols; d++) {
        // tooltip
        var tooltip = charts[0].data[d].title + ': ';
        for (c in charts) {
          tooltip +=
            charts[c].data[d].data + ' ' + axis[charts[c].axis].title + '; ';
        }

        // create point element
        html +=
          '<tr class="chart-col" style="left:' +
          colWidth * d +
          '%;width:calc(' +
          colWidth +
          '% - 10px);">';
        var barnum = 0;
        var maxHeight = 0;
        for (c in charts) {
          thisChart = charts[c];

          var lheight =
            parseFloat(thisChart.data[d].data) === 0
              ? 0
              : (parseFloat(thisChart.data[d].data) / thisChart.range) * 100;
          maxHeight = Math.max(maxHeight, lheight);
          var lwidth = 'calc(' + 100 / bars + '% - 5px)';

          if (c === 0)
            html +=
              '<th class="chart-xAxisTitle" scope="row" style="left:' +
              (-26 + ((colWidth / 100) * width) / 2 - 15) +
              'px">' +
              thisChart.data[d].title;

          var p =
            (lheight / 100) * height < 30 ? 'style="margin-top:-22px"' : '';
          if (thisChart.type == 'bar') {
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
          } else if (thisChart.type == 'line') {
            html +=
              '<td class="chart-' +
              thisChart.type +
              ' ' +
              numbers[c] +
              '" style="height:' +
              lheight +
              '%">';
            html += '<p ' + p + '></p>';
            if (!!thisChart.lastlinePointY || thisChart.lastlinePointY === 0) {
              w = (colWidth / 100) * width;
              var angle =
                (Math.atan2(
                  (lheight / 100) * height - thisChart.lastlinePointY,
                  -w,
                ) *
                  180) /
                Math.PI;
              var distance = Math.sqrt(
                Math.pow(w, 2) +
                  Math.pow(
                    (lheight / 100) * height - thisChart.lastlinePointY,
                    2,
                  ),
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
          if (thisChart.type == 'bar') barnum++;
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
      this.table.innerHTML += html + '</tbody>';
      // build grid lines. scale is every 50 px

      // titles
      html = '<caption class="chart-ticks">';
      for (c in axii) {
        style = (Math.floor(c / 2) + 1) * 55;
        side = c % 2 === 1 ? 'right' : 'left';
        if (typeof axis[a] != 'undefined') {
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

      // lines
      var ticks = parseInt(height / 50, 10);
      for (c = 1; c <= ticks; c++) {
        html +=
          '<div class="chart-ticksTick' +
          (c === 1 ? ' first ' : '') +
          (c == ticks ? ' last ' : '') +
          '">';
        for (q in range) {
          style = (Math.floor(q / 2) + 1) * (q % 2 === 1 ? 30 : 30);
          side = q % 2 === 1 ? 'right' : 'left';
          if (range[q] > 0) {
            var val = range[q] - (range[q] / ticks) * (c - 1);
            val = val < 10 ? val.toFixed(2) : Math.round(val);

            if (side == 'left') {
              style += parseFloat((val.toString().length - 1) * 3);
            }
            html += '<p style="' + side + ':-' + style + 'px">' + val + '</p>';
          }
        }
        html += '</div>';
      }
      html += '</table>';

      this.table.innerHTML += '</tbody>' + html;

      return this.table;
    };

  build.prototype = {
    mouseover: function () {
      var t = event.target;

      k(function () {
        // remove all hover class
        var hover = Array.prototype.slice.call(
          t.closest('table').getElementsByClassName('hover'),
        );
        for (var x = 0; x < hover.length; x++)
          hover[x].classList.remove('hover');

        // ie 11 does not support classlist.

        if (
          t.closest('.chart-head th') &&
          t.closest('.chart-head th').getElementsByTagName('span') &&
          t.closest('.chart-head th').getElementsByTagName('span')[0].classList
            .value != undefined
        ) {
          var lines = Array.prototype.slice.call(
            t
              .closest('table')
              .getElementsByClassName(
                'chart-line ' +
                  t
                    .closest('.chart-head th')
                    .getElementsByTagName('span')[0]
                    .classList.value.replace('chart-headClr', ''),
              ),
          );
          for (x = 0; x < lines.length; x++) lines[x].classList.add('hover');
          var bars = Array.prototype.slice.call(
            t
              .closest('table')
              .getElementsByClassName(
                'chart-bar ' +
                  t
                    .closest('.chart-head th')
                    .getElementsByTagName('span')[0]
                    .classList.value.replace('chart-headClr', ''),
              ),
          );
          for (x = 0; x < bars.length; x++) bars[x].classList.add('hover');
        }
      });
    },
    mouseout: function () {
      var t = event.target;

      k(function () {
        var hover = Array.prototype.slice.call(
          t.closest('table').getElementsByClassName('hover'),
        );
        for (var x = 0; x < hover.length; x++)
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
