using NguberData.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NguberAPI.Models.DriverModels {
  public class UpdateStatusModel : IValidatableObject {
    #region Protected Properties
    #endregion


    #region Public Properties
    [Required]
    public Driver.STATUS Status { get; set; } = Driver.STATUS.STANDBY;
    #endregion


    #region Constructors & Destructor
    #endregion


    #region Protected Methods
    IEnumerable<ValidationResult> IValidatableObject.Validate (ValidationContext validationContext) {
      var result = new List<ValidationResult>();

      if (!Enum.IsDefined(typeof(Driver.STATUS), Status))
        result.Add(new ValidationResult("Invalid value.", new string[] { "Status" }));

      if (Driver.STATUS.RESERVED <= Status)
        result.Add(new ValidationResult("Invalid value.", new string[] { "Status" }));

      return result;
    }
    #endregion


    #region Public Methods
    #endregion
  }
}
