# Test Failure Summary

**Generated:** 2026-03-05
**Test Suite:** Piranha Solution
**Total Failures:** 40+

---

## Summary by Project

### Aero.Identity.Tests
- **Status:** ✅ ALL PASSING (87/87 tests)

### Piranha.Manager.Tests
- **Status:** ✅ ALL PASSING (1/1 test)

### Piranha.Tests
- **Status:** ❌ FAILURES (40+ failures)

---

## Failure Categories

### Category 1: Comment Tests (6 failures)
**Root Cause:** Comment saving not persisting / Type casting errors

| Test | Error Type | Location |
|------|-----------|----------|
| CommentTestsMemoryCache.AddPageComment | Assert.Equal (Expected: 1, Actual: 0) | CommentTests.cs:278 |
| CommentTestsMemoryCache.AddPostComment | Assert.Equal (Expected: 1, Actual: 0) | CommentTests.cs:161 |
| CommentTestsDistributedCache.AddPostComment | Assert.Equal (Expected: 1, Actual: 0) | CommentTests.cs:161 |
| CommentTests.AddPostComment | Assert.Equal (Expected: 1, Actual: 0) | CommentTests.cs:161 |
| CommentTests.AddPageComment | InvalidCastException (Site to Post) | CommentTests.cs:110 |

**Analysis:** Comments are not being saved to the database (count = 0). One test shows a type casting error where a Site is being returned instead of a Post.

---

### Category 2: Content Tests (12 failures)
**Root Cause:** Translation/locale issues - returning Swedish content instead of English

| Test | Error Type | Location |
|------|-----------|----------|
| ContentTestsMemoryCache.GetTranslationSummary | Assert.False() Failure | ContentTests.cs:195 |
| ContentTestsMemoryCache.GetUntranslatedStatus | Assert.NotNull() - Value is null | ContentTests.cs:180 |
| ContentTestsMemoryCache.GetTranslatedStatus | Assert.NotNull() - Value is null | ContentTests.cs:167 |
| ContentTestsMemoryCache.GetById | String mismatch: "Mitt första innehåll" vs "My first content" | ContentTests.cs:144 |
| ContentTestsDistributedCache.GetTranslatedStatus | Assert.NotNull() - Value is null | ContentTests.cs:167 |
| ContentTestsDistributedCache.GetUntranslatedStatus | Assert.NotNull() - Value is null | ContentTests.cs:180 |
| ContentTestsDistributedCache.GetById | String mismatch (Swedish) | ContentTests.cs:144 |
| ContentTestsDistributedCache.GetTranslationSummary | Assert.False() Failure | ContentTests.cs:195 |
| ContentTests.GetById | String mismatch (Swedish) | ContentTests.cs:144 |
| ContentTests.GetTranslatedStatus | Assert.NotNull() - Value is null | ContentTests.cs:167 |
| ContentTests.GetUntranslatedStatus | Assert.NotNull() - Value is null | ContentTests.cs:180 |
| ContentTests.GetTranslationSummary | Assert.False() Failure | ContentTests.cs:195 |

**Analysis:** Content retrieval is returning Swedish translations instead of English content. Translation status methods returning null.

---

### Category 3: Media Tests (2 failures)
**Root Cause:** Cache key is null during delete operation

| Test | Error Type | Location |
|------|-----------|----------|
| MediaTestsMemoryCache.DeleteById | ArgumentNullException (key is null) | MediaTests.cs:312 → MediaService.cs:115,479 |
| MediaTestsDistributedCache.DeleteById | ArgumentNullException (key is null) | MediaTests.cs:312 → MediaService.cs:115,479 |

**Analysis:** MediaService.DeleteById is passing null cache key to cache removal methods.

---

### Category 4: Page Tests (22+ failures)

#### Subcategory 4a: JSON Deserialization (6+ failures)
**Root Cause:** Cannot deserialize abstract types (Block, PageBase)

| Test | Error Type | Location |
|------|-----------|----------|
| PageTestsMemoryCache.DetachShouldCopyBlocks | NotSupportedException: Abstract type 'Block' | PageTests.cs:965 → Utils.cs:898 |
| PageTestsMemoryCache.GetAllByBaseClass | NotSupportedException: Abstract type 'PageBase' | PageTests.cs:351 → Utils.cs:898 |
| PageTestsDistributedCache.UpdatingCopyShouldIgnoreBodyAndDate | NotSupportedException: Abstract type 'PageBase' | PageTests.cs:911 → DistributedCache.cs:42 |
| PageTestsDistributedCache.GetAllByBaseClass | NotSupportedException: Abstract type 'PageBase' | PageTests.cs:351 |
| PageTestsDistributedCache.DetachShouldCopyBlocks | NotSupportedException: Abstract type 'PageBase' | PageTests.cs:955 |
| PageTestsDistributedCache.DetachShouldCopyRegions | NotSupportedException: Abstract type 'PageBase' | PageTests.cs:986 |
| PageTests.GetAllByBaseClass | NotSupportedException: Abstract type 'PageBase' | PageTests.cs:351 |
| PageTests.DetachShouldCopyBlocks | NotSupportedException: Abstract type 'Block' | PageTests.cs:965 |

