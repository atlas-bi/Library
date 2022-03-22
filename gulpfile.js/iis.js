const fs = require('fs');
const gulp = require('gulp');
const path = require('path');
const { spawn } = require('child_process');
const log = require('fancy-log');
//var ks = require('node-key-sender');
var robot = require("@jitsi/robotjs");
var iis;

gulp.task('iis:kill', function (cb) {
  var kill = spawn('taskkill', ['/IM', 'IISExpress.exe', '/F'], { detached: true });
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
  env.ATLAS_PATH = path.join(__dirname, "../web");

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
  //ks.sendKey('f5');
  robot.keyTap("f5")
  log.warn(`
############################################################################################

ooooooooo.                                              .o88o.   oooooooo
'888   'Y88.                                            888 '"  dP"""""""
 888   .d88' oooo d8b  .ooooo.   .oooo.o  .oooo.o      o888oo  d88888b.
 888ooo88P'  '888""8P d88' '88b d88(  "8 d88(  "8       888        'Y88b
 888          888     888ooo888 '"Y88b.  '"Y88b.        888          ]88
 888          888     888    .o o.  )88b o.  )88b       888    o.   .88P
o888o        d888b    'Y8bod8P' 8""888P' 8""888P'      o888o   '8bd88P'



 .o88o.                                                                        .o8
 888 '"                                                                       "888
o888oo   .ooooo.  oooo d8b      oooo    ooo  .oooo.o  .ooooo.   .ooooo.   .oooo888   .ooooo.
 888    d88' '88b '888""8P       '88.  .8'  d88(  "8 d88' '"Y8 d88' '88b d88' '888  d88' '88b
 888    888   888  888            '88..8'   '"Y88b.  888       888   888 888   888  888ooo888
 888    888   888  888             '888'    o.  )88b 888   .o8 888   888 888   888  888    .o
o888o   'Y8bod8P' d888b             '8'     8""888P' 'Y8bod8P' 'Y8bod8P' 'Y8bod88P" 'Y8bod8P'



      .o8             .o8
     "888            "888
 .oooo888   .ooooo.   888oooo.  oooo  oooo   .oooooooo  .oooooooo  .ooooo.  oooo d8b
d88' '888  d88' '88b  d88' '88b '888  '888  888' '88b  888' '88b  d88' '88b '888""8P
888   888  888ooo888  888   888  888   888  888   888  888   888  888ooo888  888
888   888  888    .o  888   888  888   888  '88bod8P'  '88bod8P'  888    .o  888
'Y8bod88P" 'Y8bod8P'  'Y8bod8P'  'V88V"V8P' '8oooooo.  '8oooooo.  'Y8bod8P' d888b
                                            d"     YD  d"     YD
                                            "Y88888P'  "Y88888P'

############################################################################################
  `)
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
