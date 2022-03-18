const fs = require('fs');
const gulp = require('gulp');
const path = require('path');
const { spawn } = require('child_process');
const log = require('fancy-log');
var iis;

gulp.task('iis:kill', function (cb) {
  var kill = spawn('taskkill', ['/IM', 'IISExpress.exe'], { detached: true });
  kill.stderr.on('data', (data) => {
    log.error(data.toString().replace(/^\s+|\s+$/g, ''));
  });
  kill.stdout.on('data', (data) => {
    log.info(data.toString().replace(/^\s+|\s+$/g, ''));
  });

  kill.on('close', function (code) {
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
    { env: env, detached: true },
  );
  iis.stderr.on('data', (data) => {
    log.error(data.toString().replace(/^\s+|\s+$/g, ''));
  });
  iis.stdout.on('data', (data) => {
    log.info(data.toString().replace(/^\s+|\s+$/g, ''));
  });

  iis.on('close', function (code) {
    if (code === 8) {
      log('Error detected, waiting for changes...');
    }
  });
  cb();
});

gulp.task('iis:clean_logs', function (cb) {
  const directory = path.join(__dirname, '../web');
  fs.readdir(directory, (err, files) => {
    files.forEach((file) => {
      if (file.endsWith('.log'))
        fs.unlink(path.join(directory, file), (err) => {
          if (err) {
            log.error(err);
            return;
          }
        });
    });
  });
  cb();
});

/* needed to close down IIS properly when hitting control + c */
process.on('exit', () => {
  if (iis) iis.kill();
});
process.on('SIGINT', function () {
  process.exit();
});
process.on('SIGTERM', function () {
  process.exit();
});
