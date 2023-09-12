<h1 align="center">
    <br>
    <a href="https://www.atlas.bi">
        <img alt="atlas logo" src="https://avatars.githubusercontent.com/u/90376906?v=4" width=120 /> <span style="color:grey"> / </span> Atlas
    </a>
    <br>
</h1>
<p align="center">
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 220 25" fill="none">
<style>
.text {font:16px sans-serif; user-select:none;fill:rgb(51 65 85)}
.slash {font:20px sans-serif; user-select:none;fill:rgb(203 213 225)}
.slash:hover, *:hover > .slash {fill:rgb(14, 165, 233);}

</style>
 <image xlink:href="https://avatars.githubusercontent.com/u/90376906?v=4" width=20 x=0 y=0></image>
<text x="23" y="17" class="slash">/</text>
<text x="33" y="16" class="text">library</text>
</svg></p>
https://avatars.githubusercontent.com/u/90376906?s=400&u=7592339d75e23b1e5b5db486c48271f6491d1422&v=4
<h4 align="center">Atlas BI Library | The unified report library.</h4>

<p align="center">
    <a href="https://www.atlas.bi" target="_blank">Website</a> • <a href="https://demo.atlas.bi" target="_blank">Demo</a> • <a href="https://www.atlas.bi/docs/bi-library/" target="_blank">Documentation</a> • <a href="https://discord.gg/hdz2cpygQD" target="_blank">Chat</a>
</p>

<p align="center">
Atlas business intelligence library plugs in to your existing reporting platforms, extracts useful metadata, and displays it in a unified report library where you can easily search for, document, and launch reports.
</p>

<p align="center">
    <a href="https://www.codacy.com/gh/atlas-bi/atlas-bi-library/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=atlas-bi/atlas-bi-library&amp;utm_campaign=Badge_Grade" target="_blank"><img alt="codacy badge" src="https://app.codacy.com/project/badge/Grade/45f8f86fdb9847d98274d6ee9d3ab850" /></a>
<a href="https://codecov.io/gh/atlas-bi/atlas-bi-library" target="_blank">
  <img alt="coverage badge" src="https://codecov.io/gh/atlas-bi/atlas-bi-library/branch/master/graph/badge.svg?token=p1fWnQvPnf"/>
</a>
<a href="https://github.com/atlas-bi/atlas-bi-library/actions/workflows/test.yaml" target="_blank"><img src="https://github.com/atlas-bi/atlas-bi-library/actions/workflows/test.yaml/badge.svg" /></a>
<a href="https://discord.gg/hdz2cpygQD"><img alt="discord chat" src="https://badgen.net/discord/online-members/hdz2cpygQD/" /></a>
<a href="https://github.com/atlas-bi/atlas-bi-library/releases"><img alt="latest release" src="https://badgen.net/github/release/atlas-bi/atlas-bi-library" /></a>
</p>

## 🏃 Getting Started

> Atlas BI Library is built with [.Net 7.0](https://dotnet.microsoft.com/download/dotnet/7.0) and [Node](https://nodejs.org/en/download/). \
> Aside from those installs you will need to install ef core tools `dotnet tool install -g dotnet-ef`. \
> These guide can be run with [Visual Studio Code](https://code.visualstudio.com/download) and the built in terminal.

-   Get the code `git clone git@github.com:atlas-bi/atlas-bi-library.git`
-   Install the project dependencies `npm install` and `npm run dotnet:restore`
-   Create an `appsettings.cust.json` and `appsettings.cust.Development.json` as specified in the [docs](https://www.atlas.bi/docs/bi-library/deploy/configuration/)
-   Initialize the database and create tables `npm run db:update`
-   Run the [ETL](https://www.atlas.bi/docs/bi-library/deploy/configuration/), or just insert your account name into the `dbo.[User]` `username` column.
-   Finally, start up the website `npm start`

> If this is your first time running a dotnet webapp, you will need to trust the cert with `dotnet dev-certs https --trust` \
> Running `npm start` will build all the resources needed, start IISExpress, and then open your browser. \
> Debugging code is simple. In Visual Studio Code, simply press `f5` to activate breakpoints. Saving any file will trigger a rebuild.

## 🧪 Testing

Tests require two global dotnet tools.

```bash
dotnet tool install -g coverlet.console
dotnet tool install -g dotnet-reportgenerator-globaltool
```

-   Install dependancies `npm install` and `npm run dotnet:restore`
-   Run tests `npm run test:dev`

A hit/miss html report will be in the folder `/coverage`.

## 🚀 Deploy to IIS

Build the release with `npm run dotnet:publish`.

Copy the output of `/out` to the folder of your IIS site.

See the [docs](https://www.atlas.bi/docs/bi-library/) for aditional configuration and starting search.

## 🎁 Contributing

Contributions are welcome! Please open an [issue](https://github.com/atlas-bi/atlas-bi-library/issues) describing an issue or feature.

This repository uses commitizen. Commit code changes for pr's with `npm run commit`.

## 🏆 Credits

Atlas was originally created and made open source by the Riverside Healthcare Analytics team. See the [credits](https://www.atlas.bi/about/) for more details.

## 🔧 Tools

Special thanks to a few other tools used here.

<a href="https://automate.browserstack.com/public-build/bGhJNzFxaXI1MFFONmh2TlQwdW5MQXNyblFtYXorbEQxdU4wNnpqWFYzWT0tLVN1L2l1Mi9ueGFXQ0hIYmUxWll2c2c9PQ==--0a7425816259714011cafee8777c3fe2e15baaba"><img src='https://automate.browserstack.com/badge.svg?badge_key=bGhJNzFxaXI1MFFONmh2TlQwdW5MQXNyblFtYXorbEQxdU4wNnpqWFYzWT0tLVN1L2l1Mi9ueGFXQ0hIYmUxWll2c2c9PQ==--0a7425816259714011cafee8777c3fe2e15baaba'/></a>
<img src="https://badgen.net/badge/icon/gitguardian?icon=gitguardian&label" alt="gitguardian"> <img src="https://img.shields.io/badge/renovate-configured-green?logo=renovatebot" alt="renovate"> <a href="https://snyk.io/test/github/atlas-bi/atlas-bi-library"><img src="https://snyk.io/test/github/atlas-bi/atlas-bi-library/badge.svg" alt="snyk" /></a> <a href="https://sonarcloud.io/summary/new_code?id=atlas-bi_atlas-bi-library"><img src="https://sonarcloud.io/api/project_badges/measure?project=atlas-bi_atlas-bi-library&metric=alert_status" alt="quality gate sonar" /></a> <a href="http://commitizen.github.io/cz-cli/"><a src="https://img.shields.io/badge/commitizen-friendly-brightgreen.svg" alt="commitizen"></a>
<a href="https://github.com/semantic-release/semantic-release"><img src="https://img.shields.io/badge/%20%20%F0%9F%93%A6%F0%9F%9A%80-semantic--release-e10079.svg" alt="semantic-release" /></a> [![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=atlas-bi_atlas-bi-library&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=atlas-bi_atlas-bi-library)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=atlas-bi_atlas-bi-library&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=atlas-bi_atlas-bi-library)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=atlas-bi_atlas-bi-library&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=atlas-bi_atlas-bi-library)
