const rename = require('gulp-rename');
const autoprefexer = require('gulp-autoprefixer');
const sass = require('gulp-sass')(require('sass-embedded'));
const postcss = require('gulp-postcss');
const purgecss = require('gulp-purgecss');
const cssnano = require('gulp-cssnano');
const postcssFocusWithin = require('focus-within/postcss');
const gulp = require('gulp');

gulp.task('css:email', function () {
  return gulp
    .src('web/wwwroot/css/email_theme.scss')
    .pipe(sass().on('error', sass.logError))
    .pipe(cssnano()) // Running this first to strip comments
    .pipe(
      purgecss({
        content: ['web/Pages/Shared/_EmailTemplate.cshtml'],
        safelist: [],
        fontFace: true,
        keyframes: true,
      }),
    )
    .pipe(autoprefexer())
    .pipe(rename('email.min.css'))
    .pipe(gulp.dest('web/wwwroot/css/'));
});

gulp.task('css:build', function () {
  const plugins = [postcssFocusWithin];
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
        safelist: [
          'breadcrumb',
          'is-active',
          'editor-liveEditorPrev',
          'analytics-reviewed',
          'epic-released',
          'legacy',
          'high-risk',
          'self-service',
          'analytics-certified',
        ],
      }),
    )
    .pipe(postcss(plugins))
    .pipe(autoprefexer())
    .pipe(cssnano())
    .pipe(rename('site.min.css'))
    .pipe(gulp.dest('web/wwwroot/css/'));
});

gulp.task('css:rejected', function () {
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
        safelist: [
          'breadcrumb',
          'is-active',
          'editor-liveEditorPrev',
          'analytics-reviewed',
          'epic-released',
          'legacy',
          'high-risk',
          'self-service',
          'analytics-certified',
        ],
        rejected: true,
      }),
    )
    .pipe(rename('.rejected'))
    .pipe(gulp.dest('web/wwwroot/css/'));
});

gulp.task('styles', gulp.parallel('css:build', 'css:email'));
