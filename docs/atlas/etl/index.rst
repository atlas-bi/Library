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

ETL
===

.. toctree::
    :maxdepth: 1
    :titlesonly:
    :hidden:

    install.rst
    ldap.rst
    deploy.rst
    schedule.rst

.. toctree::
   :caption: Releases
   :hidden:

   updates/index


The Atlas ETL (designed for use with the `Atlas web application <https://github.com/Riverside-Healthcare/Atlas>`_), is used to collect report metadata from a variety of sources and merge it into a common database.

There are two primary ETLs -

- Report Metadata (split into 8 sections)
- Report Run Data

The ETL's are both SSIS packages which are scheduled to run as jobs on a Microsft SQL Server. The metadata ETl can be scheduled to run at a fairly frequent interval - every 15 minutes - hour, while the run data ETL can be run daily.

The Clarity ETL can be added here: `Clarity ETL <https://datahandbook.epic.com/Reports/Details/9000648>`_

Requirements
------------

- `Active Directory Explorer <https://docs.microsoft.com/en-us/sysinternals/downloads/adexplorer>`_, or other access to Active Directory


Steps to Run
------------
1. Run database create scripts (LDAP, Data Governance, DG Staging). Set Datagov user credentials in database.
2. If you want to add demo data you can run the Data Governance database seeding script
3. Configure and run python LDAP script. Modify script and settings.py to match your LDAP build. Schedule in SQL Agent Jobs
4. Configure and run Atlas ETL's (main ETL and run data)

   - Delete SSRS sections if not used
   - Update Clarity server and credentials
   - Update Database connection strings
   - Schedule ETL's in SQL Agent Jobs

5. Populate Atlas/appsettings.json & appsettings.Development.json
6. Run website locally in Visual Studio
7. Update publish settings & publish


Credits
-------


Atlas ETL was created by the Riverside Healthcare Analytics team -

* Paula Courville
* `Darrel Drake <https://www.linkedin.com/in/darrel-drake-57562529>`_
* `Dee Anna Hillebrand <https://github.com/DHillebrand2016>`_
* `Scott Manley <https://github.com/Scott-Manley>`_
* `Madeline Matz <mailto:mmatz@RHC.net>`_
* `Christopher Pickering <https://github.com/christopherpickering>`_
* `Sean Pickering <https://github.com/sean-pickering>`_
* `Dan Ryan <https://github.com/danryan1011>`_
* `Richard Schissler <https://github.com/schiss152>`_
* `Eric Shultz <https://github.com/eshultz>`_
