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
    test case for /reports urls
"""

import time
import unittest
import warnings

import urllib3
from selenium import webdriver
from selenium.webdriver.common.desired_capabilities import DesiredCapabilities

urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)


class Reports(unittest.TestCase):
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
        # pylint: disable=too-many-statements
        """
        test has inner function to run for both chrome and ie.
        """

        warnings.simplefilter("ignore", ResourceWarning)
        print("report")

        def test(driver, skip):
            report = self.my_site + "/reports?id=55074"

            # test tab links
            driver.get(report)
            for lnk in driver.find_elements_by_css_selector(".pageNav a.pageNav-link"):
                lnk.click()
                time.sleep(1)

            # report profile
            driver.get(report)
            driver.find_elements_by_css_selector(
                'a.nav-link[data-target="profile-modal"]'
            )[0].click()
            time.sleep(1)
            driver.find_elements_by_css_selector('a.tab-lnk[href="#top-user-table"]')[
                0
            ].click()
            time.sleep(1)
            driver.find_elements_by_css_selector('a.tab-lnk[href="#run-history"]')[
                0
            ].click()
            time.sleep(1)
            driver.find_elements_by_css_selector('a.tab-lnk[href="#run-time-table"]')[
                0
            ].click()
            time.sleep(1)
            driver.find_elements_by_css_selector('a.tab-lnk[href="#failed-runs"]')[
                0
            ].click()
            time.sleep(1)
            driver.find_elements_by_css_selector(
                'a.tab-lnk[href="#user-subscriptions"]'
            )[0].click()
            time.sleep(1)
            # close modal
            driver.find_elements_by_css_selector("#profile-modal  button.close")[
                0
            ].click()

            if skip != 1:
                get_log(report, driver)

            # favorite
            driver.get(report)
            driver.find_elements_by_css_selector(
                'a.nav-link[data-tooltip="Add to Favorites"]'
            )[0].click()
            time.sleep(1)
            driver.find_elements_by_css_selector(
                'a.nav-link[data-tooltip="Add to Favorites"]'
            )[0].click()
            time.sleep(1)

            if skip != 1:
                get_log(report, driver)

            # share report
            driver.get(report)
            driver.find_elements_by_css_selector(
                'a.nav-link[data-target="shareModal"]'
            )[0].click()
            time.sleep(1)
            driver.find_elements_by_css_selector("#shareModal input.dd-vsbl ")[
                0
            ].send_keys("Chris Pickering")
            time.sleep(1)
            driver.find_elements_by_css_selector("#shareModal .dd-rslt")[0].click()
            time.sleep(1)
            driver.find_elements_by_css_selector("#shareModal button.mlbx-newMsgSend")[
                0
            ].click()
            time.sleep(1)

            if skip != 1:
                get_log(report, driver)

            # request access
            driver.get(report)
            time.sleep(1)
            driver.find_elements_by_css_selector(
                'a.nav-link[data-target="requestAccessModal"]'
            )[0].click()
            time.sleep(1)
            driver.find_elements_by_css_selector("#requestAccessModal input.dd-vsbl")[
                0
            ].send_keys("Dan Ryan")
            time.sleep(1)
            driver.find_elements_by_css_selector(
                "#requestAccessModal  button.request-access"
            )[0].click()
            time.sleep(1)

            if skip != 1:
                get_log(report, driver)

            # positive feedback
            driver.get(report)
            driver.find_elements_by_css_selector(
                'a.nav-link[data-tooltip="Share Positive Feedback"]'
            )[0].click()
            time.sleep(1)
            driver.find_elements_by_css_selector(
                '#shareFeedback textarea[name="description"]'
            )[0].send_keys("test positive feedback")
            time.sleep(1)
            driver.find_elements_by_css_selector(
                "#shareFeedback  button.share-feedback"
            )[0].click()
            time.sleep(1)

            if skip != 1:
                get_log(report, driver)

            # negative feedback
            driver.get(report)
            driver.find_elements_by_css_selector(
                'a.nav-link[data-tooltip="Report a Problem"]'
            )[0].click()
            time.sleep(1)
            driver.find_elements_by_css_selector(
                '#shareFeedback textarea[name="description"]'
            )[0].send_keys("test negative feedback")
            time.sleep(1)
            driver.find_elements_by_css_selector(
                "#shareFeedback  button.share-feedback"
            )[0].click()
            time.sleep(5)

            if skip != 1:
                get_log(report, driver)

            # open editor
            # change to admin
            driver.get(self.my_site + "/Users?handler=ChangeRole&Id=1&Url=/")
            driver.get(report)
            driver.find_elements_by_css_selector('a.nav-link[data-target="editModal"]')[
                0
            ].click()
            time.sleep(1)
            driver.find_elements_by_css_selector(
                '#editModal a.tab-lnk[href="imagestab"]'
            )[0].click()
            time.sleep(1)
            driver.find_elements_by_css_selector(
                '#editModal a.tab-lnk[href="desctab"]'
            )[0].click()
            time.sleep(1)
            driver.find_elements_by_css_selector(
                '#editModal a.tab-lnk[href="termstab"]'
            )[0].click()
            time.sleep(1)
            driver.find_elements_by_css_selector(
                '#editModal a.tab-lnk[href="detailstab"]'
            )[0].click()
            time.sleep(1)
            driver.find_elements_by_css_selector('#editModal a.tab-lnk[href="metab"]')[
                0
            ].click()
            time.sleep(1)
            driver.find_elements_by_css_selector(
                '#editModal a.tab-lnk[href="mainttab"]'
            )[0].click()
            time.sleep(1)

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
