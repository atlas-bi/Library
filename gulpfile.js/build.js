const gulp = require('gulp');
require('./font');
require('./scripts');
require('./styles');
require('./dotnet');
require('./packages');

gulp.task(
  'build',
  gulp.series(
    gulp.parallel('packages', 'scripts', gulp.series('fonts', 'styles')),
    'dotnet:build',
  ),
);
