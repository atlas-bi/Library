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

#######
Testing
#######

All Atlas tests are run with `Selenium <https://selenium-python.readthedocs.io>`_.

If you are using ``poetry`` as your recommend package manager you can install the required packages like this:

.. code:: shell

   poetry install

If you prefer pip, there are two packages required:

.. code:: shell

   pip install selenium urllib3

Next, download the web drivers needed and place them in the ``testing`` directory:

- `Chrome web driver <https://chromedriver.chromium.org/downloads>`_
- `IE 11 web driver <https://www.microsoft.com/en-us/download/details.aspx?id=44069>`_

.. attention:: The version of the chrome web driver **must** match the version of chrome you have installed.

.. note:: By default tests will run in both IE11 and Chromium.

.. note::

   The ``url_test.py`` will attempt to access several hundred URL. It is advisable to update this list to match the top hits on your instance. Run a sql query on your instance to build the list.

   .. code:: sql

      select top 500
        concat('''',pathname , replace(search, '?EPIC=1','') ,''',')
      from
        app.Analytics
      group by
        concat('''',pathname , replace(search, '?EPIC=1','') ,''',')
      order by
        count(1) desc

Update IE 11 javascript settings to bring a popup on Javascript errors (needed to catch Hyperspace errors.) Selenium cannot pick up IE 11 console output like we can with Chrome, so it is necessary to "crash" the test to review the errors.

.. list-table::

   * - .. figure:: ../../images/development/ie_settings.png
          :alt: IE 11 settings

Start Atlas in Visual Studio in Debug mode.

.. list-table::

   * - .. figure:: ../../images/development/start_debug.png
          :alt: Start debug


Start python tests.

.. code:: python

   python master_test.py
