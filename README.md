# GeoJsonCityBuilder
![OpenUPM version](https://img.shields.io/npm/v/nl.elmarjansen.geojsoncitybuilder?label=openupm&registry_uri=https://package.openupm.com)
![GitHub issues](https://img.shields.io/github/issues/elmarj/GeoJsonCityBuilder)

Unity Package to recreate a 3D-city from a geojson file. Currently allows to create extruded objects (e.g. buildings) from polygons and to place prefabs on points.

See for an example on what can be achieved and how to use it, the [Waterlooplein 3D project](https://github.com/ElmarJ/Waterlooplein3D).

## Installation

### Option 1: Through OpenUPM

This package is [listed in the OpenUPM repository](https://openupm.com/packages/nl.elmarjansen.geojsoncitybuilder/#). Installation:

```
npm install -g openupm-cli
cd YOUR_UNITY_PROJECT_FOLDER
openupm add nl.elmarjansen.geojsoncitybuilder
```

### Option 2: As a git-link in Unity package manager

1. Click Window -> Package Manager
2. Click + -> Add Package from git URL
3. Fill in "https://github.com/ElmarJ/GeoJsonCityBuilder" and click Add

## Usage

### Blocks from geojson polygons
 
 1. Create an empty GameObject
 2. Click Add Component and add Scripts/GeoJsonCityBuilder/Position on World Coordinates
 3. Set the desired Lat/Long-coordinates for the Unity world origin in the "Position on World Coordinates" component (choose a location central to the area that you want to reconstruct).
 4. Click Add Component and add Scripts/GeoJsonCityBuilder/Blocks From Geo Json
 5. Drag your geojson-file into "Geo Json File". Make sure it has a ".json"-extension; ".geojson" will not work)
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

