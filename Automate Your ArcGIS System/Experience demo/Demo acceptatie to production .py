#-------------------------------------------------------------------------------
# Name:        Demo OTAP
# Purpose:     to demo to update a web experience
#
# Author:      evanmeijeren
#
# Created:     25-03-2024
# Copyright:   (c) Esri Nederland BV 2024
#-------------------------------------------------------------------------------
from arcgis.apps.expbuilder import WebExperience
from arcgis.gis import GIS
import Security

exp_acceptatieId = "075984b8f4a84cde9b7014d2b6f75ddd"
exp_productieId = "25d93c0b2d164e828b80136fe44e3463"

print("create GIS object")
gis = GIS(username=Security.GetUsername(), password=Security.GetPassword())

print("get WebExperience objects")
portal_exp_acceptatie = WebExperience(exp_acceptatieId, gis = gis)
portal_exp_productie = WebExperience(exp_productieId, gis = gis)

sourceLabel = portal_exp_productie.datasources['dataSource_1']['sourceLabel']
itemId = portal_exp_productie.datasources['dataSource_1']['itemId']

portal_exp_acceptatie._draft['dataSources']['dataSource_1']['sourceLabel'] = sourceLabel
portal_exp_acceptatie._draft['dataSources']['dataSource_1']['itemId'] = itemId

print("publishing the acceptetd web experience to production")
portal_exp_productie._draft = portal_exp_acceptatie._draft
portal_exp_productie.save(publish = True)

print("script complete")
