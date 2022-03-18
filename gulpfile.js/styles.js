const rename = require('gulp-rename');
const autoprefexer = require('gulp-autoprefixer');
const sass = require('gulp-sass')(require('sass'));
const postcss = require('gulp-postcss');
const purgecss = require('gulp-purgecss');
const cssnano = require('gulp-cssnano');
const postcssFocusWithin = require('focus-within/postcss');
const gulp = require('gulp');

gulp.task('css:build', function () {
  var plugins = [postcssFocusWithin];
  return gulp
    .src('web/wwwroot/css/theme.scss')
    .pipe(sass().on('error', sass.logError))
    .pipe(
      purgecss({
        content: [
          'web/Pages/**/*.cshtml',
          'web/wwwroot/js/**/*.js',
          '!web/wwwroot/js/**/*build.js',
          '!web/wwwroot/js/**/*min.js',
          'web/wwwroot/lib/**/*.js',
        ],
        safelist: ['breadcrumb', 'is-active', '.editor-liveEditorPrev'],
        whitelist: ['breadcrumb', 'is-active', '.editor-liveEditorPrev'],
      }),
    )
    .pipe(postcss(plugins))
    .pipe(autoprefexer())
    .pipe(cssnano())
    .pipe(rename('site.min.css'))
    .pipe(gulp.dest('web/wwwroot/css/'));
});

gulp.task('styles', gulp.parallel('css:build'));
