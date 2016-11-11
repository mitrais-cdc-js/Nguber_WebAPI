using System.Collections.Generic;

namespace NguberAPI.Commons.GoogleAPI.GoogleMap {
  public partial class Directions {
    public class Response {
      #region Protected Properties
      #endregion


      #region Public Properties
      public string Status { get; set; } = string.Empty;
      public string ErrorMessage { get; set; } = string.Empty;
      public List<Route> Routes { get; set; } = null;
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
