
var postcss = require('gulp-postcss');
var gulp = require('gulp');
var autoprefixer = require('autoprefixer');
var cssnano = require('cssnano');
var sass = require('gulp-sass')(require('sass'));
var sortMediaQueries = require('postcss-sort-media-queries');
var purgecss = require('gulp-purgecss');

gulp.task('style:scss', function() {
  var plugins = [
        sortMediaQueries(),
        autoprefixer(),
        cssnano(),
    ];

  return gulp.src('./web/wwwroot/scss/**/*.scss')
      .pipe(sass().on('error', sass.logError))
      .pipe(postcss(plugins))
      // .pipe(purgecss({
      //       content: ['./**/*.html.dj'],
      //       safelist: {
      //         deep: [/breadcrumb/],
      //       }
      //     }))
      .pipe(gulp.dest('./web/wwwroot/css/'));

});


gulp.task('style', gulp.parallel('style:scss'));
