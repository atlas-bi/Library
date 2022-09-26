module.exports = {
  ci: {
    upload: {
      target: 'lhci',
      serverBaseUrl: 'https://lighthouse.atlas.bi',
    },
    assert: {
      preset: 'lighthouse:no-pwa',
      assertions: {
        'color-contrast': 'warn',
        'is-crawlable': 'off',
        redirects: 'off',
        'robots-txt': 'off',
        'csp-xss': 'warn',
        'unused-css-rules': 'warn',
        'tap-targets': 'warn',
        'third-party-facades': 'warn',
        'unused-javascript': 'warn',
        'uses-responsive-images': 'warn',
        'uses-text-compression': 'warn',
        'crawlable-anchors': 'warn',
        label: 'warn',
        'link-name': 'warn',
        'heading-order': 'warn',
        'aria-allowed-attr': 'warn',
        'image-size-responsive': 'warn',
        'meta-description': 'off',
        'unsized-images': 'warn',
      },
    },
    collect: {
      startServerCommand: 'dotnet run --project web/web.csproj',
      url: ['http://localhost:5000'],
      maxAutodiscoverUrls: 10,
      settings: {
        hostname: '127.0.0.1',
      },
    },
  },
};
