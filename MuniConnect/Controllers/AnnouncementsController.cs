using Microsoft.AspNetCore.Mvc;
using MuniConnect.Data;
using MuniConnect.Models;
using System.Collections.Generic;

namespace MuniConnect.Controllers
{
    public class AnnouncementsController : Controller
    {
        private readonly AnnouncementsRepository _repo;

        public AnnouncementsController(AnnouncementsRepository repo)
        {
            _repo = repo;
        }

      
        public IActionResult Index()
        {
            var announcements = _repo.GetAll();
            ViewData["Title"] = "Municipality of Port Elizabeth Announcements";
            return View(announcements);
        }

        // JSON endpoint for events.js
        public JsonResult SearchJson(string category)
        {
            IEnumerable<Announcement> results = _repo.GetByCategory(category);
            var json = new
            {
                results = results
            };
            return Json(json);
        }


        // Get all announcements in JSON
        public JsonResult GetAllJson()
        {
            var announcements = _repo.GetAll();
            return Json(announcements);
        }
    }
}
