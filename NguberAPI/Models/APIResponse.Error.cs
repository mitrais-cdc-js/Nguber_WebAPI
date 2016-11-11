using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NguberAPI.Models {
  public partial class APIResponse {
    public class APIResponse_Error {
      #region Protected Properties
      #endregion


      #region Public Properties
      public uint Code { get; set; } = GENERAL_ERROR;
      public string Source { get; set; } = string.Empty;
      public string Message { get; set; } = "Error - Unassigned values, invalid initialization.";
      #endregion


      #region Constructors & Destructor
      public APIResponse_Error (uint Code, string Source, string Message) {
        this.Code = Code;
        this.Source = Source;
        this.Message = Message;
      }
      #endregion


      #region Protected Methods
      #endregion


      #region Public Methods
      #endregion
    }
  }
}
