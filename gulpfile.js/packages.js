const gulp = require('gulp');

gulp.task('package:markdown-it', () => {
  return gulp
    .src('node_modules/markdown-it/dist/markdown-it.min.js')
    .pipe(gulp.dest('web/wwwroot/lib/markdown-it'));
});

gulp.task('package:dompurify', () => {
  return gulp
    .src('node_modules/dompurify/dist/purify.min.js')
    .pipe(gulp.dest('web/wwwroot/lib/dompurify'));
});

gulp.task(
  'packages',
  gulp.parallel('package:markdown-it', 'package:dompurify'),
);
