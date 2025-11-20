using Microsoft.AspNetCore.Mvc;
using MuniConnect.Data;
using MuniConnect.Models;

namespace MuniConnect.Controllers
{
    public class ServiceRequestController : Controller
    {
        private readonly BSTServiceRequestRepository _bstRepo;
        private readonly GraphServiceRequestRepository _graphRepo;

        public ServiceRequestController(
            BSTServiceRequestRepository bstRepo,
            GraphServiceRequestRepository graphRepo)
        {
            _bstRepo = bstRepo;
            _graphRepo = graphRepo;
        }

        public IActionResult Index()
        {
            var all = _bstRepo.GetAll();
            return View(all);
        }

        public IActionResult Search(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return View(new List<ServiceRequest>());

            var found = _bstRepo.FindById(id);

            return View(found == null
                ? new List<ServiceRequest>()
                : new List<ServiceRequest> { found });
        }

        public IActionResult Create()
        {
            return View(new ServiceRequest());
        }

        [HttpPost]
        public IActionResult Create(ServiceRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            _bstRepo.Insert(request);
            _graphRepo.AddRequest(request);

            TempData["msg"] = "Service Request Created!";
            return RedirectToAction("Index");
        }

        public IActionResult Track(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return RedirectToAction("Index");

            var result = _graphRepo.BFS(id);

            return View("TrackProgress", result);
        }
        public IActionResult GetTraversalJson(string id)
        {
            var results = _graphRepo.BFS(id);
            return Json(results);
        }
    }
}
