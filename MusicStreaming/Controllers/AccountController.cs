using MusicStreaming.Data;
using MusicStreaming.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace MusicStreaming.Controllers
{
    public class AccountController : Controller
    {
        private readonly MusicContext _context;

        // Hardcoded admin
        private const string HardcodedAdminUsername = "admin";
        private const string HardcodedAdminPassword = "admin";
        private const string HardcodedAdminPasswordHash = "jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg="; // ran through HashPassword

        public AccountController(MusicContext context)
        {
            _context = context;
        }


        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }


        private bool VerifyPassword(string password, string hash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput == hash;
        }


        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password, bool rememberMe = false)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Username and password are required.");
                return View();
            }

            // Hardcoded admin check
            if (username == HardcodedAdminUsername && VerifyPassword(password, HardcodedAdminPasswordHash))
            {
                HttpContext.Session.SetString("Username", HardcodedAdminUsername);
                HttpContext.Session.SetString("Role", "Admin");
                HttpContext.Session.SetInt32("UserId", 0); // reserved for hardcoded admin

                if (rememberMe)
                {
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.UtcNow.AddDays(30)
                    };
                    HttpContext.Response.Cookies.Append("RememberMe_Username", HardcodedAdminUsername, cookieOptions);
                    HttpContext.Response.Cookies.Append("RememberMe_Role", "Admin", cookieOptions);
                    HttpContext.Response.Cookies.Append("RememberMe_UserId", "0", cookieOptions);
                }

                return RedirectToAction("Index", "Songs");
            }

            // Existing DB login
            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            if (user != null && VerifyPassword(password, user.Password))
            {
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Role", user.Role);
                HttpContext.Session.SetInt32("UserId", user.Id);

                if (rememberMe)
                {
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.UtcNow.AddDays(30)
                    };
                    HttpContext.Response.Cookies.Append("RememberMe_Username", user.Username, cookieOptions);
                    HttpContext.Response.Cookies.Append("RememberMe_Role", user.Role, cookieOptions);
                    HttpContext.Response.Cookies.Append("RememberMe_UserId", user.Id.ToString(), cookieOptions);
                }

                return RedirectToAction("Index", "Songs");
            }

            ModelState.AddModelError("", "Invalid username or password.");
            return View();
        }


        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(string username, string email, string password, string confirmPassword, string role)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                ModelState.AddModelError("", "All fields are required.");
                return View();
            }

            if (password != confirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
                return View();
            }

            if (_context.Users.Any(u => u.Username == username))
            {
                ModelState.AddModelError("", "Username already exists.");
                return View();
            }

            if (!new[] { "User", "Artist", "Admin" }.Contains(role))
            {
                ModelState.AddModelError("", "Invalid role selected.");
                return View();
            }

            var newUser = new User
            {
                Username = username,
                Email = email,
                Password = HashPassword(password),
                Role = role,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            HttpContext.Session.SetString("Username", newUser.Username);
            HttpContext.Session.SetString("Role", newUser.Role);
            HttpContext.Session.SetInt32("UserId", newUser.Id);

            return RedirectToAction("Index", "Songs");
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Response.Cookies.Delete("RememberMe_Username");
            HttpContext.Response.Cookies.Delete("RememberMe_Role");
            HttpContext.Response.Cookies.Delete("RememberMe_UserId");
            return RedirectToAction("Login");
        }
    }
}
