using System;
using System.Collections.Generic;
using System.Linq;
using MuniConnect.Models;

namespace MuniConnect.Data
{
    public class AnnouncementsRepository
    {
        private readonly List<Announcement> _announcements;
        public AnnouncementsRepository()
        {
            _announcements = new List<Announcement>();
        }

        public void SeedAnnouncements()
        {
            if (_announcements.Any()) return;

            var seededAnnouncements = new List<Announcement>
            {
                new Announcement { Id = 1, Title = "Water Supply Interruption", Category = "Utilities", PublishDate = DateTime.Now.AddDays(-1), Description = "Water supply will be off from 9 AM to 5 PM in Zone A." },
                new Announcement { Id = 2, Title = "Road Closure Notice", Category = "Transport", PublishDate = DateTime.Now.AddDays(-2), Description = "Main Street closed for maintenance until Friday." },
                new Announcement { Id = 3, Title = "New Library Opening", Category = "Community", PublishDate = DateTime.Now.AddDays(-3), Description = "Grand opening of the new library in Town Center." },
    

                new Announcement { Id = 4, Title = "Power Maintenance Scheduled", Category = "Utilities", PublishDate = DateTime.Now.AddDays(-1), Description = "Scheduled power outage for system upgrades on Saturday from 8 AM to 2 PM." },
                new Announcement { Id = 5, Title = "Public Transportation Changes", Category = "Transport", PublishDate = DateTime.Now.AddDays(-4), Description = "New bus routes effective next Monday. Check updated schedules online." },
                new Announcement { Id = 6, Title = "Community Garden Registration", Category = "Community", PublishDate = DateTime.Now.AddDays(-2), Description = "Registration now open for community garden plots. Limited spaces available." },
                new Announcement { Id = 7, Title = "Tax Filing Deadline Reminder", Category = "Government", PublishDate = DateTime.Now.AddDays(-5), Description = "Reminder: Individual tax returns due by April 15th." },
                new Announcement { Id = 8, Title = "School Closure Due to Weather", Category = "Education", PublishDate = DateTime.Now.AddDays(-1), Description = "All schools closed tomorrow due to severe weather conditions." },
                new Announcement { Id = 9, Title = "Free Health Screening Event", Category = "Health", PublishDate = DateTime.Now.AddDays(-3), Description = "Free blood pressure and diabetes screening at Community Health Center this Saturday." },
                new Announcement { Id = 10, Title = "Parking Regulation Updates", Category = "Transport", PublishDate = DateTime.Now.AddDays(-6), Description = "New parking time limits and rates effective in downtown area starting next month." },

                new Announcement { Id = 11, Title = "Emergency Alert System Test", Category = "Safety", PublishDate = DateTime.Now.AddDays(-2), Description = "Monthly emergency alert system test scheduled for Wednesday at 12 PM." },
                new Announcement { Id = 12, Title = "Summer Recreation Programs", Category = "Recreation", PublishDate = DateTime.Now.AddDays(-7), Description = "Registration now open for summer sports camps and swimming lessons." },
                new Announcement { Id = 13, Title = "Internet Service Upgrade", Category = "Utilities", PublishDate = DateTime.Now.AddDays(-1), Description = "Fiber optic network installation may cause temporary service disruptions this week." },
                new Announcement { Id = 14, Title = "Hiring: Public Works Department", Category = "Employment", PublishDate = DateTime.Now.AddDays(-4), Description = "Multiple positions available in public works. Apply by end of month." },
                new Announcement { Id = 15, Title = "Food Drive Initiative", Category = "Charity", PublishDate = DateTime.Now.AddDays(-3), Description = "Annual city-wide food drive begins next week. Drop-off locations listed on website." },

                new Announcement { Id = 16, Title = "Building Permit Process Changes", Category = "Government", PublishDate = DateTime.Now.AddDays(-8), Description = "New online system for building permit applications launching next week." },
                new Announcement { Id = 17, Title = "Vaccination Clinic Schedule", Category = "Health", PublishDate = DateTime.Now.AddDays(-2), Description = "Free flu and COVID-19 vaccination clinics available throughout the month." },
                new Announcement { Id = 18, Title = "Public Meeting: Zoning Changes", Category = "Government", PublishDate = DateTime.Now.AddDays(-5), Description = "Public hearing scheduled to discuss proposed zoning changes in commercial districts." },
                new Announcement { Id = 19, Title = "Library Digital Resources", Category = "Education", PublishDate = DateTime.Now.AddDays(-1), Description = "New e-book and audiobook collections now available through library app." },
                new Announcement { Id = 20, Title = "Water Conservation Measures", Category = "Environment", PublishDate = DateTime.Now.AddDays(-6), Description = "Stage 1 water conservation measures in effect due to low rainfall." },

                new Announcement { Id = 21, Title = "Senior Center Activities", Category = "Community", PublishDate = DateTime.Now.AddDays(-3), Description = "New weekly activities schedule for senior center including fitness classes and social events." },
                new Announcement { Id = 22, Title = "Bike Lane Installation", Category = "Transport", PublishDate = DateTime.Now.AddDays(-7), Description = "New protected bike lanes being installed on Oak Avenue starting Monday." },
                new Announcement { Id = 23, Title = "Emergency Preparedness Workshop", Category = "Safety", PublishDate = DateTime.Now.AddDays(-2), Description = "Free workshop on emergency preparedness and creating family emergency plans." },
                new Announcement { Id = 24, Title = "Hazardous Waste Collection", Category = "Environment", PublishDate = DateTime.Now.AddDays(-9), Description = "Special collection day for household hazardous waste this Saturday at recycling center." },
                new Announcement { Id = 25, Title = "Small Business Grant Program", Category = "Business", PublishDate = DateTime.Now.AddDays(-4), Description = "Applications now being accepted for small business development grants." },

                new Announcement { Id = 26, Title = "Swimming Pool Opening Schedule", Category = "Recreation", PublishDate = DateTime.Now.AddDays(-1), Description = "All public swimming pools open for summer season starting Memorial Day weekend." },
                new Announcement { Id = 27, Title = "Property Tax Assessment Notices", Category = "Government", PublishDate = DateTime.Now.AddDays(-10), Description = "Property tax assessment notices mailed this week. Appeal deadline is 30 days." },
                new Announcement { Id = 28, Title = "Mental Health Resources", Category = "Health", PublishDate = DateTime.Now.AddDays(-3), Description = "New 24/7 mental health crisis hotline now available for residents." }
                };
            _announcements.AddRange(seededAnnouncements);
        }

        // Return all announcements
        public IEnumerable<Announcement> GetAll() => _announcements.OrderByDescending(a => a.PublishDate);

        // Return announcements filtered by category
        public IEnumerable<Announcement> GetByCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return GetAll();

            return _announcements
                .Where(a => a.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(a => a.PublishDate);
        }
    }
}
