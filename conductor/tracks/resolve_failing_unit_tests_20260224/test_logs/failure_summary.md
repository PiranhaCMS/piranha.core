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
**Root Cause:** Comment saving not persisting / type casting errors

| Test | Error Type | Location |
|------|-----------|----------|
| CommentTestsMemoryCache.AddPageComment | Assert.Equal (Expected: 1, Actual: 0) | CommentTests.cs:278 |
| CommentTestsMemoryCache.AddPostComment | Assert.Equal (expected: 1, Actual: 0) | CommentTests.cs:161 |
| CommentTestsDistributedCache.AddPostComment | Assert.Equal (expected: 1, Actual: 0) | CommentTests.cs:161 |
| CommentTests.AddPostComment | Assert.Equal (expected: 1, Actual: 0) | CommentTests.cs:161 |
| CommentTests.AddPageComment | InvalidCastException (Site to Post) | CommentTests.cs:110 |

**Analysis:** Comments are not being saved to the database (count = 0). One test shows a type casting error where a Site is being returned instead of a Post.
