using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NguberData.Models {
  public class RideCancel {
    #region Protected Properties 
    #endregion


    #region Public Properties
    public int Id { get; set; } = 0;
    public string Reference { get; set; } = string.Empty;

    [ForeignKey("RideRequest")]
    public int  RideRequest_ID { get; set; } = 0;

    [ForeignKey("Member")]
    public string Member_ID { get; set; } = string.Empty;

    [ForeignKey("Driver")]
    public string Driver_ID { get; set; } = null;

    [ForeignKey("CancelReason")]
    public int CancelReason_ID { get; set; } = 0;

    [JsonIgnore] 
    public virtual RideRequest RideRequest { get; set; } = null;
    public virtual Member Member { get; set; } = null;
    public virtual Driver Driver { get; set; } = null;
    public virtual CancelReason CancelReason { get; set; } = null;
    #endregion


    #region Constructors & Destructor
    #endregion


    #region Protected Methods
    #endregion


    #region Public Methods
    #endregion 
  }
}
