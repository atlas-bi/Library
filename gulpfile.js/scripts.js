const concat = require('gulp-concat');
const uglify = require('gulp-uglify');
const gulp = require('gulp');
const log = require('fancy-log');
const rollup = require('rollup-stream-gulp');
const { nodeResolve } = require('@rollup/plugin-node-resolve');
const commonjs = require('@rollup/plugin-commonjs');
const { babel } = require('@rollup/plugin-babel');

const rollupConfig = {
  output: { format: 'iife', name: 'module' },
  plugins: [
    nodeResolve({ browser: true, preferBuiltins: false }),
    commonjs(),
    babel({
      babelHelpers: 'bundled',
    }),
  ],
};
const uglifyConfig = {
  ie: true,
  v8: true,
  webkit: true,
};
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
      'web/wwwroot/js/polyfill/remove.js',
      'web/wwwroot/js/polyfill/includes.js',
    ])
    .pipe(concat('polyfill.min.js'))

    .pipe(uglify(uglifyConfig))
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
      'web/wwwroot/js/utility/hamburger.js',
      'web/wwwroot/js/mini.js',
      'web/wwwroot/js/dropdown.js',
      'node_modules/chart.js/dist/Chart.js',
    ])
    .pipe(rollup(rollupConfig))
    .pipe(concat('utility.min.js'))
    .pipe(uglify(uglifyConfig))
    .pipe(gulp.dest('web/wwwroot/js/'));
});

gulp.task('js:analytics', function () {
  return gulp
    .src(['web/wwwroot/js/analytics.js'])
    .pipe(rollup(rollupConfig))
    .pipe(concat('analytics.min.js'))
    .pipe(uglify(uglifyConfig))
    .pipe(gulp.dest('web/wwwroot/js/'));
});

gulp.task('js:profile', function () {
  return gulp
    .src(['web/wwwroot/js/profile.js'])
    .pipe(rollup(rollupConfig))
    .pipe(concat('profile.min.js'))
    .pipe(uglify(uglifyConfig))
    .pipe(gulp.dest('web/wwwroot/js/'));
});

gulp.task('js:tracker', function () {
  return gulp
    .src(['web/wwwroot/js/tracker.js', 'node_modules/jsnlog/jsnlog.js'])
    .pipe(rollup(rollupConfig))
    .pipe(concat('alive.min.js'))
    .pipe(uglify(uglifyConfig))
    .pipe(gulp.dest('web/wwwroot/js/'));
});

gulp.task('js:highlighter', () => {
  return gulp
    .src('web/wwwroot/lib/highlight/highlight.js')
    .pipe(rollup(rollupConfig))
    .pipe(concat('highlight.min.js'))
    .pipe(uglify(uglifyConfig))
    .pipe(gulp.dest('web/wwwroot/js/'));
});

gulp.task('js:shared', function () {
  // shared functions should be imported as needed.
  return (
    gulp
      .src(['web/wwwroot/js/shared.js'])
      // .pipe(rollup(rollupConfig))
      .pipe(concat('shared.min.js'))
      .pipe(uglify(uglifyConfig))
      .pipe(gulp.dest('web/wwwroot/js/'))
  );
});

gulp.task('js:search', function () {
  return gulp
    .src(['web/wwwroot/js/search.js', 'web/wwwroot/js/error.js'])
    .pipe(rollup(rollupConfig))
    .pipe(concat('search.min.js'))
    .pipe(uglify(uglifyConfig))
    .pipe(gulp.dest('web/wwwroot/js/'));
});

gulp.task('js:settings', function () {
  return gulp
    .src(['web/wwwroot/js/settings.js', 'web/wwwroot/js/access.js'])
    .pipe(rollup(rollupConfig))
    .pipe(concat('settings.min.js'))
    .pipe(uglify(uglifyConfig))
    .pipe(gulp.dest('web/wwwroot/js/'));
});

gulp.task('js:editor', function () {
  return gulp
    .src([
      'web/wwwroot/js/editor.js',
      'web/wwwroot/js/utility/checkbox.js',
      'web/wwwroot/js/report-editor.js',
    ])
    .pipe(rollup(rollupConfig))
    .pipe(concat('editor.min.js'))
    .pipe(uglify(uglifyConfig))
    .pipe(gulp.dest('web/wwwroot/js/'));
});

gulp.task(
  'scripts',
  gulp.parallel(
    'js:editor',
    'js:polyfill',
    'js:settings',
    'js:search',
    'js:shared',
    'js:utility',
    'js:analytics',
    'js:tracker',
    'js:highlighter',
    'js:profile',
  ),
);
