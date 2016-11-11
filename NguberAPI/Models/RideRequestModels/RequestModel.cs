using NguberData.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace NguberAPI.Models.RideRequestModels {
  //public class RequestModel : IValidatableObject {
  //  #region Protected Properties
  //  #endregion


  //  #region Public Properties
  //  [Required]
  //  [StringLength(50)]
  //  public string PickUp { get; set; } = string.Empty;

  //  [Required]
  //  [Range(-90D, 90D)]
  //  public double PickUpLatitude { get; set; } = 0D;

  //  [Required]
  //  [Range(-180D, 180D)]
  //  public double PickUpLongitude { get; set; } = 0D;

  //  [Required]
  //  [StringLength(50)]
  //  public string Destination { get; set; } = string.Empty;

  //  [Required]
  //  [Range(-90D, 90D)]
  //  public double DestinationLatitude { get; set; } = 0D;

  //  [Required]
  //  [Range(-180D, 180D)]
  //  public double DestinationLongitude { get; set; } = 0D;

  //  [Required]
  //  public RideRequest.PAYMENT_METHOD PaymentMethod { get; set; } = RideRequest.PAYMENT_METHOD.ECASH;

  //  public string PickUpGeo {
  //    get {
  //      return string.Format(new CultureInfo("en-US"), "{1} {0}", PickUpLatitude, PickUpLongitude);
  //    }
  //  }
  //  public string DestinationGeo {
  //    get {
  //      return string.Format(new CultureInfo("en-US"), "{1} {0}", DestinationLatitude, DestinationLongitude);
  //    }
  //  }
  //  #endregion


  //  #region Constructors & Destructor
  //  #endregion


  //  #region Protected Methods
  //  IEnumerable<ValidationResult> IValidatableObject.Validate (ValidationContext validationContext) {
  //    var result = new List<ValidationResult>();

  //    if (!Enum.IsDefined(typeof(RideRequest.PAYMENT_METHOD), PaymentMethod))
  //      result.Add(new ValidationResult("Invalid value.", new string[] { "PaymentMethod" }));

  //    return result;
  //  }
  //  #endregion


  //  #region Public Methods
  //  #endregion
  //}

  public class RequestModel : IValidatableObject {
    #region Protected Properties
    #endregion


    #region Public Properties
    [Required]
    public int Id { get; set; } = 0;

    [Required]
    public RideRequest.PAYMENT_METHOD PaymentMethod { get; set; } = RideRequest.PAYMENT_METHOD.ECASH;
    #endregion


    #region Constructors & Destructor
    #endregion


    #region Protected Methods
    #endregion


    #region Public Methods
    IEnumerable<ValidationResult> IValidatableObject.Validate (ValidationContext validationContext) {
      var result = new List<ValidationResult>();

      if (!Enum.IsDefined(typeof(RideRequest.PAYMENT_METHOD), PaymentMethod))
        result.Add(new ValidationResult("Invalid value.", new string[] { "PaymentMethod" }));

      return result;
    }
    #endregion
  }
}
