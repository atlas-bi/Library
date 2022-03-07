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
const log = require('fancy-log');
const path = require('path');
var dotnet, iis;
const { spawn } = require('child_process');
var fs = require('fs');

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

gulp.task('sass', function () {
  var plugins = [postcssFocusWithin];
  return gulp
    .src('web/wwwroot/css/theme.scss')
    .pipe(sass().on('error', sass.logError))
    .pipe(
      purgecss({
        content: [
          'web/Pages/**/*.cshtml',
          'web/wwwroot/js/**/*.js',
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

gulp.task('js:settings', function () {
  return gulp
    .src(['web/wwwroot/js/settings.js', 'web/wwwroot/js/access.js'])
    .pipe(concat('settings.min.js'))
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

gulp.task('dotnet:build', function (cb) {
  if (dotnet) dotnet.kill();
  if (iis) iis.kill();
  var env = Object.create(process.env);
  env.ASPNETCORE_ENVIRONMENT = 'Development';
  // https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-build
  dotnet = spawn(
    'dotnet',
    ['build', 'web/web.csproj', '-c', 'Debug', '-v', 'm'],
    { stdio: 'inherit', env: env },
  );
  dotnet.on('close', function (code) {
    if (code === 8) {
      log('Error detected, waiting for changes...');
    }
    cb();
  });
});

gulp.task('iis:start', function (cb) {
  if (iis) iis.kill('SIGINT');
  var env = Object.create(process.env);
  env.LAUNCHER_PATH = 'bin/Debug/net6.0/Atlas_Web.exe';
  env.ASPNETCORE_ENVIRONMENT = 'Development';
  env.ENVIRONMENT = 'Development';
  iis = spawn(
    'C:/Program Files/IIS Express/iisexpress.exe',
    [
      '/config:iis_express.config',
      '/site:web',
      '/apppool:Clr4IntegratedAppPool',
      '/trace:i',
    ],
    { stdio: 'inherit', env: env },
  );
  iis.on('SIGTERM', function (code) {
    if (code === 8) {
      log('Error detected, waiting for changes...');
    }
    cb();
  });
  iis.on('kill', function (code) {
    if (code === 8) {
      log('Error detected, waiting for changes...');
    }
    cb();
  });
  cb();
});

gulp.task('iis:clean_logs', function (cb) {
  const directory = path.join(__dirname, 'web');
  fs.readdir(directory, (err, files) => {
    files.forEach((file) => {
      if (file.endsWith('.log'))
        fs.unlink(path.join(directory, file), (err) => {
          if (err) {
            log(err);
            return;
          }
        });
    });
  });
  cb();
});

gulp.task('iis:run', gulp.series('dotnet:build', 'iis:start'));

gulp.task(
  'build',
  gulp.parallel(
    'font:inter',
    'font:rasa',
    'package:markdown-it',
    'js:utility',
    'js:polyfill',
    'js:editor',
    'js:settings',
    'js:shared',
    gulp.series('iis:clean_logs', 'iis:run'),
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
      gulp.parallel(
        'js:settings',
        'js:shared',
        'js:editor',
        'js:utility',
        'sass',
      ),
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
    gulp.watch(
      [
        'web/Pages/**/*.cshtml',
        'web/Pages/**/*.cshtml.cs',
        'web/Helpers**/*.cs',
        'web/Middleware**/*.cs',
        'web/Models/**/*.cs',
        'web/Properties/**/*.json',
        'web/appsettings*.json',
        'web/Program.cs',
        'web/Startup.cs',
        'web/web/csproj',
        'web/**/*.json',
        'web/wwwroot/js/polyfill/**/*.js',
        'web/font/fontawesome/**/*.scss',
        'web/wwwroot/**/*.js',
        '!web/wwwroot/**/*.min.js',
        'web/wwwroot/**/*.scss',
        'web/wwwroot/**/*.sass',
      ],
      gulp.series('iis:run'),
    );
    cb();
  }),
);

gulp.task(
  'watch:static',
  gulp.series('build', function (cb) {
    gulp.watch('web/wwwroot/**/*.scss', gulp.series('sass'));
    gulp.watch('web/wwwroot/**/*.sass', gulp.series('sass'));
    gulp.watch(
      ['web/wwwroot/**/*.js', '!web/wwwroot/**/*.min.js'],
      gulp.parallel(
        'js:settings',
        'js:shared',
        'js:editor',
        'js:utility',
        'sass',
      ),
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

function exitHandle() {
  if (dotnet) dotnet.kill();
  if (iis) iis.kill();
}

// process.on('SIGHUP', exitHandle);
// process.on('SIGINT', exitHandle);
// process.on('SIGTERM', exitHandle);
process.on('exit', exitHandle);

process.on('SIGINT', function () {
  process.exit();
});
process.on('SIGTERM', function () {
  process.exit();
});
