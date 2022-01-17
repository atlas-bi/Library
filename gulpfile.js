const gulp = require('gulp');
const autoprefexer = require('gulp-autoprefixer');
const sass = require('gulp-sass')(require('sass'));
const replace = require('gulp-replace');
const { fontawesomeSubset } = require('fontawesome-subset');
const postcss = require('gulp-postcss');
//const del = require('del');
const purgecss = require('gulp-purgecss');
const cssnano = require('gulp-cssnano');
// const concat = require('gulp-concat');
// const rename = require('gulp-rename');
// const uglify = require('gulp-uglify');
const postcssFocusWithin = require('focus-within/postcss');

gulp.task('font:inter', function () {
  return gulp
    .src('node_modules/@fontsource/inter/**/*')
    .pipe(replace(/\.\/files\//g, '/font/inter/files/'))
    .pipe(gulp.dest('web/wwwroot/font/inter'));
});

gulp.task('font:rasa', function () {
  return gulp
    .src('node_modules/@fontsource/rasa/**/*')
    .pipe(replace(/\.\/files\//g, '/font/rasa/files/'))
    .pipe(gulp.dest('web/wwwroot/font/rasa'));
});

gulp.task('font:fontawesome', function (done) {
  //del.sync('web/wwwroot/font/fontawesome/webfonts', {force:true});
  fontawesomeSubset(
    {
      regular: ['envelope', 'thumbs-up', 'play-circle', 'star', 'paper-plane'],
      solid: [
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
      ],
    },
    'web/wwwroot/font/fontawesome/webfonts',
  );
  done();
});

gulp.task('sass', function () {
  var plugins = [postcssFocusWithin];
  return gulp
    .src('web/wwwroot/scss/**/*.scss')
    .pipe(sass().on('error', sass.logError))
    .pipe(
      purgecss({
        content: [
          'web/Pages/**/*.cshtml',
          'web/wwwroot/js/**/*.js',
          'web/wwwroot/lib/**/*.js',
        ],
        safelist: ['breadcrumb', 'is-active'],
        whitelist: ['breadcrumb', 'is-active'],
      }),
    )
    .pipe(postcss(plugins))
    .pipe(autoprefexer())
    .pipe(cssnano())
    .pipe(gulp.dest('web/wwwroot/css/'));
});

gulp.task(
  'build',
  gulp.parallel(
    'font:inter',
    'font:rasa',
    gulp.series('font:fontawesome', 'sass'),
  ),
);

gulp.task(
  'watch',
  gulp.series('build', function (cb) {
    gulp.watch('web/wwwroot/**/*.scss', gulp.series('sass'));
    gulp.watch('web/Pages/**/*.cshtml', gulp.series('sass'));
    gulp.watch(
      'web/font/fontawesome/**/*.scss',
      gulp.series('font:fontawesome', 'sass'),
    );
    cb();
  }),
);
