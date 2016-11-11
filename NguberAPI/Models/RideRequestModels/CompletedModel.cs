using System.ComponentModel.DataAnnotations;

namespace NguberAPI.Models.RideRequestModels {
  public class CompletedModel {
    #region Protected Properties
    #endregion


    #region Public Properties
    [Required]
    public int Id { get; set; } = 0;

    [Required]
    [Range(0D, 10D)]
    public int Rating { get; set; } = 10;

    [StringLength(200)]
    public string Comment { get; set; } = "Awesome :)";
    #endregion


    #region Constructors & Destructor
    #endregion


    #region Protected Methods
    #endregion


    #region Public Methods
    #endregion
  }
}
