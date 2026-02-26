# Piranha CMS Initialization & Test Flow Maps

This document provides a comprehensive overview of how Piranha CMS initializes and runs its unit tests. It is designed for agent orchestration workflows.

---

## Table of Contents

1. [Piranha CMS Initialization Map](#1-piranha-cms-initialization-map)
2. [Unit Test Initialization Map](#2-unit-test-initialization-map)

---

## 1. Piranha CMS Initialization Map

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                           ASP.NET Core Host                                  │
│  WebApplicationBuilder → AddPiranha() → builder.Build() → UsePiranha()    │
└─────────────────────────────────────────────────────────────────────────────┘
                                        │
                                        ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                        DI Service Registration                              │
│  ┌─────────────────────────────────────────────────────────────────────────┐  │
│  │ PiranhaStartupExtensions.AddPiranha()                                  │  │
│  │   ├── services.AddSingleton<IContentFactory, ContentFactory>()        │  │
│  │   ├── services.AddScoped<IApi, Api>()                                 │  │
│  │   └── services.AddScoped<Config>()                                    │  │
│  └─────────────────────────────────────────────────────────────────────────┘  │
│  ┌─────────────────────────────────────────────────────────────────────────┐  │
│  │ PiranhaRavenDbExtensions.UseRavenDb() / AddPiranhaRavenDb()             │  │
│  │   ├── services.AddSingleton<IDocumentStore>(store)                      │  │
│  │   └── Registers 14 Repositories + IAsyncDocumentSession + Factory       │  │
│  └─────────────────────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────────────────┘
                                        │
                                        ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                      Application Initialization                             │
│                                                                             │
│   App.Init(options.Api)  ─────────────────────►  Uses IApi to:             │
│   │                                              • Load ContentGroups      │
│   │  ┌────────────────────────────────────────►  • Load ContentTypes        │
│   │  │                                          • Load PageTypes            │
│   │  │                                          • Load PostTypes            │
│   │  │                                          • Load SiteTypes            │
│   │  │                                          • Call Init() on Modules   │
│   │  │                                                                   │
│   │  └─► App (Static Singleton)                                           │
│   │        • Registers default Block Types                                 │
│   │        • Registers default Field Types                                 │
│   │        • Registers Serializers                                         │
│   │        • Registers Permissions                                         │
│   │        • Creates Markdown Converter                                    │
│   │                                                                       │
│   ContentTypeBuilder.Build()  ──►  Scans assemblies for [PageType],        │
│                                     [PostType] attributes                   │
└─────────────────────────────────────────────────────────────────────────────┘
```

### Detailed Component Flow

#### 1. Startup Registration (Program.cs)

```csharp
builder.AddPiranha(options =>
{
    options.UseCms();           // ASP.NET Core services
    options.UseManager();        // Admin UI
    options.UseFileStorage();   // File-based media storage
    options.UseImageSharp();    // Image processing
    options.UseMemoryCache();   // Caching
    //options.UseRavenDb(store);   // RavenDB DocumentStore
});
```

#### 2. DI Service Registration Chain

```
PiranhaStartupExtensions.AddPiranha()
    │
    ├──► Registers IApi → Api class (scoped)
    ├──► Registers IContentFactory → ContentFactory (singleton)
    └──► Registers Config (scoped)

PiranhaRavenDbExtensions.UseRavenDb()
    │
    ├──► Adds IDocumentStore (singleton) and IAsyncDocumentSession (scoped)
    └──► RegisterServices()
          │
          ├──► Registers 14 Repositories (scoped):
          │     ├── IAliasRepository → AliasRepository
          │     ├── IArchiveRepository → ArchiveRepository
          │     ├── IContentRepository → ContentRepository
          │     ├── IContentGroupRepository → ContentGroupRepository
          │     ├── IContentTypeRepository → ContentTypeRepository
          │     ├── ILanguageRepository → LanguageRepository
          │     ├── IMediaRepository → MediaRepository
          │     ├── IPageRepository → PageRepository
          │     ├── IPageTypeRepository → PageTypeRepository
          │     ├── IParamRepository → ParamRepository
          │     ├── IPostRepository → PostRepository
          │     ├── IPostTypeRepository → PostTypeRepository
          │     ├── ISiteRepository → SiteRepository
          │     └── ISiteTypeRepository → SiteTypeRepository
          │
          ├──► Registers IDb → T (scoped)  ← The RavenDB session (DbRaven)
          └──► Registers IContentServiceFactory → ContentServiceFactory
```

#### 3. IApi Construction (Composition Root)

```
When IApi is resolved from DI:
    │
    └──► Api Constructor receives ALL dependencies:
         │
         ├──► Repositories (14 total)
         ├──► IContentFactory
         ├──► ICache (optional)
         ├──► IStorage (optional)
         ├──► IImageProcessor (optional)
         └──► ISearch (optional)
         
    Then Api creates services:
         │
         ├──► ContentGroupService(contentGroupRepository, cache)
         ├──► ContentTypeService(contentTypeRepository, cache)
         ├──► LanguageService(languageRepository, cache)
         ├──► PageTypeService(pageTypeRepository, cache)
         ├──► ParamService(paramRepository, cache)
         ├──► PostTypeService(postTypeRepository, cache)
         ├──► SiteTypeService(siteTypeRepository, cache)
         ├──► ContentService(contentRepository, contentFactory, Languages, cache, search)
         ├──► SiteService(siteRepository, contentFactory, Languages, cache)
         ├──► AliasService(aliasRepository, Sites, cache)
         ├──► MediaService(mediaRepository, Params, storage, processor, cache)
         ├──► PageService(pageRepository, contentFactory, Sites, Params, Media, cache, search)
         ├──► PostService(postRepository, contentFactory, Sites, Pages, Params, Media, cache, search)
         └──► ArchiveService(archiveRepository, Params, Posts)
```

#### 4. App Initialization

```
App.Init(IApi api)
    │
    └──► App.InitApp(IApi api)  [Thread-safe double-check lock]
         │
         ├──► _contentGroups.Init(api.ContentGroups.GetAllAsync())
         ├──► _contentTypes.Init(api.ContentTypes.GetAllAsync())
         ├──► _pageTypes.Init(api.PageTypes.GetAllAsync())
         ├──► _postTypes.Init(api.PostTypes.GetAllAsync())
         ├──► _siteTypes.Init(api.SiteTypes.GetAllAsync())
         │
         └──► For each registered module:
                   module.Instance.Init()
                   └──► e.g., Piranha.Data.RavenDb.Module.Init()
                            └──► Registers module with Piranha
```

#### 5. Runtime Request Flow

```
HTTP Request
    │
    ▼
PiranhaApplicationBuilder / PiranhaApplication
    │
    ├──► Gets IApi from DI (scoped per request)
    │
    └──► Controller/Service uses IApi.* (e.g., IApi.Pages, IApi.Sites)
              │
              ▼
         IApi.Pages (PageService)
              │
              ▼
         IPageRepository
              │
              ▼
         IDb (DbContext)  ← RavenDB Session
              │
              ▼
         Database (RavenDB Document Database)
```

### Key Interfaces Summary

| Interface | Purpose | Implementation |
|-----------|---------|----------------|
| `IApi` | Main facade providing access to all CMS services | `Api` class |
| `IAsyncDocumentSession` | Database session abstraction | RavenDB `IAsyncDocumentSession` |
| `IContentFactory` | Factory for creating content models | `ContentFactory` |
| `IRepository<T>` | Data access patterns | 14 repository implementations |

### Database Access Pattern

```
Repository (e.g., SiteRepository)
    │
    ├──► Constructor receives: IDb db (DbRaven)
    │
    └──► Uses _db for all EF operations:
         ├──► _db.Sites (DbSet)
         ├──► _db.SaveChanges()
         ├──► _db.SaveChangesAsync()
         └──► _db.Set<T>() for generic operations
```

---

## 2. Unit Test Initialization Map

### Test Architecture Overview

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                           Test Execution Flow                                │
│                                                                             │
│  ┌─────────────┐      ┌─────────────┐      ┌─────────────┐               │
│  │  dotnet     │ ───► │  xUnit       │ ───► │  Piranha    │               │
│  │  test       │      │  Test Runner │      │  Tests      │               │
│  └─────────────┘      └─────────────┘      └─────────────┘               │
│                                                      │                      │
│                        ┌─────────────────────────────┘                      │
│                        ▼                                                    │
│               ┌────────────────┐                                           │
│               │  BaseTests /   │  ← Test Base Classes                       │
│               │  BaseTestsAsync│                                            │
│               └────────────────┘                                           │
│                        │                                                    │
│         ┌──────────────┼──────────────┐                                    │
│         ▼              ▼              ▼                                     │
│  ┌────────────┐ ┌────────────┐ ┌────────────┐                            │
│  │  Create     │ │  App.Init  │ │ ContentType│                            │
│  │  Service    │ │  (api)     │ │  Builder   │                            │
│  │  Collection │ │             │ │            │                            │
│  └────────────┘ └────────────┘ └────────────┘                            │
│         │              │              │                                     │
│         ▼              ▼              ▼                                     │
│  ┌─────────────────────────────────────────┐                              │
│  │         RavenDB TestServer              │                              │
│  │    (In-Memory Document Store)           │                              │
│  └─────────────────────────────────────────┘                              │
└─────────────────────────────────────────────────────────────────────────────┘
```

### Test Infrastructure Components

#### 1. Test Framework Stack

```
┌─────────────────────────────────────────┐
│           Testing Framework              │
│  • xUnit 2.7.0                         │
│  • Microsoft.NET.Test.Sdk 17.9.0       │
│  • Coverlet (code coverage)            │
└─────────────────────────────────────────┘
           │
           ▼
┌─────────────────────────────────────────┐
│         Test Base Classes                │
│  • BaseTests (sync)                     │
│  • BaseTestsAsync (async) : IAsyncLifetime
└─────────────────────────────────────────┘
```

#### 2. BaseTests Class Hierarchy

```
┌─────────────────────────────────────────────────────────────┐
│                    BaseTests (Synchronous)                  │
│  ┌─────────────────────────────────────────────────────────┐│
│  │ protected IServiceProvider services                      ││
│  │ protected IStorage storage                               ││
│  │                                                            ││
│  │ GetSession() → IAsyncDocumentSession                    ││
│  │   └── Uses: Embedded DocumentStore from Raven TestDriver         ││
│  └─────────────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────────────┘
           │
           │ (inherits)
           ▼
┌─────────────────────────────────────────────────────────────┐
│              BaseTestsAsync : IAsyncLifetime                │
│  ┌─────────────────────────────────────────────────────────┐│
│  │ protected ICache _cache                                 ││
│  │ protected IServiceProvider _services                    ││
│  │                                                            ││
│  │ CreateServiceCollection() → IServiceCollection          ││
│  │   ├── AddPiranhaRavenDb()                              ││
│  │   ├── AddPiranha()                                      ││
│  │   ├── AddMemoryCache()                                  ││
│  │   ├── AddDistributedMemoryCache()                       ││
│  │   └── AddPiranhaFileStorage()                           ││
│  │                                                            ││
│  │ CreateApi() → IApi                                      ││
│  │   └── Manually constructs Api with all repositories     ││
│  └─────────────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────────────┘
```

### Detailed Test Initialization Flow

#### Step 1: Service Collection Creation

```csharp
// In BaseTestsAsync.CreateServiceCollection()
return new ServiceCollection()
.AddScoped(_ => session)
    .AddPiranhaStore<DbRaven>()
    .AddPiranha()
    .AddMemoryCache()
    .AddDistributedMemoryCache()
    .AddPiranhaFileStorage();
```

#### Step 2: API Creation (Manual Composition)

```csharp
// In BaseTestsAsync.CreateApi()
var factory = new ContentFactory(_services);
var serviceFactory = new ContentServiceFactory(factory);
var db = GetDb();

return new Api(
    factory,
    new AliasRepository(db),
    new ArchiveRepository(db),
    new ContentRepository(db, serviceFactory),
    new ContentGroupRepository(db),
    new ContentTypeRepository(db),
    new LanguageRepository(db),
    new MediaRepository(db),
    new PageRepository(db, serviceFactory),
    new PageTypeRepository(db),
    new ParamRepository(db),
    new PostRepository(db, serviceFactory),
    new PostTypeRepository(db),
    new SiteRepository(db, serviceFactory),
    new SiteTypeRepository(db),
    cache: _cache,           // Optional: MemoryCache or DistributedCache
    storage: _storage,       // Local.FileStorage
    processor: _processor    // ImageSharpProcessor
);
```

#### Step 3: App Initialization

```csharp
// In test InitializeAsync()
using (var api = CreateApi())
{
    // 1. Initialize the static App singleton
    Piranha.App.Init(api);
    
    // 2. Register custom test fields
    Piranha.App.Fields.Register<MyFourthField>();
    
    // 3. Build content types from code attributes
    new ContentTypeBuilder(api)
        .AddType(typeof(MyPage))
        .AddType(typeof(MyBlogPage))
        .Build();
    
    // 4. Seed test data
    var site = new Site { ... };
    await api.Sites.SaveAsync(site);
    
    var page = await MyPage.CreateAsync(api);
    await api.Pages.SaveAsync(page);
}
```

### Test Classes Organization

```
test/Piranha.Tests/
├── BaseTests.cs                    # Sync base class
├── BaseTestsAsync.cs               # Async base class (IAsyncLifetime)
├── App.cs                          # App singleton tests
├── Blocks.cs                       # Block tests
├── Fields.cs                       # Field tests
├── Serializers.cs                  # Serializer tests
├── MemCache.cs                     # Memory cache tests
├── Config.cs                       # Configuration tests
├── Permissions.cs                  # Permission tests
│
├── Services/
│   ├── PageTests.cs               # + PageTestsMemoryCache
│   │                               # + PageTestsDistributedCache
│   ├── PostTests.cs               # + PostTestsMemoryCache
│   │                               # + PostTestsDistributedCache
│   ├── SiteTests.cs               # + SiteTestsMemoryCache
│   │                               # + SiteTestsDistributedCache
│   ├── ContentTests.cs
│   ├── MediaTests.cs
│   ├── ParamTests.cs
│   ├── AliasTests.cs
│   ├── CommentTests.cs
│   ├── PageTypeTests.cs
│   ├── PostTypeTests.cs
│   └── SiteTypeTests.cs
│
├── Hooks/
│   ├── SiteHookTests.cs
│   ├── ParamHookTests.cs
│   └── AliasHookTests.cs
│
├── ImageSharp/
│   ├── ProcessorTests.cs
│   └── MediaServiceTests.cs
│
├── AttributeBuilder/
│   └── TypeBuilderTests.cs
│
└── Utils/
    ├── UI.cs
    ├── Slug.cs
    ├── FormatBytes.cs
    └── ...
```

### Test Execution Pattern (Per Test Class)

```
┌─────────────────────────────────────────────────────────────────────┐
│                   Test Execution Pattern                              │
│                                                                     │
│  [Collection("Integration tests")]                                   │
│  public class PageTests : BaseTestsAsync                            │
│  {                                                                  │
│      public readonly Guid SITE_ID = Guid.NewGuid();                 │
│      public readonly Guid PAGE_1_ID = Guid.NewGuid();               │
│                                                                     │
│      // ═══════════════════════════════════════════════════════    │
│      // PHASE 1: Initialize (IAsyncLifetime)                        │
│      // ═══════════════════════════════════════════════════════    │
│      public override async Task InitializeAsync()                  │
│      {                                                              │
│          _services = CreateServiceCollection()                      │
│              .AddSingleton<IMyService, MyService>()                 │
│              .BuildServiceProvider();                               │
│                                                                     │
│          using (var api = CreateApi())                             │
│          {                                                          │
│              Piranha.App.Init(api);                                │
│              new ContentTypeBuilder(api)                           │
│                  .AddType(typeof(MyPage))                          │
│                  .Build();                                         │
│                                                                     │
│              // Seed test data                                     │
│              await api.Sites.SaveAsync(site);                      │
│              await api.Pages.SaveAsync(page);                      │
│          }                                                          │
│      }                                                              │
│                                                                     │
│      // ═══════════════════════════════════════════════════════    │
│      // PHASE 2: Run Tests                                         │
│      // ═══════════════════════════════════════════════════════    │
│      [Fact]                                                        │
│      public async Task GetById()                                   │
│      {                                                              │
│          using (var api = CreateApi())                             │
│          {                                                          │
│              var page = await api.Pages.GetByIdAsync(PAGE_1_ID);   │
│              Assert.NotNull(page);                                 │
│          }                                                          │
│      }                                                              │
│                                                                     │
│      // ═══════════════════════════════════════════════════════    │
│      // PHASE 3: Cleanup (IAsyncLifetime)                          │
│      // ═══════════════════════════════════════════════════════    │
│      public override async Task DisposeAsync()                      │
│      {                                                              │
│          using (var api = CreateApi())                             │
│          {                                                          │
│              // Delete all test data                              │
│              foreach (var page in await api.Pages.GetAllAsync())  │
│              {                                                      │
│                  await api.Pages.DeleteAsync(page);                │
│              }                                                      │
│          }                                                          │
│      }                                                              │
│  }                                                                  │
└─────────────────────────────────────────────────────────────────────┘
```

### Cache Testing Variants

Piranha runs the **same tests with different cache implementations**:

```
┌─────────────────────────────────────────────────────────────────────┐
│              Test Collection Variants                                │
│                                                                     │
│  PageTests                      ← No cache                          │
│      │                                                                  │
│      ├──► PageTestsMemoryCache     ← IMemoryCache                  │
│      │                                                                  │
│      └──► PageTestsDistributedCache ← IDistributedCache            │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

```csharp
// MemoryCache variant
public class PageTestsMemoryCache : PageTests
{
    public override async Task InitializeAsync()
    {
        _cache = new Cache.MemoryCache(
            (IMemoryCache)_services.GetService(typeof(IMemoryCache))
        );
        await base.InitializeAsync();
    }
}

// DistributedCache variant  
public class PageTestsDistributedCache : PageTests
{
    public override async Task InitializeAsync()
    {
        _cache = new Cache.DistributedCache(
            (IDistributedCache)_services.GetService(typeof(IDistributedCache))
        );
        await base.InitializeAsync();
    }
}
```

### Database Connection

```
┌─────────────────────────────────────────────────────────────────────┐
│                    Test Database Setup                               │
│                                                                     │
│  BaseTestsAsync.CreateServiceCollection()                           │
│       │                                                             │
│       └──► .AddPiranhaRavenDb(store)                              │
│                    │                                                 │
│                    ▼                                                │
│           ┌─────────────────┐                                      │
│           │ RavenDb       │                                      │
│           │ (IDb +         │                                      │
│           │  IAsyncDocumentSession)    │                                      │
│           └─────────────────┘                                      │
│                    │                                                 │
│                    ▼                                                │
│           ┌───────────────────────┐                                │
│           │ RavenDB TestServer    │  ← DocumentStore      │
│           │                       │    (created by RavenTestDriver) │
│           └───────────────────────┘                                │
└─────────────────────────────────────────────────────────────────────┘
```

---

## Summary: How Tests Initialize Piranha

| Step | What Happens | Code Location |
|------|-------------|---------------|
| 1 | Create ServiceCollection | `BaseTestsAsync.CreateServiceCollection()` |
| 2 | Add RavenDB | `.AddRavenStores()` |
| 3 | Add Piranha services | `.AddPiranha()` |
| 4 | Add caching | `.AddMemoryCache()`, `.AddDistributedMemoryCache()` |
| 5 | Create Api manually | `CreateApi()` - constructs with all repositories |
| 6 | Initialize App | `Piranha.App.Init(api)` |
| 7 | Build content types | `ContentTypeBuilder` - scans for `[PageType]`, `[PostType]` |
| 8 | Seed test data | `api.Sites.SaveAsync()`, `api.Pages.SaveAsync()` |
| 9 | Run tests | Each `[Fact]` / `[Theory]` |
| 10 | Cleanup | `DisposeAsync()` - deletes test data |

---

## Key Files Reference

### Core Initialization Files

| File | Purpose |
|------|---------|
| `core/Piranha/App.cs` | Static singleton for app state |
| `core/Piranha/IApi.cs` | Main API interface |
| `core/Piranha/Api.cs` | API implementation |
| `data/Piranha.Data.RavenDb/IDb.cs` | Database context interface |
| `data/Piranha.Data.RavenDb/Extensions/PiranhaEFExtensions.cs` | Raven registration |
| `core/Piranha/Extensions/PiranhaStartupExtensions.cs` | DI registration |
| `core/Piranha/PiranhaServiceBuilder.cs` | Service builder |
| `examples/MvcWeb/Program.cs` | Example initialization |

### Test Files

| File | Purpose |
|------|---------|
| `test/Piranha.Tests/BaseTests.cs` | Sync test base |
| `test/Piranha.Tests/BaseTestsAsync.cs` | Async test base |
| `test/Piranha.Tests/Services/PageTests.cs` | Example test class |
| `test/Piranha.Tests/App.cs` | App initialization tests |

---

*Generated for agent orchestration workflows*
