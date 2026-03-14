# Work Plan: Fix ALL RavenDB Test Failures (Complete)

## TL;DR

> **Objective**: Fix ALL 68 failing unit tests across 10 distinct failure categories
>
> **Categories**: PageBase serialization, ID collision, SaveDraft, Delete cascade, Translations, Comments, DI fields, Null refs, Session tracking, Cache keys
>
> **Approach**: Priority order (blocking tests first) → Category-by-category fixes → Verification
>
> **Estimated Effort**: Large (10+ issues, multiple files, models, repositories)
> **Execution Mode**: Sequential with dependencies
> **Critical Path**: Serialization → ID generation → Draft saving → Cleanup → Translations → Remaining

---

## Complete Test Failure Catalog

### Category 1: PageBase Deserialization (~10 tests) ⚠️ BLOCKING
**Error**: `Deserialization of interface or abstract types is not supported. Type 'Piranha.Models.PageBase'`
**Tests**: GetAllByBaseClass, DetachShouldCopyBlocks, DetachShouldCopyRegions, UpdatingCopyShouldIgnoreBodyAndDate (all cache variants)
**Root Cause**: Distributed cache serializes PageBase but System.Text.Json cannot deserialize abstract types without type discriminators
**Fix**: Add JsonDerivedType attributes to PageBase for all concrete page types

### Category 2: InvalidCastException - ID Collision (~5 tests) ⚠️ CRITICAL
**Error**: `Unable to cast object of type 'Site' to type 'Page'` / `Page to Post`
**Tests**: GetDIGeneric, GetDIDynamic (Page & Post variants)
**Root Cause**: Snowflake ID generation creating duplicate IDs across different entity types
**Fix**: Ensure Snowflake IDs include entity type prefix or use type-specific sequences

### Category 3: SaveDraft Bug (~3 tests) ⚠️ ALREADY IDENTIFIED
**Error**: `Assert.NotEqual() Failure: Strings are equal - Expected: Not "My working copy"`
**Tests**: PageTests.SaveDraft, PageTestsMemoryCache.SaveDraft, PageTestsDistributedCache.SaveDraft
**Root Cause**: PageRepository.Save(isDraft=true) modifies page object in-memory
**Fix**: Clone page before modifying when saving draft

### Category 4: "Can Not Delete Page" Cleanup (~8 tests) ⚠️ BLOCKING
**Error**: `InvalidOperationException: Can not delete page because it has copies`
**Tests**: GetStartpage, GetGenericBySlug, GetBaseClassBySlug, CanNotUpdateCopyWithAnotherTypeIdOtherThanOriginalPageTypeId
**Root Cause**: Test cleanup tries to delete pages with orphaned copies (OriginalPageId references)
**Fix**: Cascade delete copies, or fix test cleanup to delete copies first

### Category 5: Content Translation Failures (~9 tests)
**Error**: Translation queries return wrong data, null values, false assertions
**Tests**: GetById (string mismatch), GetTranslatedStatus (null), GetUntranslatedStatus (null), GetTranslationSummary (false) - all cache variants
**Root Cause**: Translation logic broken with embedded translations in Content document
**Fix**: Check ContentRepository translation queries and embedded model structure

### Category 6: Comment Count Failures (~3 tests)
**Error**: `Assert.Equal() Failure: Expected: 1, Actual: 0`
**Tests**: AddPostComment, AddPageComment (normal & distributed cache)
**Root Cause**: Comments not being saved/retrieved correctly with embedded model
**Fix**: Check comment saving and how comments are embedded in Page/Post

### Category 7: DI Field Injection Mismatches (~6 tests)
**Error**: `Assert.Equal() Failure: Strings differ` (position 2)
**Tests**: GetDIDynamic, CreateDIGeneric, CreateDIDynamic (Page & Post variants)
**Root Cause**: DI-injected fields have wrong values after serialization/deserialization
**Fix**: Investigate DI field injection and serialization in transformed models

### Category 8: NullReferenceExceptions (~2 tests)
**Error**: `System.NullReferenceException`
**Tests**: AliasTestsMemoryCache.IsCached, FixRedirectUrl
**Root Cause**: Alias object or related data is null when expected
**Fix**: Check alias initialization and caching logic

### Category 9: NonUniqueObjectException (~1 test)
**Error**: `Attempted to associate a different object with id`
**Tests**: AliasTests.GetById
**Root Cause**: RavenDB session tracking conflict
**Fix**: Check session management and object tracking in aliases

