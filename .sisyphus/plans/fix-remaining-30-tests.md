# Work Plan: Fix Remaining 30 RavenDB Test Failures

## TL;DR

> **Objective**: Fix ALL remaining 30 failing unit tests in 4 distinct categories
>
> **Progress**: 68 tests → 30 tests remaining (38 already fixed)
>
> **Categories**: Content Translation (15), Post BySlug (8), Page Copy (2), PostType Delete (1)
>
> **Approach**: Fix root cause per category → Verify all tests in category pass → Move to next category
>
> **Estimated Effort**: Medium (4 distinct issues, well-scoped)
> **Execution Mode**: Sequential by category (parallel within category fixes)
> **Critical Path**: Category A → Category B → Category C → Category D

---

## Context

### Original Request
Fix remaining 30 failing unit tests after porting Piranha CMS from Entity Framework Core (SQL) to RavenDB (NoSQL). Tests fail due to RavenDB-specific query patterns, document modeling differences, and translation logic.

### Progress Summary
**Completed Fixes (38 tests)**:
1. ✅ PageBase Deserialization - Added JsonDerivedType attributes
2. ✅ Snowflake ID Collision - Static singleton with thread-safe locking
3. ✅ SaveDraft Bug - Clone page/model before modifying
4. ✅ Page Delete Cascade - Delete copies before original
5. ✅ Media Cache Key - Null check in DeleteAsync
6. ✅ DI Field Injection - Corrected IMyService.Value return value
7. ✅ Comment Count - WaitForNonStaleResults in queries
8. ✅ Test Isolation - Individual test runs to avoid timeouts

### Metis Review
**Identified Gaps** (addressed in plan):
- Each category requires understanding specific RavenDB query patterns
- Need to verify test expectations match NoSQL document model
- Ensure fixes don't break adjacent functionality

---

## Work Objectives

### Core Objective
Fix all 30 remaining failing tests across 4 categories by correcting RavenDB repository implementations and document model logic.

### Concrete Deliverables
- Fixed ContentRepository translation queries (15 tests)
- Fixed PostRepository GetBySlug methods (8 tests)
- Fixed PageRepository copy/detach logic (2 tests)
- Fixed PostTypeRepository delete handling (1 test)
- All 30 tests passing individually

### Definition of Done
- [ ] All 30 tests pass when run individually
- [ ] No test logic modifications (per AGENTS.md constraint)
- [ ] All fixes align with RavenDB denormalized document model
- [ ] No regression in already-passing tests

### Must Have
- Fix root cause in each category
- Preserve existing test expectations
- Follow RavenDB idioms (WaitForNonStaleResults where needed)
- Proper document cloning for drafts/copies

### Must NOT Have (Guardrails)
- DO NOT modify unit test files
- DO NOT change test expectations or assertions
- DO NOT add workarounds that mask real issues
- DO NOT break already-passing tests

---

## Verification Strategy (MANDATORY)

> **ZERO HUMAN INTERVENTION** — ALL verification is agent-executed. No exceptions.

### Test Decision
- **Infrastructure exists**: YES (xUnit)
- **Automated tests**: Tests-after (fix implementation, then run tests)
- **Framework**: dotnet test
- **Strategy**: Individual test runs to avoid RavenDB timeout noise

### QA Policy
Every task MUST include agent-executed QA scenarios.
Evidence saved to `.sisyphus/evidence/task-{N}-{scenario-slug}.{ext}`.

- **Repository/Service**: Use Bash (dotnet test) — Run specific test filters, verify pass/fail
- **Each category**: Run all tests in category individually, capture results
- **Final verification**: Run all 30 tests individually, confirm 30/30 pass

---

## Execution Strategy

### Parallel Execution Waves

```
Wave 1 (Category A - Content Translation):
├── Task 1: Fix ContentRepository.GetById translation logic [deep]
├── Task 2: Fix translation status queries [deep]
└── Task 3: Verify all 15 translation tests pass [unspecified-high]

Wave 2 (Category B - Post BySlug Queries):
├── Task 4: Fix PostRepository.GetBySlug methods [deep]
└── Task 5: Verify all 8 Post BySlug tests pass [unspecified-high]

Wave 3 (Category C - Page Copy Issues):
├── Task 6: Fix PageRepository.Detach copy logic [deep]
└── Task 7: Verify both Page Copy tests pass [quick]

Wave 4 (Category D - PostType Delete):
├── Task 8: Fix PostTypeRepository.Delete handling [quick]
└── Task 9: Verify PostType Delete test passes [quick]

Wave FINAL (After ALL fixes — independent review):
├── Task F1: Run all 30 tests individually [unspecified-high]
├── Task F2: Verify no regression in previously passing tests [unspecified-high]
├── Task F3: Code quality review [unspecified-high]
└── Task F4: RavenDB idiom compliance check [deep]

Critical Path: Task 1 → Task 3 → Task 4 → Task 5 → Task 6 → Task 7 → Task 8 → Task 9 → F1-F4
Parallel Speedup: Limited (categories are independent but sequential for safety)
Max Concurrent: 3 (within Wave 1)
```

