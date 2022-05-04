# GeoJsonCityBuilder
[![OpenUPM version](https://img.shields.io/npm/v/nl.elmarjansen.geojsoncitybuilder?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/nl.elmarjansen.geojsoncitybuilder/)
[![GitHub issues](https://img.shields.io/github/issues/elmarj/GeoJsonCityBuilder)](https://github.com/ElmarJ/GeoJsonCityBuilder/issues)

Unity Package to recreate a 3D-city from a geojson file. Currently allows to create extruded objects (e.g. buildings) from polygons and to place prefabs on points.

See for an example on what can be achieved and how to use it, the [Waterlooplein 3D project](https://github.com/ElmarJ/Waterlooplein3D).

## Installation

### Option 1: Using OpenUPM cli

This package is [listed in the OpenUPM repository](https://openupm.com/packages/nl.elmarjansen.geojsoncitybuilder/#). Installation:

```
npm install -g openupm-cli
cd YOUR_UNITY_PROJECT_FOLDER
openupm add com.virgis.geojson.net
openupm add nl.elmarjansen.geojsoncitybuilder
```

Note that this package depends on the geojson package com.virgis.geojson.net being installed.

### Option 2: As a git-link in Unity package manager

1. Click Window -> Package Manager
2. Click + -> Add Package from git URL
3. Type _https://github.com/ViRGIS-Team/GeoJSON.Net_ and click Add 
4. Type _https://github.com/ElmarJ/GeoJsonCityBuilder_ and click Add

## Usage

See https://github.com/ElmarJ/geojsoncitybuilder.devenv for a simple working example in Unity. See https://github.com/ElmarJ/Waterlooplein3D/ for a more advanced example (using the Unity HDRP pipeline).

### Blocks from geojson polygons
 
 1. Create an empty GameObject
 2. Click Add Component and add Scripts/GeoJsonCityBuilder/Position on World Coordinates
 3. Set the desired Lat/Long-coordinates for the Unity world origin in the "Position on World Coordinates" component (choose a location central to the area that you want to reconstruct).
 4. Click Add Component and add Scripts/GeoJsonCityBuilder/Blocks From Geo Json
 5. Drag your geojson-file into "Geo Json File". Make sure it has a ".json"-extension; ".geojson" will not work.
 6. Set other settings and materials
 7. Click Generate

### Prefabs from geojson polygons
 
 1. Create an empty GameObject
 2. Click Add Component and add Scripts/GeoJsonCityBuilder/Position on World Coordinates
 3. Set the desired Lat/Long-coordinates for the Unity world origin in the "Position on World Coordinates" component (choose a location central to the area that you want to reconstruct).
 4. Click Add Component and add Scripts/GeoJsonCityBuilder/Blocks From Geo Json
 5. Drag your geojson-file into "Geo Json File". Make sure it has a ".json"-extension; ".geojson" will not work)
 6. Drag your prefab into "Prefab"
 7. Click Generate

## Open source credits

This package makes use of the [JSONObject library by Matt Schoen](https://github.com/mtschoen/JSONObject)

# Links

Links kept for reference to similar or otherwise relevant projects:
 - [ViRGIS](https://www.virgis.org/) ([Github](https://github.com/ViRGIS-Team/ViRGiS_v2)): a project to bring GIS data and VR together in Unity.
 - [BlenderGis](https://github.com/domlysz/BlenderGIS): a plugin for Blender to generate objects from GIS-data
 - [Use Houdini for procedural city building](https://www.sidefx.com/tutorials/city-building-with-osm-data/)
