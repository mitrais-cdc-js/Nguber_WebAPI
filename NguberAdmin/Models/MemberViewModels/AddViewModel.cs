using System;
using System.ComponentModel.DataAnnotations;

namespace NguberAdmin.Models.MemberViewModels {
  public class AddViewModel {
    #region Protected Properties
    #endregion


    #region Public Properties
    [Required]
    [EmailAddress]
    [StringLength(30)]
    [Display(Name = "User Name")]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [Display(Name = "Password")]
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

    [Display(Name = "Last Name")]
    [StringLength(50)]
    public string NameLast { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Currency)]
    [Range(0d, double.MaxValue)]
    [DisplayFormat(DataFormatString = "{0:#,0.00}", ApplyFormatInEditMode = true)]
    [Display(Name = "Credit(s)")]
    public decimal Credit { get; set; } = 0m;
    #endregion


    #region Constructors & Destructor
    #endregion


    #region Protected Methods
    private string GeneratePassword () {
      return string.Format("Nguber@{0:yy}", DateTime.Today);
    }

    private string GenerateNguberNumber () {
      return string.Format("NGUBERmember-{0:yyMMddHHmmff}", DateTime.UtcNow);
    }
    #endregion


    #region Public Methods
    #endregion
  }
}
