namespace NguberAPI.Models {
  public partial class APIResponse {
    public class APIResponse_Status {
      #region Protected Properties
      #endregion


      #region Public Properties
      public uint Code { get; set; } = GENERAL_ERROR;
      public string Message { get; set; } = "Error - Unassigned values, invalid initialization.";
      public bool Error { get; set; } = true;
      #endregion


      #region Constructors & Destructor
      public APIResponse_Status () {
        Code = SUCCEEDED;
        Message = "Success";
        Error = false;
      }

      public APIResponse_Status (string ErrorMessage, uint ErrorCode = GENERAL_ERROR) {
        this.Code = ErrorCode;
        Message = ErrorMessage;
      }
      #endregion


      #region Protected Methods
      #endregion


      #region Public Methods
      #endregion
    }
  }
}
