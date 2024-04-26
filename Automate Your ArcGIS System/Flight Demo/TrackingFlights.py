#-------------------------------------------------------------------------------
# Name:        Demo weather
# Purpose:     to demo to update a web experience
#
# Author:      evanmeijeren
#
# Created:     25-03-2024
# Copyright:   (c) Esri Nederland BV 2024
#-------------------------------------------------------------------------------
import arcgis
from arcgis.gis import GIS
from arcgis import geometry
import requests
import Security
import time

url_opensky = "https://opensky-network.org/api/states/all"
url_arcgis_tracking= "https://services8.arcgis.com/uHOdnmf11dRV3P4N/arcgis/rest/services/TrackingFlights/FeatureServer/0"

gis = GIS(username=Security.GetUsername(), password=Security.GetPassword())
flayer = arcgis.features.FeatureLayer(url_arcgis_tracking, gis)

while True:
    url_opensky = "https://opensky-network.org/api/states/all"
    response = requests.get(f'{url_opensky}')
    vectors = response.json()['states']

    adds = []
    for vector in vectors:
        attributes = {
            "icao24": vector[0],
            "callsign": vector[1],
            "origin_country": vector[2],
            "time_position": vector[3],
            "velocity": vector[9],
            "on_ground": str(vector[8]),
            "geo_altitude": vector[13],
            "true_track" : vector[10]
        }

        feature = arcgis.features.Feature(
            { 
                "x" : vector[5], 
                "y" : vector[6],
                "spatialReference" : {"wkid" : 4326}
            },
            attributes
        )

        adds.append(feature)
    
    result = flayer.delete_features(where="objectid > 0")
    
    chunkSize = 250
    chunks_adds = [adds[x:x+chunkSize] for x in range(0, len(adds), chunkSize)]
    for chunk in chunks_adds:
        #Create Features
        response =  flayer.edit_features(adds = chunk)


 