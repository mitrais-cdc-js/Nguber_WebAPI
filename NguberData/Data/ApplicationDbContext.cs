using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NguberData.Models;

namespace NguberData.Data {
  public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<string>, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>> {
    #region Protected Properties
    #endregion


    #region Public Properties
    public virtual DbSet<Driver> Drivers { get; set; }
    public virtual DbSet<Member> Members { get; set; }
    public virtual DbSet<RideEstimate> RideEstimates { get; set; }
    public virtual DbSet<RideRequest> RideRequests { get; set; }
    public virtual DbSet<CancelReason> CancelReasons { get; set; }
    public virtual DbSet<RideCancel> RideCancels { get; set; }
    public virtual DbSet<TopUp> TopUps { get; set; }
    public virtual DbSet<Withdrawal> Withdrawals { get; set; }
    #endregion


    #region Constructors & Destructor
    public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options)
     : base(options) {
    }
    #endregion


    #region Protected Methods
    protected override void OnModelCreating (ModelBuilder builder) {
      base.OnModelCreating(builder);
      // Customize the ASP.NET Identity model and override the defaults if needed.
      // For example, you can rename the ASP.NET Identity table names and more.
      // Add your customizations after calling base.OnModelCreating(builder);
    }
    #endregion


    #region Public Methods
    #endregion
  }
}
