<h1 align="center">
    <br>
    <a href="https://www.atlas.bi">
        <img alt="atlas logo" src="https://raw.githubusercontent.com/atlas-bi/website/master/src/static/img/atlas-logo.png" width=420 />
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
    <a href="https://codeclimate.com/github/atlas-bi/atlas-bi-library/maintainability" target="_blank"><img alt="maintainablity badge" src="https://api.codeclimate.com/v1/badges/71150ba85b7f08fd9ae9/maintainability" /></a>
<a href="https://codecov.io/gh/atlas-bi/atlas-bi-library" target="_blank">
  <img alt="coverage badge" src="https://codecov.io/gh/atlas-bi/atlas-bi-library/branch/master/graph/badge.svg?token=p1fWnQvPnf"/>
</a>
<a href="https://github.com/atlas-bi/atlas-bi-library/actions/workflows/test.yaml" target="_blank"><img src="https://github.com/atlas-bi/atlas-bi-library/actions/workflows/test.yaml/badge.svg" /></a>
</p>

## :runner: Getting Started

-   Get the code `git clone git@github.com:atlas-bi/atlas-bi-library.git`
-   Install the project dependencies `npm install` and `dotnet restore`
-   Create an `appsettings.cust.json` and `appsettings.cust.Development.json` as specified in the [docs](git@github.com:atlas-bi/atlas-bi-library.git)
-   Create a `.env` file with `NODE_ENV=development`
-   Initialize the database and create tables `dotnet ef database update`
-   Finally, start up the website `npm start`

Running `npm start` will build all the resources needed, start IISExpress, and then open your browser.

Debugging code is simple. In Visual Studio Code, simply press `f5` to activate breakpoints. Saving any file will trigger a rebuild.

## :trophy: Credits

Atlas was created by the Riverside Healthcare Analytics team. See the [credits](https://www.atlas.bi/about/) for more details.
