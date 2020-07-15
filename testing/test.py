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
import unittest
from selenium import webdriver
from selenium.webdriver.common.keys import Keys

import time
import urllib3
urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)

import json

#mySite = "https://atlas-test.riversidehealthcare.net"
#browser = "ie"
browser = "chrome"
mySite = "https://localhost:44380/"

class Atlas(unittest.TestCase):

    def setUp(self):
        if browser == 'ie':
            capabilities = webdriver.DesiredCapabilities().INTERNETEXPLORER
            #capabilities['acceptSslCerts'] = True
            #capabilities['acceptInsecureCerts'] = True
            self.driver = webdriver.Ie(capabilities=capabilities)

        elif browser == 'chrome':
            options = webdriver.ChromeOptions()
            options.add_argument('--ignore-certificate-errors')
            self.driver = webdriver.Chrome(options=options)


    def testSearch(self):
        self.driver.get(mySite)
        driver = self.driver
        self.assertIn("Atlas", driver.title)
        time.sleep(10) #allow js to run
        self.getLog(driver.current_url)
        elem = driver.find_element_by_css_selector(".sr-grp > input")
        elem.clear()
        elem.send_keys("this")
        elem.send_keys(Keys.RETURN)
        time.sleep(5) #allow js to run
        self.getLog(driver.current_url)
        elem = self.driver.find_elements_by_tag_name("a")
        urls = [str(x.get_attribute('href')) for x in elem if str(x.get_attribute('href')).startswith(mySite)]
        assert "No results found." not in driver.page_source
        if len(urls) > 0:
            self.testUrls(urls)

    def findUrls(self):
        self.driver.get(mySite)
        time.sleep(5) #allow js to run
        elem = self.driver.find_elements_by_tag_name("a")
        urls = [str(x.get_attribute('href')) for x in elem if str(x.get_attribute('href')).startswith(mySite)]
        if len(urls) > 0:
            self.testUrls(urls)

    #@unittest.skip("other function")
    def testUrls(self,urls):
        for x in urls:
            self.driver.get(x)
            self.getLog(x)

        print("checked " + str(len(urls)) + " urls.")

    #@unittest.skip("other function")
    def getLog(self, url):
        q = self.driver.get_log('browser')
        if len(q) > 0:
            for log in q:
                if  log["level"] == 'SEVERE':
                    print(url + "," + log["source"]  +"," + log["level"] +"," + log["message"])



    def tearDown(self):
        self.driver.close()


if __name__ == "__main__":
    unittest.main()

