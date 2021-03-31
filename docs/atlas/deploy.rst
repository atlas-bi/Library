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

******
Deploy
******

.. attention::

   Ensure that the destination server has the same .NET *version* `Hosting Bundle`.

.. note::

   This guide assumes you have already created the Atlas Databases and run the ETL.

#########
Setup IIS
#########

Open the **Internet Information Services (IIS) Manager** on your Windows Server.

First, create a new site by right clicking on **Site** then **Create New Site**.

.. list-table::

   * - .. figure:: ../images/deploy/add_website.png
          :alt: Add website

Next, fill out the require parameters. If you have setup a DNS you can enter it in the **Host name** box.

.. list-table::

   * - .. figure:: ../images/deploy/website_config.png
          :alt: Add website configuration

.. attention::
   Ideally you will add another binding for https. There are `many tutorials <https://techexpert.tips/iis/enable-https-iis/>`_ showing how to enable HTTPS.


Finally, verify that **windows** authentication is enabled.

.. list-table::

   * - .. figure:: ../images/deploy/open_auth.png
          :alt: Open authentication settings
     - .. figure:: ../images/deploy/windows_auth.png
          :alt: Verify windows authentication


#############
Deploy to IIS
#############

.. tab:: Deploy With Visual Studio

   Deploying with Visual Studio is the preferred method. After opening the ``web.sln`` file -

   - First update ``web/appsettings.json`` with the correct settings for your database and organization.
   - In Visual Studio's menu, click **Build** then **Publish Web**
   - Create a new publish profile.

     - Choose **Web Server (IIS)** as the **Target**
     - Choose **Web Deploy** as the **Specific target**
     - Enter your IIS **Server** name
     - Enter your **Site name**. This must match the site name already created on the web server (``atlas-dev``)
     - Enter the web url in **Destination URL**
     - Optionally enter you credentials for the web server

   - After the profile is created click **Edit** to change additional settings.
   - Change to the **Settings** tab and change the **Target Runtime** to match the web servers .NET bitness.

     .. list-table::

        * - .. figure:: ../images/deploy/vs_profile.png
               :alt: Edit publish profile

   - In order to successfully publish the connection must be validated to allow self-signed certificates.

     .. list-table::

        * - .. figure:: ../images/deploy/vs_connection.png
               :alt: Validate connection
        * - .. figure:: ../images/development/ssl_warning.png
               :alt: ssl warning
        * - .. figure:: ../images/development/ssl_confirm.png
               :alt: ssl confirm


     .. attention::
        The connection must be re-verified every time Visual Studio is restarted.

   - Finally publish Atlas by clicking **Publish** button.

     .. list-table::

        * - .. figure:: ../images/deploy/vs_publish.png
               :alt: Publish Atlas

.. tab:: Manually Deploy

   Atlas is fairly simple to manually deploy.

   - First pull Atlas's source code onto the server
   - Update ``web/appsettings.json`` with the correct settings for your database and organization.
   - Run dotnet publish from the ``web`` folder to build the Atlas runtime.

     .. code:: sh

        dotnet publish -r win-x86 --self-contained false -c Release -o out

     .. attention::
        Ensure the bitness matches the bitness of the .NET version you've installed on the server!

   - Copy the contents of the newly created ``out`` directory into the ``c://inetpub/wwwroot/atlas-dev`` folder.

   **Navigate to your binding and Atlas should be available!**
