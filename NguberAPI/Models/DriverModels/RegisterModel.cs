using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NguberAPI.Models.DriverModels {
  public class RegisterModel : IValidatableObject {
    #region Protected Properties
    #endregion


    #region Public Properties
    [Required]
    [EmailAddress]
    [StringLength(50)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string PasswordConfirm { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string NameFirst { get; set; } = string.Empty;

    public string NameLast { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string DriverLicenseNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string DriverLicenseExpire { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string VehicleNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string VehicleType { get; set; } = string.Empty;
    #endregion


    #region Constructors & Destructor
    #endregion


    #region Protected Methods
    IEnumerable<ValidationResult> IValidatableObject.Validate (ValidationContext validationContext) {
      var result = new List<ValidationResult>();

      DateTime driverLicenseExpire;
      if (!DateTime.TryParseExact(DriverLicenseExpire, "yy/MM", null, System.Globalization.DateTimeStyles.None, out driverLicenseExpire)) {
        result.Add(new ValidationResult("the {0} is not in requested format (YY/MM).", new string[] { "DriverLicenseExpire" }));
      }


      return result;
    }
    #endregion


    #region Public Methods
    #endregion
  }
}
