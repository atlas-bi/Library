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

**************
How to Install
**************


################
Create Databases
################

There are two databases used -

- ``atlas-staging`` used to prepare raw data and insert it into several staging tables.
- ``atlas`` is the destination database used by the webapp.


The databases can be created by running the two database creation scripts:

- ``atlas_staging-creation_script.sql``
- ``atlas-creation_script.sql`` (kept in the `Atlas repo <https://github.com/Riverside-Healthcare/Atlas/blob/master/web/atlas-creation_script.sql>`_.)


#####################
Create Database Users
#####################

There are two user accounts needed to make the ETL's work.

1. A domain account with read access to all source databases, and write access to the staging database.
2. A local SSRS account with full access to the two databases.

SSRS User
=========

The SSRS user is created at the server level. Once created, they can be added to the databases.

The account should have reader, writer, admin and owner memberships to the two databases.

.. list-table::

    * - .. figure:: ../../images/install/add_user.png
           :alt: add user
    * - .. figure:: ../../images/install/user.png
           :alt: create user
    * - .. figure:: ../../images/install/user_cred.png
           :alt: user credentials
    * - .. figure:: ../../images/install/user_membership.png
           :alt: user membership


##################
Set ETL Parameters
##################

The ETL connection are kept in the **Connection Manager** and **Parameters**. Update the placeholder connection to valid credentials.

The default ETL uses sources of

- Tableau (see `tableau install guide <https://github.com/Riverside-Healthcare/Tableau-Metadata-Exporter>`_.)
- Crystal (see `crystal install guide <https://github.com/Riverside-Healthcare/Sqlize-Crystal-Reports>`_.)
- Two SSRS Report Servers
- Clarity
- LDAP Users (see :doc:`LDAP install guide <./ldap>`.)

If any of these are not needed, delete them from the ETL.

.. note:: The same steps can be followed to the Run Data ETL.
