using Newtonsoft.Json;

namespace NguberAPI.Commons.GoogleAPI.GoogleMap {
  public partial class Directions {
    public class Step {
      #region Protected Properties
      #endregion


      #region Public Properties
      public Distance Distance { get; set; } = null;
      public Duration Duration { get; set; } = null;
      [JsonProperty("html_instructions")]
      public string HTMLInstructions { get; set; } = string.Empty;
      public Polyline Polyline { get; set; } = null;
      [JsonProperty("start_location")]
      public GeoCode.Location LocationStart { get; set; } = null;
      [JsonProperty("end_location")]
      public GeoCode.Location LocationEnd { get; set; } = null;
      [JsonProperty("travel_mode")]
      public MODE Mode { get; set; } = MODE.DRIVING;
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
