using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NguberAPI.Models;
using NguberAPI.Models.TokenModels;
using NguberAPI.Options;
using NguberData.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace NguberAPI.Controllers {
  [AllowAnonymous]
  [Route("api/[controller]")]
  public class TokenController : Controller {
    #region Protected Properties
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly ILogger logger;
    private readonly JWTIssuerOptions jwtOptions;
    #endregion


    #region Public Properties
    #endregion


    #region Constructors & Destructor
    public TokenController (
        UserManager<ApplicationUser> UserManager,
        SignInManager<ApplicationUser> SignInManager,
        ILoggerFactory LoggerFactory,
        IOptions<JWTIssuerOptions> JWTOptions) {
      userManager = UserManager;
      signInManager = SignInManager;
      logger = LoggerFactory.CreateLogger<TokenController>();
      jwtOptions = JWTOptions.Value;
      ThrowIfInvalidOptions(jwtOptions);
    }
    #endregion


    #region Protected Methods
    private static void ThrowIfInvalidOptions (JWTIssuerOptions JWTOptions) {
      if (null == JWTOptions)
        throw new ArgumentNullException(nameof(JWTOptions));

      if (TimeSpan.Zero >= JWTOptions.ValidFor)
        throw new ArgumentException("Must be a non-zero time span.", nameof(JWTIssuerOptions.ValidFor));

      if (null == JWTOptions.SigningCredentials)
        throw new ArgumentNullException(nameof(JWTIssuerOptions.SigningCredentials));

      if (null == JWTOptions.JTIGenerator)
        throw new ArgumentNullException(nameof(JWTIssuerOptions.JTIGenerator));
    }

    private static long ToUnixEpochDate (DateTime Date)
      => (long)Math.Round((Date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

    private async Task<ClaimsIdentity> GetClaimsIdentity (LoginModel Model) {
      var result = await signInManager.PasswordSignInAsync(Model.UserName, Model.Password, false, lockoutOnFailure: false);
      if (result.Succeeded) {
        return new ClaimsIdentity(
          new GenericIdentity(Model.UserName, "token"),
          new[] {
            new Claim("Role", "Driver")
          }
        );
      }

      if (result.IsLockedOut) {
        logger.LogWarning(2, "User account locked out.");
      }

      return null;
    }
    #endregion


    #region Public Methods
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login ([FromBody] LoginModel Model) {
      if (!ModelState.IsValid)
        return BadRequest(new APIResponse("Invalid parameters.", APIResponse.INVALID_PARAMETER, ModelState));

      var user = await userManager.FindByNameAsync(Model.UserName);
      if (null == user)
        return BadRequest(new APIResponse("Invalid login credential.", APIResponse.AUTHENTICATION_INVALID_CREDENTIAL));

      if (!await userManager.CheckPasswordAsync(user, Model.Password)) {
        var result = await userManager.AccessFailedAsync(user);
        if (await userManager.IsLockedOutAsync(user)) {
          logger.LogWarning(2, "User account locked out.");
          return BadRequest(new APIResponse("Maximum fail login attempt reached, account is locked out.", APIResponse.AUTHENTICATION_LOCKOUT));
        }

        return BadRequest(new APIResponse("Invalid login credential.", APIResponse.AUTHENTICATION_INVALID_CREDENTIAL));
      }
      else {
        await userManager.ResetAccessFailedCountAsync(user);
      }

      var roles = await userManager.GetRolesAsync(user);
      if (!(roles.Contains("Driver") || roles.Contains("Member")))
        return BadRequest(new APIResponse("Invalid login credential.", APIResponse.AUTHENTICATION_INVALID_CREDENTIAL));

      var claims = new List<Claim> {
        new Claim(JwtRegisteredClaimNames.Sub, Model.UserName),
        new Claim(JwtRegisteredClaimNames.Jti, await jwtOptions.JTIGenerator()),
        new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(jwtOptions.IssuedAt ).ToString(), ClaimValueTypes.Integer64),
        new Claim("Id", user.Id),
        new Claim("UserName", user.UserName)
      };
      foreach (var role in roles) {
        claims.Add(new Claim("Role", role));
      }

      var jwt = new JwtSecurityToken(
        issuer: jwtOptions.Issuer,
        audience: jwtOptions.Audience,
        claims: claims,
        notBefore: jwtOptions.NotBefore,
        expires: jwtOptions.Expiration,
        signingCredentials: jwtOptions.SigningCredentials
      );
      var encodedJWT = new JwtSecurityTokenHandler().WriteToken(jwt);
      var response = new APIResponse<Dictionary<string, object>>(new Dictionary<string, object> {
        {"AccessToken", encodedJWT},
        {"ExpiresIn", (int) jwtOptions.ValidFor.TotalSeconds}
      });

      logger.LogInformation(1, "User logged in.");
      return Ok(response);
    }
    #endregion
  }
}
