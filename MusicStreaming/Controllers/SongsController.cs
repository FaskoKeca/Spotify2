using MusicStreaming.Data;
using MusicStreaming.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MusicStreaming.Controllers
{
    public class SongsController : Controller
    {
        private readonly MusicContext _context;

        public SongsController(MusicContext context)
        {
            _context = context;
        }

        private bool IsLoggedIn() => !string.IsNullOrEmpty(HttpContext.Session.GetString("Username"));

        private int GetUserId() => HttpContext.Session.GetInt32("UserId") ?? 0;

        private void SetViewData()
        {
            ViewData["Username"] = HttpContext.Session.GetString("Username");
            ViewData["Role"] = HttpContext.Session.GetString("Role");
        }

        public IActionResult Index()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            SetViewData();
            var userId = GetUserId(); // your session method
            if (userId > 0)
            {
                // Saved songs
                var savedSongIds = _context.SavedSongs
                    .Where(ss => ss.UserId == userId)
                    .Select(ss => ss.SongId)
                    .ToHashSet();
            
            // Saved playlists  
            var savedPlaylistIds = _context.SavedPlaylists
                .Where(sp => sp.UserId == userId)
                .Select(sp => sp.PlaylistId)
                .ToHashSet();
            ViewData["SavedSongIds"] = savedSongIds;
            ViewData["SavedPlaylistIds"] = savedPlaylistIds;
            }
            
            var songs = _context.Songs.Include(s => s.Artist).OrderByDescending(s => s.CreatedAt).ToList();
            return View(songs);
        }

        public IActionResult Details(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            SetViewData();
            var song = _context.Songs.Include(s => s.Artist).FirstOrDefault(s => s.Id == id);
            if (song == null)
                return NotFound();

            var userId = GetUserId();
            var userPlaylists = _context.Playlists.Where(p => p.UserId == userId).ToList();
            ViewData["UserPlaylists"] = userPlaylists;

            return View(song);
        }

        public IActionResult Create()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            var role = HttpContext.Session.GetString("Role");
            if (role != "Artist" && role != "Admin")
                return Forbid();

            SetViewData();
            var artists = _context.Users.Where(u => u.Role == "Artist" || u.Role == "Admin").ToList();
            ViewData["Artists"] = artists;
            return View();
        }

        [HttpPost]
        public IActionResult Create([Bind("SongName,ArtistId,Producer,SongYear")] Song song)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            var userId = GetUserId();
            var role = HttpContext.Session.GetString("Role");

            if (role == "Artist")
                song.ArtistId = userId;

            var artists = _context.Users.Where(u => u.Role == "Artist" || u.Role == "Admin").ToList();
            ViewData["Artists"] = artists;
            SetViewData();

            if (ModelState.IsValid)
            {
                _context.Songs.Add(song);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(song);
        }

        public IActionResult Edit(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            var song = _context.Songs.Find(id);
            if (song == null)
                return NotFound();

            var role = HttpContext.Session.GetString("Role");
            var userId = GetUserId();
            if (role != "Admin" && song.ArtistId != userId)
                return Forbid();

            SetViewData();
            var artists = _context.Users.Where(u => u.Role == "Artist" || u.Role == "Admin").ToList();
            ViewData["Artists"] = artists;
            return View(song);
        }

        [HttpPost]
        public IActionResult Edit(int id, [Bind("Id,SongName,ArtistId,Producer,SongYear")] Song song)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            if (id != song.Id)
                return NotFound();

            var artists = _context.Users.Where(u => u.Role == "Artist" || u.Role == "Admin").ToList();
            ViewData["Artists"] = artists;
            SetViewData();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(song);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch
                {
                    return View(song);
                }
            }

            return View(song);
        }

        public IActionResult Delete(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            var song = _context.Songs.Include(s => s.Artist).FirstOrDefault(s => s.Id == id);
            if (song == null)
                return NotFound();

            SetViewData();
            return View(song);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            var song = _context.Songs.Find(id);
            if (song != null)
            {
                _context.Songs.Remove(song);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
