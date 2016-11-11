using Newtonsoft.Json;
using System.Collections.Generic;

namespace NguberAPI.Commons.GoogleAPI.GoogleMap {
  public partial class Directions {
    public class Leg {
      #region Protected Properties
      #endregion


      #region Public Properties
      public Distance Distance { get; set; } = null;
      public Duration Duration { get; set; } = null;
      [JsonProperty("Duration_In_Traffic")]
      public DurationInTraffic DurationInTraffic { get; set; } = null;
      public List<Step> Steps { get; set; } = null;
      #endregion


      #region Constructors & Destructor
      #endregion


      #region Protected methods
      #endregion


      #region Public Methods
      #endregion
    }
  }
}
