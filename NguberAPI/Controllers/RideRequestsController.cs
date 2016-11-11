using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NguberAPI.Commons.GoogleAPI.GoogleMap;
using NguberAPI.Models;
using NguberAPI.Models.RideRequestModels;
using NguberAPI.Options;
using NguberData.Data;
using NguberData.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NguberAPI.Controllers {
  [Route("api/[Controller]")]
  public class RideRequestsController : Controller {
    #region Protected Properties
    private readonly ApplicationDbContext dbContext = null;
    private readonly JWTIssuerOptions jwtOptions;
    private readonly GoogleAPIOptions googleAPIOptions;
    #endregion


    #region Public Properties
    #endregion


    #region Constructors & Destructor
    public RideRequestsController (
      ApplicationDbContext DBContext,
      IOptions<JWTIssuerOptions> JWTOptions,
      IOptions<GoogleAPIOptions> GoogleAPIOptions) {
      dbContext = DBContext;
      jwtOptions = JWTOptions.Value;
      googleAPIOptions = GoogleAPIOptions.Value;
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

    #endregion


    #region Public Methods
    [Authorize(Policy = "MemberDriver")]
    [HttpGet("{RequestID}")]
    public async Task<IActionResult> GetDetail (int RequestID) {
      var Id = HttpContext.User.FindFirst("Id").Value;
      if (null == Id)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      var rideRequest = await dbContext.RideRequests
        .Include(x => x.Member.ApplicationUser)
        .Include(x => x.Driver.ApplicationUser)
        .SingleOrDefaultAsync(x => x.Id == RequestID && (x.Member_ID == Id || x.Driver_ID == Id));
      if (null == rideRequest)
        return NotFound(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      return Ok(new APIResponse<RideRequest>(rideRequest));
    }


    [Authorize(Policy = "Member")]
    [HttpPost("Estimate")]
    public async Task<IActionResult> Estimate ([FromBody] EstimateModel Model) {
      if (!ModelState.IsValid)
        return BadRequest(new APIResponse("Invalid parameters.", APIResponse.INVALID_PARAMETER, ModelState));

      var Id = HttpContext.User.FindFirst("Id").Value;
      if (null == Id)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      var member = await dbContext.Members.Include(x => x.ApplicationUser).SingleOrDefaultAsync(x => x.Id == Id);
      if (null == member)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      var minimumDistance = 1000L; // Minimum distance
      var minimumDuration = 0L; // Minimum duration
      //Directions.Route selectedRoute = null;

      // TODO: Craete fare, fee, discount scheme, 
      // currently all request's fare is set to constant minimum = 4000
      var minimumFare = 4000M; // Minimum fare

      var directions = new Directions();
      var directionResponse = await directions.CalculateRoute(googleAPIOptions.GoogleMap.APIKey, Model.PickUpLatitude, Model.PickUpLongitude, Model.DestinationLatitude, Model.DestinationLongitude,
        Avoids: Directions.AVOID.FERRIES | Directions.AVOID.INDOOR, DepartureTime: DateTime.UtcNow);
      if (null == directionResponse)
        return BadRequest(new APIResponse("Unable to get estimation.", APIResponse.GOOGLE_DIRECTIONS_ERROR));
      if (1 > directionResponse.Routes.Count)
        return BadRequest(new APIResponse("Unable to get routes.", APIResponse.GOOGLE_DIRECTIONS_NO_ROUTES));

      foreach (var route in directionResponse.Routes) {
        var estimateDistance = 0L;
        var estimateDuration = 0L;
        foreach (var leg in route.Legs) {
          estimateDistance += leg.Distance.Value;
          estimateDuration += leg.DurationInTraffic.Value;
        }

        //// For selecting which route's polyline to draw
        //if (estimateDistance < selectedDistance) {
        //  selectedDistance = estimateDistance;
        //  selectedRoute = route;
        //}

        minimumDistance = Math.Max(minimumDistance, estimateDistance);
        minimumDuration = Math.Max(minimumDuration, estimateDuration);
      }

      //// For drawing route's polyline
      //foreach (var leg in selectedRoute.Legs) {
      //  foreach (var step in leg.Steps) {

      //  }
      //}

      // TODO : Create discount scheme;
      var discount = 0M;

      // Calculating Fee & Tax
      var fee = 0M;
      var subTotal = (minimumFare + fee) - discount;
      var tax = subTotal * 0.1M;
      var total = subTotal + tax;

      try {
        var rideEstimate = new RideEstimate();
        rideEstimate.Member_ID = Id;
        rideEstimate.PickUp = Model.PickUp;
        rideEstimate.PickUpGeo = Model.PickUpGeo;
        rideEstimate.Destination = Model.Destination;
        rideEstimate.DestinationGeo = Model.DestinationGeo;
        rideEstimate.EstimatedDistance = minimumDistance;
        rideEstimate.EstimatedDuration = minimumDuration;
        rideEstimate.EstimatedFare = minimumFare;
        rideEstimate.Discount = discount;
        rideEstimate.Fee = fee;
        rideEstimate.Tax = tax;
        rideEstimate.Total = total;

        dbContext.Add(rideEstimate);
        if (0 < await dbContext.SaveChangesAsync())
          return (Ok(new APIResponse<RideEstimate>(rideEstimate)));
        else
          return BadRequest(new APIResponse("Invalid parameters.", APIResponse.INVALID_PARAMETER));
      }
      catch (DbUpdateConcurrencyException) {
        return BadRequest(new APIResponse("Concurrency check fail.", APIResponse.RECORD_CONCURRENCY_FAIL));
      }
    }


    [Authorize(Policy = "Member")]
    [HttpPost("Request")]
    public async Task<IActionResult> Create ([FromBody] RequestModel Model) {
      if (!ModelState.IsValid)
        return BadRequest(new APIResponse("Invalid parameters.", APIResponse.INVALID_PARAMETER, ModelState));

      var Id = HttpContext.User.FindFirst("Id").Value;
      if (null == Id)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      var member = await dbContext.Members.SingleOrDefaultAsync(x => x.Id == Id);
      if (null == member)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      var rideEstimate = await dbContext.RideEstimates.SingleOrDefaultAsync(x => x.Id == Model.Id && x.Member_ID == member.Id);
      if (null == rideEstimate)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));
      if (rideEstimate.Expire < DateTime.UtcNow)
        return BadRequest(new APIResponse("Estimate has expired.", APIResponse.RECORD_NOT_FOUND));

      // if payed by E-Cash : Check if E-Cash is adequate for fare
      try {
        var rideRequest = new RideRequest();
        rideRequest.Member_ID = Id;
        rideRequest.PickUp = rideEstimate.PickUp;
        rideRequest.PickUpGeo = rideEstimate.PickUpGeo;
        rideRequest.Destination = rideEstimate.Destination;
        rideRequest.DestinationGeo = rideEstimate.DestinationGeo;
        rideRequest.EstimatedDistance = rideEstimate.EstimatedDistance;
        rideRequest.EstimatedDuration = rideEstimate.EstimatedDuration;
        rideRequest.EstimatedFare = rideEstimate.EstimatedFare;
        rideRequest.PaymentMethod = Model.PaymentMethod;
        rideRequest.Discount = rideEstimate.Discount;
        rideRequest.Fee = rideEstimate.Fee;
        rideRequest.Tax = rideEstimate.Tax;
        rideRequest.Total = rideEstimate.Total;

        switch (Model.PaymentMethod) {
          case RideRequest.PAYMENT_METHOD.ECASH:
            // If payed by E-Cash : Check for available credit and put on hold for the sum of total
            if (rideEstimate.Total > member.Credit)
              return BadRequest(new APIResponse("Not enough E-Cash.", APIResponse.PAYMENT_REJECTED));

            member.Credit -= rideRequest.Total;
            member.CreditOnHold += rideRequest.Total;
            dbContext.Update(member);
            break;

          case RideRequest.PAYMENT_METHOD.CASH:
            break;

          case RideRequest.PAYMENT_METHOD.CREDIT:
            return BadRequest(new APIResponse("Debit payment is unavailable yet.", APIResponse.PAYMENT_REJECTED));
            break;

          case RideRequest.PAYMENT_METHOD.DEBIT:
            return BadRequest(new APIResponse("Debit payment is unavailable yet.", APIResponse.PAYMENT_REJECTED));
            break;
        }

        dbContext.Add(rideRequest);
        if (0 < await dbContext.SaveChangesAsync())
          return Ok(new APIResponse<RideRequest>(rideRequest));
        else
          return BadRequest(new APIResponse("Invalid parameters.", APIResponse.INVALID_PARAMETER));
      }
      catch (DbUpdateConcurrencyException) {
        return BadRequest(new APIResponse("Concurrency check fail.", APIResponse.RECORD_CONCURRENCY_FAIL));
      }
    }


    [Authorize(Policy = "Driver")]
    [HttpPost("GetSearching")]
    public async Task<IActionResult> GetSearching ([FromBody] GetSearchingModel Model) {
      if (!ModelState.IsValid)
        return BadRequest(new APIResponse("Invalid parameters.", APIResponse.INVALID_PARAMETER, ModelState));

      var rideRequests = await dbContext.Set<RideRequest>().FromSql($"SELECT * FROM FUNCTION_RideRequest_Searching({Model.Radius}, N'{Model.GeoLocation}')")
        .Include(x => x.Member)
        .Include(x => x.Member.ApplicationUser)
        .ToListAsync();
      return Ok(new APIResponse<List<RideRequest>>(rideRequests));
    }


    [Authorize(Policy = "Driver")]
    [HttpPut("Accept")]
    public async Task<IActionResult> Accept ([FromBody] AcceptModel Model) {
      if (!ModelState.IsValid)
        return BadRequest(new APIResponse("Invalid parameters.", APIResponse.INVALID_PARAMETER, ModelState));

      var Id = HttpContext.User.FindFirst("Id").Value;
      if (null == Id)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      var rideRequest = await dbContext.RideRequests.SingleOrDefaultAsync(x => x.Id == Model.Id && x.Status == RideRequest.STATUS.SEARCHING);
      if (null == rideRequest)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      var driver = await dbContext.Drivers.SingleOrDefaultAsync(x => x.Id == Id && Driver.STATUS.STANDBY == x.Status);
      if (null == driver)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      rideRequest.Driver_ID = driver.Id;
      rideRequest.Accepted = DateTime.UtcNow;
      rideRequest.Status = RideRequest.STATUS.WAITING;

      driver.Status = Driver.STATUS.RESERVED;

      try {
        dbContext.Update(rideRequest);
        dbContext.Update(driver);
        if (0 < await dbContext.SaveChangesAsync())
          return Ok(new APIResponse<RideRequest>(rideRequest));
        else
          return BadRequest(new APIResponse("Invalid parameters.", APIResponse.INVALID_PARAMETER));
      }
      catch (DbUpdateConcurrencyException) {
        return BadRequest(new APIResponse("Concurrency check fail.", APIResponse.RECORD_CONCURRENCY_FAIL));
      }
    }


    [Authorize(Policy = "Driver")]
    [HttpPut("PickUped")]
    public async Task<IActionResult> PickUped ([FromBody] PickUpedModel Model) {
      if (!ModelState.IsValid)
        return BadRequest(new APIResponse("Invalid parameters.", APIResponse.INVALID_PARAMETER, ModelState));

      var Id = HttpContext.User.FindFirst("Id").Value;
      if (null == Id)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      var rideRequest = await dbContext.RideRequests.SingleOrDefaultAsync(x => x.Id == Model.Id && x.Driver_ID == Id && x.Status == RideRequest.STATUS.WAITING);
      if (null == rideRequest)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      rideRequest.Pickuped = DateTime.UtcNow;
      rideRequest.Status = RideRequest.STATUS.TRAVELLING;

      try {
        dbContext.Update(rideRequest);
        if (0 < await dbContext.SaveChangesAsync())
          return Ok(new APIResponse<RideRequest>(rideRequest));
        else
          return BadRequest(new APIResponse("Invalid parameters.", APIResponse.INVALID_PARAMETER));
      }
      catch (DbUpdateConcurrencyException) {
        return BadRequest(new APIResponse("Concurrency check fail.", APIResponse.RECORD_CONCURRENCY_FAIL));
      }
    }


    [Authorize(Policy = "Driver")]
    [HttpPut("Arrived")]
    public async Task<IActionResult> Arrived ([FromBody] ArrivedModel Model) {
      if (!ModelState.IsValid)
        return BadRequest(new APIResponse("Invalid parameters.", APIResponse.INVALID_PARAMETER, ModelState));

      var Id = HttpContext.User.FindFirst("Id").Value;
      if (null == Id)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      var rideRequest = await dbContext.RideRequests.SingleOrDefaultAsync(x => x.Id == Model.Id && x.Driver_ID == Id && x.Status == RideRequest.STATUS.TRAVELLING);
      if (null == rideRequest)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      rideRequest.Arrived = DateTime.UtcNow;
      rideRequest.Status = RideRequest.STATUS.ARRIVED;

      var driver = await dbContext.Drivers.SingleOrDefaultAsync(x => x.Id == Id);
      if (null == driver)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      driver.Status = Driver.STATUS.STANDBY;

      try {
        switch (rideRequest.PaymentMethod) {
          case RideRequest.PAYMENT_METHOD.ECASH:
            driver.Credit += rideRequest.EstimatedFare;

            var member = await dbContext.Members.SingleOrDefaultAsync(x => x.Id == rideRequest.Member_ID);
            if (null == member)
              return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));
            member.CreditOnHold -= rideRequest.Total;

            dbContext.Update(member);
            break;

          case RideRequest.PAYMENT_METHOD.CASH:
          case RideRequest.PAYMENT_METHOD.CREDIT:
          case RideRequest.PAYMENT_METHOD.DEBIT:
            break;
        }

        dbContext.Update(rideRequest);
        dbContext.Update(driver);
        if (0 < await dbContext.SaveChangesAsync())
          return Ok(new APIResponse<RideRequest>(rideRequest));
        else
          return BadRequest(new APIResponse("Invalid parameters.", APIResponse.INVALID_PARAMETER));
      }
      catch (DbUpdateConcurrencyException) {
        return BadRequest(new APIResponse("Concurrency check fail.", APIResponse.RECORD_CONCURRENCY_FAIL));
      }
    }


    [Authorize(Policy = "Member")]
    [HttpPut("Completed")]
    public async Task<IActionResult> Completed ([FromBody] CompletedModel Model) {
      if (!ModelState.IsValid)
        return BadRequest(new APIResponse("Invalid parameters.", APIResponse.INVALID_PARAMETER, ModelState));

      var Id = HttpContext.User.FindFirst("Id").Value;
      if (null == Id)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      var rideRequest = await dbContext.RideRequests.SingleOrDefaultAsync(x => x.Id == Model.Id && x.Member_ID == Id && x.Status == RideRequest.STATUS.ARRIVED);
      if (null == rideRequest)
        return BadRequest(new APIResponse("Record not found.", APIResponse.RECORD_NOT_FOUND));

      rideRequest.Completed = DateTime.UtcNow;
      rideRequest.Rating = Model.Rating;
      rideRequest.Comment = Model.Comment;
      rideRequest.Status = RideRequest.STATUS.COMPLETED;

      try {
        dbContext.Update(rideRequest);
        if (0 < await dbContext.SaveChangesAsync())
          return Ok(new APIResponse<RideRequest>(rideRequest));
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
