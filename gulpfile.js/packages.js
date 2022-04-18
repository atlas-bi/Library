const gulp = require('gulp');

gulp.task('package:dompurify', () => {
  return gulp
    .src('node_modules/dompurify/dist/purify.min.js')
    .pipe(gulp.dest('web/wwwroot/lib/dompurify'));
});

gulp.task('packages', gulp.parallel('package:dompurify'));
