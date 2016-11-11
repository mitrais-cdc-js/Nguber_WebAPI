using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace NguberAPI.Models {
  public partial class APIResponse {
    #region Constants 
    // 0000 00XX = API Call Status
    public const uint SUCCEEDED = 0x00000000;
    public const uint INVALID_PARAMETER = 0x00000001;
    public const uint APICALL_ERROR = 0x000000FF;

    // 0000 01xx = Database Status
    public const uint RECORD_NOT_FOUND = 0x00000101;
    public const uint RECORD_CONCURRENCY_FAIL = 0x00000102;
    public const uint RECORD_ERROR = 0x000010FF;

    // 0000 02xx = Authentication & Authorization Status 
    public const uint AUTHENTICATION_INVALID_CREDENTIAL = 0x00000201;
    public const uint AUTHENTICATION_LOCKOUT = 0x00000202;
    public const uint AUTHENTICATION_ERROR = 0x000002FF;

    // 0000 03xx = Cryptography Status
    public const uint CRYPTOGRAPHY_EmptyString = 0x00000301;
    public const uint CRYPTOGRAPHY_ERROR = 0x000003FF;


    // 0000 70xx = Google API Status
    public const uint GOOGLE_GEOCODE_ERROR = 0x000071FF;
    public const uint GOOGLE_DIRECTIONS_NO_ROUTES = 0x00007201;
    public const uint GOOGLE_DIRECTIONS_ERROR = 0x000072FF;
    public const uint GOOGLE_ERROR = 0x00007FFF;


    // 0000 90xx = Payment Status
    public const uint PAYMENT_REJECTED = 0x00009001;
    public const uint PAYMENT_ERROR = 0x000090FF;


    

    // GENERAL ERROR
    public const uint GENERAL_ERROR = 0xFFFFFFFF;
    #endregion


    #region Protected Properties
    #endregion


    #region Public Properties
    public APIResponse_Status Status { get; set; } = null;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<APIResponse_Error> Errors { get; set; } = null;
    #endregion


    #region Constructors & Destructor
    public APIResponse () {
      Status = new Models.APIResponse.APIResponse_Status();
    }

    public APIResponse (string ErrorMessage, uint ErrorCode = GENERAL_ERROR, ModelStateDictionary ModelState = null) {
      Status = new APIResponse_Status(ErrorMessage, ErrorCode);
      if (null != ModelState) {
        Errors = new List<Models.APIResponse.APIResponse_Error>();
        foreach (var field in ModelState.Keys)
          foreach (var message in ModelState[field].Errors)
            Errors.Add(new APIResponse_Error(ErrorCode, field, message.ErrorMessage));
      }
    }
    #endregion


    #region Protected Methods
    #endregion


    #region Public Methods
    #endregion
  }

  public class APIResponse<T> : APIResponse {
    #region Constants 
    #endregion


    #region Protected Properties
    #endregion


    #region Public Properties
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public T Data = default(T);
    #endregion


    #region Constructors & Destructor
    public APIResponse (string ErrorMessage, uint ErrorCode = GENERAL_ERROR, ModelStateDictionary ModelState = null)
      : base(ErrorMessage, ErrorCode, ModelState) {
    }

    public APIResponse (T Data) : base() {
      this.Data = Data;
    }
    #endregion


    #region Protected Methods
    #endregion


    #region Public Methods
    #endregion
  }
}
