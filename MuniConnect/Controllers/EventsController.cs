using Microsoft.AspNetCore.Mvc;
using MuniConnect.Data;
using MuniConnect.Models;

namespace MuniConnect.Controllers
{
    public class EventsController : Controller
    {
        private readonly EventRepository _repo;
        private readonly AnnouncementsRepository _annRepo;

        public EventsController(EventRepository repo, AnnouncementsRepository annRepo)
        {
            _repo = repo;
            _annRepo = annRepo;

            // Seed data
            _repo.SeedLocalEvents();
            _annRepo.SeedAnnouncements();
        }

        public IActionResult Index()
        {
            var events = _repo.GetAllEvents();
            var categories = _repo.GetAllCategories();
            var userId = GetUserId();
            var recommendations = _repo.RecommendForUser(userId, 5);
            var lastViewed = _repo.GetLastViewed(userId, 5);

            ViewBag.Categories = categories;
            ViewBag.Recommendations = recommendations;
            ViewBag.LastViewed = lastViewed;

            // Add announcements
            var announcements = _annRepo.GetAll();
            ViewBag.Announcements = announcements;

            return View(events);
        }

        // GET: /Announcements 
        [HttpGet]
        public JsonResult GetAnnouncementsJson(string category)
        {
            var announcements = string.IsNullOrWhiteSpace(category)
                ? _annRepo.GetAll()
                : _annRepo.GetByCategory(category);

            var jsonData = announcements.Select(a => new
            {
                a.Id,
                a.Title,
                PublishDate = a.PublishDate.ToString("yyyy-MM-dd"),
                a.Description,
                a.Location,
                a.Category
            });

            return Json(new { announcements = jsonData });
        }

        // GET: /Events/Details/5
        public IActionResult Details(int id)
        {
            var ev = _repo.GetById(id);
            if (ev == null) return NotFound();

            var userId = User?.Identity?.Name ?? "guest";

            _repo.PushLastViewed(userId, id);
            _repo.IncrementView(id);

            var recommendations = _repo.RecommendForUser(userId, 5);
            var trending = _repo.GetTrendingEvents(3);

            ViewBag.Recommendations = recommendations;
            ViewBag.Trending = trending;

            return View(ev);
        }

        // GET: /Events/Search
        public JsonResult Search(string category, DateTime? from, DateTime? to, string sortBy = "date")
        {
            var userId = GetUserId();
            var results = _repo.Search(category, from, to, sortBy).ToList();

            // Record search
            if (!string.IsNullOrWhiteSpace(category))
            {
                _repo.RecordUserSearch(userId, category);
            }

            // Fetch recommendations based on user history or selected category
            var recommendations = !string.IsNullOrEmpty(category)
                ? _repo.RecommendByCategory(category, 5)
                : _repo.RecommendForUser(userId, 5);

            var jsonData = new
            {
                results = results.Select(e => new
                {
                    e.Id,
                    e.Title,
                    StartDate = e.StartDate.ToString("yyyy-MM-dd"),
                    EndDate = e.EndDate.ToString("yyyy-MM-dd"),
                    e.Description,
                    e.Location,
                    e.Category
                }),
                recommendations = recommendations.Select(r => new
                {
                    r.Id,
                    r.Title,
                    StartDate = r.StartDate.ToString("yyyy-MM-dd"),
                    EndDate = r.EndDate.ToString("yyyy-MM-dd"),
                    r.Description,
                    r.Location,
                    r.Category
                })
            };

            return Json(jsonData);
        }

        [HttpPost]
        public IActionResult PushLastViewed(int id)
        {
            var userId = GetUserId();
            _repo.PushLastViewed(userId, id);

            var lastViewed = _repo.GetLastViewed(userId, 5)
                                  .Select(e => new {
                                      id = e.Id,
                                      title = e.Title,
                                      startDate = e.StartDate.ToString("yyyy-MM-dd")
                                  });

            return Json(lastViewed);
        }

        [HttpGet]
        public IActionResult GetLastViewedJson(int max = 5)
        {
            var userId = GetUserId();
            var lastViewed = _repo.GetLastViewed(userId, max)
                                  .Select(e => new {
                                      id = e.Id,
                                      title = e.Title,
                                      startDate = e.StartDate.ToString("yyyy-MM-dd")
                                  });
            return Json(lastViewed);
        }

        // Simple helper to get current user ID
        private string GetUserId()
        {
            return User?.Identity?.Name ?? "guest";
        }
        [HttpPost]
        public IActionResult LikeEvent(int id)
        {
            var userId = User?.Identity?.Name ?? "guest";
            var liked = _repo.ToggleLike(userId, id);
            var ev = _repo.GetById(id);

            return Json(new { liked, likes = ev?.Likes });
        }

        [HttpPost]
        public IActionResult RateEvent(int id, double rating)
        {
            var userId = User?.Identity?.Name ?? "guest";
            _repo.AddOrUpdateRating(userId, id, rating);

            var ev = _repo.GetById(id);
            return Json(new { average = ev?.AverageRating, count = ev?.RatingCount });
        }
    }
}
