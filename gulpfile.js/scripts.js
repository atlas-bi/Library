const concat = require('gulp-concat');
const uglify = require('gulp-uglify');
const gulp = require('gulp');
const rollup = require('rollup');
const { nodeResolve } = require('@rollup/plugin-node-resolve');
const commonjs = require('@rollup/plugin-commonjs');
const { babel } = require('@rollup/plugin-babel');
const json = require('@rollup/plugin-json');
const terser = require('@rollup/plugin-terser');
const multi = require('@rollup/plugin-multi-entry');

const rollupOutputConfig = {
  format: 'iife',
  name: 'module',
};

const rollupConfig = {
  output: { format: 'iife', name: 'module' },
  plugins: [
    multi(),
    nodeResolve({ browser: true, preferBuiltins: false }),
    commonjs({ sourceMap: false }),
    babel({
      babelHelpers: 'bundled',
    }),
    json(),
    terser({
      maxWorkers: 4,
      mangle: false,
      output: { comments: false },
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
      'web/wwwroot/js/polyfill/trunc.js',
    ])
    .pipe(concat('polyfill.min.js'))
    .pipe(uglify(uglifyConfig))
    .pipe(gulp.dest('web/wwwroot/js/'));
});

gulp.task('js:hyperspace', () => {
  return rollup
    .rollup({
      input: ['web/wwwroot/js/hyperspace.js'],
      ...rollupConfig,
    })
    .then((bundle) => {
      return bundle.write({
        file: 'web/wwwroot/js/hyperspace.min.js',
        ...rollupOutputConfig,
      });
    });
});

gulp.task('js:utility', () => {
  return rollup
    .rollup({
      input: [
        'web/wwwroot/js/utility/tabs.js',
        'web/wwwroot/js/utility/collapse.js',
        'web/wwwroot/js/utility/carousel.js',
        'web/wwwroot/js/utility/table.js',
        'web/wwwroot/js/utility/drag.js',
        'web/wwwroot/js/utility/reorder.js',
        'web/wwwroot/js/utility/modal.js',
        'web/wwwroot/js/utility/lazyload.js',
        'web/wwwroot/js/utility/crumbs.js',
        'web/wwwroot/js/favorites.js',
        'web/wwwroot/js/ajax-content.js',
        'web/wwwroot/js/notification.js',
        'web/wwwroot/js/mail.js',
        'web/wwwroot/js/utility/hamburger.js',
        'web/wwwroot/js/mini.js',
        'web/wwwroot/js/dropdown.js',
        'node_modules/chart.js/dist/Chart.js',
      ],
      ...rollupConfig,
    })
    .then((bundle) => {
      return bundle.write({
        file: 'web/wwwroot/js/utility.min.js',
        ...rollupOutputConfig,
      });
    });
});
gulp.task('js:analytics', () => {
  return rollup
    .rollup({
      input: ['web/wwwroot/js/analytics.js'],
      ...rollupConfig,
    })
    .then((bundle) => {
      return bundle.write({
        file: 'web/wwwroot/js/analytics.min.js',
        ...rollupOutputConfig,
      });
    });
});

gulp.task('js:profile', () => {
  return rollup
    .rollup({
      input: ['web/wwwroot/js/profile.js'],
      ...rollupConfig,
    })
    .then((bundle) => {
      return bundle.write({
        file: 'web/wwwroot/js/profile.min.js',
        ...rollupOutputConfig,
      });
    });
});

gulp.task('js:userSettings', () => {
  return rollup
    .rollup({
      input: ['web/wwwroot/js/user-settings.js'],
      ...rollupConfig,
    })
    .then((bundle) => {
      return bundle.write({
        file: 'web/wwwroot/js/user-settings.min.js',
        ...rollupOutputConfig,
      });
    });
});

gulp.task('js:tracker', () => {
  return rollup
    .rollup({
      input: ['web/wwwroot/js/tracker.js', 'node_modules/jsnlog/jsnlog.js'],
      ...rollupConfig,
    })
    .then((bundle) => {
      return bundle.write({
        file: 'web/wwwroot/js/alive.min.js',
        ...rollupOutputConfig,
      });
    });
});

gulp.task('js:highlighter', () => {
  return rollup
    .rollup({
      input: ['web/wwwroot/lib/highlight/highlight.js'],
      ...rollupConfig,
    })
    .then((bundle) => {
      return bundle.write({
        file: 'web/wwwroot/js/highlight.min.js',
        ...rollupOutputConfig,
      });
    });
});

gulp.task('js:shared', function () {
  // Shared functions should be imported as needed.
  return gulp
    .src(['web/wwwroot/js/shared.js'])
    .pipe(concat('shared.min.js'))
    .pipe(uglify(uglifyConfig))
    .pipe(gulp.dest('web/wwwroot/js/'));
});

gulp.task('js:search', () => {
  return rollup
    .rollup({
      input: ['web/wwwroot/js/search.js', 'web/wwwroot/js/error.js'],
      ...rollupConfig,
    })
    .then((bundle) => {
      return bundle.write({
        file: 'web/wwwroot/js/search.min.js',
        ...rollupOutputConfig,
      });
    });
});

gulp.task('js:settings', () => {
  return rollup
    .rollup({
      input: ['web/wwwroot/js/settings.js', 'web/wwwroot/js/access.js'],
      ...rollupConfig,
    })
    .then((bundle) => {
      return bundle.write({
        file: 'web/wwwroot/js/settings.min.js',
        ...rollupOutputConfig,
      });
    });
});

gulp.task('js:editor', () => {
  return rollup
    .rollup({
      input: [
        'web/wwwroot/js/editor.js',
        'web/wwwroot/js/utility/checkbox.js',
        'web/wwwroot/js/report-editor.js',
      ],
      ...rollupConfig,
    })
    .then((bundle) => {
      return bundle.write({
        file: 'web/wwwroot/js/editor.min.js',
        ...rollupOutputConfig,
      });
    });
});

gulp.task(
  'scripts',
  gulp.series(
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
    'js:userSettings',
    'js:hyperspace',
  ),
);
