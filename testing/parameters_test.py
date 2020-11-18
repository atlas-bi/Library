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
    test case for /parameters urls
"""

import time
import warnings
import unittest
from selenium import webdriver
from selenium.webdriver.common.desired_capabilities import DesiredCapabilities
import urllib3

urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)


class Parameters(unittest.TestCase):
    """
    test class from access control

    setUp
    runText
        test for chrome
        test for ie
    cleanUp
    """

    def setUp(self):
        warnings.simplefilter("ignore", ResourceWarning)
        self.my_site = "https://localhost:44380"

        # ie 11 cannot get browser output somehow, so change IE setting to alert w/ errors.
        # internet options > advanced > uncheck "disabled script debugging..." 2 boxes.
        # internet options > advanced > check "display a notification..."

        capabilities = DesiredCapabilities.INTERNETEXPLORER
        capabilities.update({"logLevel": "DEBUG"})
        capabilities["ignoreProtectedModeSettings"] = True
        self.explorer = webdriver.Ie(capabilities=capabilities)

        # chrome setup
        options = webdriver.ChromeOptions()
        options.add_argument("--ignore-certificate-errors")
        self.chrome = webdriver.Chrome(options=options)

    def runTest(self):
        """
        test has inner function to run for both chrome and ie.
        """

        print("parameters")

        warnings.simplefilter("ignore", ResourceWarning)

        def test(driver, skip):
            report = self.my_site + "/parameters"
            driver.get(report)
            time.sleep(1)

            # for chrome
            if skip != 1:
                get_log(report, driver)

        def get_log(url, driver):
            logs = driver.get_log("browser")
            if len(logs) > 0:
                for log in logs:
                    if log["level"] == "SEVERE":
                        print(
                            url
                            + ","
                            + log["source"]
                            + ","
                            + log["level"]
                            + ","
                            + log["message"]
                        )
        test(self.chrome, 0)
        test(self.explorer, 1)

    def tearDown(self):
        self.chrome.close()
        self.explorer.close()


if __name__ == "__main__":
    unittest.main()
