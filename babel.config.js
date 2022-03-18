module.exports = function (api) {
  api.cache(true);
  const presets = [
    [
      '@babel/preset-env',
      {
        corejs: {
          version: '3',
          proposals: true,
        },
        useBuiltIns: 'usage',
      },
    ],
  ];
  const plugins = [['@babel/plugin-transform-runtime']];
  return {
    presets,
    plugins,
  };
};
