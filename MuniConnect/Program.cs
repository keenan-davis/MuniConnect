using MuniConnect.Data;
using MuniConnect.Models;
namespace MuniConnect
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSingleton<EventRepository>();
            builder.Services.AddSingleton<AnnouncementsRepository>();
            builder.Services.AddSingleton<BSTServiceRequestRepository>(); // ✅ Added
            builder.Services.AddSingleton<GraphServiceRequestRepository>(); // ✅ Added

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
        private static void SeedServiceRequests(IServiceProvider services)
        {
            var bstRepo = services.GetRequiredService<BSTServiceRequestRepository>();
            var graphRepo = services.GetRequiredService<GraphServiceRequestRepository>();

            var requests = new List<ServiceRequest>
            {
                new ServiceRequest { RequestId = "REQ001", Title = "Water Leak", Description = "Pipe burst on main road", Department = "Water", Status = "Pending" },
                new ServiceRequest { RequestId = "REQ002", Title = "Power Outage", Description = "Transformer issue", Department = "Electricity", Status = "In Progress" },
                new ServiceRequest { RequestId = "REQ003", Title = "Road Damage", Description = "Pothole near city hall", Department = "Transport", Status = "Completed" }
            };

            foreach (var req in requests)
            {
                bstRepo.Insert(req);
                graphRepo.AddRequest(req);
            }

            // Add relationships for BFS traversal
            graphRepo.AddDependency("REQ001", "REQ002");
            graphRepo.AddDependency("REQ002", "REQ003");
        }
    }
}
