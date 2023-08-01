(function () {
  function AglListener(event) {
    if (event && event.data && event.data.error) {
      document.dispatchEvent(
        new CustomEvent('notification', {
          cancelable: true,
          detail: {
            value: event.data.error,
          },
        }),
      );
    }

    if (
      event &&
      event.data &&
      event.data.token &&
      window.aglToken === undefined
    ) {
      console.log('AGL handshake succeeded');
      window.aglToken = event.data.token;
      setCookie('EPIC', 1, 99);

      // if agl cookie was not set, then set it
      // and refresh so the server regenerates any links

      if (!getCookie('AGL')) {
        setCookie('AGL', 1, 99);
        window.location.reload();
      }

      document.addEventListener('click', function (event) {
        if (event.target.closest('a[href]')) {
          const target = event.target.closest('a[href]');

          let aglType = target.getAttribute('href');

          const load = {};

          switch (aglType) {
            case 'AR_ITM_RECORDVIEWER': {
              const name = `${target.dataset.ini}RecordID`;

              load[name] = target.dataset.id;
              break;
            }

            case 'WM_DASHBOARD_LAUNCHER': {
              load.DashboardID = target.dataset.id;
              break;
            }

            case 'AC_RW_STATUS': {
              load.ReportID = target.dataset.id;
              break;
            }

            case 'BI_ITM_SLICERDICER': {
              if (target.dataset.ini === 'FDM') {
                load.DataModelID = target.dataset.id;
              } else {
                load.ReportRunID = target.dataset.id;
              }

              break;
            }

            case 'WM_ITM_METRIC_EDITOR': {
              load.IDNRecordID = target.dataset.id;
              break;
            }

            case 'CST_WEB_BROWSER': {
              let url = target.dataset.url;

              if (target.dataset.hyperspace === 'Y') {
                url += '&EPIC=1';
              }

              load.URL = url;
              load.Caption = target.dataset.name;
              break;
            }

            case 'WM_ITM_COMPONENT_EDITOR': {
              load.IDBRecordID = target.dataset.id;
              break;
            }

            case 'AC_RW_STATUS_TEMPLATE': {
              aglType = 'AC_RW_STATUS';
              load.ReportModelToLaunch = target.dataset.templateid;
              break;
            }

            case 'AC_REPORT_SETTINGS_WEB': {
              load.HGRRecordID = target.dataset.id;
              if (target.dataset.ini === 'HRX') {
                load.InitialReportID = target.dataset.id;
                load.HGRRecordID = target.dataset.templateid;
              }

              break;
            }

            default: {
              break;
            }
          }

          if (Object.entries(load).length > 0) {
            // Prevent hyperlinks
            event.preventDefault();
            window.parent.postMessage(
              {
                token: window.aglToken,
                action: 'Epic.Clinical.Informatics.Web.LaunchActivity',
                args: {
                  ActivityKey: `ATLAS_${aglType}`,
                  Parameters: load,
                },
              },
              '*',
            );
          }
        }
      });
    } else if (window.aglToken !== undefined) {
      console.log('agl already connected.');
    } else {
      console.log('no agl listener found.');
    }
  }

  if (window.aglToken === undefined) {
    // AGL Listener
    window.addEventListener('message', AglListener, false);

    // Shake hands
    window.parent.postMessage(
      { action: 'Epic.Clinical.Informatics.Web.InitiateHandshake' },
      '*',
    );
  }
})();
