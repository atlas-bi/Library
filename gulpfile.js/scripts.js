const concat = require('gulp-concat');
const uglify = require('gulp-uglify');
const gulp = require('gulp');
const babel = require('gulp-babel');
const log = require('fancy-log');
const rollup = require('rollup-stream-gulp');
const { nodeResolve } = require('@rollup/plugin-node-resolve');
const commonjs = require('@rollup/plugin-commonjs');

gulp.task('js:polyfill', function () {
  return gulp
    .src([
      'web/wwwroot/js/polyfill/classlist.js',
      'web/wwwroot/js/polyfill/events.js',
      'web/wwwroot/js/polyfill/focus-within.js',
      'web/wwwroot/js/polyfill/foreach.js',
      'web/wwwroot/js/polyfill/insert-after.js',
      'web/wwwroot/js/polyfill/isinstance.js',
      'web/wwwroot/js/polyfill/matches_closest.js',
      'web/wwwroot/js/polyfill/sticky.js',
    ])
    .pipe(concat('polyfill.min.js'))
    .pipe(uglify())
    .pipe(gulp.dest('web/wwwroot/js/'));
});

gulp.task('js:utility', function () {
  return (
    gulp
      .src([
        'web/wwwroot/js/utility/tabs.js',
        'web/wwwroot/js/utility/collapse.js',
        'web/wwwroot/js/utility/carousel.js',
        'web/wwwroot/js/utility/table.js',
        'web/wwwroot/js/utility/drag.js',
        'web/wwwroot/js/utility/reorder.js',
        'web/wwwroot/js/utility/charts.js',
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
        'web/wwwroot/js/utility/hamburger.js',
        'web/wwwroot/js/mini.js',
        'web/wwwroot/js/dropdown.js',
      ])

      // .pipe(
      //   babel({
      //     presets: ['@babel/preset-env'],
      //   }),
      // )
      .pipe(
        rollup({
          output: { format: 'iife', name: 'module' },
          plugins: [
            nodeResolve({ browser: true, preferBuiltins: false }),
            commonjs(),
          ],
        }),
      )
      .pipe(concat('utility.min.js'))
      .pipe(uglify())
      .pipe(gulp.dest('web/wwwroot/js/'))
  );
});

gulp.task('js:integrations:ssrs', function () {
  return gulp
    .src(['web/wwwroot/js/integrations/ssrs.js'])
    .pipe(concat('ssrs.min.js'))
    .pipe(
      babel({
        presets: ['@babel/preset-env'],
      }),
    )
    .pipe(uglify())
    .pipe(gulp.dest('web/wwwroot/js/integrations/'));
});

gulp.task('js:shared', function () {
  return (
    gulp
      .src(['web/wwwroot/js/shared.js', 'web/wwwroot/lib/chartjs/chart.js'])
      .pipe(concat('shared.min.js'))
      // .pipe(
      //   babel({
      //     presets: ['@babel/preset-env'],
      //   }),
      // )
      .pipe(uglify())
      .pipe(gulp.dest('web/wwwroot/js/'))
  );
});

gulp.task('js:search', function () {
  return gulp
    .src(['web/wwwroot/js/search.js', 'web/wwwroot/js/error.js'])
    .pipe(concat('search.min.js'))
    .pipe(
      babel({
        presets: ['@babel/preset-env'],
      }),
    )
    .pipe(uglify())
    .pipe(gulp.dest('web/wwwroot/js/'));
});

gulp.task('js:settings', function () {
  return (
    gulp
      .src(['web/wwwroot/js/settings.js', 'web/wwwroot/js/access.js'])
      .pipe(concat('settings.min.js'))
      // .pipe(
      //   babel({
      //     presets: ['@babel/preset-env'],
      //   }),
      // )
      .pipe(uglify())
      .pipe(gulp.dest('web/wwwroot/js/'))
  );
});

gulp.task('js:editor', function () {
  return gulp
    .src([
      'web/wwwroot/js/editor.js',
      'web/wwwroot/js/utility/checkbox.js',
      'web/wwwroot/js/reportEditor.js',
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

//gulp.task('scripts', gulp.parallel('webpack', 'js:integrations:ssrs'));
gulp.task(
  'scripts',
  gulp.parallel(
    'js:editor',
    'js:polyfill',
    'js:settings',
    'js:search',
    'js:shared',
    'js:utility',
    'js:integrations:ssrs',
  ),
);
