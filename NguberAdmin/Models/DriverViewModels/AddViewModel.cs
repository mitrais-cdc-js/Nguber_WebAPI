using System;
using System.ComponentModel.DataAnnotations;

namespace NguberAdmin.Models.DriverViewModels {
  public class AddViewModel {
    #region Protected Properties
    #endregion


    #region Public Properties
    [Required]
    [EmailAddress]
    [StringLength(50)]
    [Display(Name = "User Name")]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    public string Password {
      get {
        return GeneratePassword();
      }
    }

    [Required]
    [StringLength(20)]
    [Display(Name = "Nguber Number")]
    public string NguberNumber {
      get {
        return GenerateNguberNumber();
      }
    }

    [Required]
    [StringLength(50)]
    [Display(Name = "First Name")]
    public string NameFirst { get; set; } = string.Empty;

    [StringLength(50)]
    [Display(Name = "Last Name")]
    public string NameLast { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    [Display(Name = "Driver License Number")]
    public string DriverLicenseNumber { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy MMM}", ApplyFormatInEditMode = true)]
    [Display(Name = "Driver License Expire")]
    public DateTime DriverLicenseExpire { get; set; } = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

    [Required]
    [StringLength(50)]
    [Display(Name = "Vehicle Number")]
    public string VehicleNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    [Display(Name = "Vehicle Type")]
    public string VehicleType { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Currency)]
    [Range(0d, double.MaxValue)]
    [DisplayFormat(DataFormatString = "{0:#,0.00}", ApplyFormatInEditMode = true)]
    [Display(Name = "Credit(s)")]
    public decimal Credit { get; set; } = 0M;
    #endregion


    #region Constructors & Destructor
    #endregion


    #region Protected Methods
    private string GeneratePassword () {
      return string.Format("Nguber@{0:yy}", DateTime.Today);
    }

    private string GenerateNguberNumber () {
      return string.Format("NGUBERdriver-{0:yyMMddHHmmssff}", DateTime.UtcNow);
    }
    #endregion


    #region Public Methods
    #endregion
  }
}
