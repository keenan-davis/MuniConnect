using Microsoft.AspNetCore.Mvc;
using MuniConnect.Models;
using MuniConnect.Data;

namespace MuniConnect.Controllers
{
    public class IssuesController : Controller
    {
        private static readonly IssueRepository _repo = new IssueRepository();
        private static readonly int TargetIssues = 5; 

        // GET: /Issues
        public IActionResult Index()
        {
            var issues = _repo.GetAll().ToList(); 
            ViewBag.TotalIssues = issues.Count;
            ViewBag.TargetIssues = TargetIssues;
            return View(issues);
        }

        // GET: /Issues/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Issues/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Issue issue, IFormFile Attachment)
        {
            if (ModelState.IsValid)
            {
                // Handle file attachment
                if (Attachment != null && Attachment.Length > 0)
                {
                    var uploadsFolder = Path.Combine("wwwroot", "uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var filePath = Path.Combine(uploadsFolder, Attachment.FileName);
                    using var stream = new FileStream(filePath, FileMode.Create);
                    Attachment.CopyTo(stream);

                    issue.FilePath = "/uploads/" + Attachment.FileName;
                }

                // Add issue via repository
                _repo.Add(issue);
                TempData["SuccessMessage"] = "Issue submitted successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(issue);
        }

        // GET: /Issues/Details/{id}
        public IActionResult Details(int id)
        {
            var issue = _repo.GetById(id);
            if (issue == null)
                return NotFound();

            return View(issue);
        }
    }
}
