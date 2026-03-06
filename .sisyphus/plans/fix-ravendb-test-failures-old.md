# Work Plan: Fix RavenDB Test Failures

## TL;DR

> **Objective**: Fix 68 failing unit tests in Piranha CMS RavenDB port (ignore RavenDB timeout infrastructure issues)
> 
> **Root Cause**: Repository code has bugs in draft handling and navigation property queries that break with denormalized NoSQL model
> 
> **Approach**: Fix repository bugs → Run individual tests → Verify all logic failures resolved
>
> **Estimated Effort**: Medium (4-6 focused fixes + verification)
> **Execution Mode**: Sequential (each fix builds on previous)
> **Critical Path**: SaveDraft bug → ContentRepository query → Additional test runs → Final verification

---

## Context

### Original Request
Port Piranha CMS from EF Core (SQL) to RavenDB (NoSQL). Most work complete, but 68 tests failing. **IGNORE RavenDB timeout infrastructure issues** - focus on actual test logic failures when tests run individually.

### Interview Summary
**Key Decisions**:
- Ignore RavenDB Raft consensus timeouts (infrastructure issue, not code logic)
- Fix tests that fail when run individually (actual logic bugs)
- Do NOT modify unit tests (per AGENTS.md) - fix repository/model code only
- Exception: Can modify test seed/init if model changes require it

**Current State**:
- 868/935 tests passing (92.8%)
- Timeouts are RavenDB test infrastructure issue (15s Raft timeout)
- Tests run successfully when executed individually
- Repository code has bugs exposed by NoSQL denormalization

### Research Findings

**Finding 1 - SaveDraft Bug**:
- Test: `PageTests.SaveDraft` fails with assertion error
- Expected: Main page title unchanged after draft save
- Actual: Main page title changed to draft title
- Root cause: PageRepository.Save(isDraft=true) modifies page object in memory

**Finding 2 - ContentRepository Navigation Property**:
- File: `data/Piranha.Data.RavenDb/Repositories/ContentRepository.cs:144`
- Query: `.Where(t => t.Content.Type.Group == groupId)` assumes EF Core navigation
- Won't work in RavenDB without explicit loading or denormalization

**Finding 3 - Model Denormalization Pattern**:
- Blocks: Embedded in Page/Post ✓
- Categories/Tags: IDs + embedded objects ✓
- Permissions: Embedded ✓
- Translations: Embedded ✓
- Fields: Embedded ✓

**Finding 4 - Test Infrastructure**:
- Tests already use RavenDB (not EF Core)
- No .Include()/.ThenInclude() patterns found
- Test failures are repository bugs, not test code issues

---

## Work Objectives

### Core Objective
Fix all repository/model code bugs that cause actual test logic failures (not RavenDB timeout infrastructure issues).

### Concrete Deliverables
- Fixed PageRepository.Save method (draft handling)
- Fixed PostRepository.Save method (draft handling)
- Fixed ContentRepository navigation property queries
- All other repository query bugs identified and fixed
- 100% of non-timeout tests passing

### Definition of Done
- [ ] PageTests.SaveDraft passes individually
- [ ] ContentRepository queries work without navigation properties
- [ ] All individually-runnable tests pass (no assertion failures)
- [ ] Full test suite shows only RavenDB timeout failures (no logic failures)
- [ ] Code changes align with NoSQL denormalized model

### Must Have
- Draft saving must preserve main page state
- Repository queries must work with denormalized RavenDB model
- No navigation property assumptions in queries

### Must NOT Have (Guardrails)
- DO NOT modify unit test logic (per AGENTS.md)
- DO NOT add EF Core .Include()/.ThenInclude() patterns
- DO NOT try to fix RavenDB timeout infrastructure (separate concern)
- DO NOT change test assertions

---

## Verification Strategy

### Test Strategy
- **Automated tests**: Tests exist, run individually to avoid timeouts
- **Infrastructure**: RavenDB embedded in tests (xUnit)
- **Framework**: xUnit with RavenTestDriver
- **Approach**: Run individual tests to verify fixes

