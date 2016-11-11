using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NguberData.Models {
  public class RideEstimate {
    #region Protected Properties
    private DateTime requested = DateTime.UtcNow;
    private DateTime expire = DateTime.UtcNow;
    #endregion


    #region Public Properties
    public int Id { get; set; } = 0;

    [ForeignKey("Member")]
    public string Member_ID { get; set; } = string.Empty;

    //public DateTime Requested { get; set; } = DateTime.UtcNow;
    public DateTime Requested {
      get {
        return requested;
      }
      set {
        requested = value;
        expire = GetExpire();
      }
    }

    public DateTime Expire {
      get {
        return expire;
      }
      set {
        expire = value;
      }
    }
    public string PickUp { get; set; } = string.Empty;
    public string PickUpGeo { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public string DestinationGeo { get; set; } = string.Empty;
    public long EstimatedDistance { get; set; } = 0L;
    public long EstimatedDuration { get; set; } = 0L;
    public decimal EstimatedFare { get; set; } = 0M;
    public decimal Discount { get; set; } = 0M;
    public decimal Fee { get; set; } = 0M;
    public decimal Tax { get; set; } = 0M;
    public decimal Total { get; set; } = 0M;
    #endregion


    #region Constructors & Destructor
    public RideEstimate () {
      requested = DateTime.UtcNow;
      expire = GetExpire();
    }
    #endregion


    #region Protected Methods
    private DateTime GetExpire () {
      return requested.AddMinutes(10);
    }
    #endregion


    #region Public Methods
    #endregion
  }
}
