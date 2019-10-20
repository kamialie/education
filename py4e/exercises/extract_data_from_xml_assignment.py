from urllib.request import urlopen
import xml.etree.ElementTree as ET

url = input("Enter url - ")
try:
    print('Retrieving', url)
    data = urlopen(url).read()
    print('Retrieved %d characters' %len(data))
except:
    print("Couldnt open url")
    quit()

try:
    tree = ET.fromstring(data)
except:
    print("Wrong xml format")
    quit()

print('Sum:', sum([int(n.text) for n in tree.findall('.//count')]))