### Category 10: ArgumentNullException (~2 tests)
**Error**: `Value cannot be null. (Parameter 'key')`
**Tests**: MediaTestsMemoryCache.DeleteById, MediaTestsDistributedCache.DeleteById
**Root Cause**: Cache key is null when deleting from cache
**Fix**: Check media cache key generation

---

## Execution Strategy

**Wave 1: Critical Blockers** (Categories 1, 2, 4)
- PageBase serialization (unblocks ~10 tests)
- ID collision (critical data integrity)
- Delete cascade (unblocks ~8 tests)

**Wave 2: Core Functionality** (Categories 3, 5, 6)
- SaveDraft (already analyzed)
- Translations (embedded model)
- Comments (embedded model)

**Wave 3: Edge Cases** (Categories 7, 8, 9, 10)
- DI field injection
- Null references
- Session tracking
- Cache keys

**Final Wave: Verification**
- Run full test suite
- Verify 0 logic failures remain
- Document any remaining issues

---

## TODOs

- [x] 1. Fix PageBase Deserialization (Category 1)

  **What to do**:
  - Add JsonDerivedType attributes to `core/Piranha/Models/PageBase.cs` for all concrete page types
  - Similar to how Block.cs was fixed with JsonDerivedType
  - Find all concrete page types: `grep -r "class.*:.*PageBase" core/Piranha/`
  - Add attributes: `[JsonDerivedType(typeof(MyPage), "MyPage")]` for each type
  - This allows System.Text.Json to deserialize abstract PageBase in distributed cache

  **Files to modify**:
  - `core/Piranha/Models/PageBase.cs`

  **Parallelization**:
  - **Can Run In Parallel**: NO (foundational fix)
  - **Blocks**: Categories 5, 7 (serialization-dependent)
  - **Blocked By**: None

  **References**:
  - `core/Piranha/Extend/Block.cs` - Example of JsonDerivedType fix
  - `core/Piranha/Models/PageBase.cs` - Abstract class to fix
  - `core/Piranha/Cache/Internal/DistributedCache.cs` - Where serialization happens

  **Acceptance Criteria**:
  - [ ] PageBase has JsonDerivedType for all concrete page types
  - [ ] `dotnet test --filter "FullyQualifiedName~GetAllByBaseClass"` → PASS
  - [ ] `dotnet test --filter "FullyQualifiedName~DetachShouldCopyBlocks"` → PASS
  - [ ] Distributed cache can serialize/deserialize PageBase

  **Commit**: YES
  - Message: `fix(page): add JsonDerivedType to PageBase for distributed cache serialization`
  - Files: `core/Piranha/Models/PageBase.cs`

---

- [x] 2. Fix Snowflake ID Collision (Category 2)

  **What to do**:
  - Check `core/Piranha/Utils.cs` Snowflake ID generator
  - Ensure IDs are unique across ALL entity types, not just within a type
  - Options:
    1. Add entity type prefix to IDs (e.g., "page-...", "post-...", "site-...")
    2. Use separate Snowflake sequences per entity type
    3. Include type discriminator in ID generation
  - Recommended: Add type prefix for clarity and guaranteed uniqueness

  **Files to modify**:
  - `core/Piranha/Utils.cs` (Snowflake.NewId())
  - OR `data/Piranha.Data.RavenDb/Data/*.cs` (entity ID generation)

  **Parallelization**:
  - **Can Run In Parallel**: NO (data integrity critical)
  - **Blocks**: None directly, but affects all entity creation
  - **Blocked By**: None

  **References**:
  - `core/Piranha/Utils.cs` - Snowflake class
  - `data/Piranha.Data.RavenDb/Data/Page.cs` - Page entity
  - `data/Piranha.Data.RavenDb/Data/Post.cs` - Post entity
  - `data/Piranha.Data.RavenDb/Data/Site.cs` - Site entity

  **Acceptance Criteria**:
  - [ ] IDs are unique across all entity types
  - [ ] `dotnet test --filter "FullyQualifiedName~GetDIGeneric"` → PASS (Page & Post)
  - [ ] No InvalidCastException in any test
  - [ ] Can load entity by ID without type mismatch

  **Commit**: YES
  - Message: `fix(ids): ensure unique Snowflake IDs across entity types`
  - Files: `core/Piranha/Utils.cs` or entity files

---

