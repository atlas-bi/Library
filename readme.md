<h1 align="center">
    <br>
    <a href="https://www.atlas.bi">
        <img alt="atlas logo" src="https://raw.githubusercontent.com/atlas-bi/atlas-bi-library/master/web/wwwroot/img/atlas-logo-smooth.png" width=420 />
    </a>
    <br>
</h1>

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
</p>

## :runner: Getting Started

> Atlas BI Library is built with [.Net 6.0](https://dotnet.microsoft.com/download/dotnet/6.0) and [Node](https://nodejs.org/en/download/). \
> Aside from those installs you will need to install ef core tools `dotnet tool install -g dotnet-ef`. \
> These guide can be run with [Visual Studio Code](https://code.visualstudio.com/download) and the built in terminal.

-   Get the code `git clone git@github.com:atlas-bi/atlas-bi-library.git`
-   Install the project dependencies `npm install` and `npm run dotnet:restore`
-   Create an `appsettings.cust.json` and `appsettings.cust.Development.json` as specified in the [docs](git@github.com:atlas-bi/atlas-bi-library.git)
-   Create a `.env` file with `NODE_ENV=development`
-   Initialize the database and create tables `npm run db:update`
-   Finally, start up the website `npm start`

> If this is your first time running a dotnet webapp, you will need to trust the cert with `dotnet dev-certs https --trust` \
> Running `npm start` will build all the resources needed, start IISExpress, and then open your browser. \
> Debugging code is simple. In Visual Studio Code, simply press `f5` to activate breakpoints. Saving any file will trigger a rebuild.

## :test_tube: Testing

Tests require two global dotnet tools.

```bash
dotnet tool install -g coverlet.console
dotnet tool install -g dotnet-reportgenerator-globaltool
```

-   Install dependancies `npm install` and `npm run dotnet:restore`
-   Run tests `npm run test:dev`

A hit/miss html report will be in the folder `/coverage`.

## :rocket: Deploy to IIS

Build the release with `npm run dotnet:publish`.

The output of `/bin/Release/net6.0/publish` to the folder of your IIS site.

See the [docs](https://www.atlas.bi/docs/bi-library/) for aditional configuration and starting search.

## :trophy: Credits

Atlas was created by the Riverside Healthcare Analytics team. See the [credits](https://www.atlas.bi/about/) for more details.
