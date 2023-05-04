import requests
import json


def main():

    # first, let's get the most recent data
    # To get TOKEN for viewing unpublished data, go to EUI, log in, then view source, then copy token from browser
  
    endpoint = "https://ccf-api.hubmapconsortium.org/v1/hubmap/rui_locations.jsonld"
    
    # data = requests.get(endpoint, headers=headers).json()
    # print(data)
    # data = json.load("rui_locations.jsonld")

    endpoints = [
        "https://ccf-api.hubmapconsortium.org/v1/hubmap/rui_locations.jsonld",
        "https://ccf-api.hubmapconsortium.org/v1/gtex/rui_locations.jsonld"
    ]

 
    
    get_data(endpoints[0], 'hubmap')
    get_data(endpoints[1], 'gtex')
       
  
def get_data(url, consortium):
    TOKEN = ""
    search_string = 'VHFLeftKidney'
    headers = {"Authorization": "Bearer " + TOKEN}
    data = requests.get(url, headers=headers).json()
    newData = []
    for donor in data['@graph']:
        newSamples = []
        for sample in donor['samples']:
            if search_string in sample['rui_location']['placement']['target']:
             newSamples.append(sample)

        if len(newSamples) > 0:
            newData.append(donor);
            donor['samples'] = newSamples
            
    data['@graph'] = newData

    # print(data)
    with open("rui_locations_" + consortium + ".jsonld", "w") as outfile:
        json.dump(data, outfile, ensure_ascii=False, indent=4)

# driver code
if __name__ == '__main__':
    main()
