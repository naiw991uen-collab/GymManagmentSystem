<div align="center">

# 🏋️ Gym Management System

**A web application for managing gym members, subscriptions, trainers, and classes.**

![.NET 9](https://img.shields.io/badge/.NET_9-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SignalR](https://img.shields.io/badge/SignalR-Real--Time-0078D4?style=for-the-badge&logo=microsoftazure&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core_MVC-5C2D91?style=for-the-badge&logo=dotnet&logoColor=white)
![EF Core](https://img.shields.io/badge/Entity_Framework_Core-512BD4?style=for-the-badge&logo=nuget&logoColor=white)
![HTML5](https://img.shields.io/badge/HTML5-E34F26?style=for-the-badge&logo=html5&logoColor=white)
![CSS3](https://img.shields.io/badge/CSS3-1572B6?style=for-the-badge&logo=css3&logoColor=white)

</div>

---

## 📖 Overview

Gym Management System is an ASP.NET Core MVC application that helps gym owners and staff run day-to-day operations from a single dashboard: registering members, managing membership plans and subscriptions, organizing trainers and classes, and tracking performance through analytics.

## ✨ Features

- 🔐 **Authentication** – Secure admin login
- 👥 **Members Management** – Add, edit, view, and delete members
- 💳 **Membership Plans & Subscriptions** – Create plans and track member subscriptions
- 🧑‍🏫 **Trainers** – Manage trainer profiles and assignments
- 📅 **Classes** – Schedule and organize gym classes
- 📊 **Analytics Dashboard** – Overview of key gym statistics
- ⚡ **Real-Time Updates (SignalR)** – Live statistics and counters on the **Home** and **Login** pages, updated instantly without refreshing the page
- 📝 **Structured Logging** – Application events are separated by severity into dedicated log files (see below)

### 📝 Logging

Logs are split by level into three separate text files for easy monitoring and troubleshooting:

| File | Level | Purpose |
|------|-------|---------|
| `success.txt` | ✅ Success | Successful operations (e.g. successful login, record created) |
| `warning.txt` | ⚠️ Warning | Unusual but non-critical events (e.g. failed login attempt, invalid input) |
| `errors.txt` | ❌ Error | Exceptions and failures that need attention |

### ⚡ Real-Time with SignalR

The **Home** and **Login** pages use an ASP.NET Core **SignalR** hub to push live numbers to every connected browser, so the figures stay up to date automatically with no manual refresh.

## 🏗️ Architecture

The solution follows a clean **3-Layer Architecture** to separate concerns and keep the code maintainable and testable:

```
GymManagmentSystem
├── GymManagmentSystem.DAL    # Data Access Layer  → Entities, DbContext, Repositories, Migrations
├── GymManagmentSystem.BLL    # Business Logic Layer → Services, ViewModels, Business rules
└── GymManagmentSystem.MVC    # Presentation Layer → Controllers, Views, wwwroot
```

| Layer | Responsibility |
|-------|----------------|
| **DAL** | Database access using Entity Framework Core |
| **BLL** | Business rules and services between UI and data |
| **MVC** | Controllers, Razor views, and static assets |

## 🛠️ Tech Stack

| Category | Technology |
|----------|-----------|
| Framework | ASP.NET Core MVC (.NET 9) |
| Language | C# |
| Real-Time | ASP.NET Core SignalR |
| Logging | File-based logs (Success / Warning / Errors) |
| ORM | Entity Framework Core |
| Database | SQL Server |
| Frontend | HTML, CSS, JavaScript, Bootstrap |

## 🚀 Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/sql-server) (LocalDB or full instance)
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/naiw991uen-collab/GymManagmentSystem.git
   cd GymManagmentSystem
   ```

2. **Configure the connection string** in `GymManagmentSystem.MVC/appsettings.json`
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=GymManagmentSystem;Trusted_Connection=True;"
   }
   ```

3. **Apply database migrations**
   ```bash
   dotnet ef database update --project GymManagmentSystem.DAL --startup-project GymManagmentSystem.MVC
   ```

4. **Run the application**
   ```bash
   dotnet run --project GymManagmentSystem.MVC
   ```

5. Open your browser at `https://localhost:5001`

## 🗺️ Roadmap

- [x] 3-layer architecture
- [x] Login and analytics
- [x] Members and plans
- [x] Logging (Success / Warning / Errors)
- [x] Real-time updates with SignalR
- [ ] Attendance tracking
- [ ] Payment history and invoices
- [ ] Email notifications for expiring subscriptions
- [ ] Unit tests

## 🤝 Contributing

Contributions are welcome! Fork the repo, create a feature branch, and open a pull request.

```bash
git checkout -b feature/your-feature
git commit -m "Add your feature"
git push origin feature/your-feature
```

## 📄 License

This project is licensed under the [MIT License](LICENSE).

## 👤 Author

**naiw991uen-collab** – [GitHub](https://github.com/naiw991uen-collab)

---

<div align="center">⭐ If you like this project, give it a star!</div>
