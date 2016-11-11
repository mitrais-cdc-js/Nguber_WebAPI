using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguberData.Models {
  public class Driver {
    #region Constants
    public enum STATUS {
      STANDBY,
      RESTING,
      RESERVED    
    }
    #endregion 

    #region Protected Properties 
    #endregion


    #region Public Properties
    [ForeignKey("ApplicationUser")]
    public string Id { set; get; } = string.Empty;

    [ConcurrencyCheck]
    public byte[] RowVersion = null;

    [Required]
    [Display(Name = "Driver License Number")]
    [StringLength(50)]
    public string DriverLicenseNumber { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Driver License Expire")]
    [DisplayFormat(DataFormatString = "{0:yyyy MMM}", ApplyFormatInEditMode = true)]
    public DateTime DriverLicenseExpire { get; set; } = DateTime.Today;

    [Required]
    [Display(Name = "Vehicle Number")]
    [StringLength(50)]
    public string VehicleNumber { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Vehicle Type")]
    [StringLength(50)]
    public string VehicleType { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Currency)]
    [Display(Name = "Credit(s)")]
    [DisplayFormat(DataFormatString = "{0:#,0.00}", ApplyFormatInEditMode = true)]
    [Range(0d, double.MaxValue)]
    public decimal Credit { get; set; } = 0M;

    public STATUS Status { get; set; } = STATUS.STANDBY;

    public string GeoLocation { get; set; } = null;

    public virtual ApplicationUser ApplicationUser { get; set; } = null;

    [JsonIgnore]
    public virtual ICollection<RideRequest> RideRequests { get; set; } = null;
    #endregion


    #region Constructors & Destructor
    public Driver () {
      ApplicationUser = new Models.ApplicationUser("driver");
    }
    #endregion


    #region Protected Methods
    #endregion


    #region Public Methods
    #endregion
  }
}
