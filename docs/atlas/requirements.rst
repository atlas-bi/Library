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

************
Requirements
************

.. tab:: Deploy

   There are minimal requirements to get Atlas running on your servers.

   - An install of SQL Server Database 2016+ with `Full Text Index <https://codingsight.com/implementing-full-text-search-in-sql-server-2016-for-beginners/>`_
   - IIS Webserver with `Microsoft .NET SDK 5 (Hosting Bundle) <https://dotnet.microsoft.com/download/dotnet/5.0>`_

     .. list-table::

        * - .. figure:: ../images/requirements/dotnetversion.png
               :alt: Extension

   .. admonition:: SQL Server
      :class: note

      Any SQL Server license type will work. If you need a demo database we recommend `running with docker <https://schwabencode.com/blog/2019/10/27/MSSQL-Server-2017-Docker-Full-Text-Search>`_.

   .. admonition:: Web Server
      :class: note

      While Atlas will run on any OS that has **.Net 5** installed (check out our `Ubuntu docker file <https://github.com/Riverside-Healthcare/Atlas-of-Information-Management/blob/master/Dockerfile>`_!), Atlas authentication uses IIS Windows Authentication.
      Also, using Windows Server 2019 has HTTP2, which allows greater site speed.

      Ensure IIS has server roles needed for web deploy and web management service installed and started. Microsoft has a few `examples <https://docs.microsoft.com/en-us/aspnet/web-forms/overview/deployment/visual-studio-web-deployment/deploying-to-iis>`_.

   .. admonition:: Deploy Atlas
      :class: hint

      Atlas can be deployed without Visual Studio. We recommend deploying with Visual Studio for ease of use. See the :doc:`deploy guide <deploy>`.

   .. admonition:: .NET 5
      :class: attention

      While .NET 5 x86 is specified, it is not required. The only *requirement* is that Atlas is *built* and *run* in the same version and bitness.


.. tab:: Develop

   Doing development on Atlas is fairly simple. The Web App is developed using `Visual Studio 2017+ <https://visualstudio.microsoft.com/downloads/>`_. Testing is done with Python + Selenium.

   Visual Studio Requirements

   - Full install of `Visual Studio 2017+ <https://visualstudio.microsoft.com/downloads/>`_
   - Visual Studio's ``SQL Server Integration Services Projects`` extension

     .. list-table::

        * - .. figure:: ../images/requirements/vs_extension.png
               :alt: Extension

   - ``ASP.NET and web development`` component

     .. list-table::

        * - .. figure:: ../images/requirements/dotnet.png
               :alt: asp.net component

   - ``Data Storage and Processing`` component

     .. list-table::

        * - .. figure:: ../images/requirements/sqlservices.png
               :alt: sql services component

   - ``Razor Language Services`` component

     .. list-table::

        * - .. figure:: ../images/requirements/razor.png
               :alt: razor component

   - `Python 3.7+ <https://www.python.org/downloads/>`_ 
   - `Microsoft .NET SDK 5.0.103 (x86) <https://dotnet.microsoft.com/download/dotnet/5.0>`_
