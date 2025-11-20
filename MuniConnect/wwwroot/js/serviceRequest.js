document.addEventListener("DOMContentLoaded", function () {

    console.log("ServiceRequest.js loaded ");

    // SEARCH REQUEST BY AJAX 
    const searchForm = document.querySelector("#searchForm");
    if (searchForm) {
        searchForm.addEventListener("submit", function (e) {
            e.preventDefault();

            const id = document.querySelector("#searchInput").value.trim();
            if (id === "") {
                alert("Please enter a request ID");
                return;
            }

            // Redirect normally OR use AJAX
            window.location.href = `/ServiceRequest/TrackProgress?requestId=${id}`;
        });
    }

    // FETCH BFS TRAVERSAL
    const bfsBtn = document.querySelector("#btnBFS");
    if (bfsBtn) {
        bfsBtn.addEventListener("click", async function () {
            const requestId = bfsBtn.dataset.id;

            const response = await fetch(`/ServiceRequest/BFSJson?requestId=${requestId}`);
            const data = await response.json();

            const display = document.querySelector("#bfsResult");

            if (data.error) {
                display.innerHTML = `<div class='alert alert-danger'>${data.error}</div>`;
            } else {
                display.innerHTML = `
                    <div class="alert alert-info">
                        <strong>BFS Traversal:</strong><br>
                        ${data.traversal.join(" → ")}
                    </div>`;
            }
        });
    }

    // STATUS COLOR ANIMATION
    const statusBadge = document.querySelector(".status-badge");
    if (statusBadge) {
        statusBadge.classList.add("fade-in");
    }

});
