const { task } = require('gulp');
const gulp = require('gulp');
require('./font');
require('./styles');
require('./scripts');
require('./iis');
require('./dotnet');
require('./build');
require('./watch');

/*
style changes > run styles
font changes > run font > styles
js changes > run js, styles
c# changes > run dotnet
html changes > run styles, dotnet
*/
gulp.task('iis:run', gulp.series('iis:kill', 'dotnet:build', 'iis:start'));

gulp.task(
  'run',
  gulp.series(gulp.series('build', 'iis:start'), function (cb) {
    gulp.watch(
      ['web/wwwroot/**/*.scss', 'web/wwwroot/**/*.sass'],
      gulp.series('styles', 'iis:run'),
    );

    gulp.watch(
      [
        'web/wwwroot/**/*.js',
        '!web/wwwroot/**/*.min.js',
        '!web/wwwroot/**/*.build.js',
      ],
      gulp.series(gulp.parallel('scripts', 'styles'), 'iis:run'),
    );

    gulp.watch('web/Pages/**/*.cshtml', gulp.series('styles'));
    gulp.watch(
      'web/font/fontawesome/**/*.scss',
      gulp.series('fonts', 'styles', 'iis:run'),
    );
    gulp.watch(['web/Pages/**/*.cshtml'], gulp.series('styles', 'iis:run'));
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
        'web/appsettings.*.json',
      ],
      gulp.series('iis:run'),
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
