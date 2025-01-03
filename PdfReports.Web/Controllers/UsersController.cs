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
            _userService.GenerateUsers(10);
        }

        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return View(users);
        }

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
    }
}
