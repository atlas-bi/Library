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

gulp.task('font:source', () => {
  return gulp
    .src('node_modules/@fontsource/source-code-pro/**/*')
    .pipe(replace(/\.\/files\//g, '/font/source-code-pro/files/'))
    .pipe(gulp.dest('web/wwwroot/font/source-code-pro'));
});

gulp.task('font:fontawesome', (done) => {
  fontawesomeSubset(
    {
      regular: [
        'envelope',
        'thumbs-up',
        'circle-play',
        'star',
        'paper-plane',
        'image',
        'copy',
      ],
      solid: [
        'circle-plus',
        'circle-minus',
        'gears',
        'arrow-up-right-from-square',
        'code',
        'quote-left',
        'reply',
        'reply-all',
        'paper-plane',
        'heading',
        'xmark',
        'folder',
        'check',
        'play',
        'star',
        'folder-plus',
        'file-arrow-up',
        'floppy-disk',
        'angle-down',
        'sort',
        'folder-open',
        'link',
        'grip-lines',
        'sliders',
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
        'magnifying-glass',
        'pen-to-square',
        'trash',
        'star',
        'share',
        'plus',
        'chart-bar',
        'universal-access',
        'trash-can',
        'circle-notch',
        'left-long',
        'right-long',
        'eye',
        'eye-slash',
        'palette',
        'database',
        'message',
        'info',
        'paperclip',
        'user-lock',
        'lock',
        'image',
        'up-down-left-right',
        'circle',
        'book-open',
        'circle-check',
        'certificate',
        'filter',
        'users',
        'book',
        'lightbulb',
        'diagram-project',
        'caret-up',
        'caret-down',
      ],
    },
    'web/wwwroot/font/fontawesome/webfonts',
  );
  done();
});

gulp.task(
  'fonts',
  gulp.parallel('font:inter', 'font:rasa', 'font:fontawesome', 'font:source'),
);
