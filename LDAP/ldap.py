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
from ldap3 import Server, Connection, ALL, SUBTREE
import pyodbc
import settings
#https://ldap3.readthedocs.io/

# ssl is optional
s = Server(settings.server_uri, use_ssl=True, get_info=ALL)
c = Connection(s, settings.username,settings.password, auto_bind=True, auto_referrals=False)
c.start_tls()

#print(s.info) # server info 
#print(c.result) # connection status 

""" 
  1. employee search
      this will create User and Membership table
  2. group search
      this will create the Groups table

"""
search_bases = [
            'EPIC'
           ,'Employees'
           ,'Doctors'
           ,'Non-Staff'
           ,'Students'
           ,'Volunteers'
       ]

group_search_bases = [
            'Email Distribution Groups',
            'Room & Shared Mailboxes'
       ]

users = []
memberships = []
groups = []

for base in search_bases:

    # ldap only returns 1000 records at a time. generator will get all.
    generator = c.extend.standard.paged_search(search_base = 'OU=' + base + ',dc='+settings.dc+',dc=net'
                ,search_filter = '(CN=*)'
                ,search_scope = SUBTREE
                ,attributes = ['*']
                ,paged_size = 1000
                ,generator=True)
       
    for q in generator:
        data = dict(q)['attributes']
        
        row = [
               base,
               data['employeeID'] if 'employeeID' in data else '',
               settings.ad_domain + '\\' + data['sAMAccountName'] if 'sAMAccountName' in data else '', # email name
               data['displayName'] if 'displayName' in data else '',
               data['cn'] if 'cn' in data else '' or data['name'] if 'name' in data else '', #full name
               data['givenName'] if 'givenName' in data else '', # first name
               data['sn'] if 'sn' in data else '', # last name
               data['department'] if 'department' in data else '', # job title
               data['title'] if 'title' in data else '' or data['description'][0] if 'description' in data else '', #department
               data['ipPhone']  if 'ipPhone' in data else '' or data['telephoneNumber'] if 'telephoneNumber' in data else '', # phone number 
               data['mail'] if 'mail' in data else '' or data['proxyAddresses'] if 'proxyAddresses' in data else '' or data['userPrincipalName'] if 'userPrincipalName' in data else '', #email address
        ]
        
        users.append([sub.replace('\u200e', '') for sub in row])

        if 'memberOf' in data:
                     
            for r in data['memberOf']:
                memberdict = dict([x.split('=') for x in r.split(',')]) 

                memberrow = [
                    settings.ad_domain + '\\' + data['sAMAccountName'] if 'sAMAccountName' in data else '', # email name
                    memberdict['OU'] if 'OU' in r else '',
                    memberdict['CN']
                ]

                # only save three groups
                if('OU' in memberdict and memberdict['OU'] in group_search_bases):
                    memberships.append(memberrow)
                 
                                 

for base in group_search_bases:

    # ldap only returns 1000 records at a time. generator will get all.
    generator = c.extend.standard.paged_search(search_base = 'OU=' + base + ',dc='+settings.dc+',dc=net'
                ,search_filter = '(CN=*)'
                ,search_scope = SUBTREE
                ,attributes = ['*']
                ,paged_size = 1000
                ,generator=True)
       
    for q in generator:
        data = dict(q)['attributes']
        
        row = [
               base,
               data['sAMAccountName'] if 'sAMAccountName' in data else '', # email name
               data['displayName'] if 'displayName' in data else '',
               data['mail'] if 'mail' in data else '' or data['proxyAddresses'] if 'proxyAddresses' in data else '' or data['userPrincipalName'] if 'userPrincipalName' in data else '', #email address
        ]
        
        groups.append(row)
        
# close connection
c.unbind()

# insert data to db
conn = pyodbc.connect(settings.database,autocommit=True)
cursor = conn.cursor()
cursor.execute('DELETE FROM [LDAP].[dbo].[Users] where 1=1; DELETE FROM [LDAP].[dbo].[Memberships] where 1=1; DELETE FROM [LDAP].[dbo].[Groups] where 1=1; ')
cursor.executemany('INSERT INTO [LDAP].[dbo].[Users] (Base,EmployeeId,AccountName,DisplayName,FullName,FirstName,LastName,Department,Title,Phone,Email) VALUES (?,?,?,?,?,?,?,?,?,?,?)', users)
if len(memberships) > 0:
    cursor.executemany('INSERT INTO [LDAP].[dbo].[Memberships] (AccountName, GroupType, GroupName) VALUES (?,?,?)', memberships)
if len(groups) > 0:
    cursor.executemany('INSERT INTO [LDAP].[dbo].[Groups] (GroupType, AccountName, GroupName, GroupEmail) VALUES (?,?,?,?)', groups)
conn.close()