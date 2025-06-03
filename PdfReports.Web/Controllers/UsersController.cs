using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public IActionResult GeneratePdfReport()
        {
            var report = _userService.GeneratePdfReport();

            return File(report, "application/pdf");
        }
    }
}
