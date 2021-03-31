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

*********
Changelog
*********

Version 2021.03.1
-----------------

- Renamed database ``Data_Governance`` to ``atlas``
- Renamed database ``DG_Staging`` to ``atlas_staging``
- Added report certification title to report header
- Added documentation
- Added .25s delay to live searching to reduce server load
- Removed private reports from search procs
- Fixed bug with copy query button always copying first query
- Fixed bug which prevented deleting data projects
- Fixed bug which displayed html text instead of parsed html in comments
- Fixed misc dropdown bugs
- Merged thumbs up/thumbs down on reports into one button
- Updated search proc to treat terms name as high a priority as report name
- Added data projects as "ads" to search
- Fixed bug in search that was removing search filters after starting another search
- Fixed bug in search that was hiding last filter when only a few results were showing
- Update search proc to include report annotations from data projects
