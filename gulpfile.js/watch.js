const { task } = require('gulp');
const gulp = require('gulp');
require('./font');
require('./styles');
require('./scripts');
require('./iis');
require('./dotnet');
require('./build');

const open = require('open');

/*
style changes > run styles
font changes > run font > styles
js changes > run js, styles
c# changes > run dotnet
html changes > run styles, dotnet
*/
gulp.task('iis:run', gulp.series('dotnet:build', 'iis:start'));
gulp.task('browser', function (cb) {
  open('https://localhost:44381');
  cb();
});
gulp.task(
  'start',
  gulp.series(
    gulp.series('iis:kill', 'build', 'iis:start', 'browser'),
    function (cb) {
      gulp.watch(
        [
          'web/wwwroot/**/*.scss',
          'web/wwwroot/**/*.sass',
          'web/Pages/**/*.cshtml',
          'web/Services/',
          'web/Model/',
          'web/Middleware/',
          'web/Helpers',
        ],
        gulp.series(gulp.parallel('styles', 'iis:kill'), 'iis:run'),
      );

      // utility
      gulp.watch(
        [
          'web/wwwroot/js/utility/tabs.js',
          'web/wwwroot/js/utility/collapse.js',
          'web/wwwroot/js/utility/carousel.js',
          'web/wwwroot/js/utility/table.js',
          'web/wwwroot/js/utility/drag.js',
          'web/wwwroot/js/utility/reorder.js',
          'web/wwwroot/js/utility/charts.js',
          'web/wwwroot/js/utility/modal.js',
          'web/wwwroot/js/utility/lazyload.js',
          'web/wwwroot/js/utility/crumbs.js',
          'web/wwwroot/js/page.js',
          'web/wwwroot/js/hyperspace.js',
          'web/wwwroot/js/favorites.js',
          'web/wwwroot/js/ajax-content.js',
          'web/wwwroot/js/messagebox.js',
          'web/wwwroot/js/mail.js',
          'web/wwwroot/js/utility/hamburger.js',
          'web/wwwroot/js/mini.js',
          'web/wwwroot/js/dropdown.js',
          'node_modules/chart.js/dist/chart.js',
        ],
        gulp.series(
          gulp.parallel('js:utility', 'styles', 'iis:kill'),
          'iis:run',
        ),
      );
      // analytics
      gulp.watch(
        ['web/wwwroot/js/analytics.js'],
        gulp.series(
          gulp.parallel('js:analytics', 'styles', 'iis:kill'),
          'iis:run',
        ),
      );
      // polyfill
      gulp.watch(
        [
          'web/wwwroot/js/polyfill/classlist.js',
          'web/wwwroot/js/polyfill/events.js',
          'web/wwwroot/js/polyfill/focus-within.js',
          'web/wwwroot/js/polyfill/foreach.js',
          'web/wwwroot/js/polyfill/insert-after.js',
          'web/wwwroot/js/polyfill/isinstance.js',
          'web/wwwroot/js/polyfill/matches_closest.js',
          'web/wwwroot/js/polyfill/sticky.js',
          'web/wwwroot/js/polyfill/remove.js',
          'web/wwwroot/js/polyfill/includes.js',
          'web/wwwroot/js/polyfill/trunc.js',
        ],
        gulp.series(
          gulp.parallel('js:polyfill', 'styles', 'iis:kill'),
          'iis:run',
        ),
      );

      // tracker
      gulp.watch(
        ['web/wwwroot/js/tracker.js'],
        gulp.series(
          gulp.parallel('js:tracker', 'styles', 'iis:kill'),
          'iis:run',
        ),
      );

      // highlighter
      gulp.watch(
        ['web/wwwroot/lib/highlight/highlight.js'],
        gulp.series(
          gulp.parallel('js:highlighter', 'styles', 'iis:kill'),
          'iis:run',
        ),
      );

      // shared
      gulp.watch(
        ['web/wwwroot/js/shared.js'],
        gulp.series(
          gulp.parallel('js:shared', 'styles', 'iis:kill'),
          'iis:run',
        ),
      );

      // search
      gulp.watch(
        ['web/wwwroot/js/search.js', 'web/wwwroot/js/error.js'],
        gulp.series(
          gulp.parallel('js:search', 'styles', 'iis:kill'),
          'iis:run',
        ),
      );

      // profile
      gulp.watch(
        'web/wwwroot/js/profile.js',
        gulp.series(
          gulp.parallel('js:profile', 'styles', 'iis:kill'),
          'iis:run',
        ),
      );

      // settings
      gulp.watch(
        ['web/wwwroot/js/settings.js', 'web/wwwroot/js/access.js'],
        gulp.series(
          gulp.parallel('js:settings', 'styles', 'iis:kill'),
          'iis:run',
        ),
      );

      // editor
      gulp.watch(
        [
          'web/wwwroot/js/editor.js',
          'web/wwwroot/js/utility/checkbox.js',
          'web/wwwroot/js/report-editor.js',
        ],
        gulp.series(
          gulp.parallel('js:editor', 'styles', 'iis:kill'),
          'iis:run',
        ),
      );

      gulp.watch('web/Pages/**/*.cshtml', gulp.series('styles'));
      gulp.watch(
        'web/font/fontawesome/**/*.scss',
        gulp.series(
          gulp.parallel('iis:kill', gulp.series('fonts', 'styles')),
          'iis:run',
        ),
      );
      gulp.watch(
        [
          'web/Pages/**/*.cshtml.cs',
          'web/Helpers**/*.cs',
          'web/Middleware**/*.cs',
          'web/Models/**/*.cs',
          'web/Properties/**/*.json',
          'web/appsettings*.json',
          'web/Program.cs',
          'web/Startup.cs',
          'web/web.csproj',
          'web/appsettings*.json',
        ],
        gulp.series('iis:kill', 'iis:run'),
      );
      cb();
    },
  ),
);

gulp.task(
  'watch',
  gulp.series('build', function (cb) {
    gulp.watch(
      ['web/wwwroot/**/*.scss', 'web/wwwroot/**/*.sass'],
      gulp.series('styles'),
    );
    gulp.watch(
      [
        'web/wwwroot/**/*.js',
        '!web/wwwroot/**/*.min.js',
        '!web/wwwroot/**/*.build.js',
      ],
      gulp.parallel('scripts', 'styles'),
    );
    gulp.watch('web/Pages/**/*.cshtml', gulp.series('styles'));
    gulp.watch(
      'web/font/fontawesome/**/*.scss',
      gulp.series('fonts', 'styles'),
    );
    cb();
  }),
);
