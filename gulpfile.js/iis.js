const fs = require('fs');
const gulp = require('gulp');
const path = require('path');
const { spawn } = require('child_process');
const log = require('fancy-log');
var iis;

gulp.task('iis:kill', function (cb) {
  var kill = spawn('taskkill', ['/IM', 'IISExpress.exe', '/F'], {
    detached: true,
  });
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
  env.ATLAS_PATH = path.join(__dirname, '../web');

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
  log.warn(`
################################################################################
       .o88o.   oooooooo      .o8             .o8
       888 '"  dP"""""""     "888            "888
       o888oo  d88888b.   .oooo888   .ooooo.   888oooo.  oooo  oooo   .oooooooo
       888        'Y88b  d88' '888  d88' '88b  d88' '88b '888  '888  888' '88b
       888          ]88  888   888  888ooo888  888   888  888   888  888   888
       888    o.   .88P  888   888  888    .o  888   888  888   888  '88bod8P'
      o888o   '8bd88P'   'Y8bod88P" 'Y8bod8P'  'Y8bod8P'  'V88V"V8P' '8oooooo.
                                                                      d"     YD
                                                                      "Y88888P'
################################################################################
  `);
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
