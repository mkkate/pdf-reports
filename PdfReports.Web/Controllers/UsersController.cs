using Microsoft.AspNetCore.Mvc;
using PdfReports.Web.Models;
using PdfReports.Web.Services;

namespace PdfReports.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return View(users);
        }

        #region Create
        public IActionResult Create()
        {
            var user = new User();
            return View(user);
        }

        [HttpPost]
        public IActionResult Create(User userModel)
        {
            var user = _userService.Create(userModel);

            //pdf report for generated user

            return RedirectToAction(nameof(GetAll));
        }
        #endregion
    }
}
