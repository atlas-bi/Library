import requests
import os

values = {'Id': (None,'1'), 'File':(
        os.path.basename('./Data Governance WebApp/wwwroot/img/Placeholder Report Screenshot.PNG'), 
        open('./Data Governance WebApp/wwwroot/img/Placeholder Report Screenshot.PNG', 'rb'), 
        'image/png'
    )}

# print(requests.Request('post','http://localhost:1234/Reports?handler=AddImage', files=values).prepare().body)
r = requests.post('http://localhost:1234/Reports?handler=AddImage', files=values)
print(r.status_code)
