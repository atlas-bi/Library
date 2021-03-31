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
import time
import unittest
import warnings

import urllib3
from selenium import webdriver
from selenium.webdriver.common.desired_capabilities import DesiredCapabilities
from selenium.webdriver.common.keys import Keys

urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)


class Urls(unittest.TestCase):
    """
    test class from urls

    setUp
    runText
        test for chrome
        test for ie
    cleanUp
    """

    def setUp(self):
        warnings.simplefilter("ignore", ResourceWarning)

        capabilities = DesiredCapabilities.INTERNETEXPLORER
        capabilities.update({"logLevel": "DEBUG"})
        capabilities["ignoreProtectedModeSettings"] = True
        # ie 11 cannot get browser output somehow, so change IE setting to alert w/ errors.
        # internet options > advanced > uncheck "disabled script debugging..." 2 boxes.
        # internet options > advanced > check "display a notification..."
        self.explorer = webdriver.Ie(capabilities=capabilities)

        options = webdriver.ChromeOptions()
        options.add_argument("--ignore-certificate-errors")
        self.chrome = webdriver.Chrome(options=options)

        self.my_site = "https://localhost:44380"

        self.top_urls = [
            "/",
            "/users",
            "/projects",
            "/Reports?id=21187",
            "/Projects?id=17",
            "/Search?s=COVID",
            "/Reports?id=72846",
            "/analytics",
            "/tasks",
            "/Reports?id=73711",
            "/Projects?id=12",
            "/Terms",
            "/Projects?id=10",
            "/Projects?id=18",
            "/accesscontrol",
            "/initiatives",
            "/Search?s=observation",
            "/Search?s=Dx",
            "/Reports?id=115703",
            "/Search?s=daily%20census",
            "/Search?s=readmission",
            "/Reports?id=123103",
            "/Search?s=r",
            "/Reports?id=73897",
            "/Reports?id=113831",
            "/Reports?id=76969",
            "/Reports?id=110726",
            "/Projects?id=19",
            "/Reports?id=108662",
            "/Search?s=the",
            "/Reports?id=73895",
            "/Search?s=c",
            "/parameters",
            "/Reports?id=87504",
            "/Reports?id=122570",
            "/users?id=602",
            "/Reports?id=109708",
            "/Search?s=D",
            "/Search?s=bcma",
            "/Reports?id=73916",
            "/Reports?id=116442",
            "/Reports?id=123609",
            "/Reports?id=113868",
            "/Search?s=Medicare%20wellness",
            "/Search?s=t",
            "/Search?s=Daily%20Census%20Report",
            "/Search?s=ltc",
            "/Reports?id=113155",
            "/Search?s=rmc%20",
            "/Search?s=rmc%20hb%20",
            "/Search?s=ED%20all",
            "/Reports?id=107250",
            "/Reports?id=87487",
            "/Search?s=sample",
            "/Search?s=L",
            "/Reports?id=21286",
            "/Reports?id=57845",
            "/Search?s=a",
            "/Reports?id=38661",
            "/Search?s=this",
            "/Reports?id=112874",
            "/Search?s=H",
            "/Reports?id=123268",
            "/Reports?id=122466",
            "/Search?s=s",
            "/Reports?id=87476",
            "/Reports?id=110225",
            "/Reports?id=116412",
            "/Reports?id=116573",
            "/Search?s=O",
            "/reports?id=58338",
            "/Reports?id=123755",
            "/Reports?id=75041",
            "/Initiatives?id=2",
            "/Reports?id=73911",
            "/Reports?id=122713",
            "/Reports?id=110898",
            "/Search?s=charge",
            "/Search?s=p",
            "/Search?s=ob",
            "/Reports?id=73970",
            "/Reports?id=58137",
            "/Projects?id=15",
            "/Reports?id=112759",
            "/Search?s=M",
            "/Reports?id=114373",
            "/Search?s=daily%20",
            "/Reports?id=113948",
            "/Search?s=covid%2019%20dashboard",
            "/Reports?id=74764",
            "/Reports?id=38550",
            "/Search?s=lt",
            "/Search?s=covid%20dash",
            "/Reports?id=12048",
            "/Search?s=covid%20dashboard",
            "/Projects?id=20",
            "/Reports?id=13525",
            "/Search?s=obser",
            "/Reports?id=121081",
            "/Reports?id=107244",
            "/Reports?id=121904",
            "/Reports?id=108801",
            "/Reports?id=73703",
            "/Search?s=RM",
            "/Search?s=RMG",
            "/Search?s=e",
            "/Reports?id=110825",
            "/Search?s=ect",
            "/Reports?id=73709",
            "/Reports?id=121209",
            "/Search?s=b",
            "/Search?s=daily",
            "/Search?s=hcc",
            "/Reports?id=73621",
            "/Reports?id=77029",
            "/Reports?id=108767",
            "/Reports?id=110104",
            "/Reports?id=108517",
            "/Search?s=medicare",
            "/Reports?id=38346",
            "/Search?s=Daily%20Censu",
            "/Search?s=Length%20of%20stay",
            "/Reports?id=116572",
            "/Reports?id=108765",
            "/Reports?id=58254",
            "/Projects?id=11",
            "/Reports?id=21168",
            "/Reports?id=73715",
            "/Reports?id=75186",
            "/Search?s=contaminated",
            "/Search?s=i",
            "/Search?s=deferral",
            "/Reports?id=114566",
            "/Search?s=teletracking",
            "/Search?s=rmg%20medicare%20wellness",
            "/Reports?id=53329",
            "/Search?s=smr",
            "/Reports?id=121388",
            "/Reports?id=76758",
            "/Reports?id=53503",
            "/Reports?id=109901",
            "/Reports?id=57790",
            "/Reports?id=87513",
            "/Reports?id=73755",
            "/Reports?id=73695",
            "/Reports?id=113544&EPIC=1&msg=1",
            "/Reports?id=72821",
            "/Search?s=pacu%20trans",
            "/Search?s=rmc%20hh",
            "/Reports?id=110545",
            "/Search?s=readmissions",
            "/Reports?id=60527",
            "/Reports?id=121565",
            "/Search?s=test",
            "/Reports?id=115722",
            "/Reports?id=73700",
            "/Search?s=121",
            "/Reports?id=73740",
            "/Reports?id=73725",
            "/Search?s=antimicrobial",
            "/Reports?id=116516",
            "/Reports?id=115363",
            "/Reports?id=75104",
            "/Initiatives?id=4",
            "/Search?s=3",
            "/Reports?id=58372",
            "/Reports?id=66746",
            "/Reports?id=73948",
            "/Reports?id=74911",
            "/Reports?id=98634",
            "/Reports?id=55074",
            "/Reports?id=73854",
            "/Search?s=ed",
            "/Reports?id=75022",
            "/Reports?id=123157",
            "/Reports?id=116700",
            "/Reports?id=72853",
            "/Search?s=RVU",
            "/Reports?id=121144",
            "/Reports?id=110793",
            "/Reports?id=121337",
            "/Reports?id=114490",
            "/Reports?id=87479&EPIC=0",
            "/Search?s=discharge",
            "/Reports?id=114012",
            "/Search?s=COVID&f=28&sf=",
            "/Search?s=ssrs",
            "/Reports?id=115367",
            "/Search?s=depart_dttm&f=&sf=Query",
            "/Search?s=LTC_",
            "/Search?s=napbc",
            "/Reports?id=66633",
            "/Search?s=rmc",
            "/Reports?id=87535&EPIC=1&msg=1",
            "/Search?s=post%20acute",
            "/Reports?id=110791",
            "/Reports?id=114406",
            "/Search?s=deferra",
            "/Search?s=bcma%20one%20user",
            "/Search?s=admission",
            "/Search?s=adt",
            "/Search?s=workqueue",
            "/Reports?id=77417",
            "/Search?s=ltc%20",
            "/Reports?id=113947",
            "/Reports?id=123787",
            "/Reports?id=109104",
            "/Search?s=mrsa",
            "/Search?s=covid%20",
            "/Search?s=covid%20department",
            "/Search?s=RMG%20",
            "/Reports?id=58225",
            "/Search?s=spinal%20fusion",
            "/Search?s=RMC%20LTC%20",
            "/Reports?id=53504",
            "/Reports?id=58254&EPIC=1&msg=1",
            "/Reports?id=114912",
            "/Reports?id=72824",
            "/Reports?id=64259",
            "/Reports?id=115437",
            "/Reports?id=108768",
            "/Reports?id=93131",
            "/Reports?id=115353",
            "/Reports?id=87527",
            "/Reports?id=109663",
            "/Reports?id=52000",
            "/Reports?id=83157",
            "/Reports?id=14366",
            "/Search?s=obs",
            "/Search?s=workqueue%20459",
            "/Reports?id=73255",
            "/Search?s=covid%20death",
            "/Reports?id=110355",
            "/Reports?id=109192",
            "/Reports?id=113866",
            "/Reports?id=110873",
            "/Reports?id=113654",
            "/Search?s=dail",
            "/Search?s=ed%20",
            "/Search?s=daily%20cen",
            "/Search?s=sickle%20cell",
            "/Search?s=all%20my%20patients",
            "/Reports?id=60415",
            "/Search?s=antibiotic",
            "/Search?s=consult",
            "/Reports?id=102265",
            "/Reports?id=109994",
            "/Search?s=PRE%20OP%20COVID",
            "/Reports?id=14571",
            "/Reports?id=109856",
            "/reports?id=58187",
            "/Reports?id=115357",
            "/Reports?id=64625",
            "/Reports?id=114491",
            "/Reports?id=67046",
            "/Reports?id=107251",
            "/Search?s=medication",
            "/Search?s=MDRO",
            "/Search?s=observ",
            "/Reports?id=87505",
            "/Search?s=224",
            "/Reports?id=114010",
            "/Reports?id=108556",
            "/Reports?id=114875",
            "/Search?s=rvu&f=21",
            "/Reports?id=55091",
            "/Reports?id=121543",
            "/Search?s=OR%20Manager%20Metrics",
            "/Search?s=v",
            "/Search?s=PB%20and%20HB%20overview",
            "/Reports?id=14480",
            "/Reports?id=109832",
            "/Reports?id=122611",
            "/Reports?id=121836",
            "/Reports?id=55075",
            "/Search?s=patients%20with%20observation%20time",
            "/Reports?id=58165",
            "/Reports?id=69501",
            "/Reports?id=108799",
            "/Search?s=daily%20cens",
            "/Reports?id=108908",
            "/Reports?id=121289",
            "/Search?s=census",
            "/Reports?id=110255",
            "/Reports?id=74898",
            "/Reports?id=67291",
            "/Terms?id=29",
            "/Reports?id=116736",
            "/Search?s=missing%20phone",
            "/Reports?id=73739",
            "/Reports?id=108800",
            "/Search?s=wou",
            "/Search?s=Lengt",
            "/Search?s=Sample%20Meds",
            "/Reports?id=73578",
            "/Search?s=Daily%20Ce",
            "/Reports?id=12045",
            "/Search?s=read",
            "/Reports?id=109664",
            "/Reports?id=123104",
            "/Reports?id=75077",
            "/Search?s=OBSERVA",
            "/Reports?id=73930",
            "/Reports?id=77487",
            "/Search?s=physician%20medicare",
            "/Search?s=timely%20filing%20deadline%20",
            "/Search?s=th",
            "/Reports?id=116259",
            "/Reports?id=21215",
            "/Reports?id=73848",
            "/Reports?id=114673",
            "/Reports?id=14248",
            "/Search?s=ncdr",
            "/Reports?id=109579",
            "/Reports?id=122699",
            "/Reports?id=123443",
            "/Search?s=MEdicaid",
            "/Reports?id=73992",
            "/Reports?id=55076",
            "/Reports?id=112898",
            "/Search?s=ip%20mh",
            "/Reports?id=13534",
            "/Reports?id=76969&EPIC=1&msg=1",
            "/Reports?id=77485",
            "/Reports?id=113717",
            "/Search?s=401771",
            "/Reports?id=53142",
            "/Reports?id=122396",
            "/Reports?id=114297",
            "/Reports?id=66655",
            "/reports?id=58186",
            "/Reports?id=109833",
            "/Reports?id=73886",
            "/Search?s=399",
            "/Reports?id=113544",
            "/Reports?id=87507",
            "/Reports?id=14630",
            "/Reports?id=72839",
            "/Reports?id=73954",
            "/Search?s=losi&f=28&sf=",
            "/Reports?id=74012",
            "/Search?s=rmg%20medicare",
            "/Reports?id=123972",
            "/Search?s=nyha",
            "/Search?s=cardiology%20read",
            "/Search?s=blerg",
            "/Search?s=pacu%20transport",
            "/Search?s=Patient",
            "/Search?s=V_ACCESS_LOG",
            "/Projects?id=17&EPIC=1",
            "/Reports?id=112853",
            "/Reports?id=114759",
            "/Search?s=chest%20pain",
            "/reports?id=58176",
            "/Reports?id=108569",
            "/Reports?id=75348",
            "/Search?s=heart%20failure",
            "/Reports?id=21015",
            "/Reports?id=108637",
            "/Search?s=OBSERVATION%20",
            "/Search?s=COVID-19",
            "/Reports?id=71181",
            "/reports?id=58175",
            "/Reports?id=58356",
            "/Reports?id=38977",
            "/Reports?id=59177",
            "/Reports?id=123021",
            "/Search?s=wellness",
            "/Search?s=the%20lazy%20jump",
            "/Reports?id=72847",
            "/Search?s=71990",
            "/Search?s=mobile%20rx",
            "/Reports?id=71178",
            "/Search?s=referral",
            "/Reports?id=109405",
            "/Reports?id=73936",
            "/Reports?id=113912",
            "/Reports?id=20901",
            "/Reports?id=58104",
            "/Search?s=ed%20dashboard",
            "/Reports?id=115816",
            "/Reports?id=114634",
            "/Reports?id=114655",
            "/Reports?id=116513",
            "/Search?s=radiology",
            "/Reports?id=14245",
            "/Search?s=phys%20advisor",
            "/Reports?id=59277",
            "/Reports?id=73716",
            "/Reports?id=114665",
            "/Search?s=mst",
            "/Reports?id=95128",
            "/Reports?id=112935",
            "/Reports?id=93129",
            "/Initiatives?id=3",
            "/Reports?id=110893",
            "/Reports?id=58391",
            "/Reports?id=107139",
            "/Reports?id=123527",
            "/Search?s=revenue%20and%20usage",
            "/Search?s=ADT%20",
            "/Search?s=RVUs",
            "/Reports?id=113154",
            "/Search?s=observatio",
            "/Reports?id=58125",
            "/Search?s=pharmacy",
            "/Reports?id=51478",
            "/Search?s=zip%20code",
            "/Search?s=1",
            "/Reports?id=102265&EPIC=1&msg=1",
            "/Reports?id=116656",
            "/Search?s=draft",
            "/Search?s=mh",
            "/Reports?id=77131",
            "/Search?s=ip%20to%20op",
            "/Search?s=rover%20scan%20compliance",
            "/Reports?id=109860&EPIC=1&msg=1",
            "/Search?s=blood%20culture",
            "/Reports?id=108947",
            "/Search?s=ED%20discharges",
            "/Search?s=glucommander",
            "/Reports?id=110546",
            "/Reports?id=121600",
            "/Search?s=Length%20o",
            "/Reports?id=76780",
            "/Search?s=dh",
            "/Reports?id=114646",
            "/Search?s=readmi",
            "/Reports?id=109860",
            "/Reports?id=121403",
            "/Search?s=electroconvulsive%20",
            "/Reports?id=93130",
            "/Search?s=block%20util",
            "/Reports?id=110895",
            "/Reports?id=116515",
            "/Search?s=RMC%20RAC",
            "/Reports?id=74053",
            "/Reports?id=57898",
            "/Search?s=teletra",
            "/Reports?id=123415",
            "/Search?s=ip",
            "/Search?s=Su",
            "/Search?s=readmis",
            "/Reports?id=116025",
            "/Reports?id=54628",
            "/Search?s=superuser",
            "/Reports?id=116268",
            "/Search?s=pacu%20tran",
            "/Reports?id=58398",
            "/Reports?id=64551",
            "/Terms?id=74",
            "/Reports?id=75255",
            "/Reports?id=57838",
            "/Reports?id=14488",
            "/Reports?id=73761",
            "/Search?s=covid&f=20",
            "/Search?s=LOS",
            "/Reports?id=121684",
            "/Reports?id=57801",
            "/Search?s=pre-op%20covid",
            "/Reports?id=114650",
            "/Search?s=adt%20inpatient%20transfers",
            "/Reports?id=73796",
            "/Reports?id=21278",
            "/Reports?id=57773",
            "/Search?s=chaplain",
            "/Search?s=sepsis",
            "/Reports?id=124100",
            "/Search?s=hb%20",
            "/Reports?id=116636",
            "/Search?s=miller",
            "/Reports?id=73880",
            "/Reports?id=59241",
            "/Reports?id=110896",
            "/Reports?id=114895",
            "/Reports?id=108403",
            "/Search?s=covid%20empl",
            "/reports?id=58182",
            "/Search?s=ccl",
            "/reports?id=76702",
            "/Terms?id=80",
            "/Reports?id=12046",
            "/users?id=172",
            "/Search?s=length",
            "/Search?s=Medica",
            "/reports?id=65913",
            "/Search?s=LAB30739",
            "/Search?s=readm",
            "/Reports?id=122528",
            "/Search?s=AR",
            "/Reports?id=66990",
            "/Reports?id=58340",
            "/Reports?id=115659",
            "/Reports?id=61867",
            "/users?id=667",
        ]

    def runTest(self):
        """
        test has inner function to run for both chrome and ie.
        """

        warnings.simplefilter("ignore", ResourceWarning)
        print("url test")

        def test(driver, skip):
            # top urls
            for x in self.top_urls:
                print(x)
                driver.get(self.my_site + x)
                if skip != 1:
                    get_log(self.my_site + x, driver)

            print("checked " + str(len(self.top_urls)) + " urls.")
            # search
            driver.get(self.my_site)

            self.assertIn("Atlas", driver.title)
            time.sleep(10)  # allow js to run
            if skip != 1:
                get_log(driver.current_url, driver)
            elem = driver.find_element_by_css_selector(".sr-grp > input")
            elem.clear()
            elem.send_keys("covid")
            elem.send_keys(Keys.RETURN)
            time.sleep(5)  # allow js to run
            if skip != 1:
                get_log(driver.current_url, driver)
            elem = driver.find_elements_by_tag_name("a")
            urls = [
                str(x.get_attribute("href"))
                for x in elem
                if str(x.get_attribute("href")).startswith(self.my_site)
            ]
            assert "No results found." not in driver.page_source
            if len(urls) > 0:
                for x in urls:
                    print(x)
                    driver.get(x)
                    if skip != 1:
                        get_log(x, driver)

                print("checked " + str(len(urls)) + " urls.")

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

        test(self.explorer, 1)
        test(self.chrome, 0)

    def tearDown(self):
        self.explorer.close()
        self.chrome.close()


if __name__ == "__main__":
    unittest.main()
