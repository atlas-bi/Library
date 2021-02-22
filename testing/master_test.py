"""
  Atlas of Information Management business intelligence library and documentation database.
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

"""

"""

	Master Atlas Test Suite

"""

import unittest

from accessControl_test import AccessControl
from analytics_test import Analytics
from group_test import Groups
from parameters_test import Parameters
from project_test import Projects
from report_test import Reports
from task_test import Tasks
from term_test import Terms
from url_test import Urls
from user_test import Users


def suite():
    """
    build a list of tests to run
    """

    tests = unittest.TestSuite()

    tests.addTest(Groups())
    tests.addTest(Users())
    tests.addTest(AccessControl())
    tests.addTest(Analytics())
    tests.addTest(Parameters())
    tests.addTest(Projects())
    tests.addTest(Reports())
    tests.addTest(Tasks())
    tests.addTest(Terms())
    tests.addTest(Urls())

    return tests


if __name__ == "__main__":
    runner = unittest.TextTestRunner()
    runner.run(suite())
