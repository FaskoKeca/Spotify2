using MusicStreaming.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MusicStreaming.Controllers
{
    public class LibraryController : Controller
    {
        private readonly MusicContext _context;

        public LibraryController(MusicContext context)
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
            var userId = GetUserId();
            var savedSongs = _context.SavedSongs
                .Where(s => s.UserId == userId)
                .Include(s => s.Song)
                .ThenInclude(s => s.Artist)
                .OrderByDescending(s => s.SavedAt)
                .ToList();

            var savedPlaylists = _context.SavedPlaylists
                .Where(p => p.UserId == userId)
                .Include(p => p.Playlist)
                .OrderByDescending(p => p.SavedAt)
                .ToList();

            ViewData["SavedSongs"] = savedSongs;
            ViewData["SavedPlaylists"] = savedPlaylists;

            return View();
        }

        [HttpPost]
        public IActionResult SaveSong(int songId)
        {
            if (!IsLoggedIn())
                return Unauthorized();

            var userId = GetUserId();
            var existing = _context.SavedSongs.FirstOrDefault(s => s.UserId == userId && s.SongId == songId);

            if (existing == null)
            {
                var savedSong = new Models.SavedSong
                {
                    UserId = userId,
                    SongId = songId,
                    SavedAt = DateTime.Now
                };
                _context.SavedSongs.Add(savedSong);
                _context.SaveChanges();
            }

            return Ok(new { message = "Song saved" });
        }

        [HttpPost]
        public IActionResult RemoveSong(int songId)
        {
            if (!IsLoggedIn())
                return Unauthorized();

            var userId = GetUserId();
            var saved = _context.SavedSongs.FirstOrDefault(s => s.UserId == userId && s.SongId == songId);

            if (saved != null)
            {
                _context.SavedSongs.Remove(saved);
                _context.SaveChanges();
            }

            return Ok(new { message = "Song removed" });
        }

        [HttpPost]
        public IActionResult SavePlaylist(int playlistId)
        {
            if (!IsLoggedIn())
                return Unauthorized();

            var userId = GetUserId();
            var existing = _context.SavedPlaylists.FirstOrDefault(p => p.UserId == userId && p.PlaylistId == playlistId);

            if (existing == null)
            {
                var savedPlaylist = new Models.SavedPlaylist
                {
                    UserId = userId,
                    PlaylistId = playlistId,
                    SavedAt = DateTime.Now
                };
                _context.SavedPlaylists.Add(savedPlaylist);
                _context.SaveChanges();
            }

            return Ok(new { message = "Playlist saved" });
        }

        [HttpPost]
        public IActionResult RemovePlaylist(int playlistId)
        {
            if (!IsLoggedIn())
                return Unauthorized();

            var userId = GetUserId();
            var saved = _context.SavedPlaylists.FirstOrDefault(p => p.UserId == userId && p.PlaylistId == playlistId);

            if (saved != null)
            {
                _context.SavedPlaylists.Remove(saved);
                _context.SaveChanges();
            }

            return Ok(new { message = "Playlist removed" });
        }

        [HttpGet]
        public IActionResult IsSongSaved(int songId)
        {
            if (!IsLoggedIn())
                return Ok(new { saved = false });

            var userId = GetUserId();
            var saved = _context.SavedSongs.Any(s => s.UserId == userId && s.SongId == songId);
            return Ok(new { saved });
        }

        [HttpGet]
        public IActionResult IsPlaylistSaved(int playlistId)
        {
            if (!IsLoggedIn())
                return Ok(new { saved = false });

            var userId = GetUserId();
            var saved = _context.SavedPlaylists.Any(p => p.UserId == userId && p.PlaylistId == playlistId);
            return Ok(new { saved });
        }
    }
}