### Dependency Matrix

- **1-3**: — — 4-9, F1-F4 (Category A complete before B)
- **4-5**: — — 6-9, F1-F4 (Category B complete before C)
- **6-7**: — — 8-9, F1-F4 (Category C complete before D)
- **8-9**: — — F1-F4 (Category D complete before final)
- **F1-F4**: All 1-9 — — (Final after all categories)

### Agent Dispatch Summary

- **Wave 1**: **3** — T1 → `deep`, T2 → `deep`, T3 → `unspecified-high`
- **Wave 2**: **2** — T4 → `deep`, T5 → `unspecified-high`
- **Wave 3**: **2** — T6 → `deep`, T7 → `quick`
- **Wave 4**: **2** — T8 → `quick`, T9 → `quick`
- **FINAL**: **4** — F1 → `unspecified-high`, F2 → `unspecified-high`, F3 → `unspecified-high`, F4 → `deep`

---

## TODOs

> Implementation + Test = ONE Task per category
> EVERY task MUST have: Recommended Agent Profile + Parallelization info + QA Scenarios.
- [ ] 1. Fix ContentRepository.GetById Translation Logic (Category A)

  **What to do**:
  - Investigate `ContentRepository.GetById` - currently returns translated content instead of original
  - Test expects: GetById("en") returns "My first content" (original), not "Mitt första innehåll" (translation)
  - Check how translations are stored in Content document
  - Fix GetById to return original content by default, translations only when explicitly requested
  - Verify translation status queries return correct IsUpToDate values

  **Root Cause**: GetById is returning the translated version instead of the original content

  **Must NOT do**:
  - DO NOT modify test expectations
  - DO NOT change how translations are stored (keep embedded model)
  - DO NOT add workarounds that break translation queries

  **Recommended Agent Profile**:
  - **Category**: `deep`
    - Reason: Requires understanding translation logic, document model, and query patterns
  - **Skills**: []
    - No special skills needed - code investigation and logic fix

  **Parallelization**:
  - **Can Run In Parallel**: NO (foundational fix for Category A)
  - **Parallel Group**: Wave 1 (Task 1)
  - **Blocks**: Task 2, Task 3
  - **Blocked By**: None

  **References**:
  - `data/Piranha.Data.RavenDb/Repositories/ContentRepository.cs:GetById` - Method to fix
  - `data/Piranha.Data.RavenDb/Data/Content.cs:SetTranslation` - How translations are stored
  - `test/Piranha.Tests/Services/ContentTests.cs:136` - GetById test expectation (READ ONLY)
  - `test/Piranha.Tests/Services/ContentTests.cs:162` - GetTranslatedStatus test (READ ONLY)

  **Acceptance Criteria**:
  - [ ] GetById returns original content, not translated version
  - [ ] Translation status queries return correct IsUpToDate values
  - [ ] `dotnet test --filter "FullyQualifiedName~ContentTests.GetById"` → PASS

  **QA Scenarios**:

  ```
  Scenario: GetById returns original content
    Tool: Bash (dotnet test)
    Preconditions: Test database with content having translations
    Steps:
      1. Run: dotnet test test/Piranha.Tests/Piranha.Tests.csproj --filter "FullyQualifiedName~ContentTests.GetById" --no-build
      2. Verify test passes (not fails with string mismatch)
    Expected Result: Test passes, no assertion failure
    Failure Indicators: "Expected: My first content, Actual: Mitt första innehåll"
    Evidence: .sisyphus/evidence/task-1-getbyid-original.txt

  Scenario: Translation status is correct
    Tool: Bash (dotnet test)
    Preconditions: Content with translations in different states
    Steps:
      1. Run: dotnet test test/Piranha.Tests/Piranha.Tests.csproj --filter "FullyQualifiedName~ContentTests.GetTranslatedStatus" --no-build
      2. Verify UpToDateCount is 1, not 0
    Expected Result: Test passes, UpToDateCount = 1
    Failure Indicators: "Expected: 1, Actual: 0"
    Evidence: .sisyphus/evidence/task-1-translation-status.txt
  ```

  **Commit**: NO (groups with Task 2, 3)

