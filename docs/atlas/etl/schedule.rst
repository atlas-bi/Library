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

########
Schedule
########


Once the ETL's are published to SSRS they can easily be scheduled to run on intervals.

1. Create a new job

   .. list-table::

      * - .. figure:: ../../images/schedule/create_job.png
             :alt: create job

2. Give the job a name

   .. list-table::

      * - .. figure:: ../../images/schedule/name_job.png
             :alt: name job

3. Add a step to the ETL. The first step must be Setup and the last two steps must be Merge and Post Processing.

   .. list-table::

      * - .. figure:: ../../images/schedule/schedule_choose_job.png
             :alt: create job step

   .. note:: At this step, choose the domain account that has read access to all data source and write access to the ``atlas-staging`` db.

4. When multiple steps, be sure to end the last step with 'Quit the job reporting success'.

   .. list-table::

         * - .. figure:: ../../images/schedule/MultipleSteps.png
                :alt: multiple steps

5. Create a schedule

   .. list-table::

      * - .. figure:: ../../images/schedule/schedule_job.png
             :alt: create schedule
