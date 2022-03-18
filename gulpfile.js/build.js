const gulp = require('gulp');
require('./font');
require('./scripts');
require('./styles');
require('./iis');
require('./dotnet');
require('./packages');
gulp.task(
  'build',
  gulp.series(
    gulp.parallel(
      'packages',
      'scripts',
      'iis:clean_logs',
      'iis:kill',
      gulp.series('fonts', 'styles'),
    ),
    'dotnet:build',
  ),
);
