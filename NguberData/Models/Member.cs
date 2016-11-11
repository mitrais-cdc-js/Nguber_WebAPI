using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguberData.Models {
  public class Member {
    #region Protected Properties 
    #endregion


    #region Public Properties
    [ForeignKey("ApplicationUser")]
    public string Id { get; set; } = string.Empty;

    [ConcurrencyCheck]
    public byte[] RowVersion = null;

    [Required]
    [DataType(DataType.Currency)]
    [Display(Name = "Credit(s)")]
    [DisplayFormat(DataFormatString = "{0:#,0.00}", ApplyFormatInEditMode = true)]
    [Range(0d, double.MaxValue)]
    public decimal Credit { get; set; } = 0M;

    [Required]
    [DataType(DataType.Currency)]
    [Display(Name = "Credit(s)")]
    [DisplayFormat(DataFormatString = "{0:#,0.00}", ApplyFormatInEditMode = true)]
    [Range(0d, double.MaxValue)]
    public decimal CreditOnHold{ get; set; } = 0M;    
    public string GeoLocation { get; set; } = null;

    public virtual ApplicationUser ApplicationUser { get; set; } = null;
    #endregion


    #region Constructors & Destructor
    public Member () {
      ApplicationUser = new Models.ApplicationUser("member");
    }
    #endregion


    #region Protected Methods
    #endregion


    #region Public Methods
    #endregion 
  }
}