---

- [ ] 2. Fix Translation Status Query Logic (Category A)

  **What to do**:
  - Fix translation status queries in ContentRepository
  - Ensure IsUpToDate comparison logic is correct: `translation.LastModified >= contentLastModified`
  - Fix GetTranslationSummary to return correct IsUpToDate boolean
  - Fix GetUntranslatedStatus to identify untranslated content correctly
  - Add WaitForNonStaleResults() if needed for fresh index data

  **Root Cause**: Translation status logic returns wrong values (True instead of False, 0 instead of 1)

  **Must NOT do**:
  - DO NOT change test assertions
  - DO NOT modify how translations are embedded in Content

  **Recommended Agent Profile**:
  - **Category**: `deep`
    - Reason: Complex query logic involving dates and status calculation
  - **Skills**: []

  **Parallelization**:
  - **Can Run In Parallel**: NO (depends on Task 1)
  - **Parallel Group**: Wave 1 (Task 2)
  - **Blocks**: Task 3
  - **Blocked By**: Task 1

  **References**:
  - `data/Piranha.Data.RavenDb/Repositories/ContentRepository.cs:558` - Translation status check
  - `test/Piranha.Tests/Services/ContentTests.cs:175` - GetTranslationSummary test (READ ONLY)
  - `test/Piranha.Tests/Services/ContentTests.cs:188` - GetUntranslatedStatus test (READ ONLY)

  **Acceptance Criteria**:
  - [ ] GetTranslationSummary returns IsUpToDate = False when expected
  - [ ] GetUntranslatedStatus returns IsUpToDate = False when expected
  - [ ] `dotnet test --filter "FullyQualifiedName~ContentTests.GetTranslationSummary"` → PASS
  - [ ] `dotnet test --filter "FullyQualifiedName~ContentTests.GetUntranslatedStatus"` → PASS

  **QA Scenarios**:

  ```
  Scenario: Translation summary correct
    Tool: Bash (dotnet test)
    Steps:
      1. Run: dotnet test test/Piranha.Tests/Piranha.Tests.csproj --filter "FullyQualifiedName~ContentTests.GetTranslationSummary" --no-build
      2. Verify IsUpToDate is False, not True
    Expected Result: Test passes, IsUpToDate = False
    Evidence: .sisyphus/evidence/task-2-translation-summary.txt

  Scenario: Untranslated status correct
    Tool: Bash (dotnet test)
    Steps:
      1. Run: dotnet test test/Piranha.Tests/Piranha.Tests.csproj --filter "FullyQualifiedName~ContentTests.GetUntranslatedStatus" --no-build
      2. Verify IsUpToDate is False, not True
    Expected Result: Test passes, IsUpToDate = False
    Evidence: .sisyphus/evidence/task-2-untranslated-status.txt
  ```

  **Commit**: NO (groups with Task 1, 3)

---

- [ ] 3. Verify All Category A Translation Tests Pass (15 tests)

  **What to do**:
  - Run all 15 translation-related tests individually
  - Verify each test passes
  - Capture evidence for each test
  - List: GetById, GetTranslatedStatus, GetTranslationSummary, GetUntranslatedStatus (all cache variants)

  **Tests to verify**:
  1. ContentTests.GetById
  2. ContentTests.GetTranslatedStatus
  3. ContentTests.GetTranslationSummary
  4. ContentTests.GetUntranslatedStatus
  5-8. ContentTestsMemoryCache variants of 1-4
  9-12. ContentTestsDistributedCache variants of 1-4
  13-15. Additional translation tests

  **Recommended Agent Profile**:
  - **Category**: `unspecified-high`
    - Reason: Verification task requiring careful test execution
  - **Skills**: []

  **Parallelization**:
  - **Can Run In Parallel**: NO (verification after fixes)
  - **Parallel Group**: Wave 1 (Task 3)
  - **Blocks**: Wave 2 (Category B)
  - **Blocked By**: Task 1, Task 2

  **Acceptance Criteria**:
  - [ ] All 15 Category A tests pass individually
  - [ ] Evidence captured for each test
  - [ ] No assertion failures or null reference errors

  **QA Scenarios**:

  ```
  Scenario: All 15 translation tests pass
    Tool: Bash (dotnet test)
    Steps:
      1. Run each of the 15 tests individually
      2. Capture pass/fail status for each
      3. Verify 15/15 pass
    Expected Result: 15/15 tests pass
    Evidence: .sisyphus/evidence/task-3-all-translation-tests.txt
  ```

  **Commit**: YES
  - Message: `fix(content): correct translation queries and GetById logic`
  - Files: ContentRepository.cs, possibly Content.cs

