const gulp = require('gulp');
const open = require('open');
require('./font');
require('./styles');
require('./scripts');
require('./dotnet');
require('./build');

/*
Style changes > run styles
font changes > run font > styles
js changes > run js, styles
c# changes > run dotnet
html changes > run styles, dotnet
*/
gulp.task('browser', function (cb) {
  open('https://localhost:5001');
  cb();
});
gulp.task(
  'start',
  gulp.series(gulp.series('build', 'dotnet:run', 'browser'), function (cb) {
    gulp.watch(
      [
        'web/wwwroot/**/*.scss',
        'web/wwwroot/**/*.sass',
        'web/Pages/**/*.cshtml',
        'web/Controllers/**/*',
        'web/Services/',
        'web/Model/',
        'web/Middleware/',
        'web/Helpers',
        'web/Authorization',
      ],
      gulp.series('styles', 'dotnet:run'),
    );

    // hyperspace
    gulp.watch(
      ['web/wwwroot/js/hyperspace.js'],
      gulp.series(gulp.parallel('js:hyperspace', 'styles'), 'dotnet:run'),
    );
    // Utility
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
        'web/wwwroot/js/notification.js',
        'web/wwwroot/js/mail.js',
        'web/wwwroot/js/utility/hamburger.js',
        'web/wwwroot/js/mini.js',
        'web/wwwroot/js/dropdown.js',
        'node_modules/chart.js/dist/chart.js',
      ],
      gulp.series(gulp.parallel('js:utility', 'styles'), 'dotnet:run'),
    );
    // Analytics
    gulp.watch(
      ['web/wwwroot/js/analytics.js'],
      gulp.series(gulp.parallel('js:analytics', 'styles'), 'dotnet:run'),
    );
    // Polyfill
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
        'web/wwwroot/js/polyfill/object.js',
      ],
      gulp.series(gulp.parallel('js:polyfill', 'styles'), 'dotnet:run'),
    );

    // Tracker
    gulp.watch(
      ['web/wwwroot/js/tracker.js'],
      gulp.series(gulp.parallel('js:tracker', 'styles'), 'dotnet:run'),
    );

    // Highlighter
    gulp.watch(
      ['web/wwwroot/lib/highlight/highlight.js'],
      gulp.series(gulp.parallel('js:highlighter', 'styles'), 'dotnet:run'),
    );

    // Shared
    gulp.watch(
      ['web/wwwroot/js/shared.js'],
      gulp.series(gulp.parallel('js:shared', 'styles'), 'dotnet:run'),
    );

    // Search
    gulp.watch(
      ['web/wwwroot/js/search.js', 'web/wwwroot/js/error.js'],
      gulp.series(gulp.parallel('js:search', 'styles'), 'dotnet:run'),
    );

    // Profile
    gulp.watch(
      'web/wwwroot/js/profile.js',
      gulp.series(gulp.parallel('js:profile', 'styles'), 'dotnet:run'),
    );

    // User settings
    gulp.watch(
      'web/wwwroot/js/user-settings.js',
      gulp.series(gulp.parallel('js:userSettings', 'styles'), 'dotnet:run'),
    );

    // Settings
    gulp.watch(
      ['web/wwwroot/js/settings.js', 'web/wwwroot/js/access.js'],
      gulp.series(gulp.parallel('js:settings', 'styles'), 'dotnet:run'),
    );

    // Editor
    gulp.watch(
      [
        'web/wwwroot/js/editor.js',
        'web/wwwroot/js/utility/checkbox.js',
        'web/wwwroot/js/report-editor.js',
      ],
      gulp.series(gulp.parallel('js:editor', 'styles'), 'dotnet:run'),
    );

    gulp.watch('web/Pages/**/*.cshtml', gulp.series('styles'));
    gulp.watch(
      'web/font/fontawesome/**/*.scss',
      gulp.series('fonts', 'styles', 'dotnet:run'),
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
        'web/wwwroot/js/essential.js',
        'web/wwwroot/lib/scrollbars/simple-scrollbar.js',
      ],
      gulp.series('dotnet:run'),
    );
    cb();
  }),
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
