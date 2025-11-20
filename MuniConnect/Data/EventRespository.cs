using MuniConnect.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MuniConnect.Data
{
    public class EventRepository
    {
        private readonly SortedDictionary<DateTime, List<Event>> _eventsByDate = new();

        private readonly Dictionary<string, List<Event>> _eventsByCategory = new(StringComparer.OrdinalIgnoreCase);

        private readonly HashSet<string> _categories = new(StringComparer.OrdinalIgnoreCase);

        private readonly PriorityQueue<Event, DateTime> _upcomingQueue = new();

        private readonly Dictionary<string, Queue<string>> _recentSearchesByUser = new(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, int> _globalSearchCounts = new(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, Stack<int>> _lastViewedByUser = new(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, string> _lastViewedCategoryByUser = new(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, HashSet<int>> _userLikes = new(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<(string userId, int eventId), double> _userRatings = new();

        private readonly List<Event> _allEvents = new();  

        private int _nextId = 1;

        // Add new event
        public void AddEvent(Event ev)
        {
            ev.Id = _nextId++;

            _allEvents.Add(ev);

            var dateKey = ev.StartDate.Date;
            if (!_eventsByDate.TryGetValue(dateKey, out var dateList))
                _eventsByDate[dateKey] = dateList = new List<Event>();
            dateList.Add(ev);

            if (!_eventsByCategory.TryGetValue(ev.Category, out var catList))
                _eventsByCategory[ev.Category] = catList = new List<Event>();
            catList.Add(ev);

            _categories.Add(ev.Category);
            _upcomingQueue.Enqueue(ev, ev.StartDate);
        }

        public IEnumerable<Event> GetAllEvents() =>
            _allEvents.OrderBy(e => e.StartDate);

        // Search and filter
        public IEnumerable<Event> Search(string category = null, DateTime? from = null, DateTime? to = null, string sortBy = "date")
        {
            IEnumerable<Event> result = _allEvents;

            if (!string.IsNullOrWhiteSpace(category))
                result = result.Where(e => e.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

            if (from.HasValue || to.HasValue)
            {
                var start = from ?? DateTime.MinValue;
                var end = to ?? DateTime.MaxValue;
                result = result.Where(e => e.StartDate >= start && e.StartDate <= end);
            }

            return sortBy?.ToLower() switch
            {
                "name" => result.OrderBy(e => e.Title),
                "category" => result.OrderBy(e => e.Category).ThenBy(e => e.StartDate),
                _ => result.OrderBy(e => e.StartDate)
            };
        }

        public IEnumerable<Event> GetUpcoming(int count = 5)
        {
            var temp = new PriorityQueue<Event, DateTime>();
            foreach (var item in _upcomingQueue.UnorderedItems)
                temp.Enqueue(item.Element, item.Priority);

            var upcoming = new List<Event>();
            while (upcoming.Count < count && temp.Count > 0)
                upcoming.Add(temp.Dequeue());
            return upcoming;
        }

        public void RecordUserSearch(string userId, string term)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(term))
                return;

            term = term.Trim().ToLowerInvariant();
            _globalSearchCounts[term] = _globalSearchCounts.GetValueOrDefault(term, 0) + 1;

            if (!_recentSearchesByUser.TryGetValue(userId, out var queue))
                _recentSearchesByUser[userId] = queue = new Queue<string>();

            if (queue.Count > 0 && queue.Last().Equals(term, StringComparison.OrdinalIgnoreCase))
                return;

            queue.Enqueue(term);
            while (queue.Count > 10) queue.Dequeue();

            var unique = queue.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            _recentSearchesByUser[userId] = new Queue<string>(unique);
        }

        // Recommend events by category 
        public IEnumerable<Event> RecommendByCategory(string category, int max = 5)
        {
            if (string.IsNullOrWhiteSpace(category))
                return Enumerable.Empty<Event>();

            return _allEvents
                .Where(e => e.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(e => e.StartDate)
                .Take(max)
                .ToList();
        }

        // Track recently viewed events and update user’s category preference 
        public void PushLastViewed(string userId, int eventId)
        {
            if (string.IsNullOrWhiteSpace(userId)) userId = "default";

            if (!_lastViewedByUser.TryGetValue(userId, out var stack))
                _lastViewedByUser[userId] = stack = new Stack<int>();

            var list = stack.ToList();
            list.RemoveAll(x => x == eventId);
            list.Insert(0, eventId);

            if (list.Count > 20)
                list = list.Take(20).ToList();

            _lastViewedByUser[userId] = new Stack<int>(list.Reverse<int>());

           
            var viewedEvent = GetById(eventId);
            if (viewedEvent != null)
                _lastViewedCategoryByUser[userId] = viewedEvent.Category;
        }

        // Retrieve recent events
        public IEnumerable<Event> GetLastViewed(string userId, int max = 5)
        {
            if (string.IsNullOrWhiteSpace(userId) || !_lastViewedByUser.TryGetValue(userId, out var stack))
                return Enumerable.Empty<Event>();

            return stack.Take(max)
                        .Select(id => GetById(id))
                        .Where(e => e != null)!;
        }

        public Event? GetById(int id) => _allEvents.FirstOrDefault(e => e.Id == id);

        // Recommend events for user based on last viewed category 
        public IEnumerable<Event> RecommendForUser(string userId, int max = 5)
        {
            
            if (_lastViewedCategoryByUser.TryGetValue(userId, out var lastCategory))
                return RecommendByCategory(lastCategory, max);

            // Fallback to search-based recommendations
            var searches = _recentSearchesByUser.TryGetValue(userId, out var q) ? q.ToList() : new List<string>();

            var topCats = searches
                .GroupBy(x => x, StringComparer.OrdinalIgnoreCase)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .Take(3)
                .ToList();

            if (!topCats.Any())
                topCats = _globalSearchCounts.OrderByDescending(ev => ev.Value).Select(ev => ev.Key).Take(3).ToList();

            var candidates = new HashSet<Event>();

            foreach (var term in topCats)
            {
                foreach (var (cat, list) in _eventsByCategory)
                {
                    if (cat.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                        term.Contains(cat, StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (var e in list)
                            candidates.Add(e);
                    }
                }
            }

            foreach (var e in GetUpcoming(10))
                candidates.Add(e);

            if (!candidates.Any())
                return GetUpcoming(max);

            var scored = new List<(Event ev, double score)>();
            foreach (var e in candidates)
            {
                double score = 0.0;
                if (topCats.Any(c => e.Category.Contains(c, StringComparison.OrdinalIgnoreCase)))
                    score += 5.0;

                var days = (e.StartDate - DateTime.UtcNow).TotalDays;
                score += Math.Max(0, 2 - days / 30.0);
                scored.Add((e, score));
            }

            return scored.OrderByDescending(s => s.score).Select(s => s.ev).Take(max).ToList();
        }
        // Get all categories
        public IEnumerable<string> GetAllCategories() => _categories.OrderBy(x => x);

        // Increment views when a user visits an event
        public void IncrementView(int eventId)
        {
            var ev = GetById(eventId);
            if (ev != null)
                ev.Views++;
        }

        // Toggle like/unlike
        public bool ToggleLike(string userId, int eventId)
        {
            if (string.IsNullOrWhiteSpace(userId)) userId = "guest";

            if (!_userLikes.TryGetValue(userId, out var likedEvents))
                _userLikes[userId] = likedEvents = new HashSet<int>();

            var ev = GetById(eventId);
            if (ev == null) return false;

            if (likedEvents.Contains(eventId))
            {
                likedEvents.Remove(eventId);
                ev.Likes = Math.Max(0, ev.Likes - 1);
                return false; // unliked
            }
            else
            {
                likedEvents.Add(eventId);
                ev.Likes++;
                return true; // liked
            }
        }

        // Add or update a rating
        public void AddOrUpdateRating(string userId, int eventId, double rating)
        {
            if (rating < 1 || rating > 5) return;

            var ev = GetById(eventId);
            if (ev == null) return;

            var key = (userId, eventId);
            if (_userRatings.TryGetValue(key, out var previousRating))
            {
                // Update existing rating
                ev.AverageRating = ((ev.AverageRating * ev.RatingCount) - previousRating + rating) / ev.RatingCount;
                _userRatings[key] = rating;
            }
            else
            {
                // Add new rating
                ev.RatingCount++;
                ev.AverageRating = ((ev.AverageRating * (ev.RatingCount - 1)) + rating) / ev.RatingCount;
                _userRatings[key] = rating;
            }
        }

        // Get trending events based on engagement
        public IEnumerable<Event> GetTrendingEvents(int max = 5)
        {
            return _allEvents
                .OrderByDescending(e => e.EngagementScore)
                .Take(max);
        }

        // Seed local events 
        public void SeedLocalEvents()
        {   //Prevent reseeding if events already exist
            if (_eventsByDate.Any())
                return; 

            var seededEvents = new List<Event>
        {
        new Event { Id = 1, Title = "Community Cleanup", Description = "Join us to clean up the town park.", Location = "Town Park", StartDate = DateTime.Today.AddDays(2), EndDate = DateTime.Today.AddDays(2), Category = "Community" },
        new Event { Id = 2, Title = "Tech Workshop", Description = "Learn about C# and .NET basics.", Location = "Library Hall", StartDate = DateTime.Today.AddDays(5), EndDate = DateTime.Today.AddDays(6), Category = "Workshop" },
        new Event { Id = 3, Title = "Soccer Tournament", Description = "Friendly local matches all weekend.", Location = "Sports Field", StartDate = DateTime.Today.AddDays(7), EndDate = DateTime.Today.AddDays(8), Category = "Sports" },
        new Event { Id = 4, Title = "Farmers Market", Description = "Local produce and crafts.", Location = "Town Square", StartDate = DateTime.Today.AddDays(3), EndDate = DateTime.Today.AddDays(3), Category = "Community" },
        new Event { Id = 5, Title = "Coding Bootcamp", Description = "Intensive programming workshop.", Location = "Tech Hub", StartDate = DateTime.Today.AddDays(10), EndDate = DateTime.Today.AddDays(12), Category = "Workshop" },

        new Event { Id = 6, Title = "Jazz in the Park", Description = "Live jazz performances from local artists.", Location = "Riverside Park", StartDate = DateTime.Today.AddDays(4), EndDate = DateTime.Today.AddDays(4), Category = "Music" },
        new Event { Id = 7, Title = "Yoga for Beginners", Description = "Gentle introduction to yoga and mindfulness.", Location = "Community Center", StartDate = DateTime.Today.AddDays(1), EndDate = DateTime.Today.AddDays(1), Category = "Wellness" },
        new Event { Id = 8, Title = "Art Exhibition Opening", Description = "Featuring works by local contemporary artists.", Location = "City Art Gallery", StartDate = DateTime.Today.AddDays(6), EndDate = DateTime.Today.AddDays(30), Category = "Arts" },
        new Event { Id = 9, Title = "Marathon Training", Description = "Group training session for upcoming city marathon.", Location = "Central Park Track", StartDate = DateTime.Today.AddDays(3), EndDate = DateTime.Today.AddDays(3), Category = "Sports" },
        new Event { Id = 10, Title = "Startup Pitch Competition", Description = "Local entrepreneurs pitch their business ideas.", Location = "Innovation Center", StartDate = DateTime.Today.AddDays(8), EndDate = DateTime.Today.AddDays(8), Category = "Business" },

        new Event { Id = 11, Title = "Food Festival", Description = "Taste dishes from around the world.", Location = "Waterfront Plaza", StartDate = DateTime.Today.AddDays(12), EndDate = DateTime.Today.AddDays(14), Category = "Food" },
        new Event { Id = 12, Title = "Book Club Meeting", Description = "Discussion of this month's selected novel.", Location = "Public Library", StartDate = DateTime.Today.AddDays(7), EndDate = DateTime.Today.AddDays(7), Category = "Education" },
        new Event { Id = 13, Title = "Charity Gala", Description = "Fundraising event for local children's hospital.", Location = "Grand Hotel Ballroom", StartDate = DateTime.Today.AddDays(15), EndDate = DateTime.Today.AddDays(15), Category = "Charity" },
        new Event { Id = 14, Title = "Photography Walk", Description = "Guided tour of scenic spots for photography.", Location = "Historic District", StartDate = DateTime.Today.AddDays(9), EndDate = DateTime.Today.AddDays(9), Category = "Arts" },
        new Event { Id = 15, Title = "Science Fair", Description = "Student projects and scientific demonstrations.", Location = "High School Gym", StartDate = DateTime.Today.AddDays(11), EndDate = DateTime.Today.AddDays(11), Category = "Education" },

        new Event { Id = 16, Title = "Wine Tasting", Description = "Sample local wines and learn about winemaking.", Location = "Vineyard Estate", StartDate = DateTime.Today.AddDays(13), EndDate = DateTime.Today.AddDays(13), Category = "Food" },
        new Event { Id = 17, Title = "Comedy Night", Description = "Stand-up comedy featuring local comedians.", Location = "Downtown Comedy Club", StartDate = DateTime.Today.AddDays(5), EndDate = DateTime.Today.AddDays(5), Category = "Entertainment" },
        new Event { Id = 18, Title = "Gardening Workshop", Description = "Learn organic gardening techniques.", Location = "Botanical Gardens", StartDate = DateTime.Today.AddDays(8), EndDate = DateTime.Today.AddDays(8), Category = "Workshop" },
        new Event { Id = 19, Title = "Film Screening", Description = "Outdoor screening of classic movies.", Location = "Park Amphitheater", StartDate = DateTime.Today.AddDays(10), EndDate = DateTime.Today.AddDays(10), Category = "Entertainment" },
        new Event { Id = 20, Title = "Career Fair", Description = "Connect with local employers and recruiters.", Location = "Convention Center", StartDate = DateTime.Today.AddDays(16), EndDate = DateTime.Today.AddDays(16), Category = "Business" },

        new Event { Id = 21, Title = "Meditation Retreat", Description = "Day-long meditation and mindfulness practice.", Location = "Peaceful Haven Center", StartDate = DateTime.Today.AddDays(18), EndDate = DateTime.Today.AddDays(18), Category = "Wellness" },
        new Event { Id = 22, Title = "Cricket Tournament", Description = "High school cricket finals.", Location = "School Stadium", StartDate = DateTime.Today.AddDays(14), EndDate = DateTime.Today.AddDays(14), Category = "Sports" },
        new Event { Id = 23, Title = "Craft Beer Festival", Description = "Local breweries showcase their best beers.", Location = "Brewery District", StartDate = DateTime.Today.AddDays(20), EndDate = DateTime.Today.AddDays(21), Category = "Food" },
        new Event { Id = 24, Title = "Poetry Reading", Description = "Local poets share their work.", Location = "Cozy Book Cafe", StartDate = DateTime.Today.AddDays(7), EndDate = DateTime.Today.AddDays(7), Category = "Arts" },
        new Event { Id = 25, Title = "Volunteer Day", Description = "Help maintain local hiking trails.", Location = "Forest Preserve", StartDate = DateTime.Today.AddDays(19), EndDate = DateTime.Today.AddDays(19), Category = "Community" }
    };

            foreach (var ev in seededEvents)
            {
                AddEvent(ev);
            }
        }

    }
}
