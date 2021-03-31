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
Development
***********

.. toctree::
    :maxdepth: 1
    :titlesonly:
    :hidden:

    development
    search
    auth
    test


See :doc:`Development Requirements <../requirements>` before getting started.


Other Useful Tools
------------------

- `Pyenv <https://github.com/pyenv/pyenv>`_ for managing python environments
- `Poetry <https://python-poetry.org>`_ for managing dependencies
- `Precommit <https://pre-commit.com>`_  for reformating code before committing
- `Tox <https://tox.readthedocs.io/en/latest/index.html>`_  running tests, verifying code



#############
Basic Process
#############

Prepare to make changes
=======================

The code is revision controlled with `git <https://git-scm.com>`_. You will also have a code server to safely store the code.

.. note::

   Having private repositories make it easier to manage passwords. If you do not already have a git server setup, `GitLab <https://about.gitlab.com/install/>`_ or `Gitea <https://gitea.com>`_ are both free and very simple to setup and manage.

   GitLab includes "GitLab Runner" which can be used for auto testing and deploys. Gitea is more lightweight but integrates very nicely with `Drone-Ci <https://www.drone.io>`_.


The code most likely ended up on your workstation from the `Github repository <https://github.com/Riverside-Healthcare/Atlas-of-Information-Management>`_. For the sake of this guide we will assume that it is your code server.

.. code:: shell

   # you've create an empty folder and are now inside it
   git init
   git remote add origin git@github.com:Riverside-Healthcare/Atlas-of-Information-Management.git
   git pull origin master

   # you have already installed pre-commit, right?
   # we can now add the git hooks for it.
   pre-commit install


.. admonition:: Why Pre-Commit?
   :class: note

   Pre-Commit is used to automatically run formatting tests after you make code changes. Check out the ``.pre-commit-config.yaml`` for all tests.

   - reformat c# code
   - reformat and check json config files
   - reformat and check toml and yaml python config files
   - fix end of files for consistency
   - reformat python code for consistency
   - reformat & check js & css code

Make some updates
=================

Now, assume you've made some changes to the code. You've:
- tweaked one of the c# helper functions
- changed some colors in the UI
- modified the analytics javascript
- fixed some spelling errors in the docs

Visually test your changes by running the website locally -

.. list-table::

   * - .. figure:: ../../images/development/iisexpress.png
          :alt: Start debug


The first time you start up the app there will most likely be two SSL prompts.

.. list-table::

   * - .. figure:: ../../images/development/ssl_warning.png
          :alt: ssl warning
   * - .. figure:: ../../images/development/ssl_confirm.png
          :alt: ssl confirm


- it is wise to run the :doc:`Selenium tests <test>` to check for browser issues
- next you can commit your code, since you've installed pre-commit your code will be validated.

  .. code:: shell

     git add . && git commit -m "did some cool updates" && git push


If you have CI turned on on your git server, you can auto generate the new documentation. Take a look in ``.gitlab-ci.yml`` for the steps to do that in Gitlab Runner, or ``.github/workflows`` for how to do it in Github or Drone-CI.

Deploy
======

If you changes have tested nicely you can :doc:`deploy <../deploy>` the updates.

It is strongly recommended to have two instances of Atlas (Atlas and Atlas-Test).

First, :doc:`deploy <../deploy>` to test, and if user testing is ok, then :doc:`deploy <../deploy>` to prod.
