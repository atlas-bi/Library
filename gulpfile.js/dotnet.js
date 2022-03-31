const log = require('fancy-log');
const gulp = require('gulp');
const path = require('path');

var dotnet, iis;
const { spawn } = require('child_process');

gulp.task('dotnet:build', function (cb) {
  if (dotnet) dotnet.kill();
  // if (iis) iis.kill();
  var env = Object.create(process.env);
  env.ASPNETCORE_ENVIRONMENT = 'Development';
  env.DOTNET_SYSTEM_CONSOLE_ALLOW_ANSI_COLOR_REDIRECTION = '1';
  // https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-build
  dotnet = spawn(
    'dotnet',
    ['build', 'web/web.csproj', '-c', 'Debug', '-v', 'm'],
    { env: env, detached: true },
  );
  dotnet.stderr.on('data', (data) => {
    log.error(data.toString().replace(/^\s+|\s+$/g, ''));
  });
  dotnet.stdout.on('data', (data) => {
    log.info(data.toString().replace(/^\s+|\s+$/g, ''));
  });
  dotnet.on('close', function (code) {
    if (code === 8) {
      log('Error detected, waiting for changes...');
    }
    // only return after completing
    cb();
  });
});

process.on('exit', () => {
  if (dotnet) dotnet.kill();
});
