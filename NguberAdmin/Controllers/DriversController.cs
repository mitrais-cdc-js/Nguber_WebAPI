using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NguberAdmin.Models.DriverViewModels;
using NguberData.Data;
using NguberData.Models;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace NguberAdmin.Controllers {
  [Authorize(Roles = "Admin")]
  public class DriversController : Controller {
    #region Protected Properties
    private readonly UserManager<ApplicationUser> userManager;
    private readonly ApplicationDbContext dbContext;
    #endregion


    #region Public Properties
    #endregion


    #region Contructors & Destructor
    public DriversController (
      UserManager<ApplicationUser> UserManager,
      ApplicationDbContext DBContext) {
      userManager = UserManager;
      dbContext = DBContext;
    }
    #endregion


    #region Protected Methods
    #endregion


    #region Public Methods
    [HttpGet]
    public async Task<IActionResult> Index () {
      var model = new IndexViewModel();
      model.Drivers = await dbContext.Drivers.Include(x => x.ApplicationUser).ToListAsync();
      return View(model);
    }


    [HttpGet]
    public async Task<IActionResult> Detail (string Id) {
      if (null == Id)
        return NotFound();

      var driver = await dbContext.Drivers.Include(x => x.ApplicationUser).SingleOrDefaultAsync(x => x.Id == Id);
      if (null == driver) {
        return NotFound();
      }

      return View(driver);
    }


    [HttpGet]
    public IActionResult Add () {
      return View(new AddViewModel());
    }

    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Add (AddViewModel Model) {
    //  if (ModelState.IsValid) {
    //    try {
    //      var user = new ApplicationUser("driver") {
    //        UserName = Model.UserName,
    //        Email = Model.UserName,
    //        NguberNumber = Model.NguberNumber,
    //        NameFirst = Model.NameFirst,
    //        NameLast = Model.NameLast
    //      };
    //      var result = await userManager.CreateAsync(user, Model.Password);
    //      if (result.Succeeded) {
    //        var driver = new Driver {
    //          Id = user.Id,
    //          DriverLicenseNumber = Model.DriverLicenseNumber,
    //          DriverLicenseExpire = Model.DriverLicenseExpire,
    //          VehicleNumber = Model.VehicleNumber,
    //          VehicleType = Model.VehicleType,
    //          Credit = Model.Credit
    //        };

    //        dbContext.Add(driver);
    //        if (0 < await dbContext.SaveChangesAsync()) {
    //          return RedirectToAction(nameof(DriversController.Index), "Drivers");
    //        }
    //        else {
    //          ModelState.AddModelError(string.Empty, "Failed to update data.");
    //        }
    //      }
    //      else {
    //        foreach (var error in result.Errors) {
    //          ModelState.AddModelError(string.Empty, error.Description);
    //        }
    //      }
    //    }
    //    catch (DbUpdateConcurrencyException) {
    //      ModelState.AddModelError(string.Empty, "concurrency check fails.");
    //    }
    //  }

    //  return View(Model);
    //}

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add (AddViewModel Model) {
      if (ModelState.IsValid) {
        try {
          var user = new ApplicationUser("driver");
          user.UserName = Model.UserName;
          user.Email = Model.UserName;
          user.NguberNumber = Model.NguberNumber;
          user.NameFirst = Model.NameFirst;
          user.NameLast = Model.NameLast;
          user.Driver = new Driver();
          user.Driver.DriverLicenseNumber = Model.DriverLicenseNumber;
          user.Driver.DriverLicenseExpire = Model.DriverLicenseExpire;
          user.Driver.VehicleNumber = Model.VehicleNumber;
          user.Driver.VehicleType = Model.VehicleType;
          user.Driver.Credit = Model.Credit;

          var result = await userManager.CreateAsync(user, Model.Password);
          if (result.Succeeded) {
            return RedirectToAction(nameof(DriversController.Index), "Drivers");
          }
          else {
            foreach (var error in result.Errors) {
              ModelState.AddModelError(string.Empty, error.Description);
            }
          }
        }
        catch (DbUpdateConcurrencyException) {
          ModelState.AddModelError(string.Empty, "concurrency check fails.");
        }
      }

      return View(Model);
    }


    [HttpGet]
    public async Task<IActionResult> Edit (string Id) {
      if (null == Id)
        return NotFound();

      var driver = await dbContext.Drivers.Include(x => x.ApplicationUser).SingleOrDefaultAsync(x => x.Id == Id);
      if (null == driver) {
        return NotFound();
      }

      return View(driver);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit (string Id, Driver Model) {
      if (null == Id)
        return NotFound();

      if (ModelState.IsValid) {
        var driver = await dbContext.Drivers.Include(x => x.ApplicationUser).SingleOrDefaultAsync(x => x.Id == Id);
        if (null == driver)
          return NotFound();

        var fieldsBind = new string[] {
            "ApplicationUser.Email", "ApplicationUser.NameFirst", "ApplicationUser.NameLast",
            "DriverLicenseNumber", "DriverLicenseExpire", "VehicleNumber", "VehicleType", "Credit"
        };

        if (await TryUpdateModelAsync(driver)) {
          try {
            dbContext.Update(driver);
            if (0 < await dbContext.SaveChangesAsync())
              return RedirectToAction(nameof(DriversController.Index), "Drivers");
            else
              ModelState.AddModelError(string.Empty, "Failed to update data.");
          }
          catch (DbUpdateConcurrencyException concurrencyEx) {
            foreach (var entry in concurrencyEx.Entries) {
              if (entry.Entity is Driver)
                Model = driver;
            }

            ModelState.AddModelError(string.Empty, "Concurrency check fails.");
          }
        }
      }

      return View(Model);
    }
    #endregion
  }
}
