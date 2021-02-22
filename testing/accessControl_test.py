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
    test case for /accesscontrol urls
"""

import time
import unittest
import warnings

import urllib3
from selenium import webdriver
from selenium.webdriver.common.desired_capabilities import DesiredCapabilities
from selenium.webdriver.common.keys import Keys

urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)


class AccessControl(unittest.TestCase):
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

        warnings.simplefilter("ignore", ResourceWarning)
        print("access control")

        def test(driver, skip):
            report = self.my_site + "/accesscontrol"
            driver.get(report)
            time.sleep(1)

            # search for user
            driver.find_elements_by_css_selector("#userroles .dd-inpt .dd-vsbl")[
                0
            ].send_keys("Chris Pickering")
            time.sleep(1)
            driver.find_elements_by_css_selector("#userroles .dd-rslt")[0].click()
            time.sleep(1)
            # get user id
            user_id = driver.find_elements_by_css_selector(
                "#userroles .dd-wrp input.dd-hdn"
            )[0].get_attribute("value")

            # search for role
            driver.find_elements_by_css_selector(
                '#userroles .dd-inpt .dd-vsbl[search-area="user-roles"]'
            )[0].send_keys("User")
            time.sleep(1)
            driver.find_elements_by_css_selector(
                '#userroles .dd-inpt .dd-rslt[value="5"]'
            )[0].click()
            time.sleep(2)

            driver.find_elements_by_css_selector(
                '#userroles .dd-inpt .dd-vsbl[search-area="user-roles"]'
            )[0].send_keys(Keys.ENTER)
            time.sleep(1)
            driver.find_elements_by_css_selector(
                'form[action="/AccessControl?handler=AddUserPermission"] button'
            )[0].click()
            time.sleep(3)

            # delete new user role perm
            driver.find_elements_by_css_selector(
                'a[href="/AccessControl?handler=RemoveUserPermission&UserId='
                + user_id
                + '&Id=5"'
            )[0].click()
            time.sleep(2)

            # open 2nd tab
            driver.find_elements_by_css_selector('a.tab-lnk[href="#rolepriviledges"]')[
                0
            ].click()
            time.sleep(1)

            # should create role and verify and then delete
            driver.find_elements_by_css_selector("#rolepriviledges span.input-span")[
                0
            ].send_keys("TestRole")
            time.sleep(1)

            driver.find_elements_by_css_selector(
                "#rolepriviledges .input-newWrapper button.input-submit"
            )[0].click()
            time.sleep(3)

            # change tab back and delete
            driver.find_elements_by_css_selector('a.tab-lnk[href="#rolepriviledges"]')[
                0
            ].click()
            time.sleep(1)
            driver.find_element_by_xpath("//th[text()='TestRole']/a").click()
            time.sleep(1)

            # confirm
            driver.switch_to.alert.accept()
            time.sleep(1)

            # for chrome
            if skip != 1:
                get_log(report, driver)

        def get_log(url, driver):
            """
            retrieve the browser console output. only works for chrome.
            in IE we need to make the browser alert any errors
            """

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
