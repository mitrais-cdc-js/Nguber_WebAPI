using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NguberAPI.Options;
using NguberData.Data;
using NguberData.Models;
using System;
using System.Text;

namespace NguberAPI {
  public class Startup {
    #region Protected Properties
    //private const string SecretKey = "absurdlongsecretkeybecauseshortcauseIDX10603ExceptionAlgorithmHS256cannothavelestthan128bitskeysize";
    //private readonly SymmetricSecurityKey signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection(nameof(JWTIssuerOptions)[nameof(JWTIssuerOptions.SecretKey)]));
    #endregion


    #region Public Properties
    public IConfigurationRoot Configuration { get; }
    #endregion


    #region Constructors & Destructor
    public Startup (IHostingEnvironment env) {
      var builder = new ConfigurationBuilder()
          .SetBasePath(env.ContentRootPath)
          .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

      if (env.IsEnvironment("Development")) {
        // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
        builder.AddApplicationInsightsSettings(developerMode: true);
      }

      builder.AddEnvironmentVariables();
      Configuration = builder.Build();
    }
    #endregion


    #region Protected Methods
    #endregion


    #region Public Methods
    // This method gets called by the runtime. Use this method to add services to the container
    public void ConfigureServices (IServiceCollection services) {
      // Add framework services.
      services.AddApplicationInsightsTelemetry(Configuration);

      services.AddDbContext<ApplicationDbContext>(options =>
          options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

      services.AddMvc(config => {
        var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
        config.Filters.Add(new AuthorizeFilter(policy));
      });

      var jwtAppSettingOptions = Configuration.GetSection(nameof(JWTIssuerOptions));
      var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtAppSettingOptions[nameof(JWTIssuerOptions.SecretKey)]));
      services.Configure<JWTIssuerOptions>(options => {
        options.Issuer = jwtAppSettingOptions[nameof(JWTIssuerOptions.Issuer)];
        options.Audience = jwtAppSettingOptions[nameof(JWTIssuerOptions.Audience)];
        options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
      });

      services.AddAuthorization(options => {
        options.AddPolicy("Driver",
          policy => policy.RequireClaim("Role", new[] {
            "Driver"
          })
        );
    
        options.AddPolicy("Member",
          policy => policy.RequireClaim("Role", new[] {
            "Member"
          })
        );

        options.AddPolicy("MemberDriver",
          policy => policy.RequireClaim("Role", new[] {
            "Driver", "Member"
          })
        );
      });

      services.AddIdentity<ApplicationUser, IdentityRole<string>>()
        .AddEntityFrameworkStores<ApplicationDbContext, string>()
        .AddDefaultTokenProviders();
      
      services.AddCors(options => {
        options.AddPolicy("AllowAllOrigins",
          builder => {
            builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
          });
      });

      services.Configure<MvcOptions>(options => {
        options.Filters.Add(new CorsAuthorizationFilterFactory("AllowAllOrigins"));
      });

      services.Configure<GoogleAPIOptions>(Configuration.GetSection("GoogleAPIOptions"));
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
    public void Configure (IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) {
      loggerFactory.AddConsole(Configuration.GetSection("Logging"));
      loggerFactory.AddDebug();

      app.UseApplicationInsightsRequestTelemetry();

      if (env.IsDevelopment()) {
        app.UseDeveloperExceptionPage();
        app.UseDatabaseErrorPage();
      }

      app.UseApplicationInsightsExceptionTelemetry();

      app.UseIdentity();

      var jwtAppSettingOptions = Configuration.GetSection(nameof(JWTIssuerOptions));
      var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtAppSettingOptions[nameof(JWTIssuerOptions.SecretKey)]));
      var tokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = true,
        ValidIssuer = jwtAppSettingOptions[nameof(JWTIssuerOptions.Issuer)],
        ValidateAudience = true,
        ValidAudience = jwtAppSettingOptions[nameof(JWTIssuerOptions.Audience)],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey =  signingKey,
        RequireExpirationTime = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
      };
      app.UseJwtBearerAuthentication(new JwtBearerOptions {
        AutomaticAuthenticate = true,
        AutomaticChallenge = true,
        TokenValidationParameters = tokenValidationParameters
      });

      app.UseCors("AllowAllOrigins");

      app.UseMvc();
    }
    #endregion
  }
}
