using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NguberAdmin.Models.MemberViewModels;
using NguberData.Data;
using NguberData.Models;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace NguberAdmin.Controllers {
  [Authorize(Roles = "Admin")]
  public class MembersController : Controller {
    #region Protected Properties
    private readonly UserManager<ApplicationUser> userManager;
    private readonly ApplicationDbContext dbContext;
    #endregion


    #region Public Properties
    #endregion


    #region Contructors & Destructor
    public MembersController (
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
      model.Members = await dbContext.Members.Include(x => x.ApplicationUser).ToListAsync();
      return View(model);
    }


    [HttpGet]
    public async Task<IActionResult> Detail (string Id) {
      if (null == Id)
        return NotFound();

      var Member = await dbContext.Members.SingleOrDefaultAsync(x => x.Id == Id);
      if (null == Member) {
        return NotFound();
      }

      return View(Member);
    }


    [HttpGet]
    public IActionResult Add () {
      return View(new AddViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add (AddViewModel Model) {
      if (ModelState.IsValid) {
        try {
          var user = new ApplicationUser {
            UserName = Model.UserName,
            Email = Model.UserName,
            NguberNumber = Model.NguberNumber,
            NameFirst = Model.NameFirst,
            NameLast = Model.NameLast
          };
          var result = await userManager.CreateAsync(user, Model.Password);
          if (result.Succeeded) {
            var Member = new Member {
              Id = user.Id,
              Credit = Model.Credit
            };

            dbContext.Add(Member);
            if (0 < await dbContext.SaveChangesAsync()) {
              return RedirectToAction(nameof(MembersController.Index), "Members");
            }
            else {
              ModelState.AddModelError(string.Empty, "Failed to update data.");
            }
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

      var Member = await dbContext.Members.Include(x => x.ApplicationUser).SingleOrDefaultAsync(x => x.Id == Id);
      if (null == Member) {
        return NotFound();
      }

      return View(Member);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit (string Id, Member Model) {
      if (null == Id)
        return NotFound();

      if (ModelState.IsValid) {
        var Member = await dbContext.Members.Include(x => x.ApplicationUser).SingleOrDefaultAsync(x => x.Id == Id);
        if (null == Member)
          return NotFound();

        var fieldsBind = new string[] {
            "Id", "ApplicationUser.NguberNumber", "ApplicationUser.NameFirst", "ApplicationUser.NameLast",
            "MemberLicenseNumber", "MemberLicenseExpire", "VehicleNumber", "VehicleType", "Credit"
        };

        if (await TryUpdateModelAsync(Member)) {
          try {
            dbContext.Update(Member);
            if (0 < await dbContext.SaveChangesAsync()) {
              return RedirectToAction(nameof(MembersController.Index), "Members");
            }
            else {
              ModelState.AddModelError(string.Empty, "Failed to update data.");
            }
          }
          catch (DbUpdateConcurrencyException concurrencyEx) {
            foreach (var entry in concurrencyEx.Entries) {
              if (entry.Entity is Member)
                Model = Member;
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
