using System.ComponentModel.DataAnnotations;

namespace NguberAPI.Models.TokenModels {
  public class LoginModel {
    #region Protected Properties
    #endregion


    #region Public Properties
    [Required]
    [EmailAddress]
    [StringLength(50)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [StringLength(100)]
    public string Password { get; set; } = string.Empty;
    #endregion


    #region Constructors & Destructor
    #endregion


    #region Protected Methods
    #endregion


    #region Public Methods
    #endregion 
  }
}
