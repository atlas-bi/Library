const gulp = require('gulp');
const replace = require('gulp-replace');
const { fontawesomeSubset } = require('fontawesome-subset');

gulp.task('font:inter', () => {
  return gulp
    .src('node_modules/@fontsource/inter/**/*')
    .pipe(replace(/\.\/files\//g, '/font/inter/files/'))
    .pipe(gulp.dest('web/wwwroot/font/inter'));
});

gulp.task('font:rasa', () => {
  return gulp
    .src('node_modules/@fontsource/rasa/**/*')
    .pipe(replace(/\.\/files\//g, '/font/rasa/files/'))
    .pipe(gulp.dest('web/wwwroot/font/rasa'));
});

gulp.task('font:fontawesome', (done) => {
  fontawesomeSubset(
    {
      regular: [
        'envelope',
        'thumbs-up',
        'play-circle',
        'star',
        'paper-plane',
        'image',
      ],
      solid: [
        'plus-circle',
        'minus-circle',
        'cogs',
        'external-link-square-alt',
        'code',
        'quote-left',
        'reply',
        'reply-all',
        'paper-plane',
        'heading',
        'times',
        'folder',
        'check',
        'play',
        'star',
        'folder-plus',
        'file-upload',
        'save',
        'angle-down',
        'sort',
        'folder-open',
        'link',
        'grip-lines',
        'sliders-h',
        'wrench',
        'italic',
        'user',
        'list-ol',
        'list-ul',
        'chevron-left',
        'chevron-right',
        'chevron-down',
        'chevron-up',
        'bold',
        'search',
        'edit',
        'trash',
        'star',
        'share',
        'plus',
        'chart-bar',
        'universal-access',
        'trash-alt',
        'circle-notch',
        'long-arrow-alt-left',
        'long-arrow-alt-right',
        'eye',
        'eye-slash',
        'palette',
        'database',
        'comment-alt',
        'info',
        'paperclip',
        'user-lock',
        'lock',
        'image',
        'arrows-alt',
        'circle',
        'exlamation-triangle',
        'book-open',
        'check-circle',
        'certificate',
        'filter',
        'users',
        'book',
        'lightbulb',
        'project-diagram',
      ],
    },
    'web/wwwroot/font/fontawesome/webfonts',
  );
  done();
});

gulp.task(
  'fonts',
  gulp.parallel('font:inter', 'font:rasa', 'font:fontawesome'),
);
