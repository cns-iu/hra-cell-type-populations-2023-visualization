import requests
import json


def main():

    # first, let's get the most recent data
    # To get TOKEN for viewing unpublished data, go to EUI, log in, then view source, then copy token from browser
    TOKEN = ""
    endpoint = "https://ccf-api.hubmapconsortium.org/v1/hubmap/rui_locations.jsonld"
    headers = {"Authorization": "Bearer " + TOKEN}
    data = requests.get(endpoint, headers=headers).json()
    # print(data)
    # data = json.load("rui_locations.jsonld")
    
    search_string = 'VHFLeftKidney'
    # template = 
    
    counter = 0
    for donor in data['@graph']:
        for sample in donor['samples']:
            if search_string not in sample['rui_location']['placement']['target']:
                counter = counter +1
                sample['rui_location'] = ""
    print(counter)
    print(data)
    # with open("rui_locations.jsonld", "w") as outfile:

    #     json.dump(data, outfile)
           
# driver code
if __name__ == '__main__':
    main()