- [ ] 3. Fix Page Delete Cascade (Category 4)

  **What to do**:
  - Modify `data/Piranha.Data.RavenDb/Repositories/PageRepository.cs` Delete method (line 408)
  - When deleting a page, first delete all copies (pages where OriginalPageId == page.Id)
  - Or provide cascade delete option
  - Code:
    ```csharp
    // Delete copies first
    var copies = await _db.Pages.Where(p => p.OriginalPageId == id).ToListAsync();
    foreach (var copy in copies) {
        _db.session.Delete(copy.Id);
    }
    // Then delete original
    _db.session.Delete(id);
    ```

  **Files to modify**:
  - `data/Piranha.Data.RavenDb/Repositories/PageRepository.cs`

  **Parallelization**:
  - **Can Run In Parallel**: NO (after Task 1)
  - **Blocks**: ~8 tests blocked by cleanup failures
  - **Blocked By**: None (but Task 1 helps with verification)

  **References**:
  - `data/Piranha.Data.RavenDb/Repositories/PageRepository.cs:400-446` - Delete method
  - `test/Piranha.Tests/Services/PageTests.cs` - Tests failing in cleanup

  **Acceptance Criteria**:
  - [ ] Deleting a page with copies succeeds
  - [ ] All copies deleted when original is deleted
  - [ ] `dotnet test --filter "FullyQualifiedName~GetGenericBySlug"` → PASS
  - [ ] `dotnet test --filter "FullyQualifiedName~GetStartpage"` → PASS
  - [ ] Test cleanup no longer throws "Can not delete page because it has copies"

  **Commit**: YES
  - Message: `fix(page): cascade delete copies when deleting original page`
  - Files: `data/Piranha.Data.RavenDb/Repositories/PageRepository.cs`

---

- [x] 4. Fix SaveDraft In-Memory Modification (Category 3)

  **What to do**:
  - Fix `data/Piranha.Data.RavenDb/Repositories/PageRepository.cs` Save method (line 563)
  - When `isDraft=true`, clone page object before modifying:
    ```csharp
    if (isDraft) {
        var draftPage = JsonSerializer.Deserialize<Page>(JsonSerializer.Serialize(page));
        // Modify draftPage instead of page
        // Serialize draftPage to revision
    }
    ```
  - This prevents in-memory page object from being polluted

  **Files to modify**:
  - `data/Piranha.Data.RavenDb/Repositories/PageRepository.cs`

  **Parallelization**:
  - **Can Run In Parallel**: YES (with Tasks 5-10)
  - **Blocks**: None
  - **Blocked By**: None

  **References**:
  - `data/Piranha.Data.RavenDb/Repositories/PageRepository.cs:563-708` - Save method
  - `test/Piranha.Tests/Services/PageTests.cs:740-759` - SaveDraft test

  **Acceptance Criteria**:
  - [ ] `dotnet test --filter "FullyQualifiedName~SaveDraft"` → PASS (all variants)
  - [ ] Main page title unchanged after SaveDraftAsync
  - [ ] Draft revision has modified title
  - [ ] GetByIdAsync returns original page

  **Commit**: YES
  - Message: `fix(page): clone page when saving draft to avoid in-memory modification`
  - Files: `data/Piranha.Data.RavenDb/Repositories/PageRepository.cs`

---

- [ ] 5. Fix PostRepository SaveDraft (Mirror Task 4)

  **What to do**:
  - Apply same fix as Task 4 to PostRepository
  - Check if PostRepository.Save has same issue
  - Clone post before modifying when isDraft=true

  **Files to modify**:
  - `data/Piranha.Data.RavenDb/Repositories/PostRepository.cs`

  **Parallelization**:
  - **Can Run In Parallel**: YES (with Task 4)
  - **Blocks**: None
  - **Blocked By**: None

  **Acceptance Criteria**:
  - [ ] Post SaveDraft works correctly (if test exists)
  - [ ] Same pattern as Page SaveDraft

  **Commit**: YES
  - Message: `fix(post): clone post when saving draft`
  - Files: `data/Piranha.Data.RavenDb/Repositories/PostRepository.cs`

---

