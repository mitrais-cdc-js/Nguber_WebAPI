using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace NguberAPI.Models.RideRequestModels {
  public class EstimateModel {
    #region Protected Properties
    #endregion


    #region Public Properties
    [Required]
    [StringLength(50)]
    public string PickUp { get; set; } = string.Empty;

    [Required]
    [Range(-90D, 90D)]
    public double PickUpLatitude { get; set; } = 0D;

    [Required]
    [Range(-180D, 180D)]
    public double PickUpLongitude { get; set; } = 0D;

    [Required]
    [StringLength(50)]
    public string Destination { get; set; } = string.Empty;

    [Required]
    [Range(-90D, 90D)]
    public double DestinationLatitude { get; set; } = 0D;

    [Required]
    [Range(-180D, 180D)]
    public double DestinationLongitude { get; set; } = 0D;

    public string PickUpGeo {
      get {
        return string.Format(new CultureInfo("en-US"), "{1} {0}", PickUpLatitude, PickUpLongitude);
      }
    }
    public string DestinationGeo {
      get {
        return string.Format(new CultureInfo("en-US"), "{1} {0}", DestinationLatitude, DestinationLongitude);
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
