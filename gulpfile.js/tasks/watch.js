// const { watch } = require('gulp');
// var path = require('path');

// watch(['input/*.js', '!input/something.js'], function(cb) {
//   // body omitted
//   cb();
// });

var gulp = require('gulp');
var path = require('path');

const paths = ({
    'style:scss': path.join('./web/wwwroot/static/**/*.scss'),
    'packages:fontawesome': path.join('./gulpfile.js/packages.js'),
});


gulp.task('watch', gulp.series('build', function (cb) {
    gulp.watch(paths['style:scss'], gulp.series('style:scss'));
    gulp.watch(paths['packages:fontawesome'], gulp.series('packages:fontawesome'));
}));
