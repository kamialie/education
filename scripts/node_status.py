import jenkins
import json
import sys

# Wrapper info
# https://pypi.org/project/python-jenkins/

# Run as below to disable ssl verification
#CURL_CA_BUNDLE="" python3 node_status.py

# To ignore warning of insecure connection from urllib
import warnings
warnings.filterwarnings("ignore")

# Usage
if (len(sys.argv) != 2):
    print('usage:   python3 <script.py> <data.json>')
    exit(0)

# Preparation
data = sys.argv[1].lower()
fh = open(data)
info = json.loads(fh.read())

# Connecting to server
server = jenkins.Jenkins(info['server'], username=info['user_name'], password=info['password'])

# General info
user = server.get_whoami()
version = server.get_version()
print('Hello %s from Jenkins %s' % (user['fullName'], version))

# Info on nodes
nodes = info['nodes']
print("Node status:")
for node in nodes:
    node_config = server.get_node_info(node)
    print("\t%s: " % node, end='')
    if (node_config['offline'] == True):
        print("❌")
    else:
        print("✅")
