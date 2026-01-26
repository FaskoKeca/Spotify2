using MusicStreaming.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MusicStreaming.Controllers
{
    public class SearchController : Controller
    {
        private readonly MusicContext _context;

        public SearchController(MusicContext context)
        {
            _context = context;
        }

        private void SetViewData()
        {
            ViewData["Username"] = HttpContext.Session.GetString("Username");
            ViewData["Role"] = HttpContext.Session.GetString("Role");
        }

        public IActionResult Results(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return RedirectToAction("Index", "Songs");

            SetViewData();

            var songs = _context.Songs
                .Include(s => s.Artist)
                .Where(s => s.SongName.Contains(query))
                .ToList();

            var users = _context.Users
                .Where(u => u.Username.Contains(query))
                .OrderByDescending(u => u.Role == "Artist" || u.Role == "Admin")
                .ToList();

            var playlists = _context.Playlists
                .Include(p => p.User)
                .Where(p => p.PlaylistName.Contains(query))
                .ToList();

            ViewData["Query"] = query;
            ViewData["Songs"] = songs;
            ViewData["Users"] = users;
            ViewData["Playlists"] = playlists;

            return View();
        }
    }
}