---

## Final Verification Wave (MANDATORY — after ALL implementation tasks)

- [ ] F1. **Run All 30 Tests Individually** — `unspecified-high`
  Execute each of the 30 failing tests individually using dotnet test with specific filters. Capture pass/fail status for each. Save results to `.sisyphus/evidence/final-all-30-tests.txt`.
  Output: `Tests [30/30 pass] | VERDICT: APPROVE/REJECT`

- [ ] F2. **No Regression Check** — `unspecified-high`
  Run previously passing tests to ensure no new failures introduced. Focus on tests related to modified repositories (Content, Post, Page, PostType).
  Output: `Regression Tests [N/N pass] | VERDICT`

- [ ] F3. **Code Quality Review** — `unspecified-high`
  Run `dotnet build` and review modified files for: RavenDB idioms, proper async/await, null checks, correct use of WaitForNonStaleResults. Check for AI slop patterns.
  Output: `Build [PASS/FAIL] | Code Quality [N issues] | VERDICT`

- [ ] F4. **RavenDB Idiom Compliance** — `deep`
  Verify all fixes follow RavenDB best practices: proper document structure, correct query patterns, appropriate use of indexes, correct session management.
  Output: `RavenDB Idioms [N/N compliant] | VERDICT`

---
- [ ] 4. Fix PostRepository.GetBySlug Methods (Category B)

  **What to do**:
  - Investigate why all GetBySlug methods return null in PostRepository
  - Methods: GetBySlug<T>(blogId, slug), GetBySlug<T>(slug), GetBySlug(blogId, slug)
  - Check slug query implementation in RavenDB
  - Ensure slug is indexed or query uses WaitForNonStaleResults
  - Verify test data setup creates posts with correct slugs

  **Root Cause**: GetBySlug queries return null - likely missing RavenDB query implementation or index issue

  **Must NOT do**:
  - DO NOT modify test data setup
  - DO NOT change test expectations

  **Recommended Agent Profile**:
  - **Category**: `deep`
    - Reason: Requires understanding RavenDB query patterns and indexing
  - **Skills**: []

  **Parallelization**:
  - **Can Run In Parallel**: NO (Category B foundation)
  - **Parallel Group**: Wave 2 (Task 4)
  - **Blocks**: Task 5
  - **Blocked By**: Task 3 (Category A complete)

  **References**:
  - `data/Piranha.Data.RavenDb/Repositories/PostRepository.cs` - GetBySlug methods
  - `test/Piranha.Tests/Services/PostTests.cs:467` - GetGenericBySlug test (READ ONLY)
  - `test/Piranha.Tests/Services/PostTests.cs:479` - GetBaseClassBySlug test (READ ONLY)
  - `test/Piranha.Tests/Services/PostTests.cs:501` - GetInfoBySlug test (READ ONLY)
  - `test/Piranha.Tests/Services/PostTests.cs:525` - GetDynamicBySlug test (READ ONLY)

  **Acceptance Criteria**:
  - [ ] GetBySlug<T>(blogId, slug) returns correct post
  - [ ] GetBySlug<T>(slug) returns correct post
  - [ ] All GetBySlug variants work correctly
  - [ ] `dotnet test --filter "FullyQualifiedName~PostTests.GetGenericBySlug"` → PASS

  **QA Scenarios**:

  ```
  Scenario: GetBySlug returns post
    Tool: Bash (dotnet test)
    Steps:
      1. Run: dotnet test test/Piranha.Tests/Piranha.Tests.csproj --filter "FullyQualifiedName~PostTests.GetGenericBySlug" --no-build
      2. Verify post is not null
    Expected Result: Test passes, post returned
    Failure Indicators: "Expected: Not null, Actual: null"
    Evidence: .sisyphus/evidence/task-4-getbyslug-generic.txt

  Scenario: GetBySlug works for all variants
    Tool: Bash (dotnet test)
    Steps:
      1. Run: dotnet test test/Piranha.Tests/Piranha.Tests.csproj --filter "FullyQualifiedName~PostTests.GetBaseClassBySlug" --no-build
      2. Run: dotnet test test/Piranha.Tests/Piranha.Tests.csproj --filter "FullyQualifiedName~PostTests.GetInfoBySlug" --no-build
      3. Run: dotnet test test/Piranha.Tests/Piranha.Tests.csproj --filter "FullyQualifiedName~PostTests.GetDynamicBySlug" --no-build
      4. Verify all return posts, not null
    Expected Result: All 3 tests pass
    Evidence: .sisyphus/evidence/task-4-getbyslug-variants.txt
  ```

  **Commit**: NO (groups with Task 5)

