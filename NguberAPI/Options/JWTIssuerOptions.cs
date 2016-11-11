using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;

namespace NguberAPI.Options {
  public class JWTIssuerOptions {
    internal JWTIssuerOptions value;
    #region Protected Properties
    #endregion


    #region Public Properties
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public DateTime NotBefore { get; set; } = DateTime.UtcNow;
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    public TimeSpan ValidFor { get; set; } = TimeSpan.FromDays(30);
    public DateTime Expiration => IssuedAt.Add(ValidFor);
    public SigningCredentials SigningCredentials { get; set; } = null;
    #endregion


    #region Constructors & Destructor
    #endregion


    #region Protected Methods
    #endregion 


    #region Public Methods
    public Func<Task<string>> JTIGenerator => () => Task.FromResult(Guid.NewGuid().ToString());
    #endregion
  }
}
