const gulp = require('gulp');

gulp.task('package:dompurify', () => {
  return gulp
    .src('node_modules/dompurify/dist/purify.min.js')
    .pipe(gulp.dest('web/wwwroot/lib/dompurify'));
});

gulp.task('package:dompurify:map', () => {
  return gulp
    .src('node_modules/dompurify/dist/purify.min.js.map')
    .pipe(gulp.dest('web/wwwroot/js'));
});

gulp.task(
  'packages',
  gulp.parallel('package:dompurify', 'package:dompurify:map'),
);
