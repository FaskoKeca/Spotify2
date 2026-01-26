using MusicStreaming.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MusicStreaming.Controllers
{
    public class FollowController : Controller
    {
        private readonly MusicContext _context;

        public FollowController(MusicContext context)
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
            var following = _context.UserFollows
                .Where(uf => uf.FollowerId == userId)
                .Include(uf => uf.Following)
                .OrderByDescending(uf => uf.FollowedAt)
                .ToList();

            return View(following);
        }

        [HttpPost]
        public IActionResult Follow([FromQuery] int followingId)
        {
            if (!IsLoggedIn())
                return Unauthorized();

            var userId = GetUserId();
            if (userId == followingId)
                return BadRequest(new { message = "Cannot follow yourself" });

            var existing = _context.UserFollows
                .FirstOrDefault(uf => uf.FollowerId == userId && uf.FollowingId == followingId);

            if (existing == null)
            {
                var follow = new Models.UserFollow
                {
                    FollowerId = userId,
                    FollowingId = followingId,
                    FollowedAt = DateTime.Now
                };
                _context.UserFollows.Add(follow);
                _context.SaveChanges();
            }

            return Ok(new { message = "Following" });
        }

        [HttpPost]
        public IActionResult Unfollow([FromQuery] int followingId)
        {
            if (!IsLoggedIn())
                return Unauthorized();

            var userId = GetUserId();
            var follow = _context.UserFollows
                .FirstOrDefault(uf => uf.FollowerId == userId && uf.FollowingId == followingId);

            if (follow != null)
            {
                _context.UserFollows.Remove(follow);
                _context.SaveChanges();
            }

            return Ok(new { message = "Unfollowed" });
        }

        [HttpGet]
        public IActionResult IsFollowing(int followingId)
        {
            if (!IsLoggedIn())
                return Ok(new { isFollowing = false });

            var userId = GetUserId();
            var isFollowing = _context.UserFollows
                .Any(uf => uf.FollowerId == userId && uf.FollowingId == followingId);

            return Ok(new { isFollowing });
        }
    }
}
