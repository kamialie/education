import requests
import json

URL = 'https://api.intra.42.fr'
UID = "c34b683a08081bcb7c7acb8cfedda188c17f3a6cf71a3ea9e7fd1560e739c37b"
SECRET = "797496b328b7f6a1e7860aef21f44b3aab22169bfad871c77dcb09e9707bba7a"

def get_token():
    TOKEN_URL = URL + "/oauth/token"
    post_data = { "grant_type" : "client_credentials" }
    response = requests.post(TOKEN_URL, post_data, auth = (UID, SECRET))
    if (response.status_code == 200):
        TOKEN = response.json()["access_token"]
        print('token is %s' % TOKEN)
        return (TOKEN)
    else:
        print("authentication error occured!")
        exit()

TOKEN = get_token()
print(string)
#response = requests.get(URL + '/v2/me', headers={'Authorization': TOKEN})
#print(response)
