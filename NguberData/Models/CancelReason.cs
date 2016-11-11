using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguberData.Models {
  public class CancelReason {
    #region Protected Properties 
    #endregion


    #region Public Properties
    public int Id { get; set; } = 0;

    [ForeignKey("Role")]
    public string Role_ID { get; set; } = string.Empty;

    public RideRequest.STATUS OnStatus { get; set; } = RideRequest.STATUS.SEARCHING;
    public string Message { get; set; } = string.Empty;

    public virtual IdentityRole<string> Role { get; set; } = null;
    #endregion


    #region Constructors & Destructor
    #endregion


    #region Protected Methods
    #endregion


    #region Public Methods
    #endregion 
  }
}
