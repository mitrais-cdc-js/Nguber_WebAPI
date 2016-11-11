using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace NguberData.Models {
  // Add profile data for application users by adding properties to the ApplicationUser class
  public class ApplicationUser : IdentityUser {
    #region Protected Properties
    #endregion


    #region Public Properties
    [Required]
    [Display(Name = "Nguber Number")]
    [StringLength(20)]
    public string NguberNumber { get; set; } = string.Empty;

    [Required]
    [Display(Name = "First Name")]
    [StringLength(50)]
    public string NameFirst { get; set; } = string.Empty;

    [Display(Name = "Last Name")]
    [StringLength(50)]
    public string NameLast { get; set; } = string.Empty;

    [JsonIgnore]
    public virtual Driver Driver { get; set; } = null;

    [JsonIgnore]
    public virtual Member Member { get; set; } = null;
    #endregion


    #region Constructors & Destructor
    public ApplicationUser () {
    }

    public ApplicationUser (string Role) {
      NguberNumber = GenerateNguberNumber(Role);
    }
    #endregion


    #region Protected Methods
    protected string GenerateNguberNumber (string Role) {
      return string.Format("NGUBER{0}-{1:yyMMddHHmmddssff}", Role.ToLower(), DateTime.UtcNow);
    }
    #endregion


    #region Public Methods
    #endregion
  }
}
