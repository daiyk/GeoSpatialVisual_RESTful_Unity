# VLab Unity GIS Test
A code test project for fetching GIS data from ArcGIS REST APIs and do data visualization.

## Dependencies
- Unity 2020.3.6f LTS
- [RestSharp](https://www.nuget.org/packages/RestSharp/)
- [RestSharp.Serializers.SystemTextJson](https://www.nuget.org/packages/RestSharp.Serializers.SystemTextJson/)
- [NuGetForUnity](https://github.com/GlitchEnzo/NuGetForUnity)
## Usage
1. All init parameters should be already set up, click play in Unity it should shows the data points represented by red spheres for 1000m\*1000m square around the init position.
2. Change the main camera's transform->position in editor and move above 300 meters should trigger the request again and generate new set of data points around the new position, 
e.g. move from (0,0,0) -> (300,0,0) should trigger new request, and move back (300,0,0)->(0,0,0) should trigger another request.
3. Any update on data points and request will print out log information on Unity editor, you can check it for verfication.