**Analysis:** RavenDB JSON deserializer cannot handle abstract types. Needs custom JSON converter with type discriminators.

---

#### Subcategory 4b: Page Copy Cascade Delete (8+ failures)
**Root Cause:** InvalidOperationException - Cannot delete pages with copies

| Test | Error Type | Location |
|------|-----------|----------|
| PageTestsMemoryCache.Update | InvalidOperationException | PageTests.cs:238 → PageRepository.cs:408 |
| PageTestsMemoryCache.GetNoneBySlug | InvalidOperationException | PageTests.cs:238 |
| PageTestsMemoryCache.GetCopyGenericBySlug | InvalidOperationException | PageTests.cs:238 |
| PageTestsMemoryCache.GetDynamicCollectionPage | InvalidOperationException | PageTests.cs:238 |
| PageTestsMemoryCache.CanNotUpdateCopyOriginalPageWithAnotherCopy | InvalidOperationException | PageTests.cs:238 |
| PageTestsDistributedCache.GetCopyGenericBySlug | InvalidOperationException | PageTests.cs:238 |
| PageTestsDistributedCache.CanNotUpdateCopyOriginalPageWithAnotherCopy | InvalidOperationException | PageTests.cs:238 |
| PageTestsDistributedCache.GetStartpage | InvalidOperationException | PageTests.cs:238 |
| PageTestsDistributedCache.GetInfoBySlug | InvalidOperationException | PageTests.cs:238 |
| PageTests.IsCached | InvalidOperationException | PageTests.cs:238 |
| PageTests.GetNoneBySlug | InvalidOperationException | PageTests.cs:238 |

**Analysis:** Tests are trying to delete pages that have copies, but the repository is throwing exceptions during cleanup. This suggests test isolation issues or incorrect cascade delete logic.

---

#### Subcategory 4c: DI String Comparison (8 failures)
**Root Cause:** Service injection returning different string values than expected

| Test | Error Type | Expected vs Actual |
|------|-----------|-------------------|
| PageTestsMemoryCache.GetDIGeneric | Assert.Equal | "My service value" vs "MyCustomServiceValue" |
| PageTestsMemoryCache.GetDIDynamic | Assert.Equal | String mismatch |
| PageTestsMemoryCache.CreateDIGeneric | Assert.Equal | String mismatch |
| PageTestsMemoryCache.CreateDIDynamic | Assert.Equal | String mismatch |
| PageTestsDistributedCache.GetDIDynamic | Assert.Equal | String mismatch |
| PageTestsDistributedCache.CreateDIGeneric | Assert.Equal | String mismatch |
| PageTestsDistributedCache.GetDIGeneric | Assert.Equal | String mismatch |
| PageTestsDistributedCache.CreateDIDynamic | Assert.Equal | String mismatch |
| PageTests.GetDIDynamic | Assert.Equal | String mismatch |
| PageTests.CreateDIDynamic | Assert.Equal | String mismatch |
| PageTests.GetDIGeneric | Assert.Equal | String mismatch |
| PageTests.CreateDIGeneric | Assert.Equal | String mismatch |

**Analysis:** DI-injected services are returning "MyCustomServiceValue" instead of "My service value". Service registration or resolution issue.

---

#### Subcategory 4d: Draft Saving (3 failures)
**Root Cause:** Draft content not being saved correctly

| Test | Error Type | Location |
|------|-----------|----------|
| PageTestsMemoryCache.SaveDraft | Assert.NotEqual() - Strings are equal | PageTests.cs:753 |
| PageTestsDistributedCache.SaveDraft | Assert.NotEqual() - Strings are equal | PageTests.cs:753 |
| PageTests.SaveDraft | Assert.NotEqual() - Strings are equal | PageTests.cs:753 |

**Analysis:** Page drafts are not being saved - original and draft content are the same.

---

## Recommended Fix Priority

1. **HIGH - JSON Deserialization**: Add polymorphic JSON converters for abstract types (Block, PageBase)
   - Affects: 8+ tests
   - Impact: Core functionality

2. **HIGH - Page Copy Cascade**: Fix page deletion cascade logic or test cleanup
   - Affects: 11+ tests
   - Impact: Test isolation

3. **MEDIUM - DI Service Resolution**: Fix service registration string values
   - Affects: 12 tests
   - Impact: Dependency injection

4. **MEDIUM - Comment Persistence**: Fix comment save functionality
   - Affects: 6 tests
   - Impact: Core feature

5. **MEDIUM - Content Translation**: Fix locale/translation retrieval
   - Affects: 12 tests
   - Impact: Multi-language support

6. **LOW - Media Cache Key**: Add null check for cache key
   - Affects: 2 tests
   - Impact: Edge case

7. **LOW - Page Drafts**: Fix draft saving logic
   - Affects: 3 tests
   - Impact: Draft feature
