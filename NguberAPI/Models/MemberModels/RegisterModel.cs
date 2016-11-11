using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NguberAPI.Models.MemberModels {
  public class RegisterModel {
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
    #endregion


    #region Constructors & Destructor
    #endregion


    #region Protected Methods
    #endregion


    #region Public Methods
    #endregion
  }
}
