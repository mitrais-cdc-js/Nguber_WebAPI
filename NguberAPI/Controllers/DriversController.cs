using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NguberAPI.Models;
using NguberAPI.Models.DriverModels;
using NguberData.Data;
using NguberData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NguberAPI.Controllers {
  [Route("api/[controller]")]
  public class DriversController : Controller {
    #region Protected Properties
    private readonly UserManager<ApplicationUser> userManager = null;
    private readonly ApplicationDbContext dbContext = null;
    #endregion


    #region Public Properties
    #endregion


    #region Constructors & Destructor
    public DriversController (UserManager<ApplicationUser> UserManager, ApplicationDbContext DBContext) {
      userManager = UserManager;
      dbContext = DBContext;
    }
    #endregion


    #region Protected Methods
    #endregion


    #region Public Methods
    [Authorize(Policy = "Driver")]
    [HttpGet]
    public async Task<IActionResult> GetDetail () {
      var Id = HttpContext.User.FindFirst("Id").Value;
      if (null == Id)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      var driver = await dbContext.Drivers.Include(x => x.ApplicationUser).SingleOrDefaultAsync(x => x.Id == Id);
      if (null == driver)
        return NotFound(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      return Ok(new APIResponse<Driver>(driver));
    }

    [Authorize(Policy = "Member")]
    [HttpGet("{Id}")]
    public async Task<IActionResult> GetDetail (string Id) {
      if (null == Id)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      var driver = await dbContext.Drivers.Include(x => x.ApplicationUser).SingleOrDefaultAsync(x => x.Id == Id);
      if (null == driver)
        return NotFound(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      return Ok(new APIResponse<Driver>(driver));
    }


    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Register ([FromBody] RegisterModel Model) {
      if (!ModelState.IsValid)
        return BadRequest(new APIResponse("Invalid parameters.", APIResponse.INVALID_PARAMETER, ModelState));

      try {
        var user = new ApplicationUser("driver");
        user.UserName = Model.Email;
        user.Email = Model.Email;
        user.NameFirst = Model.NameFirst;
        user.NameLast = Model.NameLast;
        user.Driver = new Driver();
        user.Driver.DriverLicenseNumber = Model.DriverLicenseNumber;
        user.Driver.DriverLicenseExpire = DateTime.ParseExact(Model.DriverLicenseExpire, "yy/MM", null);
        user.Driver.VehicleNumber = Model.VehicleNumber;
        user.Driver.VehicleType = Model.VehicleType;

        var result = await userManager.CreateAsync(user, Model.Password);
        if (result.Succeeded) {
          await userManager.AddToRoleAsync(user, "DRIVER");
          return Ok(new APIResponse<Driver>(user.Driver));
        }
        else {
          var response = new APIResponse("Invalid parameters.", APIResponse.INVALID_PARAMETER);
          response.Errors = new List<APIResponse.APIResponse_Error>();
          foreach (var error in result.Errors)
            response.Errors.Add(new Models.APIResponse.APIResponse_Error(APIResponse.INVALID_PARAMETER, string.Empty, error.Description));

          return BadRequest(response);
        }
      }
      catch (DbUpdateConcurrencyException) {
        return Ok(new APIResponse<Driver>(new Driver()));
      }
    }


    [Authorize(Policy = "Driver")]
    [HttpPut]
    public async Task<IActionResult> Update ([FromBody] UpdateModel Model) {
      if (!ModelState.IsValid)
        return BadRequest(new APIResponse("Invalid parameters.", APIResponse.INVALID_PARAMETER, ModelState));

      var Id = HttpContext.User.FindFirst("Id").Value;
      if (null == Id)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      var driver = await dbContext.Drivers.Include(x => x.ApplicationUser).SingleOrDefaultAsync(x => x.Id == Id);
      if (null == driver)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      driver.ApplicationUser.Email = Model.Email;
      driver.ApplicationUser.NameFirst = Model.NameFirst;
      driver.ApplicationUser.NameLast = Model.NameLast;
      driver.DriverLicenseNumber = Model.DriverLicenseNumber;
      driver.DriverLicenseExpire = DateTime.ParseExact(Model.DriverLicenseExpire, "yy/MM", null);
      driver.VehicleNumber = Model.VehicleNumber;
      driver.VehicleType = Model.VehicleType;

      try {
        dbContext.Update(driver);
        if (0 < await dbContext.SaveChangesAsync())
          return Ok(new APIResponse<Driver>(driver));
        else
          return BadRequest(new APIResponse("Invalid parameters.", APIResponse.INVALID_PARAMETER));
      }
      catch (DbUpdateConcurrencyException) {
        return BadRequest(new APIResponse("Concurrency check fail.", APIResponse.RECORD_CONCURRENCY_FAIL));
      }
    }


    [Authorize(Policy = "Driver")]
    [HttpPut("UpdateStatus")]
    public async Task<IActionResult> UpdateStatus ([FromBody] UpdateStatusModel Model) {
      if (!ModelState.IsValid)
        return BadRequest(new APIResponse("Invalid parameters.", APIResponse.INVALID_PARAMETER, ModelState));

      var Id = HttpContext.User.FindFirst("Id").Value;
      if (null == Id)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      var driver = await dbContext.Drivers.Include(x => x.ApplicationUser).SingleOrDefaultAsync(x => x.Id == Id && Driver.STATUS.RESERVED != x.Status);
      if (null == driver)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      driver.Status = Model.Status;

      try {
        dbContext.Update(driver);
        if (0 < await dbContext.SaveChangesAsync())
          return Ok(new APIResponse<Driver>(driver));
        else
          return BadRequest(new APIResponse("Invalid parameters.", APIResponse.INVALID_PARAMETER));
      }
      catch (DbUpdateConcurrencyException) {
        return BadRequest(new APIResponse("Concurrency check fail.", APIResponse.RECORD_CONCURRENCY_FAIL));
      }
    }


    [Authorize(Policy = "Driver")]
    [HttpPut("UpdateLocation")]
    public async Task<IActionResult> UpdateLocation ([FromBody] UpdateLocationModel Model) {
      if (!ModelState.IsValid)
        return BadRequest(new APIResponse("Invalid parameters.", APIResponse.INVALID_PARAMETER, ModelState));

      var Id = HttpContext.User.FindFirst("Id").Value;
      if (null == Id)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      var driver = await dbContext.Drivers.Include(x => x.ApplicationUser).SingleOrDefaultAsync(x => x.Id == Id);
      if (null == driver)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      driver.GeoLocation = Model.GeoLocation;

      try {
        dbContext.Update(driver);
        if (0 < await dbContext.SaveChangesAsync())
          return Ok(new APIResponse<Driver>(driver));
        else
          return BadRequest(new APIResponse("Invalid parameters.", APIResponse.INVALID_PARAMETER));
      }
      catch (DbUpdateConcurrencyException) {
        return BadRequest(new APIResponse("Concurrency check fail.", APIResponse.RECORD_CONCURRENCY_FAIL));
      }
    }


    [Authorize(Policy = "Driver")]
    [HttpGet("RideRequestHistory")]
    public async Task<IActionResult> RideRequestHistory () {
      var Id = HttpContext.User.FindFirst("Id").Value;
      if (null == Id)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      var rideRequests = await dbContext.RideRequests.Where(x => x.Driver_ID == Id)
        .Include(x => x.Member)
        .Include(x => x.RideCancels)
        .ToListAsync();

      return Ok(new APIResponse<List<RideRequest>>(rideRequests));
    }


    [Authorize(Policy = "Driver")]
    [HttpGet("GetCancelReasons")]
    public async Task<IActionResult> GetCancelReasons () {
      var cancelReasons = await dbContext.CancelReasons.Where(x => "DRIVER" == x.Role.NormalizedName && RideRequest.STATUS.WAITING == x.OnStatus).ToListAsync();
      return Ok(new APIResponse<List<CancelReason>>(cancelReasons));
    }


    [Authorize(Policy = "Driver")]
    [HttpPut("CancelRideRequest")]
    public async Task<IActionResult> CancelRideRequest ([FromBody] CancelRideRequestModel Model) {
      var Id = HttpContext.User.FindFirst("Id").Value;
      if (null == Id)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      var rideRequest = await dbContext.RideRequests.SingleOrDefaultAsync(x => x.Id == Model.Id && x.Status == RideRequest.STATUS.WAITING && x.Driver_ID == Id);
      if (null == rideRequest)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      var cancelReason = await dbContext.CancelReasons.SingleOrDefaultAsync(x => x.Id == Model.CancelReasonID);
      if (null == cancelReason)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));
      if (cancelReason.OnStatus != rideRequest.Status)
        return BadRequest(new APIResponse("Invalid parameters.", APIResponse.INVALID_PARAMETER));

      var driver = await dbContext.Drivers.SingleOrDefaultAsync(x => x.Id == Id);
      if (null == driver)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      rideRequest.Driver_ID = null;
      rideRequest.Status = RideRequest.STATUS.SEARCHING;

      var rideCancel = new RideCancel();
      rideCancel.RideRequest_ID = rideRequest.Id;
      rideCancel.Member_ID = rideRequest.Member_ID;
      rideCancel.Driver_ID = Id;
      rideCancel.CancelReason_ID = Model.CancelReasonID;

      driver.Status = Driver.STATUS.STANDBY;

      try {
        dbContext.Update(rideRequest);
        dbContext.RideCancels.Add(rideCancel);
        dbContext.Update(driver);

        if (0 < await dbContext.SaveChangesAsync())
          return Ok(new APIResponse<RideCancel>(rideCancel));
        else
          return BadRequest(new APIResponse("Invalid parameters.", APIResponse.INVALID_PARAMETER));
      }
      catch (DbUpdateConcurrencyException) {
        return BadRequest(new APIResponse("Concurrency check fail.", APIResponse.RECORD_CONCURRENCY_FAIL));
      }
    }


    [Authorize(Policy = "Member")]
    [HttpPost("GetNearBy")]
    public async Task<IActionResult> GetNearBy ([FromBody] GetNearByModel Model) {
      if (!ModelState.IsValid)
        return BadRequest(new APIResponse("Invalid parameters.", APIResponse.INVALID_PARAMETER, ModelState));

      var drivers = await dbContext.Set<Driver>().FromSql($"SELECT * FROM FUNCTION_Drivers_NearBy({Model.Radius}, N'{Model.GeoLocation}')").Include(x => x.ApplicationUser).ToListAsync();
      return Ok(new APIResponse<List<Driver>>(drivers));
    }

    #endregion
  }
}