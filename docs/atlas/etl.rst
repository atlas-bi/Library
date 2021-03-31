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

*****
ETL
*****

See the ETL documentation.


ETL
===

There are two ETL's in the project. The first (ETL) brings in the report & user metadata. The second ETL (ETL-Run Data) brings in the run data.

User data is imported from LDAP, Hyperspace and SSRS servers. You can scheduled the scripts from /LDAP to run daily on your SQL Server.

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