---

- [ ] 5. Verify All Category B Post BySlug Tests Pass (8 tests)

  **What to do**:
  - Run all 8 Post BySlug-related tests individually
  - Verify each test passes
  - Capture evidence for each test

  **Tests to verify**:
  1. PostTests.GetGenericBySlug
  2. PostTests.GetBaseClassBySlug
  3. PostTests.GetInfoBySlug
  4. PostTests.GetDynamicBySlug
  5. PostTests.UpdateCollectionPost (line 695 - GetBySlug returns null)
  6-8. PostTestsMemoryCache and PostTestsDistributedCache variants

  **Recommended Agent Profile**:
  - **Category**: `unspecified-high`
    - Reason: Verification task
  - **Skills**: []

  **Parallelization**:
  - **Can Run In Parallel**: NO (verification after fix)
  - **Parallel Group**: Wave 2 (Task 5)
  - **Blocks**: Wave 3 (Category C)
  - **Blocked By**: Task 4

  **Acceptance Criteria**:
  - [ ] All 8 Category B tests pass individually
  - [ ] Evidence captured for each test
  - [ ] No null return errors

  **QA Scenarios**:

  ```
  Scenario: All 8 Post BySlug tests pass
    Tool: Bash (dotnet test)
    Steps:
      1. Run each of the 8 tests individually
      2. Capture pass/fail status for each
      3. Verify 8/8 pass
    Expected Result: 8/8 tests pass
    Evidence: .sisyphus/evidence/task-5-all-postbyslug-tests.txt
  ```

  **Commit**: YES
  - Message: `fix(post): implement GetBySlug queries for RavenDB`
  - Files: PostRepository.cs

## Commit Strategy

- **Category A**: `fix(content): correct translation queries and GetById logic` — ContentRepository.cs, Content.cs
- **Category B**: `fix(post): implement GetBySlug queries for RavenDB` — PostRepository.cs
- **Category C**: `fix(page): correct copy/detach logic for regions and blocks` — PageRepository.cs
- **Category D**: `fix(posttype): correct delete handling and cache invalidation` — PostTypeRepository.cs

---

## Success Criteria

