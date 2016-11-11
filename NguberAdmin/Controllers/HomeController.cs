using Microsoft.AspNetCore.Mvc;

namespace NguberAdmin.Controllers {
  public class HomeController : Controller {
    #region Protected Properties
    #endregion


    #region Public Properties
    #endregion


    #region Constructors & Destructor
    #endregion


    #region Protected Methods
    #endregion


    #region Public Methods
    public IActionResult Index () {
      return View();
    }

    public IActionResult About () {
      ViewData["Message"] = "Nguber Administration Page.";

      return View();
    }

    public IActionResult Contact () {
      ViewData["Message"] = "Nguber Team, Lorensius - Mitrais.";

      return View();
    }

    public IActionResult Error () {
      return View();
    }
    #endregion    
  }
}
