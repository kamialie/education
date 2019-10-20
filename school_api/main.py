# Add path to modules directory
import sys
sys.path.insert(0, './modules')

# Install requests first - head over to install_requests_module.txt
# 1. Install pip(pip3)  - https://pip.pypa.io/en/stable/installing/
#   a. download file get-pip.py - 'curl https://bootstrap.pypa.io/get-pip.py -o get-pip.py'
#   b. run the file - 'python get-pip.py'
# 2. install requests module - pip3 install requests
# P.S. pay attention if using python3 install module for python3 - that is use pip3 not pip
import requests
import json
import time

# Install py-zabbix first - https://github.com/adubkov/py-zabbix
from pyzabbix.api import ZabbixAPI
from sys import argv
from statistics import mean

# Go to account(at the up right) -> preferences -> generate (token, button on the right)
token = 'B3Pynn5Y7UY9mmY8B1qJAFyn'

# Dynamic infrastructure url
# P.S. looks like first one is old
#url = 'http://mvp.esrt.cloud.sbrf.ru/api/v1/'
url = 'https://cs.cloud.sberbank.ru/api/v1/'

# Authorization through token
headers = {'Content-type' : 'application/json', 'Accept' : 'text/plain', 'authorization' : 'Token %s' % token}

# Can use edz or any other zone(currently only ipz is avalaible)
netzone = 'ipz'

# Temporary solution includes turning off certificate verification by 'verify=False' parameter
def apicall(method, verb = 'GET', body = ''):
    body = body.encode(encoding='utf-8')
    if verb == 'GET':
        response = requests.get('%s%s' % (url, method), headers = headers, verify=False)
    elif verb == 'POST':
        response = requests.post('%s%s' % (url, method), headers = headers, data = body, verify=False)
    elif verb == 'PATCH':
        response = requests.patch('%s%s' % (url, method), headers = headers, data = body, verify=False)
    elif verb == 'DELETE':
        response = requests.delete('%s%s' % (url, method), headers = headers, data = body, verify=False)
    else:
        print('Unrecognized verb')
        return (1, 0)
    if (response.ok):
        # TEMPORARY - delete request didnt return no body
        if (verb == 'DELETE'):
            return (0, 0)
            #print(response.content)
        jdata = json.loads(response.content)
        return (0, jdata)
    else:
        print('Error:%s %s %s' % (response.status_code, response.url, response.content))

# Server start/stop
def server_operation(server_id, job):
    state_info_object = {
            job : 'null'
            }
    request_json = json.dumps(state_info_object)
    answer = apicall('servers/%s/action' % server_id, 'PATCH', request_json)
    if (job == 'os-stop'):
        print('Stopping server...')
        current_job = 'stopping'
    else:
        print('Starting server...')
        current_job = 'starting'
    server_info = apicall('servers/%s' % server_id)[1]['server']
    while (server_info['state'] == current_job):
        time.sleep(3)
        server_info = apicall('servers/%s' % server_id)[1]['server']
    print(server_info['state'], server_info['id'], server_info['ip'])


# Get info about zabbix item
def zab_item_info(hostid, item_name):
    result = zapi.item.get(hostids=hostid, output='extend', search={'key_':item_name}, sortfield='name')[0]
    return(result)

hosts = []
# time range to monitor(default is 15 mins)
time_range = 15
server_name = None
upper_limit = None
lower_limit = None

# Get values from user: hosts to monitor, time_range, support_server_name
for item in argv[1::]:
    if (item.startswith('host=')):
        value = item[item.find('=') + 1::]
        if (value not in hosts):
            hosts.append(value)
    elif (item.startswith('time=')):
        time_range = item[item.find('=') + 1::]
    elif (item.startswith('server=')):
        server_name = item[item.find('=') + 1::]
    elif (item.startswith('upper_limit=')):
        upper_limit = float(item[item.find('=') + 1::])
    elif (item.startswith('lower_limit=')):
        lower_limit = float(item[item.find('=') + 1::])
    else:
        raise ValueError('passed invalid flag -', item)

if (len(hosts) == 0):
    raise ValueError('no host name found')
#print(hosts)

# Getting server's id
if (server_name == None):
    raise ValueError('support server name was not passed')
elif (upper_limit == None):
    raise ValueError('upper_limit value was not passed')
elif (lower_limit == None):
    raise ValueError('lower value was not passed')
elif (upper_limit <= lower_limit):
    raise ValueError('incorrect values for upper and lower limit')

server_info = apicall('servers?name=%s' % server_name)[1]['servers']
if (len(server_info) == 0):
    raise ValueError('server not found')
else:
    server_id = server_info[0]['id']

# Create ZabbixAPI class instance
zapi = ZabbixAPI(url='http://10.34.245.118', user='sberdisk', password='sberdisk')

# Get hosts info from zabbix
result = zapi.host.get(monitored_hosts=0)
zabbix_hosts = {}
for host in result:
    zabbix_hosts[host['host']] = host['hostid']

# Get host ids to monitor
hostids = []
for host in hosts:
    if (host not in zabbix_hosts.keys()):
        raise ValueError(host + ' not found in zabbix')
    hostids.append(zabbix_hosts[host])

# Get item ids to monitor
itemids = []
search_pattern = {'key_':'sys.cpu.util'}
#print('hostid item_name item_id - last_value')
for hostid in hostids:
    result = zapi.item.get(hostids=hostid, output='extend', search=search_pattern, sortfield='name')[0]
    print('host(%s) %s(%s) - %s%%' % (hostid, result['name'], result['itemid'], result['lastvalue']))
    itemids.append(result['itemid'])

# Get average values(cpu util) - using history
cpu_util_values = []
for itemid in itemids:
    result = zapi.history.get(history=0, itemids=itemid, sortfield='clock', sortorder='DESC', limit=time_range)
    item_values = [float(item['value']) for item in result]
    cpu_util_values.append(mean(item_values))
print('cpu_util_values -',cpu_util_values)

# Analyze data
flag = 0
for value in cpu_util_values:
    if (value > upper_limit):
        flag = 1
        server_state = apicall('servers/%s' % server_id)[1]['server']['state']
        if (server_state == 'running'):
            print('support server already running')
        else:
            server_operation(server_id, 'os-start')
        break
    elif (value < lower_limit):
        flag = 1
        server_state = apicall('servers/%s' % server_id)[1]['server']['state']
        server_state = apicall('servers/%s' % server_id)[1]['server']['state']
        if (server_state == 'stopped'):
            print('support server already stopped')
        else:
            server_operation(server_id, 'os-start')
        break
if (flag == 0):
    print('cpu_util is within limits')
        
# Logout from Zabbix
zapi.user.logout()
