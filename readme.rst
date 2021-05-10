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

|docs| |codacy| |codeql| |climate|

Atlas of Information Management
===============================

Atlas of Information Management is a business intelligence library and documentation database. ETL processes collect metadata from various reporting platforms and store it in a centraly located database. A modern web UI is used to add additional documentation to the report objects and also to provide an intuative way to search, favorite and share reporting content.

See the `project documentation <https://riverside-healthcare.github.io/Atlas/>`_

Included Content
----------------

- Workbench Reports
- Dashboards
- Microsoft SSRS
- SAP Crystal
- Tableau Dashboards
- Components
- Metrics
- Slicer-Dicer
- User Information
- Report Usage

Key Features
------------

- Clearly define terms and link to reports
- Easily group reports and terms into projects
- Create and track Data Initiatives
- Ability to run and open editors directly from Atlas
- Atlas allows very expansive documentation!
  - Screenshots
  - Large editor form with multiple sections
  - Markdown support
  - Maintenance schedules and log workflow
  - Tiered report validation levels
  - Report relationships are fully ETL'd
  - Report/term/project/initiative relationships can be added
  - Source report SQL is included in ETL's
- Enterprise Data Glossary - custom term definitions can be added and linked to content
- Usability
  - Fully searchable - users can find all publicly available content
  - Native favoriting and sharing
  - Webapp is embeddable and works in common browsers
  - Request access forms
  - Single sign on for all system users
- Excellent Search
  - Full Text Index on most fields
  - SQL is searchable
  - Search filters
- Reports, editors and record viewers can be launched directly from Atlas
- Recommended reports based on users interests

Coming Soon
-----------

- powerBI integration
- SSAS Cube documentation and ETL
- Slicer Dicer User Saved Sessions
- Database metadata with linkages from report SQL back to the item number in Epic
- Expanded documentation for terms and reports related to regulated quality measures, including data submission tracking
- Tracking and documentation of data flows out of the organization including ETL for Bridges interfaces
- Manual tracking for other submissions like Teletracking

Credits
-------

Atlas was created by the Riverside Healthcare Analytics team -

* Paula Courville
* `Darrel Drake <https://www.linkedin.com/in/darrel-drake-57562529>`_
* `Dee Anna Hillebrand <https://github.com/DHillebrand2016>`_
* `Scott Manley <https://github.com/Scott-Manley>`_
* `Madeline Matz <mailto:mmatz@RHC.net>`_
* `Christopher Pickering <https://github.com/christopherpickering>`_
* `Dan Ryan <https://github.com/danryan1011>`_
* `Richard Schissler <https://github.com/schiss152>`_
* `Eric Shultz <https://github.com/eshultz>`_

.. |docs| image:: https://img.shields.io/badge/Atlas-Documentation-orange
   :target: https://riverside-healthcare.github.io/Atlas/

.. |codacy| image:: https://app.codacy.com/project/badge/Grade/5238d35fb338443fb784b852337fe75f
   :target: https://www.codacy.com/gh/Riverside-Healthcare/Atlas/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=Riverside-Healthcare/Atlas&amp;utm_campaign=Badge_Grade

.. |codeql| image:: https://github.com/Riverside-Healthcare/extract_management/workflows/CodeQL/badge.svg
   :target: https://github.com/Riverside-Healthcare/extract_management/actions/workflows/codeql-analysis.yml
   :alt: CodeQL

.. |climate| image:: https://api.codeclimate.com/v1/badges/385f0450d811190e7e43/maintainability
   :target: https://codeclimate.com/github/Riverside-Healthcare/Atlas/maintainability
   :alt: Maintainability
