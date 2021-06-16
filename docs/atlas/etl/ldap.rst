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
Set Up LDAP
***********


It is presumed that most user profile information will be coming from LDAP. There are `LDAP scripts <https://github.com/Riverside-Healthcare/Atlas/tree/master/LDAP>`_ provided to scrape the necessary data from LDAP server(s) into a separate database.

This can be setup to run as a scheduled job with `Extract Management <https://github.com/Riverside-Healthcare/extract_management>`_, or another tool, to provide data to the Atlas ETL.
