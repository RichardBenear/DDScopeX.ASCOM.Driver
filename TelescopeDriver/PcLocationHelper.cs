using ASCOM.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace ASCOM.DDScopeX.Utility
{
  public static class PcLocationHelper
  {
    private class GeoResponse
    {
      public string status { get; set; }
      public double lat { get; set; }
      public double lon { get; set; }
      public string timezone { get; set; }
    }

    private class ElevationResult
    {
      public List<ElevationEntry> results { get; set; }
    }

    private class ElevationEntry
    {
      public double elevation { get; set; }
    }

    public static double GetPcLatitude()
    {
      var data = GetGeoData();
      return data?.lat ?? throw new Exception("Latitude not available.");
    }

    public static double GetPcLongitude(TraceLogger tl)
    {
      var data = GetGeoData();
      if (data == null)
        throw new Exception("Longitude not available.");

      double lon = Math.Abs(data.lon); // Force positive for West for OnStepX compatibility
      //double lon = data.lon;

      tl.LogMessage("GetPcLongitude", $"PC Longitude: {lon}");

      // Convert -180 to +180 into 0 to 360 range
      //return (lon < 0) ? lon + 360.0 : lon;
      return lon;
    }

    public static string GetPcTimeZone()
    {
      var data = GetGeoData();
      return data?.timezone ?? throw new Exception("Timezone not available.");
    }

    public static double GetPcElevation()
    {
      using (var client = new HttpClient())
      {
        // Step 1: Get Site lat/lon from IP
        const string locationUrl = "http://ip-api.com/json";
        var geoResponse = client.GetStringAsync(locationUrl).Result;
        var geoData = JsonSerializer.Deserialize<GeoResponse>(geoResponse);

        if (geoData == null || geoData.status != "success")
          throw new Exception("Failed to retrieve PC location for elevation lookup.");

        double lat = geoData.lat;
        double lon = geoData.lon;

        // Step 2: Get elevation using lat/lon
        string elevationUrl = $"https://api.opentopodata.org/v1/srtm90m?locations={lat},{lon}";
        var elevResponse = client.GetStringAsync(elevationUrl).Result;
        var elevData = JsonSerializer.Deserialize<ElevationResult>(elevResponse);

        if (elevData?.results?.Count > 0)
          return elevData.results[0].elevation;

        throw new Exception("Failed to retrieve elevation from API.");
      }
    }


    private static GeoResponse GetGeoData()
    {
      const string url = "http://ip-api.com/json/";

      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
      request.UserAgent = "DDScopeX-ASCOM-Driver";

      using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
      using (Stream stream = response.GetResponseStream())
      using (StreamReader reader = new StreamReader(stream))
      {
        string json = reader.ReadToEnd();
        var geo = JsonSerializer.Deserialize<GeoResponse>(json);

        if (geo == null || geo.status != "success")
          throw new Exception("Geolocation failed.");

        return geo;
      }
    }
  }
}
