using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NguberAPI.Options {
  public class GoogleAPIOptions {
    internal GoogleAPIOptions value;

    public class GoogleMapOptions {
      public string APIKey { get; set; } = string.Empty;
    }


    #region Protected Properties
    #endregion


    #region Public Properties
    public GoogleMapOptions GoogleMap { get; set; } = null;
    #endregion


    #region Constructors & Destructor
    #endregion


    #region Protected Methods
    #endregion


    #region Public methods
    #endregion
  }
}