### QA Policy
Every fix MUST be verified by running the specific failing test individually:
```bash
dotnet test --filter "FullyQualifiedName~PageTests.SaveDraft" --no-build
dotnet test --filter "FullyQualifiedName~ContentTests" --no-build
```

Evidence captured in terminal output showing test pass/fail.

---

## Execution Strategy

### Sequential Execution (Dependencies Critical)

```
Task 1 → Task 2 → Task 3 → Task 4 → Task 5
```

**Why Sequential**:
- Task 1 (SaveDraft) is foundational - may affect other tests
- Task 2 (ContentRepository) is independent but needs verification
- Task 3 (PostRepository) mirrors Task 1 pattern
- Task 4 needs all fixes in place to find remaining issues
- Task 5 is final verification sweep

---

## TODOs

- [ ] 1. Fix PageRepository SaveDraft Bug

  **What to do**:
  - Fix `data/Piranha.Data.RavenDb/Repositories/PageRepository.cs` Save method (line 563)
  - When `isDraft=true` (line 573):
    - Current code modifies loaded `page` object directly (lines 658-666)
    - This causes GetByIdAsync to return modified object instead of original
  - Solution: Clone page object before modifying for draft:
    ```csharp
    if (isDraft)
    {
        // Clone page to avoid modifying the original
        var draftPage = JsonSerializer.Deserialize<Page>(JsonSerializer.Serialize(page));
        // Modify draftPage instead of page
        // Serialize draftPage to PageRevision.Data
    }
    ```
  - Alternative: Don't modify page object at all when isDraft=true, only create revision

  **Must NOT do**:
  - Do not change test assertions in PageTests.SaveDraft
  - Do not add EF Core patterns

  **Recommended Agent Profile**:
  - **Category**: `unspecified-high`
    - Reason: Repository bug fix requiring understanding of object references and serialization
  - **Skills**: None needed
    - Basic C# and RavenDB knowledge sufficient

  **Parallelization**:
  - **Can Run In Parallel**: NO
  - **Parallel Group**: Sequential (foundational fix)
  - **Blocks**: Tasks 3, 4, 5
  - **Blocked By**: None

  **References**:
  - `data/Piranha.Data.RavenDb/Repositories/PageRepository.cs:563-708` - Save method with isDraft logic
  - `test/Piranha.Tests/Services/PageTests.cs:740-759` - SaveDraft test showing expected behavior
  - `data/Piranha.Data.RavenDb/Data/PageRevision.cs` - Revision storage structure

  **Acceptance Criteria**:
  - [ ] `dotnet test --filter "FullyQualifiedName~PageTests.SaveDraft"` → PASS
  - [ ] Main page title unchanged after SaveDraftAsync
  - [ ] Draft revision contains modified title
  - [ ] GetByIdAsync returns original page (not modified in-memory version)

  **QA Scenarios**:
  ```
  Scenario: SaveDraft preserves main page state
    Tool: Bash (dotnet test)
    Steps:
      1. Run: dotnet test --filter "FullyQualifiedName~PageTests.SaveDraft" --no-build
      2. Check output for "Passed Piranha.Tests.Services.PageTests.SaveDraft"
    Expected Result: Test passes without assertion failures
    Failure Indicators: "Assert.NotEqual() Failure", "Failed Piranha.Tests.Services.PageTests.SaveDraft"
    Evidence: .sisyphus/evidence/task-1-savedraft-fix.txt

  Scenario: Draft revision created correctly
    Tool: Bash (dotnet test)
    Steps:
      1. Run SaveDraft test
      2. Verify GetDraftByIdAsync returns page with modified title
    Expected Result: Draft has "My working copy" title, main page has original title
    Evidence: .sisyphus/evidence/task-1-draft-verification.txt
  ```

  **Commit**: YES
  - Message: `fix(page): clone page object when saving draft to avoid in-memory modification`
  - Files: `data/Piranha.Data.RavenDb/Repositories/PageRepository.cs`
  - Pre-commit: `dotnet test --filter "FullyQualifiedName~PageTests.SaveDraft"`

---

