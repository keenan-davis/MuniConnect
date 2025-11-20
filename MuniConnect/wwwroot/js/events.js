// Update Recently Viewed container
function updateLastViewedContainer(events) {
    const container = document.getElementById('lastViewedList');
    if (!container) return;

    if (!events || events.length === 0) {
        container.innerHTML = `<p class="text-muted text-center">No events viewed yet.</p>`;
        return;
    }

    let html = '<ul class="list-group list-group-flush">';
    events.forEach(e => {
        html += `<li class="list-group-item">
                    <strong>${e.title}</strong><br/>
                    <small class="text-muted">${e.startDate}</small><br/>
                    <a href="/Events/Details/${e.id}" class="btn btn-sm btn-outline-secondary mt-2">Revisit</a>
                 </li>`;
    });
    html += '</ul>';
    container.innerHTML = html;
}

// Push an event as viewed
async function pushLastViewed(eventId) {
    if (!eventId) return;

    const res = await fetch('/Events/PushLastViewed', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ id: eventId })
    });

    if (res.ok) {
        const lastViewed = await res.json();
        updateLastViewedContainer(lastViewed);
    }
}

// Update Recommended Events container
function updateRecommendedEventsContainer(events) {
    const recContainer = document.getElementById("recommendations");
    if (!recContainer) return;

    recContainer.innerHTML = "";

    if (!events || events.length === 0) {
        recContainer.innerHTML = `<p class='text-center text-muted'>No recommendations found.</p>`;
        return;
    }

    events.forEach(r => {
        recContainer.innerHTML += `
            <div class="col-md-3 mb-4">
                <div class="card shadow-sm h-100 border-success">
                    <div class="card-body">
                        <h6 class="card-title">${r.title}</h6>
                        <p class="text-muted">${r.startDate}</p>
                        <span class="badge bg-success">${r.category}</span>
                    </div>
                    <div class="card-footer bg-transparent border-0">
                        <a href="/Events/Details/${r.id}" class="btn btn-outline-success w-100">View</a>
                    </div>
                </div>
            </div>`;
    });
}

// Update Announcements container
function updateAnnouncementsContainer(announcements) {
    const container = document.getElementById('announcementsList');
    if (!container) return;

    if (!announcements || announcements.length === 0) {
        container.innerHTML = `<p class="text-muted text-center">No announcements available.</p>`;
        return;
    }

    let html = '';
    announcements.forEach(a => {
        html += `<div class="col-md-6 mb-4">
                    <div class="card h-100 shadow-sm">
                        <div class="card-body">
                            <h5 class="card-title">${a.title}</h5>
                            <p class="text-muted">${a.publishDate}</p>
                            <p>${a.description}</p>
                            <span class="badge bg-secondary">${a.category}</span>
                        </div>
                    </div>
                 </div>`;
    });

    container.innerHTML = html;
}

// Fetch announcements from server
async function loadAnnouncements() {
    const res = await fetch('/Events/GetAnnouncementsJson');
    if (!res.ok) return;

    const data = await res.json();
    updateAnnouncementsContainer(data.announcements);
}

// Fetch events based on search or category
async function fetchEvents(params = "") {
    const res = await fetch(`/Events/Search?${params}`);
    if (!res.ok) return null;
    return await res.json();
}

// Index Page: Search & Filter
const searchForm = document.getElementById("searchForm");
if (searchForm) {
    searchForm.addEventListener("submit", async function (e) {
        e.preventDefault();
        const params = new URLSearchParams(new FormData(this));

        const data = await fetchEvents(params.toString());
        if (!data) return;

        // Update main events
        const eventsContainer = document.getElementById("eventsList");
        eventsContainer.innerHTML = "";
        if (!data.results || data.results.length === 0) {
            eventsContainer.innerHTML = `<p class='text-center text-muted'>No events found.</p>`;
        } else {
            data.results.forEach(e => {
                eventsContainer.innerHTML += `
                    <div class="col-md-4 mb-4">
                        <div class="card h-100 shadow-sm">
                            <div class="card-body">
                                <h5 class="card-title">${e.title}</h5>
                                <p class="text-muted">${e.startDate} - ${e.location}</p>
                                <p>${e.description}</p>
                                <span class="badge bg-info text-dark">${e.category}</span>
                            </div>
                            <div class="card-footer bg-transparent border-top-0">
                                <a href="/Events/Details/${e.id}" class="btn btn-outline-primary w-100">View Details</a>
                            </div>
                        </div>
                    </div>`;
            });
        }

        // Update recommendations
        updateRecommendedEventsContainer(data.recommendations);

        // Update last viewed
        const lastViewedRes = await fetch('/Events/GetLastViewedJson');
        const lastViewedData = await lastViewedRes.json();
        updateLastViewedContainer(lastViewedData);

        // Load announcements (always same list)
        loadAnnouncements();
    });
}

// Category dropdown: Update recommendations immediately
const categorySelect = document.getElementById("category");
if (categorySelect) {
    categorySelect.addEventListener("change", async () => {
        const selectedCategory = categorySelect.value;
        const params = new URLSearchParams({ category: selectedCategory });
        const data = await fetchEvents(params.toString());
        if (data) updateRecommendedEventsContainer(data.recommendations);
    });
}

// Details Page: Push current event
document.addEventListener('DOMContentLoaded', () => {
    const currentEventId = document.getElementById('currentEventId')?.value;
    if (currentEventId) pushLastViewed(currentEventId);

    // Load announcements on page load
    loadAnnouncements();
});