- [ ] 6. Fix Content Translation Queries (Category 5)

  **What to do**:
  - Check how translations are embedded in Content document
  - Fix `data/Piranha.Data.RavenDb/Repositories/ContentRepository.cs` translation queries
  - May need to denormalize fields or fix query patterns
  - Tests show GetById returns translated content instead of original

  **Files to modify**:
  - `data/Piranha.Data.RavenDb/Data/Content.cs` (if model changes needed)
  - `data/Piranha.Data.RavenDb/Repositories/ContentRepository.cs`

  **Parallelization**:
  - **Can Run In Parallel**: YES (with Tasks 4, 5, 7-10)
  - **Blocks**: None
  - **Blocked By**: Task 1 (PageBase serialization)

  **References**:
  - `test/Piranha.Tests/Services/ContentTests.cs` - Failing tests
  - `data/Piranha.Data.RavenDb/Data/Content.cs` - Translation embedding
  - `data/Piranha.Data.RavenDb/Repositories/ContentRepository.cs` - Queries

  **Acceptance Criteria**:
  - [ ] `dotnet test --filter "FullyQualifiedName~ContentTests"` → All PASS
  - [ ] GetById returns correct language content
  - [ ] GetTranslatedStatus returns valid data
  - [ ] GetUntranslatedStatus returns valid data
  - [ ] GetTranslationSummary correct

  **Commit**: YES
  - Message: `fix(content): fix translation queries for embedded model`
  - Files: `data/Piranha.Data.RavenDb/Repositories/ContentRepository.cs`, possibly Content.cs

---

- [ ] 7. Fix Comment Count Issues (Category 6)

  **What to do**:
  - Check how comments are stored in Page/Post documents
  - Verify comment saving increments count correctly
  - May need to fix embedded comment list or count tracking

  **Files to modify**:
  - `data/Piranha.Data.RavenDb/Repositories/PageRepository.cs` (SaveComment)
  - `data/Piranha.Data.RavenDb/Repositories/PostRepository.cs` (SaveComment)
  - Possibly `data/Piranha.Data.RavenDb/Data/Page.cs` or `Post.cs`

  **Parallelization**:
  - **Can Run In Parallel**: YES
  - **Blocks**: None
  - **Blocked By**: None

  **References**:
  - `test/Piranha.Tests/Services/CommentTests.cs` - Failing tests
  - `data/Piranha.Data.RavenDb/Repositories/PageRepository.cs:306-331` - SaveComment

  **Acceptance Criteria**:
  - [ ] `dotnet test --filter "FullyQualifiedName~CommentTests"` → All PASS
  - [ ] AddPostComment count = 1
  - [ ] AddPageComment count = 1

  **Commit**: YES
  - Message: `fix(comments): ensure comment count correct with embedded model`
  - Files: Relevant repository files

---

- [ ] 8. Fix DI Field Injection Serialization (Category 7)

  **What to do**:
  - Investigate DI field injection in transformed models
  - Check if DI-injected fields are being serialized/deserialized correctly
  - May need to mark DI fields as [JsonIgnore] or handle separately
  - String mismatches suggest serialization issue

  **Files to modify**:
  - Possibly `core/Piranha/Services/Internal/ContentService.cs` (TransformAsync)
  - Possibly model transformation logic

  **Parallelization**:
  - **Can Run In Parallel**: YES
  - **Blocks**: None
  - **Blocked By**: Task 1 (serialization)

  **References**:
  - `test/Piranha.Tests/Services/PageTests.cs` - DI-related tests
  - `core/Piranha/Services/Internal/ContentService.cs` - Transformation

  **Acceptance Criteria**:
  - [ ] `dotnet test --filter "FullyQualifiedName~GetDIGeneric"` → PASS
  - [ ] `dotnet test --filter "FullyQualifiedName~CreateDIGeneric"` → PASS
  - [ ] DI fields have correct values after transformation

  **Commit**: YES
  - Message: `fix(di): ensure DI-injected fields serialize correctly`
  - Files: To be determined during investigation

---

- [ ] 9. Fix Alias NullReferenceExceptions (Category 8)

  **What to do**:
  - Debug `AliasTestsMemoryCache.IsCached` and `FixRedirectUrl`
  - Find what object is null
  - Fix alias initialization or caching logic

  **Files to modify**:
  - Likely `data/Piranha.Data.RavenDb/Repositories/AliasRepository.cs`
  - Possibly `test/Piranha.Tests/Seed.cs` (test initialization)

  **Parallelization**:
  - **Can Run In Parallel**: YES
  - **Blocks**: None
  - **Blocked By**: None

  **References**:
  - `test/Piranha.Tests/Services/AliasTests.cs` - Failing tests
  - `data/Piranha.Data.RavenDb/Repositories/AliasRepository.cs`

  **Acceptance Criteria**:
  - [ ] `dotnet test --filter "FullyQualifiedName~AliasTests"` → All PASS
  - [ ] No NullReferenceException
  - [ ] Alias objects properly initialized

  **Commit**: YES
  - Message: `fix(alias): fix null reference in alias tests`
  - Files: Relevant files

