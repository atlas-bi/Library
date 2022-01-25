const gulp = require('gulp');
const autoprefexer = require('gulp-autoprefixer');
const sass = require('gulp-sass')(require('sass'));
const replace = require('gulp-replace');
const { fontawesomeSubset } = require('fontawesome-subset');
const postcss = require('gulp-postcss');
//const del = require('del');
const purgecss = require('gulp-purgecss');
const cssnano = require('gulp-cssnano');
const concat = require('gulp-concat');
const rename = require('gulp-rename');
const uglify = require('gulp-uglify');
const postcssFocusWithin = require('focus-within/postcss');
const babel = require('gulp-babel');

gulp.task('package:markdown-it', function () {
  return gulp
    .src('node_modules/markdown-it/dist/markdown-it.min.js')
    .pipe(gulp.dest('web/wwwroot/lib/markdown-it'));
});

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
        'circle-notch',
        'long-arrow-alt-left',
        'long-arrow-alt-right',
        'eye',
        'eye-slash',
      ],
    },
    'web/wwwroot/font/fontawesome/webfonts',
  );
  done();
});

gulp.task('sass', function () {
  var plugins = [postcssFocusWithin];
  return gulp
    .src('web/wwwroot/scss/theme.scss')
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
    .pipe(rename('site.min.css'))
    .pipe(gulp.dest('web/wwwroot/css/'));
});

gulp.task('js:polyfill', function () {
  return gulp
    .src([
      'web/wwwroot/js/polyfill/foreach.js',
      'web/wwwroot/js/polyfill/isinstance.js',
      'web/wwwroot/js/polyfill/matches_closest.js',
      'web/wwwroot/js/polyfill/sticky.js',
      'web/wwwroot/js/polyfill/insert-after.js',
      'web/wwwroot/js/polyfill/event.js',
      'web/wwwroot/js/polyfill/focus-within.js',
      'web/wwwroot/js/polyfill/classlist.js',
    ])
    .pipe(concat('polyfill.min.js'))
    .pipe(
      babel({
        presets: ['@babel/preset-env'],
      }),
    )
    .pipe(uglify())
    .pipe(gulp.dest('web/wwwroot/js/'));
});
gulp.task('js:utility', function () {
  return gulp
    .src([
      'web/wwwroot/js/utility/tabs.js',
      'web/wwwroot/js/utility/collapse.js',
      'web/wwwroot/js/utility/carousel.js',
      'web/wwwroot/js/utility/table.js',
      'web/wwwroot/js/utility/drag.js',
      'web/wwwroot/js/utility/charts.js',
      'web/wwwroot/js/input.js',
      'web/wwwroot/js/comments.js',
      'web/wwwroot/js/dropdown.js',
      'web/wwwroot/js/utility/modal.js',
      'web/wwwroot/js/utility/lazyload.js',
      'web/wwwroot/js/utility/crumbs.js',
      'web/wwwroot/js/page.js',
      'web/wwwroot/js/hyperspace.js',
      'web/wwwroot/js/favorites.js',
      'web/wwwroot/js/ajax-content.js',
      'web/wwwroot/js/messagebox.js',
      'web/wwwroot/js/mail.js',
      'web/wwwroot/js/analytics.js',
      'web/wwwroot/js/analytics.js',
      'web/wwwroot/js/utility/hamburger.js',
      'web/wwwroot/js/mini.js',
    ])
    .pipe(concat('utility.min.js'))
    .pipe(
      babel({
        presets: ['@babel/preset-env'],
      }),
    )
    .pipe(uglify())
    .pipe(gulp.dest('web/wwwroot/js/'));
});

gulp.task('js:polyfill', function () {
  return gulp
    .src('web/wwwroot/js/polyfill/**/*.js')
    .pipe(concat('polyfill.min.js'))
    .pipe(
      babel({
        presets: ['@babel/preset-env'],
      }),
    )
    .pipe(uglify())
    .pipe(gulp.dest('web/wwwroot/js/'));
});

gulp.task('js:shared', function () {
  return gulp
    .src('web/wwwroot/js/shared.js')
    .pipe(concat('shared.min.js'))
    .pipe(
      babel({
        presets: ['@babel/preset-env'],
      }),
    )
    .pipe(uglify())
    .pipe(gulp.dest('web/wwwroot/js/'));
});

gulp.task('js:editor', function () {
  return gulp
    .src([
      'web/wwwroot/js/editor.js',
      'web/wwwroot/js/utility/checkbox.js',
      'web/wwwroot/js/reportEditor.js',
      'web/wwwroot/js/collectionEditor.js',
    ])
    .pipe(concat('editor.min.js'))
    .pipe(
      babel({
        presets: ['@babel/preset-env'],
      }),
    )
    .pipe(uglify())
    .pipe(gulp.dest('web/wwwroot/js/'));
});

gulp.task(
  'build',
  gulp.parallel(
    'font:inter',
    'font:rasa',
    'package:markdown-it',
    'js:utility',
    'js:polyfill',
    'js:editor',
    gulp.series('font:fontawesome', 'sass'),
  ),
);

gulp.task(
  'watch',
  gulp.series('build', function (cb) {
    gulp.watch('web/wwwroot/**/*.scss', gulp.series('sass'));
    gulp.watch('web/wwwroot/**/*.sass', gulp.series('sass'));
    gulp.watch(
      ['web/wwwroot/**/*.js', '!web/wwwroot/**/*.min.js'],
      gulp.parallel('js:shared', 'js:editor', 'js:utility', 'sass'),
    );
    gulp.watch(
      'web/wwwroot/js/polyfill/**/*.js',
      gulp.parallel('js:polyfill', 'sass'),
    );
    gulp.watch('web/Pages/**/*.cshtml', gulp.series('sass'));
    gulp.watch(
      'web/font/fontawesome/**/*.scss',
      gulp.series('font:fontawesome', 'sass'),
    );
    cb();
  }),
);
