using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NguberAPI.Commons.GoogleAPI.GoogleMap {
  public partial class GeoCode {
    #region constants
    protected const string ServiceURL = "https://maps.googleapis.com/maps/api/geocode/json";
    #endregion


    #region Protected Properties
    #endregion


    #region Public Properties
    #endregion


    #region Constructors & Destructor
    #endregion


    #region Protected Methods
    #endregion


    #region Public Methods
    public async Task<Response> GetGeoCode (string APIKey, string Address, bool Approximate = true, string ServiceURL = ServiceURL) {
      var url = new UriBuilder(ServiceURL);
      var query = QueryHelpers.ParseQuery(url.Uri.ToString());
      query["key"] = APIKey;
      query["address"] = Address;
      query["location_type"] = Approximate ? "APPROXIMATE" : "ROOFTOP";
      url.Query = query.ToString();

      using (var httpClient = new HttpClient()) {
        try {
          var httpResponse = await httpClient.GetStringAsync(url.Uri);
          return JsonConvert.DeserializeObject<Response>(httpResponse, new JsonSerializerSettings {
            ObjectCreationHandling = ObjectCreationHandling.Replace
          });
        }
        catch {
          return null;
        }
      }
    }
  }
  #endregion
}