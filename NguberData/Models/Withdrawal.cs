using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NguberData.Models {
  public class Withdrawal {
    #region Constants
    public enum STATUS {
      PENDING,
      ACCEPTED,
      REJECTED,
      CANCELED
    }
    #endregion


    #region Protected Properties
    #endregion


    #region Public Properties
    public int Id { get; set; } = 0;
    public string Reference { get; set; } = string.Empty;
    public string User_Id { get; set; } = string.Empty;
    public DateTime Requested { get; set; } = DateTime.UtcNow;
    public decimal Amount { get; set; } = 0M;
    public decimal Tax { get; set; } = 0M;
    public decimal Fee { get; set; } = 0M;
    public string Gateway { get; set; } = string.Empty;
    public DateTime? GatewayDate { get; set; } = null;
    public string GatewayReference { get; set; } = string.Empty;
    public string GatewayAccount { get; set; } = string.Empty;
    public decimal GatewayFee { get; set; } = 0M;
    public decimal GatewayTax { get; set; } = 0M;
    public string GatewayResponse { get; set; } = string.Empty;
    public decimal Total { get; set; } = 0M;
    public STATUS Status { get; set; } = STATUS.PENDING;

    public ApplicationUser User { get; }
    #endregion


    #region Constructors & Destructor
    #endregion


    #region Protected Methods
    #endregion


    #region Public Methods
    #endregion
  }
}
