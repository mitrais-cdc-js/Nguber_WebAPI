using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguberData.Models {
  public class RideRequest {
    #region Constants
    public enum PAYMENT_METHOD {
      ECASH,
      CASH,
      CREDIT,
      DEBIT
    }

    public enum STATUS {
      SEARCHING,
      WAITING,
      TRAVELLING,
      ARRIVED,
      COMPLETED,
      CANCELED
    }
    #endregion


    #region Protected Properties
    #endregion


    #region Public Properties
    public int Id { get; set; } = 0;
    public string Reference { get; set; } = string.Empty;

    [ForeignKey("Member")]
    public string Member_ID { get; set; } = string.Empty;

    [ForeignKey("Driver")]
    public string Driver_ID { get; set; } = null;

    public DateTime Requested { get; set; } = DateTime.UtcNow;
    public string PickUp { get; set; } = string.Empty;
    public string PickUpGeo { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public string DestinationGeo { get; set; } = string.Empty;
    public long EstimatedDistance { get; set; } = 0L;
    public long EstimatedDuration { get; set; } = 0L;
    public decimal EstimatedFare { get; set; } = 0M;
    public PAYMENT_METHOD PaymentMethod { get; set; } = PAYMENT_METHOD.ECASH;
    public DateTime? Accepted { get; set; } = null;
    public DateTime? Pickuped { get; set; } = null;
    public DateTime? Arrived { get; set; } = null;
    public DateTime? Completed { get; set; } = null;
    public decimal Discount { get; set; } = 0M;
    public decimal Fee { get; set; } = 0M;
    public decimal Tax { get; set; } = 0M;
    public decimal Total { get; set; } = 0M;
    public int Rating { get; set; } = 0;
    public string Comment { get; set; } = string.Empty;
    public STATUS Status { get; set; } = STATUS.SEARCHING;


    public virtual Member Member { get; set; } = null;
    public virtual Driver Driver { get; set; } = null;
    public virtual ICollection<RideCancel> RideCancels { get; set; } = null;

    public string PaymentMethodNormalized {
      get {
        return (PAYMENT_METHOD.ECASH == PaymentMethod)? "E-CASH" : Enum.GetName(typeof(PAYMENT_METHOD), PaymentMethod);
      }
    }
    public string StatusNormalized {
      get {
        return Enum.GetName(typeof(STATUS), Status);
      }
    }
    #endregion


    #region Constructors & Destructor
    public RideRequest () {
      Reference = GenerateReference();
    }
    #endregion


    #region Protected Methods
    protected string GenerateReference () {
      return string.Format("NGUBERride-{0:yyMMddHHmmddssff}", DateTime.UtcNow);
    }
    #endregion


    #region Public Methods
    #endregion
  }
}
