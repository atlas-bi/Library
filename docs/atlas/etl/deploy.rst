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

**********
Deploy
**********


Once the ETL performs to your satisfaction in Visual Studio it can be deployed to an SSRS server and scheduled as a job.

- Open ETL solution (ETL.sln) in Visual Studio

- Highlight all the packages you want to deploy and right click on the ETL > deploy.

  .. list-table::

      * - .. figure:: ../../images/deploy/packages.png
            :alt: visual studio deploy

- Choose SSIS as the deploy target

  .. list-table::

      * - .. figure:: ../../images/deploy/target.png
            :alt: visual studio deploy

- Enter the destination server name, and path the ETL location. You can create the folder and project.

  .. list-table::

      * - .. figure:: ../../images/deploy/destination.png
            :alt: destination
      * - .. figure:: ../../images/deploy/folder.png
            :alt: folder name
      * - .. figure:: ../../images/deploy/project_name.png
            :alt: project name

- Click "Deploy"

  .. list-table::

      * - .. figure:: ../../images/deploy/deploy_button.png
            :alt: deploy button

- Verify "Success"

  .. list-table::

      * - .. figure:: ../../images/deploy/results.png
            :alt: deploy success