- [ ] 2. Fix ContentRepository Navigation Property Query

  **What to do**:
  - Fix `data/Piranha.Data.RavenDb/Repositories/ContentRepository.cs:144`
  - Current query: `.Where(t => t.Content.Type.Group == groupId)` assumes navigation property
  - RavenDB doesn't support navigation property traversal in queries without explicit loading
  - Solution options:
    1. **Denormalize**: Add `GroupId` field to `ContentTranslation` model
    2. **Use RavenDB Includes**: `.Include(t => t.Content)` then load related
    3. **Restructure query**: Query Content collection directly with GroupId filter
  - Recommended: Denormalize `GroupId` in ContentTranslation (consistent with NoSQL pattern)

  **Implementation**:
  ```csharp
  // 1. Add to ContentTranslation model:
  public string GroupId { get; set; } // Denormalized from Content.Type.Group
  
  // 2. Update ContentRepository query:
  var translations = await _db.ContentTranslations
      .Where(t => t.GroupId == groupId)
      .OrderBy(t => t.ContentId)
      .ToListAsync();
  
  // 3. Ensure GroupId is set when ContentTranslation is created/updated
  ```

  **Must NOT do**:
  - Do not add .Include()/.ThenInclude() (EF Core pattern)
  - Do not change test logic

  **Recommended Agent Profile**:
  - **Category**: `unspecified-high`
    - Reason: Requires understanding RavenDB query limitations and denormalization
  - **Skills**: None needed

  **Parallelization**:
  - **Can Run In Parallel**: NO (depends on understanding from Task 1)
  - **Parallel Group**: Sequential
  - **Blocks**: Task 4
  - **Blocked By**: Task 1 (should establish pattern first)

  **References**:
  - `data/Piranha.Data.RavenDb/Repositories/ContentRepository.cs:144` - Problematic query
  - `data/Piranha.Data.RavenDb/Data/ContentTranslation.cs` - Model to update
  - `ravendb_architecture.md` - Denormalization strategy docs
  - `raven_db_models.md` - Model documentation

  **Acceptance Criteria**:
  - [ ] ContentTranslation has denormalized GroupId field
  - [ ] ContentRepository query uses GroupId instead of navigation property
  - [ ] GroupId is populated when ContentTranslation is created
  - [ ] Related content tests pass (ContentTests.*)

  **QA Scenarios**:
  ```
  Scenario: ContentRepository query works without navigation properties
    Tool: Bash (dotnet test)
    Steps:
      1. Run: dotnet test --filter "FullyQualifiedName~ContentTests" --no-build
      2. Check all ContentTests pass
    Expected Result: All content-related tests pass
    Failure Indicators: NullReferenceException, navigation property errors
    Evidence: .sisyphus/evidence/task-2-content-query.txt
  ```

  **Commit**: YES
  - Message: `fix(content): denormalize GroupId in ContentTranslation for RavenDB queries`
  - Files: `data/Piranha.Data.RavenDb/Data/ContentTranslation.cs`, `data/Piranha.Data.RavenDb/Repositories/ContentRepository.cs`
  - Pre-commit: `dotnet test --filter "FullyQualifiedName~ContentTests"`

---

- [ ] 3. Fix PostRepository SaveDraft Bug (Mirror Task 1)

  **What to do**:
  - Apply same fix as Task 1 to `data/Piranha.Data.RavenDb/Repositories/PostRepository.cs`
  - PostRepository likely has same Save(isDraft=true) bug
  - Check for same pattern: modifying post object in memory when isDraft=true
  - Clone post object before modifying for draft revision

  **Must NOT do**:
  - Do not change test logic
  - Do not introduce different pattern than Task 1 (consistency)

  **Recommended Agent Profile**:
  - **Category**: `quick`
    - Reason: Mirrors Task 1 pattern, straightforward application
  - **Skills**: None needed

  **Parallelization**:
  - **Can Run In Parallel**: NO
  - **Parallel Group**: Sequential (must follow Task 1)
  - **Blocks**: Task 4
  - **Blocked By**: Task 1 (pattern established there)

  **References**:
  - `data/Piranha.Data.RavenDb/Repositories/PostRepository.cs` - Post repository
  - Task 1 implementation - Pattern to follow

  **Acceptance Criteria**:
  - [ ] PostRepository.Save(isDraft=true) clones post before modifying
  - [ ] `dotnet test --filter "FullyQualifiedName~PostTests.SaveDraft"` → PASS (if test exists)
  - [ ] Main post state preserved after draft save

  **QA Scenarios**:
  ```
  Scenario: Post draft saving works correctly
    Tool: Bash (dotnet test)
    Steps:
      1. Run: dotnet test --filter "FullyQualifiedName~PostTests" --no-build
      2. Check for SaveDraft-related test passes
    Expected Result: All post draft tests pass
    Evidence: .sisyphus/evidence/task-3-post-draft.txt
  ```

  **Commit**: YES
  - Message: `fix(post): clone post object when saving draft to avoid in-memory modification`
  - Files: `data/Piranha.Data.RavenDb/Repositories/PostRepository.cs`
  - Pre-commit: `dotnet test --filter "FullyQualifiedName~PostTests"`

