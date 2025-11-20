# 🏙️ Municipality of Port Elizabeth – Community Issue Reporting Platform

Municipality of Port Elizabeth Serivces is a web-based application designed to empower citizens to **report municipal issues** such as potholes, water leaks, or broken streetlights.  
The platform creates a **feedback loop** between citizens and municipalities, ensuring that problems are logged, tracked, and resolved more efficiently.  

This repository currently focuses on the **Report Issues** feature (Phase 1 of the project).  
Future updates will add **Local Events, Announcements, and Service Request Status** tracking.

---

## ✨ Key Features (Implemented in Phase 1)
- ✅ Citizens can **submit new issues** with a title, description, and optional file attachment (e.g., photo evidence).  
- ✅ All reported issues are displayed in a modern, professional **list view**.  
- ✅ Individual issue details are viewable for clarity.  
- ✅ A **feedback loop mechanism** shows progress towards a reporting target.  
- ✅ File uploads stored securely in `/wwwroot/uploads`.  

---

---
## 🌟 Additional Features (MuniConnect)

- **📅 Local Events**: Browse upcoming municipal events with categories, dates, and locations.

- **⭐ Event Interactions**: Like, rate, and receive personalized recommendations based on user activity.

- **📰 Announcements**: View the latest municipal announcements with publish date, category, and description.

- **🗂️ Recently Viewed Events**: Track and revisit previously viewed events.
---


## 🛠️ Technology Stack
- **Framework:** ASP.NET Core 8 (MVC)  
- **Language:** C#  
- **Frontend:** Razor Pages, Bootstrap 5, custom CSS for a modern UI  
- **Data Storage:** In-memory repository (extensible to EF Core + SQL Server)  
- **File Uploads:** ASP.NET Core `IFormFile`, stored in `wwwroot/uploads`  

---

## 📂 Project Structure
```yaml
MuniConnect/
├── Controllers/
│ ├── IssuesController.cs         
│ ├── EventsController.cs         
├── Models/
│ ├── Issue.cs
│ ├── Event.cs
│ ├── Announcement.cs
├── Data/
│ ├── IssueRepository.cs
│ ├── EventRepository.cs
│ ├── AnnouncementsRepository.cs
├── Views/
│ ├── Issues/
│ │ ├── Index.cshtml
│ │ ├── Create.cshtml
│ │ └── Details.cshtml
│ ├── Events/
│ │ ├── Index.cshtml
│ │ └── Details.cshtml
├── wwwroot/
│ ├── uploads/                     # Stores uploaded files
└── README.md

```

---

## ⚙️ Compilation & Setup Instructions

### 1️⃣ Prerequisites
Ensure you have the following installed:
- [Visual Studio 2022 or later](https://visualstudio.microsoft.com/) (with **ASP.NET and web development workload**)  
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)  
- Git (for cloning the repository)  

Verify installation:
```bash
dotnet --version
```
### 2️⃣ Clone the Repository
```bash
git clone https://github.com/your-username/MuniConnect.git
cd MuniConnect
```
### 3️⃣ Build the Application
You can build the app via CLI:
``` bash
dotnet build
```
Or directly through Visual Studio 
```
Build → Build Solution
```
### 4️⃣ Run the Application 
Using .NET CLI:
```
dotnet run
```
Using Visual Studio:

- Press Ctrl + F5 to run without debugging.

- Or use IIS Express/Kestrel profile to launch.

By default, the app runs at:
``` arduino
https://localhost:5001
http://localhost:5000
```
## 📝 Usage Guide
### Report an Issue

1. Navigate to ```Issues → Create.```

2. Fill in:

- Title (short description, e.g., "Broken Streetlight on Main Rd")

- Description (detailed explanation)

- File Upload (optional) → e.g., attach a photo of the issue.

3. Submit → Issue is saved and visible in the issue list.

#### View Issues

- Go to ``` Issues → Index``` → See all issues with summary info.

#### View Details

- Click an issue → View the full description and attached file.

#### Feedback Loop

- A progress bar (on Issues Index) shows the number of issues reported against a target (```TargetIssues``` is set to 5 in ```IssuesController```).

#### Events & Announcements

- Navigate to ```Events → Index```.

- Browse upcoming events, like/rate events, and view recommendations.

- Latest municipal announcements are displayed below events.

- Recently viewed events are shown in the sidebar.

## 🔄 Extensibility

The app is designed with clean architecture principles:

- Replace the ```IssueRepository``` with Entity Framework Core + SQL Server for persistence.

- Add authentication/authorization for role-based access (e.g., Admin vs Citizen).

- Integrate with municipal service APIs for real-time status tracking.

## 📌 License

This project is licensed under the MIT License – feel free to use, modify, and distribute with attribution.

## 👨‍💻 Author

Keenan Davis - 
ST10201316
