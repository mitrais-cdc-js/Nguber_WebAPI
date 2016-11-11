using NguberData.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace NguberAPI.Models.MemberModels {
  public class UpdateLocationModel {
    #region Protected Properties
    #endregion


    #region Public Properties
    [Required]
    [Range(-90D, 90D)]
    public double Latitude { get; set; } = 0D;

    [Required]
    [Range(-180D, 180D)]
    public double Longitude { get; set; } = 0D;

    public string GeoLocation {
      get {
        return string.Format(new CultureInfo("en-US") ,"{1} {0}", Latitude, Longitude);
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
