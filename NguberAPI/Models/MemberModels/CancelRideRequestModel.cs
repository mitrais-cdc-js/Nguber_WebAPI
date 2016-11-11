using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NguberAPI.Models.MemberModels {
  public class CancelRideRequestModel {
    #region Protected Properties
    #endregion


    #region Public Properties
    [Required]
    public int Id { get; set; } = 0;

    [Required]
    public int CancelReasonID { get; set; } = 0;
    #endregion


    #region Constructors & Destructor
    #endregion


    #region Protected Methods
    #endregion


    #region Public Methods
    #endregion
  }
}
