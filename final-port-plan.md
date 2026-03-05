# Final Port Plan: RavenDB Native Optimization for Piranha CMS

## 1. Executive Summary

This document outlines the strategic plan to fully optimize Piranha CMS for a native RavenDB NoSQL document structure. 

Currently, Piranha CMS employs a multi-database architecture designed to support both Relational (SQL) and Document (NoSQL) stores gracefully. This requires a strict separation between internal **Data Entities** (e.g., `Piranha.Data.RavenDb.Data.Post`) and public **Domain Models** (e.g., `Piranha.Models.PostBase`). While flexible, this approach acts as an anti-pattern when exclusively using RavenDB, resulting in unnecessary memory allocation, duplication of class definitions, and processing overhead due to constant object mapping.

This plan details the steps required to transition to a pure, high-performance RavenDB architecture where the database interacts directly with the rich Domain Models.

---

## 2. Analysis of the Current Architecture

### The "Mirroring" Problem
At present, fetching a single piece of content follows this flow:
1. RavenDB queries the database and instantiates a lightweight, lowest-common-denominator data entity (`Piranha.Data.RavenDb.Data.Post`).
2. The Repository passes this entity to an `IContentService` (the mapper).
3. The mapper reflects/copies the fields, instantiates a complex public model (e.g., the user-defined `MyPost` inheriting from `Models.PostBase`), and returns it to the `IApi`.

### Identified Inefficiencies
- **Double Serializaton / Mapping Tax:** The system pays a CPU and memory allocation tax simply to move identical data from a raw internal entity bucket into a structured domain model.
- **Maintenance Burden (DRY Violation):** Any structural addition requires updating multiple parallel class hierarchies and instructing the mapping layer how to move the new fields.
- **Relational Artifacts in NoSQL:** Patterns like maintaining global taxonomy ID lookups (e.g., attaching globally tracked `Category` GUIDs to posts) mimic relational constraint logic. Document databases prefer structural embedding over foreign-keys.

---

## 3. The Target Architecture: Native RavenDB Flow

By fully committing to RavenDB, Piranha CMS can completely bypass the internal generic data layer. RavenDB thrives on serializing complex, polymorphic, nested aggregates.

### The New Flow
**Web Page** → **`IApi`** → **`Repositories`** → **`IAsyncDocumentSession` (RavenDB)**

When a Repository requests data:
```csharp
// The Repository directly asks RavenDB to serialize the JSON into the rich Domain Model (T)
var post = await _session.LoadAsync<T>(id); 
```
There is no intermediate data entity layer. `T` is the final business object (e.g., `MyPost`). 

---

## 4. Key Architectural Changes to Implement

### A. Model Unification
- **Action:** Delete the internal data models located in `Piranha.Data.RavenDb\Data\` (e.g., `Post.cs`, `Page.cs`, `Block.cs`, `Taxonomy.cs`).
- **Goal:** Rely exclusively on `Piranha.Models.*` classes as the absolute source of truth. RavenDB's JSON serializer will deserialize directly into these classes.

### B. Repository Refactoring & Simplification
- **Action:** Remove all dependencies on `IContentService` inside the RavenDB Repositories.
- **Action:** Rewrite read/write methods inside repositories (like `PostRepository`) to deal strictly with `<T> where T : Models.PostBase`.
- **Goal:** Repositories become drastically thinner, serving merely as query builders around `IAsyncDocumentSession`, delegating the heavy casting and mapping entirely to the database provider.

### C. Eliminate "Relational Data Seeding" Habits
- **Action:** Seed data should avoid manual ID tracking mimicking relational tables. Ensure that structural data (Categories, Tags, Blocks) are seeded purely as embedded objects/strings rather than references to externally managed keys.

---

## 5. Performance Optimizations

1. **Zero-Copy Deserialization:** Eliminating the secondary mapping layer drastically reduces garbage collection pressure on high-traffic sites by preventing the double instantiation of entities for every read request.
2. **Mitigating Eventual Consistency:**
   - **ID Lookups:** Repositories will heavily prioritize `_session.LoadAsync<T>(id)`. Direct `LoadAsync` hits the actual document store, entirely bypassing indexes and ensuring instant read-after-write accuracy.
   - **Queries:** When querying by fields (like `Slug`), repositories will maintain `.Customize(x => x.WaitForNonStaleResults())` to ensure tests and dynamic content lists accurately block until indexes finish asynchronous updates.

---

## 6. Execution Port Plan

### Phase 1: Clean Slate (Data Layer)
- Strip out the internal `Piranha.Data.RavenDb.Data` namespace.
- Update `IDb` collections (`IRavenQueryable`) to reference `Piranha.Models` interfaces/bases instead of the internal data types.

### Phase 2: Refactor Repositories
- Rewrite `PostRepository`, `PageRepository`, `SiteRepository`, etc., removing mapping logic. 
- Ensure all saving methods use `_session.StoreAsync(model)`.
- Ensure all lookups by explicit ID swap `FirstOrDefaultAsync(p => p.Id == id)` to `LoadAsync<T>(id)` for performance.

### Phase 3: Update Automated Tests & Seeding
- Validate that `AddSampleData` functions purely via rich object initialization (e.g., string assignments for taxonomies).
- Ensure existing integration tests run without `InvalidOperationExceptions` or empty asserts caused by mapping mismatches.

### Phase 4: Validation
- Verify `IApi` functions normally.
- Run the full XUnit suite. Check query duration metrics to confirm that the removal of `IContentService` mapping results in a measurable performance drop in processing milliseconds on large payloads.
