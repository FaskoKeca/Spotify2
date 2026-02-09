using MusicStreaming.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using MusicStreaming.Models;

namespace MusicStreaming.Controllers
{
    public class UserProfileController : Controller
    {
        private readonly MusicContext _context;

        public UserProfileController(MusicContext context)
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

        public IActionResult Profile(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            SetViewData();

            var user = _context.Users
                .Include(u => u.Playlists)
                .Include(u => u.Followers)
                .Include(u => u.Following)
                .FirstOrDefault(u => u.Id == id);

            if (user == null)
                return NotFound();

            var currentUserId = GetUserId();
            var isFollowing = _context.UserFollows
                .Any(uf => uf.FollowerId == currentUserId && uf.FollowingId == id);

            ViewData["User"] = user;
            ViewData["UserPlaylists"] = user.Playlists ?? new List<Playlist>();
            ViewData["IsFollowing"] = isFollowing;
            ViewData["IsCurrentUser"] = (user.Id == currentUserId);
            ViewData["FollowerCount"] = user.Followers?.Count ?? 0;
            ViewData["FollowingCount"] = user.Following?.Count ?? 0;

            return View(user);
        }

        public IActionResult Artist(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            SetViewData();

            var artist = _context.Users
                .Include(u => u.Songs)
                .Include(u => u.Followers)
                .Include(u => u.Following)
                .FirstOrDefault(u => u.Id == id && (u.Role == "Artist" || u.Role == "Admin"));

            if (artist == null)
                return NotFound();

            var currentUserId = GetUserId();
            var isFollowing = _context.UserFollows
                .Any(uf => uf.FollowerId == currentUserId && uf.FollowingId == id);

            // saved songs for this user
            var savedSongIds = currentUserId > 0
                ? _context.SavedSongs
                    .Where(ss => ss.UserId == currentUserId)
                    .Select(ss => ss.SongId)
                    .ToHashSet()
                : new HashSet<int>();

            ViewData["User"] = artist;
            ViewData["ArtistSongs"] = artist.Songs ?? new List<Song>();
            ViewData["IsFollowing"] = isFollowing;
            ViewData["IsCurrentUser"] = (artist.Id == currentUserId);
            ViewData["FollowerCount"] = artist.Followers?.Count ?? 0;
            ViewData["FollowingCount"] = artist.Following?.Count ?? 0;
            ViewData["SavedSongIds"] = savedSongIds;  // this enables the button state

            return View(artist);
        }
    }
}
