..
    Atlas of Information Management
    Copyright (C) 2020  Riverside Healthcare, Kankakee, IL

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.

***********
Development
***********



About the Web UI
================

The app is built using C# Razor Pages, HTML, CSS and Javascript.

The app is currently using SQL Server, with a "database first" model.

Database
==========

The database can be created by running the script "Data Governance WebaApp/DatabaseCreationScript.sql".

Updates to the database are pulled in with the following scafold commmand:

.. code:: bash

   # after running this commande we need to manually delete connection string from db context file
   Scaffold-DbContext "Server=rhbidb01;Database=atlas;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -force -Context Atlas_WebContext -Namespace Atlas_Web.Models



Upon DB changes the file "webapp/DatabaseCreationScript.sql" should be updated. This can be done in SSMS by right clicking the db > clicking tasks > script database.

Customizations for a model are created in the model/customizations/ folder.

CSS
==========
Attempt made to follow wise guidance from www.maintainablecss.com. :smile:

There is a primary css file (``main.min.css``) containing items used on ever page (or could be used on every page, like search), this is loaded immediately. There is a secondary css file (``differed.min.css``) that is loaded async. The visible stuff on the page should not rely on this file - it should contain animations, hidden items, etc.

Any page specific css can be inlined.

Javascript
==========
Javascript is broken up in to mini self executing functions. These are compliled into a few files -

* a primary file that contains only what is required to render all pages correctly
* a utility file containing functions required on all pages, but not used during render
* many smalll files with functions used for "fancy features" that are loaded "as needed"
* a unique files containing code only used in one section of the site. These files are then either lazy loaded or inlined into the page, depending on when they are required

In order to lazyload javascript and still keep the file version identifier we use a textarea and then load the contents of it as scripts.

Place any scripts to lazyload inside ``<textarea class="postLoadScripts"></textarea>`` tags.

Tabs
====

A typical tab layout -

.. code :: html

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


Tables
======
A table is created using this syntax - classes are optional.

.. code :: html

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


Links
=====
Links are simple created with an href and left classless. Style are applied at the tag level. When possile use links that are not Razor Pages dependent.
.. code :: html

  <a href="\Reports?id=@this.id">


Collapse
========

A typical collapse -

.. code :: html

  <h1 data-toggle="clps" data-target="collapse-one">Collapse Link</h1>

  <div id="collapse-one" class="clps" role="tabpanel" >
    Collapse contents go here.
  </div>


Carousel
========

A carousel is created with a basic outline -

.. code :: html

  div.crsl
    div.crsl-itm
    div.crsl-itm
    div.crsl-ind
      span.crsl-dot
      span.crsl-dot
    a.crsl-prev
  a.crsl-next


Charts
======

A chart is created with json inside a script tag -

.. code :: html

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


Progress bar
============

A progress bar exists on all pages and can be triggered with javascript events -

.. code :: js

  document.dispatchEvent(new CustomEvent('progress-reset'));
  document.dispatchEvent(new CustomEvent('progress-start'));
  document.dispatchEvent(new CustomEvent('progress-finish'));

or to specify a value besides 100% to get to

.. code :: js

  document.dispatchEvent(new CustomEvent('progress-start', {
                                  cancelable: true,
                                  detail: {
                                      value: 90 //max %
                                  }
                              }));

Tooltips
========

Tooltips can be added to any item and will appear to the left on hover.

.. code :: html

   <a href="#" data-tooltip="Nice Tip!">Link to nowhere</a>


To make the tooltip appear above the elementy add a class ``tt-top``.

.. code :: html

   <a class="tt-top" href="#" data-tooltip="Nice Tip!">Link to nowhere</a>


Banner Messages
===============

Add a global site paramter in the webapp settings > global settings.
Name: msg
Description: <Message Text>
Value: <id used to show message>

Show message by adding the message ID to the url:

www.atlas.com?msg=1

To have a sitewide message that does not require a URL param, leave the ``Value`` box blank when creating the param.

### Editor

Most editor description type fields support markdown. Most description fields also support flow charts from flowchart.js, wrapped in tripple \` with mermaid tag. 

.. code :: md

   ```mermaid
   ```


Ajax content
============

Content can be dynamically loaded by creating a div with a few attributes. The returned content *must* be wrapped in a single tag.

.. code :: html

   <div data-ajax="yes" data-url="pate/to/page" data-param="stuff to append to url" data-loadtag="optional tag to load from response"></div>

Add the "cache" attribute to attempt to locally cache the data. The data will cache for 30 mins.

The dynamic content must be wrapped with a single element as it will completely replace the original element. The data attributes will be copied to the new content so that it will be possible to refresh it later on.


Hyperspace
==========

Linking to Atlas from a Dashboard

1. Create a link component
2. Select a source of Component Editor
3. Select an Activity type link
4. Give your link a label
5. In the parameters, list the Atlas activity, and then runparams:<URL of the content> (like below)

.. code ::

   ATLAS_WEBSITE,RunParams:https://my_host.net/Reports?id=73740
