from urllib.request import urlopen
import json

url = input("Enter url - ")
try:
    print('Retrieving', url)
    data = urlopen(url).read()
    print('Retrieved %d characters' %len(data))
except:
    print("Couldnt open url")
    quit()
 
try:
    comments = json.loads(data)
except:
    print('Couldnt load json')
    quit()

print('Sum:', sum([int(n['count']) for n in comments['comments']]))