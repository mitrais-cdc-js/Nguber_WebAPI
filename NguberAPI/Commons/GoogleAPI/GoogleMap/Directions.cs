using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NguberAPI.Commons.GoogleAPI.GoogleMap {
  public partial class Directions {
    #region Constants
    protected const string ServiceURL = "https://maps.googleapis.com/maps/api/directions/json";

    public enum MODE {
      DRIVING,
      WALKING,
      BICYCLING,
      TRANSIT
    }

    public enum AVOID {
      NONE = 0x00,
      TOLLS = 0x01,
      HIGHWAYS = 0x02,
      FERRIES = 0x04,
      INDOOR = 0x08,
      ALL = 0xFF
    }

    public enum UNITS {
      METRIC,
      IMPERIAL
    }

    public enum TRAFFIC_MODEL {
      BEST_GUESS,
      PESSIMISTIC,
      OPTIMISTIC
    }

    public enum TRANSIT_MODE {
      NONE = 0x00,
      BUS = 0x01,
      SUBWAY = 0x02,
      TRAIN = 0x04,
      TRAM = 0x08,
      RAIL = 0x0E,
      ALL = 0xFF
    }

    public enum TRANSIT_ROUTING_PREFERENCE {
      LESS_WALKING,
      FEWER_TRANSFERS
    }
    #endregion


    #region Protected Properties
    #endregion


    #region Public Properties
    #endregion


    #region Constructors & Destructor
    #endregion


    #region Protected Methods
    private string ComposeAvoids (AVOID Avoids) {
      var avoids = new List<string>();
      if (0 < (AVOID.TOLLS & Avoids))
        avoids.Add("tolls");

      if (0 < (AVOID.HIGHWAYS & Avoids))
        avoids.Add("highways");

      if (0 < (AVOID.FERRIES & Avoids))
        avoids.Add("ferries");

      if (0 < (AVOID.INDOOR & Avoids))
        avoids.Add("indoor");

      return string.Join("|", avoids.ToArray());
    }

    private string ComposeTransitModes (TRANSIT_MODE TransitModes) {
      var transitModes = new List<string>();
      if (0 < (TRANSIT_MODE.BUS & TransitModes))
        transitModes.Add("bus");

      if (0 < (TRANSIT_MODE.RAIL & TransitModes))
        transitModes.Add("rail");
      else {
        if (0 < (TRANSIT_MODE.SUBWAY & TransitModes))
          transitModes.Add("subway");

        if (0 < (TRANSIT_MODE.TRAIN & TransitModes))
          transitModes.Add("train");

        if (0 < (TRANSIT_MODE.TRAM & TransitModes))
          transitModes.Add("tram");
      }

      return string.Join("|", transitModes.ToArray());
    }
    #endregion


    #region Public Methods
    public async Task<Response> CalculateRoute (string APIKey,
        double Origin_Latitude, double Origin_Longitude, double Destination_Latitude, double Destination_Longitude,
        MODE Mode = MODE.DRIVING, bool Alternatives = true, AVOID Avoids = AVOID.NONE, UNITS Units = UNITS.METRIC,
        TRAFFIC_MODEL TrafficModel = TRAFFIC_MODEL.BEST_GUESS, DateTime? ArrivalTime = null, DateTime? DepartureTime = null,
        TRANSIT_MODE TransitModes = TRANSIT_MODE.NONE, TRANSIT_ROUTING_PREFERENCE TransitRoutingPreference = TRANSIT_ROUTING_PREFERENCE.LESS_WALKING) {
      var url = new UriBuilder(ServiceURL);
      //var query = QueryHelpers.ParseQuery(url.Query);
      var query = new Dictionary<string, string>();
      query["key"] = APIKey;
      query["origin"] = string.Format(new CultureInfo("en-US"), "{0},{1}", Origin_Latitude, Origin_Longitude);
      query["destination"] = string.Format(new CultureInfo("en-US"), "{0},{1}", Destination_Latitude, Destination_Longitude);
      query["alternatives"] = Alternatives ? "true" : "false";
      var avoids = ComposeAvoids(Avoids);
      if (!string.IsNullOrWhiteSpace(avoids))
        query["avoid"] = avoids;
      query["units"] = UNITS.METRIC == Units ? "metric" : "imperial";

      switch (Mode) {
        case MODE.WALKING:
          break;

        case MODE.BICYCLING:
          break;

        case MODE.TRANSIT:
          query["mode"] = "transit";
          if (ArrivalTime.HasValue) {
            query["arrival_time"] = Utilities.GetEpoch(ArrivalTime.Value).ToString();
          }
          else {
            if (DepartureTime.HasValue)
              query["departure_time"] = Utilities.GetEpoch(DepartureTime.Value).ToString();
          }

          var transitModes = ComposeTransitModes(TransitModes);
          if (!string.IsNullOrWhiteSpace(transitModes))
            query["transit_mode"] = transitModes;

          switch (TransitRoutingPreference) {
            case TRANSIT_ROUTING_PREFERENCE.LESS_WALKING:
              query["transit_routing_preference"] = "less_walking";
              break;

            case TRANSIT_ROUTING_PREFERENCE.FEWER_TRANSFERS:
              query["transit_routing_preference"] = "fewer_transfers";
              break;

            default:
              break;
          }
          break;

        default:
          query["mode"] = "driving";
          if (DepartureTime.HasValue)
            query["departure_time"] = Utilities.GetEpoch(DepartureTime.Value).ToString();
          break;
      }

      switch (TrafficModel) {
        case TRAFFIC_MODEL.PESSIMISTIC:
          query["traffic_model"] = "pessimistic";
          break;

        case TRAFFIC_MODEL.OPTIMISTIC:
          query["traffic_model"] = "optimistic";
          break;

        default:
          query["traffic_model"] = "best_guess";
          break;
      }

      using (var httpClient = new HttpClient()) {
        try {
          var httpResponse = await httpClient.GetStringAsync(QueryHelpers.AddQueryString(url.ToString(), query));
          return JsonConvert.DeserializeObject<Response>(httpResponse, new JsonSerializerSettings {
            ObjectCreationHandling = ObjectCreationHandling.Replace
          });
        }
        catch (Exception ex) {
          return null;
        }
      }
    }
    #endregion
  }
}
