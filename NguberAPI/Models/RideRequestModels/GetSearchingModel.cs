using NguberData.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace NguberAPI.Models.RideRequestModels {
  public class GetSearchingModel {
    #region Protected Properties
    #endregion


    #region Public Properties
    [Required]
    [Range(-90d, 90d)]
    public double Latitude { get; set; } = 0D;

    [Required]
    [Range(-180d, 180d)]
    public double Longitude { get; set; } = 0D;

    [Range(100, 10000)]
    public uint Radius { get; set; } = 10000U;

    public string GeoLocation {
      get {
        return string.Format(new CultureInfo("en-US"), "{1} {0}", Latitude, Longitude);
      }
    }
    #endregion


    #region Constructors & Destructor
    #endregion


    #region Protected Methods
    #endregion


    #region Public Methods
    #endregion
  }
}
