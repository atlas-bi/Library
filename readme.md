# Atlas Documentation

Atlas of Information Management is a business intelligence library and documentation database. ELT processes collect metadata from various reporting platforms and store it in a centraly located database. A modern web UI is used to add additional documentation to the report objects and also to provide an intuative way to search, favorite and share reporting content.


<img width="800" src="/docs/images/atlas-report.png" alt="atlas-report"/>

## Live Demo
> :point_right: **[Live Atlas Demo with Docker](https://hub.docker.com/r/christopherpickering/rmc-atlas-demo)**

The live demo is running with Docker and Ubuntu 16.04. Check out the Dockerfile for build steps.

The demo can be run locally with our public docker image -
```sh
docker run -i -t -p 1234:1234 -e PORT=1234  -u 0 christopherpickering/rmc-atlas-demo:latest
```
or, you can clone this repository and build your own docker image by running these basic commands -
```sh
docker build  --tag atlas_demo .
docker run -i -t -p 1234:1234 -e PORT=1234  -u 0 atlas_demo:latest
# web app will be running @ http://localhost:1234
# see Dockerfile for db access
```

### Atlas can be run in an [online sandbox](https://labs.play-with-docker.com/) - it does require a docker.com login.

1. Click "start"
2. Click "Settings" > 1 Manager and 1 Worker
3. Click on the Manager instance. Atlas is large and doesn't run in the worker.
4. Paste in ```docker run -i -t -p 1234:1234 -e PORT=1234  -u 0 christopherpickering/rmc-atlas-demo:latest```
5. Wait about 1-2 mins for app to download and startup. Output will say ```Now listening on: http://[::]:1234``` when ready.
6. Click "Open Port" and type ```1234```
7. App will open in new tab. The URL should be valid for 3-4 hrs.

## Credits

Atlas was created by the Riverside Healthcare Analytics team -

* Paula Courville
* [Darrel Drake](https://www.linkedin.com/in/darrel-drake-57562529)
* [Dee Anna Hillebrand](https://github.com/DHillebrand2016)
* [Scott Manley](https://github.com/Scott-Manley)
* [Madeline Matz](mailto:mmatz@RHC.net)
* [Christopher Pickering](https://github.com/christopherpickering)
* [Dan Ryan](https://github.com/danryan1011)
* [Richard Schissler](https://github.com/schiss152)
* [Eric Shultz](https://github.com/eshultz)

## Getting Started

### Requirements
1. SQL Server Database (we use 2016 or newer, any license type) with Full Text Index installed
2. IIS Webserver with Microsoft .NET Core SDK 2.2.105 (x86)
3. Dev Machine with the following
    * Visual Studio 2017 or later
      * Analysis Service
      * SSIS Integration Services
      * ASP.net and webdevelopment component
      * Razor Language Pack component
      * SSDT Tools
    * Python 3.7, virtualenv
    * Active Directory Explorer, or other access to Active Directory
    * [Microsoft .NET Core SDK 2.2.105 (x86)](https://dotnet.microsoft.com/download/dotnet-core/2.2)

Atlas can run on any server OS that is capable of running Dotnet 2.2, and the database on any server that is capable of running Sql Server + Full Text Index.

### Steps to Run
1. Run database create scripts (LDAP, Data Governance, DG Staging). Set Datagov user credentials in database.
2. If you want to add demo data you can run the Data Governance database seeding script
2. Configure and run python LDAP script. Modify script and settings.py to match your LDAP build. Schedule in SQL Agent Jobs
3. Configure and run Atlas ETL's (main ETL and run data)
    * Delete SSRS sections if not used
    * Update Clarity server and credentials
    * Update Database connection strings
    * Schedule ETL's in SQL Agent Jobs
4. Populate Atlas/appsettings.json & appsettings.Development.json
5. Run website locally in Visual Studio
6. Update publish settings & publish

### Testing

Test are run using pythons + selenium. Each section of the webpage has its own test script, but all can be run at the same time by running /testing/master_test.py.

## About the Web UI

The app is built using C# Razor Pages, HTML, CSS and Javascript.

The app is currently using SQL Server, with a "database first" model.

### Database

The database can be created by running the script "Data Governance WebaApp/DatabaseCreationScript.sql".

Updates to the database are pulled in with the following scafold commmand:

```
# after running this commande we need to manually delete connection string from db context file
Scaffold-DbContext "Server=rhbidb01;Database=Data_Governance_Dev;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -force

```

Upon DB changes the file "webapp/DatabaseCreationScript.sql" should be updated. This can be done in SSMS by right clicking the db > clicking tasks > script database.

Customizations for a model are created in the model/customizations/ folder.

### CSS
Attempt made to follow wise guidance from www.maintainablecss.com. ::smile::

There is a primary css file (```main.min.css```) containing items used on ever page (or could be used on every page, like search), this is loaded immediately. There is a secondary css file (```differed.min.css```) that is loaded async. The visible stuff on the page should not rely on this file - it should contain animations, hidden items, etc.

Any page specific css can be inlined.

### Javascript

Javascript is broken up in to mini self executing functions. These are compliled into a few files -

* a primary file that contains only what is required to render all pages correctly
* a utility file containing functions required on all pages, but not used during render
* many smalll files with functions used for "fancy features" that are loaded "as needed"
* a unique files containing code only used in one section of the site. These files are then either lazy loaded or inlined into the page, depending on when they are required

In order to lazyload javascript and still keep the file version identifier we use a textarea and then load the contents of it as scripts.

Place any scripts to lazyload inside ```<textarea class="postLoadScripts"></textarea>``` tags.

### Tabs

A typical tab layout -
```html
<!-- tab control -->
<div class="tab">
    <a class="tab-lnk active" href="#tabone">Tab One</a>
    <a class="tab-lnk" href="#tabtwo">Tab Two</a>
</div>
<!-- tab body -->

<div class="tab-cnt">
    <div class="tab-dta active" id="tabone">
        Contents of Tab One
    </div>
    <div class="tab-dta" id="tabtwo">
        Contents of Tab Two
    </div>
</div>
```

### Tables
A table is created using this syntax - classes are optional.
```html
<div class="table-responsive">
<table class="table sort table-border">
  <thead>
    <tr>
      <th>head</th>
    </tr>
  <tbody>
    <tr>
      <td>cell</td>
    </tr>
  </tbody>
</table>
```

### Links
Links are simple created with an href and left classless. Style are applied at the tag level. When possile use links that are not Razor Pages dependent.
```html
<a href="\Reports?id=@this.id">
```

### Collapse
A typical collapse -
```html
<h1 data-toggle="clps" data-target="collapse-one">Collapse Link</h1>

<div id="collapse-one" class="clps" role="tabpanel" >
  Collapse contents go here.
</div>
```

### Carousel
A carousel is created with a basic outline -
```html
div.crsl
  div.crsl-itm
  div.crsl-itm
  div.crsl-ind
    span.crsl-dot
    span.crsl-dot
  a.crsl-prev
a.crsl-next
```

### Charts
A chart is created with json inside a script tag -
```html
<script type="application/json">
a = {
  axis: {
    0: {
      title: 'hits'
    },
    1: {
      title: 'seconds'
    },
    2: {
      title: 'clicks'
    }
  },
  type: 'bar',
  height: 400,
  title: "Cool Chart",
  data: [{
    title: "Run Time (s)",
    axis: 0,
    type: "bar",
    units: "(s)",
    data: [{
      title: "Stack 1",
      data: 12
    }]
  }, {
    title: "Runs",
    type: "bar",
    axis: 1,
    data: [{
      title: "Stack 1",
      data: 641
    }],
  }, {
    title: "Clicks",
    type: "line",
    axis: 2,
    data: [{
      title: "Stack 1",
      data: 61
    }],
  }],
  options: {
    legend: false
  }
</script>
```

### Progress bar
A progress bar exists on all pages and can be triggered with javascript events -
```js
document.dispatchEvent(new CustomEvent('progress-reset'));
document.dispatchEvent(new CustomEvent('progress-start'));
document.dispatchEvent(new CustomEvent('progress-finish'));
```
or to specify a value besides 100% to get to

```js
document.dispatchEvent(new CustomEvent('progress-start', {
                                cancelable: true,
                                detail: {
                                    value: 90 //max %
                                }
                            }));
```

### Tooltips
Tooltips can be added to any item and will appear to the left on hover.
```html
<a href="#" data-tooltip="Nice Tip!">Link to nowhere</a>
```

To make the tooltip appear above the elementy add a class ```tt-top```.
```html
<a class="tt-top" href="#" data-tooltip="Nice Tip!">Link to nowhere</a>
```

### Banner Messages

Add a global site paramter in the webapp settings > global settings.
Name: msg
Description: <Message Text>
Value: <id used to show message>

Show message by adding the message ID to the url:

www.atlas.com?msg=1

To have a sitewide message that does not require a URL param, leave the ```Value``` box blank when creating the param.

### Editor

Most editor description type fields support markdown. Most description fields also support flow charts from flowchart.js, wrapped in tripple \` with mermaid tag. 
```
\```mermaid
\```
```

### Ajax content

Content can be dynamically loaded by creating a div with a few attributes. The returned content *must* be wrapped in a single tag.
```html
<div data-ajax="yes" data-url="pate/to/page" data-param="stuff to append to url" data-loadtag="optional tag to load from response"></div>
```
Add the "cache" attribute to attempt to locally cache the data. The data will cache for 30 mins.

The dynamic content must be wrapped with a single element as it will completely replace the original element. The data attributes will be copied to the new content so that it will be possible to refresh it later on.


## ETL

There are two ETL's in the project. The first (ETL) brings in the report & user metadata. The second ETL (ETL-Run Data) brings in the run data.

User data is imported from LDAP, Hyperspace and SSRS servers. You can scheduled the scripts from /LDAP to run daily on your SQL Server.


## Hyperspace

Linking to Atlas from a Dashboard
1. Create a link component
2. Select a source of Component Editor
3. Select an Activity type link
4. Give your link a label
5. In the parameters, list the Atlas activity, and then runparams:<URL of the content> (like below)
```
ATLAS_WEBSITE,RunParams:https://my_host.net/Reports?id=73740
```
=======
# Atlas Documentation

Atlas of Information Management is a business intelligence library and documentation database. ELT processes collect metadata from various reporting platforms and store it in a centraly located database. A modern web UI is used to add additional documentation to the report objects and also to provide an intuative way to search, favorite and share reporting content.

<link rel="stylesheet" href="https://analyticsgit.riversidehealthcare.net/Data_Governance/atlas-information-management/-/raw/master/Data%20Governance%20WebApp/wwwroot/css/utility/carousel.css">
<script></script>
<div class="crsl">
  <div class="crsl-itm">
    <img src="/docs/images/atlas-report.png">
  </div>
  <div class="crsl-itm">
    <img src="/docs/images/atlas-analytics.png">
  </div>
  <div class="crsl-itm">
    <img src="/docs/images/atlas-home.png">
  </div>
  <div class="crsl-itm">
    <img src="/docs/images/atlas-mail.png">
  </div>
  <div class="crsl-itm">
    <img src="/docs/images/atlas-parameters.png">
  </div>
  <div class="crsl-itm">
    <img src="/docs/images/atlas-report-editor-2.png">
  </div>
  <div class="crsl-itm">
    <img src="/docs/images/atlas-report-editor.png">
  </div>
  <div class="crsl-itm">
    <img src="/docs/images/atlas-profile.png">
  </div>
  <div class="crsl-itm">
    <img src="/docs/images/atlas-profile-2.png">
  </div>
  <div class="crsl-itm">
    <img src="/docs/images/atlas-report.png">
  </div>
  <div class="crsl-itm">
    <img src="/docs/images/atlas-security-2.png">
  </div>
  <div class="crsl-itm">
    <img src="/docs/images/atlas-security.png">
  </div>
  <div class="crsl-itm">
    <img src="/docs/images/atlas-tasks.png">
  </div>
  <div class="crsl-itm">
    <img src="/docs/images/atlas-terms.png">
  </div>
  <div class="crsl-itm">
    <img src="/docs/images/atlas-user-2.png">
  </div>
  <div class="crsl-itm">
    <img src="/docs/images/atlas-user.png">
  </div>

  <div class="crsl-ind">
    <span class="crsl-dot"></span>
    <span class="crsl-dot"></span>
    <span class="crsl-dot"></span>
    <span class="crsl-dot"></span>
    <span class="crsl-dot"></span>
    <span class="crsl-dot"></span>
    <span class="crsl-dot"></span>
    <span class="crsl-dot"></span>
    <span class="crsl-dot"></span>
    <span class="crsl-dot"></span>
    <span class="crsl-dot"></span>
    <span class="crsl-dot"></span>
    <span class="crsl-dot"></span>
    <span class="crsl-dot"></span>
    <span class="crsl-dot"></span>
    <span class="crsl-dot"></span>
  </div>
  <a class="crsl-prev"></a>
  <a class="crsl-next"></a>
</div>
<script src="https://analyticsgit.riversidehealthcare.net/Data_Governance/atlas-information-management/-/raw/master/Data%20Governance%20WebApp/wwwroot/js/utility/carousel.js" type="text/javascript"></script>

## Live Demo
> :point_right: **[Live Atlas Demo with Docker](https://hub.docker.com/r/christopherpickering/rmc-atlas-demo)**

The live demo is running with Docker and Ubuntu 16.04. Check out the Dockerfile for build steps.

The demo can be run locally with our public docker image -
```sh
docker run -i -t -p 1234:1234 -e PORT=1234  -u 0 christopherpickering/rmc-atlas-demo:latest
```
or, you can clone this repository and build your own docker image by running these basic commands -
```sh
docker build  --tag atlas_demo .
docker run -i -t -p 1234:1234 -e PORT=1234  -u 0 atlas_demo:latest
# web app will be running @ http://localhost:1234
# see Dockerfile for db access
```

### Atlas can be run in an [online sandbox](https://labs.play-with-docker.com/) - it does require a docker.com login.

1. Click "start"
2. Click "Settings" > 1 Manager and 1 Worker
3. Click on the Manager instance. Atlas is large and doesn't run in the worker.
4. Paste in ```docker run -i -t -p 1234:1234 -e PORT=1234  -u 0 christopherpickering/rmc-atlas-demo:latest```
5. Wait about 1-2 mins for app to download and startup. Output will say ```Now listening on: http://[::]:1234``` when ready.
6. Click "Open Port" and type ```1234```
7. App will open in new tab. The URL should be valid for 3-4 hrs.

## Credits

Atlas was created by the Riverside Healthcare Analytics team -

* Paula Courville
* [Darrel Drake](https://www.linkedin.com/in/darrel-drake-57562529)
* [Dee Anna Hillebrand](https://github.com/DHillebrand2016)
* [Scott Manley](https://github.com/Scott-Manley)
* [Madeline Matz](mailto:mmatz@RHC.net)
* [Christopher Pickering](https://github.com/christopherpickering)
* [Dan Ryan](https://github.com/danryan1011)
* [Richard Schissler](https://github.com/schiss152)
* [Eric Shultz](https://github.com/eshultz)

## Getting Started

### Requirements
1. SQL Server Database (we use 2016 or newer, any license type) with Full Text Index installed
2. IIS Webserver with Microsoft .NET Core SDK 2.2.105 (x86)
3. Dev Machine with the following
    * Visual Studio 2017 or later
      * Analysis Service
      * SSIS Integration Services
      * ASP.net and webdevelopment component
      * Razor Language Pack component
      * SSDT Tools
    * Python 3.7, virtualenv
    * Active Directory Explorer, or other access to Active Directory
    * [Microsoft .NET Core SDK 2.2.105 (x86)](https://dotnet.microsoft.com/download/dotnet-core/2.2)

Atlas can run on any server OS that is capable of running Dotnet 2.2, and the database on any server that is capable of running Sql Server + Full Text Index.

### Steps to Run
1. Run database create scripts (LDAP, Data Governance, DG Staging). Set Datagov user credentials in database.
2. If you want to add demo data you can run the Data Governance database seeding script
2. Configure and run python LDAP script. Modify script and settings.py to match your LDAP build. Schedule in SQL Agent Jobs
3. Configure and run Atlas ETL's (main ETL and run data)
    * Delete SSRS sections if not used
    * Update Clarity server and credentials
    * Update Database connection strings
    * Schedule ETL's in SQL Agent Jobs
4. Populate Atlas/appsettings.json & appsettings.Development.json
5. Run website locally in Visual Studio
6. Update publish settings & publish

### Testing

Test are run using pythons + selenium. Each section of the webpage has its own test script, but all can be run at the same time by running /testing/master_test.py.

## About the Web UI

The app is built using C# Razor Pages, HTML, CSS and Javascript.

The app is currently using SQL Server, with a "database first" model.

### Database

The database can be created by running the script "Data Governance WebaApp/DatabaseCreationScript.sql".

Updates to the database are pulled in with the following scafold commmand:

```
# after running this commande we need to manually delete connection string from db context file
Scaffold-DbContext "Server=rhbidb01;Database=Data_Governance_Dev;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -force

```

Upon DB changes the file "webapp/DatabaseCreationScript.sql" should be updated. This can be done in SSMS by right clicking the db > clicking tasks > script database.

Customizations for a model are created in the model/customizations/ folder.

### CSS
Attempt made to follow wise guidance from www.maintainablecss.com. ::smile::

There is a primary css file (```main.min.css```) containing items used on ever page (or could be used on every page, like search), this is loaded immediately. There is a secondary css file (```differed.min.css```) that is loaded async. The visible stuff on the page should not rely on this file - it should contain animations, hidden items, etc.

Any page specific css can be inlined.

### Javascript

Javascript is broken up in to mini self executing functions. These are compliled into a few files -

* a primary file that contains only what is required to render all pages correctly
* a utility file containing functions required on all pages, but not used during render
* many smalll files with functions used for "fancy features" that are loaded "as needed"
* a unique files containing code only used in one section of the site. These files are then either lazy loaded or inlined into the page, depending on when they are required

In order to lazyload javascript and still keep the file version identifier we use a textarea and then load the contents of it as scripts.

Place any scripts to lazyload inside ```<textarea class="postLoadScripts"></textarea>``` tags.

### Tabs

A typical tab layout -
```html
<!-- tab control -->
<div class="tab">
    <a class="tab-lnk active" href="#tabone">Tab One</a>
    <a class="tab-lnk" href="#tabtwo">Tab Two</a>
</div>
<!-- tab body -->

<div class="tab-cnt">
    <div class="tab-dta active" id="tabone">
        Contents of Tab One
    </div>
    <div class="tab-dta" id="tabtwo">
        Contents of Tab Two
    </div>
</div>
```

### Tables
A table is created using this syntax - classes are optional.
```html
<div class="table-responsive">
<table class="table sort table-border">
  <thead>
    <tr>
      <th>head</th>
    </tr>
  <tbody>
    <tr>
      <td>cell</td>
    </tr>
  </tbody>
</table>
```

### Links
Links are simple created with an href and left classless. Style are applied at the tag level. When possile use links that are not Razor Pages dependent.
```html
<a href="\Reports?id=@this.id">
```

### Collapse
A typical collapse -
```html
<h1 data-toggle="clps" data-target="collapse-one">Collapse Link</h1>

<div id="collapse-one" class="clps" role="tabpanel" >
  Collapse contents go here.
</div>
```

### Carousel
A carousel is created with a basic outline -
```html
div.crsl
  div.crsl-itm
  div.crsl-itm
  div.crsl-ind
    span.crsl-dot
    span.crsl-dot
  a.crsl-prev
a.crsl-next
```

### Charts
A chart is created with json inside a script tag -
```html
<script type="application/json">
a = {
  axis: {
    0: {
      title: 'hits'
    },
    1: {
      title: 'seconds'
    },
    2: {
      title: 'clicks'
    }
  },
  type: 'bar',
  height: 400,
  title: "Cool Chart",
  data: [{
    title: "Run Time (s)",
    axis: 0,
    type: "bar",
    units: "(s)",
    data: [{
      title: "Stack 1",
      data: 12
    }]
  }, {
    title: "Runs",
    type: "bar",
    axis: 1,
    data: [{
      title: "Stack 1",
      data: 641
    }],
  }, {
    title: "Clicks",
    type: "line",
    axis: 2,
    data: [{
      title: "Stack 1",
      data: 61
    }],
  }],
  options: {
    legend: false
  }
</script>
```

### Progress bar
A progress bar exists on all pages and can be triggered with javascript events -
```js
document.dispatchEvent(new CustomEvent('progress-reset'));
document.dispatchEvent(new CustomEvent('progress-start'));
document.dispatchEvent(new CustomEvent('progress-finish'));
```
or to specify a value besides 100% to get to

```js
document.dispatchEvent(new CustomEvent('progress-start', {
                                cancelable: true,
                                detail: {
                                    value: 90 //max %
                                }
                            }));
```

### Tooltips
Tooltips can be added to any item and will appear to the left on hover.
```html
<a href="#" data-tooltip="Nice Tip!">Link to nowhere</a>
```

To make the tooltip appear above the elementy add a class ```tt-top```.
```html
<a class="tt-top" href="#" data-tooltip="Nice Tip!">Link to nowhere</a>
```

### Banner Messages

Add a global site paramter in the webapp settings > global settings.
Name: msg
Description: <Message Text>
Value: <id used to show message>

Show message by adding the message ID to the url:

www.atlas.com?msg=1

To have a sitewide message that does not require a URL param, leave the ```Value``` box blank when creating the param.

### Editor

Most editor description type fields support markdown. Most description fields also support flow charts from flowchart.js, wrapped in tripple \` with mermaid tag. 
```
\```mermaid
\```
```

### Ajax content

Content can be dynamically loaded by creating a div with a few attributes. The returned content *must* be wrapped in a single tag.
```html
<div data-ajax="yes" data-url="pate/to/page" data-param="stuff to append to url" data-loadtag="optional tag to load from response"></div>
```
Add the "cache" attribute to attempt to locally cache the data. The data will cache for 30 mins.

The dynamic content must be wrapped with a single element as it will completely replace the original element. The data attributes will be copied to the new content so that it will be possible to refresh it later on.


## ETL

There are two ETL's in the project. The first (ETL) brings in the report & user metadata. The second ETL (ETL-Run Data) brings in the run data.

User data is imported from LDAP, Hyperspace and SSRS servers. You can scheduled the scripts from /LDAP to run daily on your SQL Server.


## Hyperspace

Linking to Atlas from a Dashboard
1. Create a link component
2. Select a source of Component Editor
3. Select an Activity type link
4. Give your link a label
5. In the parameters, list the Atlas activity, and then runparams:<URL of the content> (like below)
```
ATLAS_WEBSITE,RunParams:https://my_host.net/Reports?id=73740
```
>>>>>>> df51f5505d2dd16746ad2777a7f88b9baa944c92
