using Microsoft.AspNetCore.Mvc;
using MusicStreaming.Data;
using MusicStreaming.Models;

namespace MusicStreaming.Controllers
{
    public class AdminController : Controller
    {
        private readonly MusicContext _context;

        public AdminController(MusicContext context)
        {
            _context = context;
        }

        private bool IsAdmin() =>
            HttpContext.Session.GetString("Role") == "Admin";

        private void SetViewData()
        {
            ViewData["Username"] = HttpContext.Session.GetString("Username");
            ViewData["Role"]    = HttpContext.Session.GetString("Role");
        }

        // GET /Admin/Users
        public IActionResult Users()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            SetViewData();

            var users = _context.Users.ToList();
            return View(users);
        }

        // POST /Admin/DeleteUser
        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            if (!IsAdmin())
                return Forbid();

            var user = _context.Users
                .FirstOrDefault(u => u.Id == id);

            if (user == null)
                return NotFound();

            // Remove user from DB
            _context.Users.Remove(user);
            _context.SaveChanges();

            TempData["SuccessMessage"] = $"User '{user.Username}' deleted.";
            return RedirectToAction("Users");
        }

        // POST /Admin/DemoteUser
        [HttpPost]
        public IActionResult DemoteUser(int id)
        {
            if (!IsAdmin())
                return Forbid();

            var user = _context.Users
                .FirstOrDefault(u => u.Id == id);

            if (user == null || user.Role != "Admin")
                return NotFound();

            user.Role = "User"; // or "Artist" if you prefer
            _context.SaveChanges();

            TempData["SuccessMessage"] = $"User '{user.Username}' demoted from Admin.";
            return RedirectToAction("Users");
        }
    }
}