---

- [ ] 10. Fix Alias NonUniqueObjectException (Category 9)

  **What to do**:
  - Fix RavenDB session tracking issue in AliasTests.GetById
  - Ensure aliases are tracked correctly in session
  - May need to use NoTracking() or clear session between operations

  **Files to modify**:
  - `data/Piranha.Data.RavenDb/Repositories/AliasRepository.cs`

  **Parallelization**:
  - **Can Run In Parallel**: YES (with Task 9)
  - **Blocks**: None
  - **Blocked By**: None

  **Acceptance Criteria**:
  - [ ] `dotnet test --filter "FullyQualifiedName~AliasTests.GetById"` → PASS
  - [ ] No NonUniqueObjectException

  **Commit**: YES
  - Message: `fix(alias): fix session tracking issue`
  - Files: `data/Piranha.Data.RavenDb/Repositories/AliasRepository.cs`

---

- [ ] 11. Fix Media Cache Key Issue (Category 10)

  **What to do**:
  - Fix ArgumentNullException in MediaTests.DeleteById
  - Check cache key generation for media
  - Ensure key is not null when deleting from cache

  **Files to modify**:
  - `core/Piranha/Services/Internal/MediaService.cs` (cache deletion)
  - Or `data/Piranha.Data.RavenDb/Repositories/MediaRepository.cs`

  **Parallelization**:
  - **Can Run In Parallel**: YES
  - **Blocks**: None
  - **Blocked By**: None

  **Acceptance Criteria**:
  - [ ] `dotnet test --filter "FullyQualifiedName~MediaTests"` → All PASS
  - [ ] No ArgumentNullException
  - [ ] Media cache key valid

  **Commit**: YES
  - Message: `fix(media): fix null cache key in delete`
  - Files: Relevant files

---

- [ ] 12. Final Verification - Run Full Test Suite

  **What to do**:
  - Run: `dotnet test test/Piranha.Tests/Piranha.Tests.csproj --no-build`
  - Verify 0 logic failures (assertions, null refs, invalid casts)
  - Accept only RavenDB timeout infrastructure failures
  - Document any remaining issues

  **Parallelization**:
  - **Can Run In Parallel**: NO (final verification)
  - **Blocks**: None (final task)
  - **Blocked By**: All previous tasks

  **Acceptance Criteria**:
  - [ ] Full test suite executed
  - [ ] 0 logic failures
  - [ ] All failures are RavenDB infrastructure (timeouts, IO)
  - [ ] All individually-runnable tests pass

  **Commit**: NO (verification task)

---

## Success Criteria

### Verification Commands
```bash
# Test each category
dotnet test --filter "FullyQualifiedName~PageTests.SaveDraft" --no-build
dotnet test --filter "FullyQualifiedName~GetAllByBaseClass" --no-build
dotnet test --filter "FullyQualifiedName~GetDIGeneric" --no-build
dotnet test --filter "FullyQualifiedName~ContentTests" --no-build
dotnet test --filter "FullyQualifiedName~CommentTests" --no-build
dotnet test --filter "FullyQualifiedName~AliasTests" --no-build

# Final full run
dotnet test test/Piranha.Tests/Piranha.Tests.csproj --no-build
```

### Final Checklist
- [ ] Category 1: PageBase serialization fixed (10 tests)
- [ ] Category 2: ID collision fixed (5 tests)
- [ ] Category 3: SaveDraft fixed (3 tests)
- [ ] Category 4: Delete cascade fixed (8 tests)
- [ ] Category 5: Translations fixed (9 tests)
- [ ] Category 6: Comments fixed (3 tests)
- [ ] Category 7: DI fields fixed (6 tests)
- [ ] Category 8: Null refs fixed (2 tests)
- [ ] Category 9: Session tracking fixed (1 test)
- [ ] Category 10: Cache keys fixed (2 tests)
- [ ] 0 logic failures in full test suite
- [ ] All code changes align with NoSQL denormalized model
- [ ] No test logic modified (per AGENTS.md)
