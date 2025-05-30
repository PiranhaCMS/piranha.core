# Piranha.core Fork, UA 2025
This is a private fork of the Piranha.core repository, intended for student use in the UA 2025 course, Software Architecture (Arquiteturas de Software).

## Members:
- Bruno Páscoa Nº107418
- Daniel Pedrinho Nº107378
- Lia Cardoso Nº107548
- Liliana Ribeiro Nº108713

## Project Structure:

```
/home/lilikas11/Curso/AS/piranha.core/
├── README.md
├── src/
│   ├── Piranha/
│   │   ├── Controllers/
│   │   │   ├── HomeController.cs
│   │   │   └── ApiController.cs
│   │   ├── Models/
│   │   │   ├── PageModel.cs
│   │   │   └── PostModel.cs
│   │   ├── Services/
│   │   │   ├── ContentService.cs
│   │   │   └── UserService.cs
│   │   └── Piranha.csproj
│   ├── Piranha.Manager/
│   │   ├── Controllers/
│   │   │   ├── DashboardController.cs
│   │   │   └── SettingsController.cs
│   │   ├── Views/
│   │   │   ├── Dashboard/
│   │   │   │   └── Index.cshtml
│   │   │   └── Settings/
│   │   │       └── Index.cshtml
│   │   └── Piranha.Manager.csproj
│   └── Piranha.Web/
│       ├── Pages/
│       │   ├── HomePage.cs
│       │   └── BlogPage.cs
│       ├── wwwroot/
│       │   ├── css/
│       │   │   └── site.css
│       │   ├── js/
│       │   │   └── site.js
│       │   └── images/
│       │       └── logo.png
│       └── Piranha.Web.csproj
├── tests/
│   ├── Piranha.Tests/
│   │   ├── Unit/
│   │   │   ├── ContentServiceTests.cs
│   │   │   └── UserServiceTests.cs
│   │   ├── Integration/
│   │   │   ├── ApiIntegrationTests.cs
│   │   │   └── WebIntegrationTests.cs
│   │   └── Piranha.Tests.csproj
│   └── Piranha.Manager.Tests/
│       ├── DashboardTests.cs
│       └── Piranha.Manager.Tests.csproj
├── docs/
│   ├── architecture.md
│   ├── setup.md
│   └── usage.md
└── .gitignore
```