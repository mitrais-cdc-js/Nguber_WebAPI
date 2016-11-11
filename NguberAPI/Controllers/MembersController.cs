using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NguberAPI.Models;
using NguberAPI.Models.MemberModels;
using NguberData.Data;
using NguberData.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace NguberAPI.Controllers {
  [Route("api/[controller]")]
  public class MembersController : Controller {
    #region Protected Properties
    private readonly UserManager<ApplicationUser> userManager = null;
    private readonly ApplicationDbContext dbContext = null;
    #endregion


    #region Public Properties
    #endregion


    #region Constructors & Destructor
    public MembersController (UserManager<ApplicationUser> UserManager, ApplicationDbContext DBContext) {
      userManager = UserManager;
      dbContext = DBContext;
    }
    #endregion


    #region Protected Methods
    #endregion


    #region Public Methods
    [Authorize(Policy = "Member")]
    [HttpGet]
    public async Task<IActionResult> GetDetail () {
      var Id = HttpContext.User.FindFirst("Id").Value;
      if (null == Id)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      var member = await dbContext.Members.Include(x => x.ApplicationUser).SingleOrDefaultAsync(x => x.Id == Id);
      if (null == member)
        return NotFound(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      return Ok(new APIResponse<Member>(member));
    }

    [Authorize(Policy = "Driver")]
    [HttpGet("{Id}")]
    public async Task<IActionResult> GetDetail (string Id) {
      if (null == Id)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      var member = await dbContext.Members.Include(x => x.ApplicationUser).SingleOrDefaultAsync(x => x.Id == Id);
      if (null == member)
        return NotFound(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      return Ok(new APIResponse<Member>(member));
    }


    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Register ([FromBody] RegisterModel Model) {
      if (!ModelState.IsValid)
        return BadRequest(new APIResponse("Invalid parameters.", APIResponse.INVALID_PARAMETER, ModelState));

      try {
        var user = new ApplicationUser("member");
        user.UserName = Model.Email;
        user.Email = Model.Email;
        user.NameFirst = Model.NameFirst;
        user.NameLast = Model.NameLast;
        user.Member = new Member();

        var result = await userManager.CreateAsync(user, Model.Password);
        if (result.Succeeded) {
          await userManager.AddToRoleAsync(user, "MEMBER");
          return Ok(new APIResponse<Member>(user.Member));
        }
        else {
          var response = new APIResponse("Invalid parameters.", APIResponse.INVALID_PARAMETER);
          response.Errors = new List<APIResponse.APIResponse_Error>();
          foreach (var error in result.Errors)
            response.Errors.Add(new Models.APIResponse.APIResponse_Error(APIResponse.INVALID_PARAMETER, error.Code, error.Description));

          return BadRequest(response);
        }
      }
      catch (DbUpdateConcurrencyException) {
        return Ok(new APIResponse<Member>(new Member()));
      }
    }


    [Authorize(Policy = "Member")]
    [HttpPut]
    public async Task<IActionResult> Update ([FromBody] UpdateModel Model) {
      if (!ModelState.IsValid)
        return BadRequest(new APIResponse("Invalid parameters.", APIResponse.INVALID_PARAMETER, ModelState));

      var Id = HttpContext.User.FindFirst("Id").Value;
      if (null == Id)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      var member = await dbContext.Members.Include(x => x.ApplicationUser).SingleOrDefaultAsync(x => x.Id == Id);
      if (null == member)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      member.ApplicationUser.Email = Model.Email;
      member.ApplicationUser.NameFirst = Model.NameFirst;
      member.ApplicationUser.NameLast = Model.NameLast;

      try {
        dbContext.Update(member);
        if (0 < await dbContext.SaveChangesAsync())
          return Ok(new APIResponse<Member>(member));
        else
          return BadRequest(new APIResponse("Invalid parameters.", APIResponse.INVALID_PARAMETER));
      }
      catch (DbUpdateConcurrencyException) {
        return BadRequest(new APIResponse("Concurrency check fail.", APIResponse.RECORD_CONCURRENCY_FAIL));
      }
    }


    [Authorize(Policy = "Member")]
    [HttpPut("UpdateLocation")]
    public async Task<IActionResult> UpdateLocation ([FromBody] UpdateLocationModel Model) {
      if (!ModelState.IsValid)
        return BadRequest(new APIResponse("Invalid parameters.", APIResponse.INVALID_PARAMETER, ModelState));

      var Id = HttpContext.User.FindFirst("Id").Value;
      if (null == Id)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      var member = await dbContext.Members.Include(x => x.ApplicationUser).SingleOrDefaultAsync(x => x.Id == Id);
      if (null == member)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      member.GeoLocation = Model.GeoLocation;

      try {
        dbContext.Update(member);
        if (0 < await dbContext.SaveChangesAsync())
          return Ok(new APIResponse<Member>(member));
        else
          return BadRequest(new APIResponse("Invalid parameters.", APIResponse.INVALID_PARAMETER));
      }
      catch (DbUpdateConcurrencyException) {
        return BadRequest(new APIResponse("Concurrency check fail.", APIResponse.RECORD_CONCURRENCY_FAIL));
      }
    }


    [Authorize(Policy = "Member")]
    [HttpGet("RideRequestHistory")]
    public async Task<IActionResult> RideRequestHistory () {
      var Id = HttpContext.User.FindFirst("Id").Value;
      if (null == Id)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      var rideRequests = await dbContext.RideRequests.Where(x => x.Member_ID == Id)
        .Include(x => x.Driver)
        .Include(x => x.RideCancels)
        .ToListAsync();

      return Ok(new APIResponse<List<RideRequest>>(rideRequests));
    }


    [Authorize(Policy = "Member")]
    [HttpGet("GetCancelReasons")]
    public async Task<IActionResult> GetCancelReasons () {
      var cancelReasons = await dbContext.CancelReasons.Where(x => "MEMBER" == x.Role.NormalizedName).ToListAsync();
      return Ok(new APIResponse<List<CancelReason>>(cancelReasons));
    }


    [Authorize(Policy = "Member")]
    [HttpPut("CancelRideRequest")]
    public async Task<IActionResult> CancelRideRequest ([FromBody] CancelRideRequestModel Model) {
      var Id = HttpContext.User.FindFirst("Id").Value;
      if (null == Id)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      var rideRequest = await dbContext.RideRequests.SingleOrDefaultAsync(x => x.Id == Model.Id && x.Status <= RideRequest.STATUS.WAITING && x.Member_ID == Id);
      if (null == rideRequest)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      var cancelReason = await dbContext.CancelReasons.SingleOrDefaultAsync(x => x.Id == Model.CancelReasonID);
      if (null == cancelReason)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));
      if (cancelReason.OnStatus != rideRequest.Status)
        return BadRequest(new APIResponse("Invalid parameters.", APIResponse.INVALID_PARAMETER));

      if (RideRequest.STATUS.WAITING == rideRequest.Status) {
        var driver = await dbContext.Drivers.SingleOrDefaultAsync(x => x.Id == rideRequest.Driver_ID);
        if (null == driver)
          return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

        driver.Status = Driver.STATUS.STANDBY;
        dbContext.Update(driver);
      }

      rideRequest.Status = RideRequest.STATUS.CANCELED;

      var rideCancel = new RideCancel();
      rideCancel.RideRequest_ID = rideRequest.Id;
      rideCancel.Member_ID = Id;
      rideCancel.Driver_ID = rideRequest.Driver_ID;
      rideCancel.CancelReason_ID = Model.CancelReasonID;

      try {
        // If payed by E-Cash : Release on hold credit for the sum of total
        if (RideRequest.PAYMENT_METHOD.ECASH == rideRequest.PaymentMethod) {
          var member = await dbContext.Members.SingleOrDefaultAsync(x => x.Id == rideRequest.Member_ID);
          if (null == member)
            return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

          member.Credit += rideRequest.Total;
          member.CreditOnHold -= rideRequest.Total;
          dbContext.Update(member);
        }

        dbContext.Update(rideRequest);
        dbContext.RideCancels.Add(rideCancel);

        if (0 < await dbContext.SaveChangesAsync())
          return Ok(new APIResponse<RideCancel>(rideCancel));
        else
          return BadRequest(new APIResponse("Invalid parameters.", APIResponse.INVALID_PARAMETER));
      }
      catch (DbUpdateConcurrencyException) {
        return BadRequest(new APIResponse("Concurrency check fail.", APIResponse.RECORD_CONCURRENCY_FAIL));
      }
    }
    #endregion
  }
}