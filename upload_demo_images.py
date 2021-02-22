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

import os

import requests

reports = [
    1,
    1,
    2,
    2,
    3,
    3,
    4,
    4,
    5,
    5,
    6,
    6,
    7,
    7,
    8,
    8,
    9,
    9,
    10,
    10,
    11,
    11,
    12,
    12,
    13,
    13,
    14,
    14,
    15,
    15,
    16,
    16,
    17,
    17,
    18,
    18,
    19,
    19,
    20,
    20,
    21,
    21,
    22,
    22,
    23,
    23,
    24,
    24,
    25,
    25,
    26,
    26,
    27,
    27,
    28,
    28,
    29,
    29,
    30,
    30,
    31,
    31,
    32,
    32,
    33,
    33,
    34,
    34,
    35,
    35,
    36,
    36,
    37,
    37,
    38,
    38,
    39,
    39,
    40,
    40,
    41,
    41,
    42,
    42,
    43,
    43,
    44,
    44,
    45,
    45,
    46,
    46,
    47,
    47,
    48,
    48,
    49,
    49,
    50,
    50,
]

web_url = (
    "http://ip172-18-0-84-bt1vj7lim9m0009khfu0-1234.direct.labs.play-with-docker.com"
)
for x in reports:
    print(x)
    values = {
        "Id": (None, x),
        "File": (
            os.path.basename(
                "./Data Governance WebApp/wwwroot/img/Placeholder Report Screenshot.PNG"
            ),
            open(
                "./Data Governance WebApp/wwwroot/img/Placeholder Report Screenshot.PNG",
                "rb",
            ),
            "image/png",
        ),
    }

    # print(requests.Request('post',web_url + '/Reports?handler=AddImage', files=values).prepare().body)
    r = requests.post(web_url + "/Reports?handler=AddImage", files=values)
    print(r.status_code)
