const gulp = require('gulp');

gulp.task('package:markdown-it', () => {
  return gulp
    .src('node_modules/markdown-it/dist/markdown-it.min.js')
    .pipe(gulp.dest('web/wwwroot/lib/markdown-it'));
});

gulp.task('package:chart.js', () => {
  return gulp
    .src('node_modules/chart.js/dist/**/*')
    .pipe(gulp.dest('web/wwwroot/lib/chartjs'));
});

gulp.task('packages', gulp.parallel('package:markdown-it', 'package:chart.js'));
