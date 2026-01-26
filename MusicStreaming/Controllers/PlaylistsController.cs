using MusicStreaming.Data;
using MusicStreaming.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MusicStreaming.Controllers
{
    public class PlaylistsController : Controller
    {
        private readonly MusicContext _context;

        public PlaylistsController(MusicContext context)
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
            var playlists = _context.Playlists
                .Where(p => p.UserId == userId)
                .Include(p => p.PlaylistSongs)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();
            return View(playlists);
        }

        public IActionResult Details(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            SetViewData();
            var playlist = _context.Playlists
                .Include(p => p.PlaylistSongs)
                .ThenInclude(ps => ps.Song)
                .ThenInclude(s => s.Artist)
                .FirstOrDefault(p => p.Id == id);

            if (playlist == null)
                return NotFound();

            var allSongs = _context.Songs.Include(s => s.Artist).ToList();
            ViewData["AvailableSongs"] = allSongs;

            return View(playlist);
        }

        public IActionResult Create()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            SetViewData();
            return View();
        }

        [HttpPost]
        public IActionResult Create([Bind("PlaylistName,Description")] Playlist playlist)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            var userId = GetUserId();
            playlist.UserId = userId;
            playlist.CreatedAt = DateTime.Now;

            SetViewData();

            if (ModelState.IsValid)
            {
                _context.Playlists.Add(playlist);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(playlist);
        }

        public IActionResult Edit(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            var playlist = _context.Playlists.Find(id);
            if (playlist == null)
                return NotFound();

            var userId = GetUserId();
            if (playlist.UserId != userId)
                return Forbid();

            SetViewData();
            return View(playlist);
        }

        [HttpPost]
        public IActionResult Edit(int id, [Bind("Id,PlaylistName,Description")] Playlist playlist)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            if (id != playlist.Id)
                return NotFound();

            SetViewData();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingPlaylist = _context.Playlists.Find(id);
                    if (existingPlaylist != null)
                    {
                        existingPlaylist.PlaylistName = playlist.PlaylistName;
                        existingPlaylist.Description = playlist.Description;
                        _context.Update(existingPlaylist);
                        _context.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
                catch
                {
                    return View(playlist);
                }
            }

            return View(playlist);
        }

        public IActionResult Delete(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            var playlist = _context.Playlists.Find(id);
            if (playlist == null)
                return NotFound();

            SetViewData();
            return View(playlist);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            var playlist = _context.Playlists.Find(id);
            if (playlist != null)
            {
                _context.Playlists.Remove(playlist);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AddSong(int playlistId, int songId)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            var existingPlaylistSong = _context.PlaylistSongs
                .FirstOrDefault(ps => ps.PlaylistId == playlistId && ps.SongId == songId);

            if (existingPlaylistSong == null)
            {
                var playlistSong = new PlaylistSong
                {
                    PlaylistId = playlistId,
                    SongId = songId,
                    AddedAt = DateTime.Now
                };
                _context.PlaylistSongs.Add(playlistSong);
                _context.SaveChanges();
            }

            return RedirectToAction("Details", new { id = playlistId });
        }

        [HttpPost]
        public IActionResult RemoveSong(int playlistId, int songId)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            var playlistSong = _context.PlaylistSongs
                .FirstOrDefault(ps => ps.PlaylistId == playlistId && ps.SongId == songId);

            if (playlistSong != null)
            {
                _context.PlaylistSongs.Remove(playlistSong);
                _context.SaveChanges();
            }

            return RedirectToAction("Details", new { id = playlistId });
        }
    }
}