---

- [ ] 4. Search for Additional Navigation Property Queries

  **What to do**:
  - Search all repositories for navigation property patterns:
    ```bash
    grep -r "\.Content\." data/Piranha.Data.RavenDb/Repositories/
    grep -r "\.Type\." data/Piranha.Data.RavenDb/Repositories/
    grep -r "\.Site\." data/Piranha.Data.RavenDb/Repositories/
    grep -r "\.Blog\." data/Piranha.Data.RavenDb/Repositories/
    grep -r "\.Category\." data/Piranha.Data.RavenDb/Repositories/
    ```
  - Check for queries assuming related data is loaded
  - Fix any found issues using denormalization pattern from Task 2

  **Must NOT do**:
  - Do not add .Include() EF Core patterns
  - Do not break working queries

  **Recommended Agent Profile**:
  - **Category**: `unspecified-high`
    - Reason: Requires code review and pattern recognition
  - **Skills**: None needed

  **Parallelization**:
  - **Can Run In Parallel**: NO
  - **Parallel Group**: Sequential (needs Tasks 1-3 complete)
  - **Blocks**: Task 5
  - **Blocked By**: Tasks 1, 2, 3

  **References**:
  - All repository files in `data/Piranha.Data.RavenDb/Repositories/`
  - Task 2 denormalization pattern

  **Acceptance Criteria**:
  - [ ] No navigation property queries found in repositories
  - [ ] All identified issues fixed with denormalization
  - [ ] Code compiles without errors

  **QA Scenarios**:
  ```
  Scenario: No navigation property queries remain
    Tool: Bash (grep)
    Steps:
      1. Run grep commands to find navigation property patterns
      2. Verify no problematic patterns found
    Expected Result: No matches or all matches are legitimate
    Evidence: .sisyphus/evidence/task-4-search-results.txt
  ```

  **Commit**: YES (if issues found)
  - Message: `fix: denormalize additional fields for RavenDB query compatibility`
  - Files: Various repositories as needed
  - Pre-commit: `dotnet build`

---

- [ ] 5. Run Additional Individual Tests to Find Remaining Failures

  **What to do**:
  - Run diverse tests individually to find other failures:
    ```bash
    dotnet test --filter "FullyQualifiedName~PageTests.GetBySlug" --no-build
    dotnet test --filter "FullyQualifiedName~PageTests.GetDIGeneric" --no-build
    dotnet test --filter "FullyQualifiedName~PostTests.GetBySlug" --no-build
    dotnet test --filter "FullyQualifiedName~AliasTests" --no-build
    dotnet test --filter "FullyQualifiedName~MediaTests" --no-build
    ```
  - Document each failure pattern
  - Identify root causes
  - Create additional fix tasks as needed

  **Must NOT do**:
  - Do not run full test suite (will timeout)
  - Do not modify test logic

  **Recommended Agent Profile**:
  - **Category**: `quick`
    - Reason: Test execution and documentation
  - **Skills**: None needed

  **Parallelization**:
  - **Can Run In Parallel**: NO
  - **Parallel Group**: Sequential (needs all previous fixes)
  - **Blocks**: Task 6
  - **Blocked By**: Tasks 1, 2, 3, 4

  **References**:
  - `test/Piranha.Tests/Services/PageTests.cs` - Page tests
  - `test/Piranha.Tests/Services/PostTests.cs` - Post tests
  - All other test files

  **Acceptance Criteria**:
  - [ ] At least 10 diverse tests run individually
  - [ ] All failures documented with error messages
  - [ ] Root causes identified for each failure
  - [ ] Additional fix tasks created if needed

  **QA Scenarios**:
  ```
  Scenario: Diverse tests executed individually
    Tool: Bash (dotnet test)
    Steps:
      1. Run each test individually
      2. Capture output showing pass/fail
      3. Document failures
    Expected Result: Clear list of remaining issues
    Evidence: .sisyphus/evidence/task-5-test-results.txt
  ```

  **Commit**: NO (documentation task)

