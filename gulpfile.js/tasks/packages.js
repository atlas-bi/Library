
const { task, series, parallel, src, dest } = require('gulp');
var fontawesomeSubset = require('fontawesome-subset');
var replace = require('gulp-replace');
var del = require('del');
const Font = require('fonteditor-core').Font;
const fs = require('fs');
const woff2 = require('fonteditor-core').woff2;

task('packages:handlebars', function() {
    return src('node_modules/handlebars/dist/handlebars.min.js').pipe(dest('web/wwwroot/vendor/handlebars/'));
});

task('packages:inter_font', function() {
  return src('node_modules/@fontsource/inter/**/*').pipe(replace(/\.\/files\//g, '/static/font/inter/files/')).pipe(dest('web/wwwroot/font/inter'))
});

task('packages:rasa_font', function() {
  return src('node_modules/@fontsource/rasa/**/*').pipe(replace(/\.\/files\//g, '/static/font/rasa/files/')).pipe(dest('web/wwwroot/font/rasa'))
});

// write a rasa woff font to ttf for python pillow user icons
task('packages:rasa_to_ttf', function(cb){

woff2.init().then(() => {
  // read
  let buffer = fs.readFileSync('node_modules/@fontsource/rasa/files/rasa-latin-600-normal.woff2');
  console.log(buffer)
  let font = Font.create(buffer, {
    type: 'woff2'
  });
  // write
  fs.writeFileSync('web/wwwroot/font/rasa/files/rasa-latin-600-normal.ttf', font.write({type: 'ttf'}));
});




  cb();
})

// build fontawesome
/*
 * run from atlas root::
 *
 * node scripts/fontawesome.js
 */

task('packages:fontawesome', function(done) {
    del.sync('web/wwwroot/font/fontawesome/webfonts', {force:true});
    fontawesomeSubset({
      regular:['envelope', 'thumbs-up', 'play-circle'],
      solid: ['sliders-h', 'wrench',  'user', 'list-ul', 'chevron-left', 'chevron-right', 'search', 'edit', 'trash', 'star', 'share', 'plus', 'chart-bar', 'universal-access']
          }, 'web/wwwroot/font/fontawesome/webfonts');

    done();
});

task('packages', parallel('packages:fontawesome', 'packages:handlebars', 'packages:inter_font', series('packages:rasa_font', 'packages:rasa_to_ttf')));
