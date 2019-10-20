from urllib.request import urlopen
from bs4 import BeautifulSoup
import ssl

# Ignore SSL certificate errors
ctx = ssl.create_default_context()
ctx.check_hostname = False
ctx.verify_mode = ssl.CERT_NONE

# Input absolutely not safe ;)
url = input('Enter url - ')
count = int(input('Enter count - '))
position = int(input('Enter position - ')) - 1
print('Retrieving:', url)
html = urlopen(url, context=ctx).read()
soup = BeautifulSoup(html, "html.parser")

for i in range(count):
    html = urlopen(url, context=ctx).read()
    soup = BeautifulSoup(html, "html.parser")
    url = [tag.get('href', None) for tag in soup('a')][position]
    print('Retrieving:', url)