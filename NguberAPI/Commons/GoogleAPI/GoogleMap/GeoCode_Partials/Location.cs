using Newtonsoft.Json;

namespace NguberAPI.Commons.GoogleAPI.GoogleMap {
  public partial class GeoCode {
    public class Location {
      #region Protected Properties
      #endregion


      #region Public Properties
      [JsonProperty("lat")]
      public decimal Latitude { get; set; } = 0M;

      [JsonProperty("lng")]
      public decimal Longitude { get; set; } = 0M;
      #endregion


      #region Constructors & Destructor
      #endregion


      #region Protected Methods
      #endregion


      #region Public Methods
      #endregion
    }
  }
}