### Verification Commands
```bash
# Category A - Translations (15 tests)
dotnet test test/Piranha.Tests/Piranha.Tests.csproj --filter "FullyQualifiedName~ContentTests.GetById" --no-build
dotnet test test/Piranha.Tests/Piranha.Tests.csproj --filter "FullyQualifiedName~ContentTests.GetTranslatedStatus" --no-build
dotnet test test/Piranha.Tests/Piranha.Tests.csproj --filter "FullyQualifiedName~ContentTests.GetTranslationSummary" --no-build
dotnet test test/Piranha.Tests/Piranha.Tests.csproj --filter "FullyQualifiedName~ContentTests.GetUntranslatedStatus" --no-build

# Category B - Post BySlug (8 tests)
dotnet test test/Piranha.Tests/Piranha.Tests.csproj --filter "FullyQualifiedName~PostTests.GetGenericBySlug" --no-build
- [ ] 6. Fix PageRepository.Detach Copy Logic (Category C)

  **What to do**:
  - Investigate PageRepository.Detach method - Body is null after detach
  - Ensure regions and blocks are deep-cloned when creating copy
  - Verify OriginalPageId is set correctly on copy
  - Check that detach creates independent copy (not reference)
  - May need to use JsonSerializer deep clone or manual clone

  **Root Cause**: Detach not cloning regions/blocks correctly - Body ends up null

  **Must NOT do**:
  - DO NOT modify test expectations
  - DO NOT break existing copy functionality

  **Recommended Agent Profile**:
  - **Category**: `deep`
    - Reason: Complex cloning logic for nested structures
  - **Skills**: []

  **Parallelization**:
  - **Can Run In Parallel**: NO (Category C foundation)
  - **Parallel Group**: Wave 3 (Task 6)
  - **Blocks**: Task 7
  - **Blocked By**: Task 5 (Category B complete)

  **References**:
  - `data/Piranha.Data.RavenDb/Repositories/PageRepository.cs` - Detach method
  - `core/Piranha/Models/PageBase.cs` - Deep clone implementation
  - `test/Piranha.Tests/Services/PageTests.cs:902` - DetachShouldCopyRegions test (READ ONLY)
  - `test/Piranha.Tests/Services/PageTests.cs:977` - UpdatingCopyShouldIgnoreBodyAndDate test (READ ONLY)

  **Acceptance Criteria**:
  - [ ] Detach creates deep copy of page
  - [ ] Regions and blocks are cloned
  - [ ] Body is not null after detach
  - [ ] `dotnet test --filter "FullyQualifiedName~PageTests.DetachShouldCopyRegions"` → PASS

  **QA Scenarios**:

  ```
  Scenario: Detach copies regions correctly
    Tool: Bash (dotnet test)
    Steps:
      1. Run: dotnet test test/Piranha.Tests/Piranha.Tests.csproj --filter "FullyQualifiedName~PageTests.DetachShouldCopyRegions" --no-build
      2. Verify Body is not null after detach
    Expected Result: Test passes, Body has content
    Failure Indicators: "Expected: Not null, Actual: null" or NullReferenceException
    Evidence: .sisyphus/evidence/task-6-detach-regions.txt

  Scenario: Updating copy preserves original
    Tool: Bash (dotnet test)
    Steps:
      1. Run: dotnet test test/Piranha.Tests/Piranha.Tests.csproj --filter "FullyQualifiedName~PageTests.UpdatingCopyShouldIgnoreBodyAndDate" --no-build
      2. Verify no NullReferenceException
      3. Verify copy can be updated independently
    Expected Result: Test passes, no exception
    Failure Indicators: NullReferenceException
    Evidence: .sisyphus/evidence/task-6-update-copy.txt
  ```

  **Commit**: NO (groups with Task 7)

---

- [ ] 7. Verify Both Category C Page Copy Tests Pass

  **What to do**:
  - Run both Page Copy tests individually
  - Verify each test passes
  - Capture evidence

  **Tests to verify**:
  1. PageTestsDistributedCache.DetachShouldCopyRegions
  2. PageTestsDistributedCache.UpdatingCopyShouldIgnoreBodyAndDate

  **Recommended Agent Profile**:
  - **Category**: `quick`
    - Reason: Simple verification task, only 2 tests
  - **Skills**: []

  **Parallelization**:
  - **Can Run In Parallel**: NO (verification after fix)
  - **Parallel Group**: Wave 3 (Task 7)
  - **Blocks**: Wave 4 (Category D)
  - **Blocked By**: Task 6

  **Acceptance Criteria**:
  - [ ] Both Category C tests pass
  - [ ] Evidence captured
  - [ ] No null reference errors

  **QA Scenarios**:

  ```
  Scenario: Both Page Copy tests pass
    Tool: Bash (dotnet test)
    Steps:
      1. Run: dotnet test test/Piranha.Tests/Piranha.Tests.csproj --filter "FullyQualifiedName~PageTests.DetachShouldCopyRegions" --no-build
      2. Run: dotnet test test/Piranha.Tests/Piranha.Tests.csproj --filter "FullyQualifiedName~PageTests.UpdatingCopyShouldIgnoreBodyAndDate" --no-build
      3. Verify 2/2 pass
    Expected Result: 2/2 tests pass
    Evidence: .sisyphus/evidence/task-7-page-copy-tests.txt
  ```

  **Commit**: YES
  - Message: `fix(page): correct copy/detach logic for regions and blocks`
  - Files: PageRepository.cs

- [ ] 8. Fix PostTypeRepository.Delete Handling (Category D)

  **What to do**:
  - Investigate PostTypeRepository.Delete - PostType is null after delete
  - Check if delete returns deleted entity or if cache invalidation is broken
  - Verify cache is properly invalidated when PostType is deleted
  - May need to return deleted entity or fix cache key

  **Root Cause**: Delete returns null instead of deleted entity, or cache issue

  **Must NOT do**:
  - DO NOT modify test expectations
  - DO NOT break existing delete functionality

  **Recommended Agent Profile**:
  - **Category**: `quick`
    - Reason: Simple delete fix, likely cache or return value issue
  - **Skills**: []

  **Parallelization**:
  - **Can Run In Parallel**: NO (Category D foundation)
  - **Parallel Group**: Wave 4 (Task 8)
  - **Blocks**: Task 9
  - **Blocked By**: Task 7 (Category C complete)

  **References**:
  - `data/Piranha.Data.RavenDb/Repositories/PostTypeRepository.cs` - Delete method
  - `test/Piranha.Tests/Repositories/PostTypeTests.cs:220` - Delete test (READ ONLY)

  **Acceptance Criteria**:
  - [ ] Delete returns deleted PostType entity
  - [ ] Cache properly invalidated
  - [ ] `dotnet test --filter "FullyQualifiedName~PostTypeTests.Delete"` → PASS

  **QA Scenarios**:

  ```
  Scenario: Delete returns deleted entity
    Tool: Bash (dotnet test)
    Steps:
      1. Run: dotnet test test/Piranha.Tests/Piranha.Tests.csproj --filter "FullyQualifiedName~PostTypeTests.Delete" --no-build
      2. Verify PostType is not null after delete
    Expected Result: Test passes, PostType returned
    Failure Indicators: "Expected: Not null, Actual: null"
    Evidence: .sisyphus/evidence/task-8-posttype-delete.txt
  ```

  **Commit**: NO (groups with Task 9)

---

- [ ] 9. Verify Category D PostType Delete Test Passes

  **What to do**:
  - Run PostType Delete test
  - Verify test passes
  - Capture evidence

  **Tests to verify**:
  1. PostTypeTestsMemoryCache.Delete

  **Recommended Agent Profile**:
  - **Category**: `quick`
    - Reason: Simple verification, single test
  - **Skills**: []

  **Parallelization**:
  - **Can Run In Parallel**: NO (verification after fix)
  - **Parallel Group**: Wave 4 (Task 9)
  - **Blocks**: Final Verification Wave
  - **Blocked By**: Task 8

  **Acceptance Criteria**:
  - [ ] PostType Delete test passes
  - [ ] Evidence captured
  - [ ] No null reference errors

  **QA Scenarios**:

  ```
  Scenario: PostType Delete test passes
    Tool: Bash (dotnet test)
    Steps:
      1. Run: dotnet test test/Piranha.Tests/Piranha.Tests.csproj --filter "FullyQualifiedName~PostTypeTests.Delete" --no-build
      2. Verify test passes
    Expected Result: Test passes
    Evidence: .sisyphus/evidence/task-9-posttype-delete-test.txt
  ```

  **Commit**: YES
  - Message: `fix(posttype): correct delete handling and cache invalidation`
  - Files: PostTypeRepository.cs
dotnet test test/Piranha.Tests/Piranha.Tests.csproj --filter "FullyQualifiedName~PostTests.GetBaseClassBySlug" --no-build
dotnet test test/Piranha.Tests/Piranha.Tests.csproj --filter "FullyQualifiedName~PostTests.GetInfoBySlug" --no-build
dotnet test test/Piranha.Tests/Piranha.Tests.csproj --filter "FullyQualifiedName~PostTests.GetDynamicBySlug" --no-build

# Category C - Page Copy (2 tests)
dotnet test test/Piranha.Tests/Piranha.Tests.csproj --filter "FullyQualifiedName~PageTests.DetachShouldCopyRegions" --no-build
dotnet test test/Piranha.Tests/Piranha.Tests.csproj --filter "FullyQualifiedName~PageTests.UpdatingCopyShouldIgnoreBodyAndDate" --no-build

# Category D - PostType Delete (1 test)
dotnet test test/Piranha.Tests/Piranha.Tests.csproj --filter "FullyQualifiedName~PostTypeTests.Delete" --no-build
```

### Final Checklist
- [ ] Category A: All 15 translation tests pass
- [ ] Category B: All 8 Post BySlug tests pass
- [ ] Category C: Both Page Copy tests pass
- [ ] Category D: PostType Delete test passes
- [ ] Total: 30/30 tests passing
- [ ] No regression in previously passing tests
- [ ] All fixes align with NoSQL denormalized model
- [ ] No test logic modified (per AGENTS.md)