---

- [ ] 6. Final Verification - Run Full Test Suite

  **What to do**:
  - Run full test suite: `dotnet test test/Piranha.Tests/Piranha.Tests.csproj --no-build`
  - Ignore RavenDB timeout failures (expected)
  - Verify NO logic failures remain (assertion errors, null references, etc.)
  - Count logic failures vs timeout failures
  - Target: 0 logic failures, all timeouts acceptable

  **Must NOT do**:
  - Do not try to fix timeout infrastructure (separate concern)
  - Do not skip test categories

  **Recommended Agent Profile**:
  - **Category**: `quick`
    - Reason: Test execution and verification
  - **Skills**: None needed

  **Parallelization**:
  - **Can Run In Parallel**: NO
  - **Parallel Group**: Sequential (final verification)
  - **Blocks**: None (final task)
  - **Blocked By**: All previous tasks

  **References**:
  - All test files
  - All fixed repository files

  **Acceptance Criteria**:
  - [ ] Full test suite executed
  - [ ] 0 logic failures (assertion errors, null refs, etc.)
  - [ ] All failures are RavenDB timeout infrastructure issues
  - [ ] Tests that run individually all pass

  **QA Scenarios**:
  ```
  Scenario: Full test suite shows only timeout failures
    Tool: Bash (dotnet test)
    Steps:
      1. Run: dotnet test test/Piranha.Tests/Piranha.Tests.csproj --no-build 2>&1 | tee /tmp/full-test-run.txt
      2. Filter output for logic failures: grep -v "TimeoutException" /tmp/full-test-run.txt | grep "Failed"
      3. Verify no logic failures remain
    Expected Result: All failures mention TimeoutException or RavenException (infrastructure)
    Failure Indicators: Assert.Equal failures, NullReferenceException, InvalidCastException (logic bugs)
    Evidence: .sisyphus/evidence/task-6-final-verification.txt
  ```

  **Commit**: NO (verification task)

---

## Commit Strategy

- **Task 1**: `fix(page): clone page object when saving draft to avoid in-memory modification` — PageRepository.cs
- **Task 2**: `fix(content): denormalize GroupId in ContentTranslation for RavenDB queries` — ContentTranslation.cs, ContentRepository.cs
- **Task 3**: `fix(post): clone post object when saving draft to avoid in-memory modification` — PostRepository.cs
- **Task 4**: `fix: denormalize additional fields for RavenDB query compatibility` — Various files
- **Tasks 5-6**: No commits (verification tasks)

---

## Success Criteria

### Verification Commands
```bash
# Verify SaveDraft fix
dotnet test --filter "FullyQualifiedName~PageTests.SaveDraft" --no-build
# Expected: Test passes

# Verify ContentRepository fix
dotnet test --filter "FullyQualifiedName~ContentTests" --no-build
# Expected: All tests pass

# Verify no logic failures remain
dotnet test test/Piranha.Tests/Piranha.Tests.csproj --no-build 2>&1 | grep -v "TimeoutException" | grep "Failed"
# Expected: No output (all failures are timeouts)
```

### Final Checklist
- [ ] PageTests.SaveDraft passes individually
- [ ] PostRepository SaveDraft works correctly (if applicable)
- [ ] ContentRepository queries work without navigation properties
- [ ] No other navigation property queries found
- [ ] All individually-runnable tests pass
- [ ] Full test suite shows only RavenDB timeout failures (no logic failures)
- [ ] All changes align with NoSQL denormalized model
- [ ] No test logic modified (only repository/model code)
