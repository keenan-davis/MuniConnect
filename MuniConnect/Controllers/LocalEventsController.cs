using Microsoft.AspNetCore.Mvc;
using MuniConnect.Data;
using MuniConnect.Models;

namespace MuniConnect.Controllers
{
    public class LocalEventsController : Controller
    {
        private readonly EventRepository _repository;

        public LocalEventsController()
        {
            _repository = new EventRepository();
            _repository.SeedLocalEvents(); 
        }

        // Display all events
        public IActionResult Index(string? category, string? searchTerm)
        {
            IEnumerable<Event> events;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                events = _repository.GetAllEvents()
                    .Where(e =>
                        e.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        e.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        e.Category.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

                if (!events.Any())
                {
                    ViewBag.Message = "No matching events found.";
                }
            }
            else if (!string.IsNullOrWhiteSpace(category))
            {
                events = _repository.Search(category);
                if (!events.Any())
                {
                    ViewBag.Message = $"No events found in '{category}' category.";
                }
            }
            else
            {
                events = _repository.GetAllEvents();
            }

            ViewBag.Categories = _repository.GetAllCategories();
            return View(events);
        }

        // Display event details
        public IActionResult Details(int id)
        {
            var ev = _repository.GetById(id);
            if (ev == null)
            {
                return NotFound();
            }

            // Track user viewing for recommendations
            _repository.PushLastViewed("default", ev.Id);

            return View(ev);
        }

        // Show recommended events for user
        public IActionResult Recommendations()
        {
            var recommended = _repository.RecommendForUser("default");

            if (!recommended.Any())
            {
                ViewBag.Message = "No recommendations available yet. Try viewing or searching for events.";
            }

            return View(recommended);
        }
    }
}
