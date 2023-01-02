const log = require('fancy-log');
const gulp = require('gulp');

let dotnet;
const { spawn } = require('child_process');

gulp.task('dotnet:run', function (cb) {
  if (dotnet) dotnet.kill();
  const env = Object.create(process.env);
  env.ASPNETCORE_ENVIRONMENT = 'Development';
  env.DOTNET_SYSTEM_CONSOLE_ALLOW_ANSI_COLOR_REDIRECTION = '1';
  env.DOTNET_CLI_TELEMETRY_OPTOUT = '1';
  // https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-build
  dotnet = spawn(
    'dotnet',
    ['run', '--project', 'web/web.csproj', '-c', 'Debug', '-v', 'm'],
    { env },
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
  });
  // Return before completing
  cb();
});

gulp.task('dotnet:build', function (cb) {
  if (dotnet) dotnet.kill();
  // somehow the hot reload on dotnet watch does not work, so we stick w/ dotnet build.
  const env = Object.create(process.env);
  env.ASPNETCORE_ENVIRONMENT = 'Development';
  env.DOTNET_SYSTEM_CONSOLE_ALLOW_ANSI_COLOR_REDIRECTION = '1';
  // https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-build
  dotnet = spawn(
    'dotnet',
    ['build', 'web/web.csproj', '-c', 'Debug', '-v', 'm'],
    { env, detached: true },
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
    // Only return after completing
    cb();
  });
});

process.on('exit', () => {
  if (dotnet) dotnet.kill();
});
