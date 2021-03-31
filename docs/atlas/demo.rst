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
Atlas Demo
***********

Try Atlas out! Its easy to start up, you won't look back!


..  youtube:: d_IGbnuXvJ8
    :width: 100%



.. tab:: Online Demo

    Atlas can be run online `online playground <https://labs.play-with-docker.com/>`_ you can demo in.

    Create an Instance by clicking "Settings" > 1 Manager and 1 Worker

    .. panels::

        .. list-table::

            * - .. figure:: ../images/demo/demo1.png
                   :alt: Change Settings

        ---

        .. list-table::

            * - .. figure:: ../images/demo/demo2.png
                   :alt: Create an instance

    Click on the Manager instance. It seems to allow more space/ram. Paste in the docker command and hit "enter".

    .. code:: python

        docker run -i -t -p 1234:1234 -e PORT=1234  -u 0 christopherpickering/rmc-atlas-demo:latest

    .. list-table::

        * - .. figure:: ../images/demo/demo3.png
               :alt: Task list

    .. note:: Wait about 1-2 mins for app to download and startup. Output will say ``Now listening on: http://[::]:1234`` when ready.

    Click "Open Port" and type ``1234``

    App will open in new tab. The URL should be valid for 3-4 hrs.


.. tab:: Local Demo

    There is a dockerized Atlas demo available, it can be run with a single command -

    .. code:: bash

        docker run -i -t -p 1234:1234 -e PORT=1234  -u 0 christopherpickering/rmc-atlas-demo:latest

    Alternatively, you can clone the repository and build your own docker image -

    .. code:: bash

        docker build  --tag atlas_demo .
        docker run -i -t -p 1234:1234 -e PORT=1234  -u 0 atlas_demo:latest
        # web app will be running @ http://localhost:1234
        # see Dockerfile for db access
