# Categorized Test Failures

## Category 1: PageBase Deserialization (CRITICAL - ~10 tests)
**Error**: `Deserialization of interface or abstract types is not supported. Type 'Piranha.Models.PageBase'`

**Affected Tests**:
- PageTestsDistributedCache.GetAllByBaseClass
- PageTestsDistributedCache.UpdatingCopyShouldIgnoreBodyAndDate
- PageTestsDistributedCache.DetachShouldCopyBlocks
- PageTestsDistributedCache.DetachShouldCopyRegions
- PageTestsMemoryCache.GetAllByBaseClass
- (Any test that deserializes cached PageBase)

**Root Cause**: Distributed cache serializes PageBase (abstract) but System.Text.Json cannot deserialize abstract types without type discriminators.

**Solution**: Add JsonDerivedType attributes to PageBase (like we did for Block), OR use different serialization approach for cache.

---

## Category 2: InvalidCastException - ID Collision (~5 tests)
**Error**: `Unable to cast object of type 'Site' to type 'Page'` or `Page to Post`

**Affected Tests**:
- PageTests.GetDIGeneric
- PageTests.GetDIDynamic
- PostTests.GetDIGeneric
- PostTests.GetDIDynamic

**Root Cause**: Snowflake ID generation may be creating duplicate IDs across different entity types (Site, Page, Post).

**Solution**: Ensure ID generation includes entity type prefix or uses separate sequences.

---

## Category 3: SaveDraft Bug (~3 tests)
**Error**: `Assert.NotEqual() Failure: Strings are equal - Expected: Not "My working copy", Actual: "My working copy"`

**Affected Tests**:
- PageTests.SaveDraft
- PageTestsMemoryCache.SaveDraft
- PageTestsDistributedCache.SaveDraft

**Root Cause**: PageRepository.Save(isDraft=true) modifies page object in-memory, so GetByIdAsync returns modified version.

**Solution**: Clone page before modifying when saving draft.

---

## Category 4: "Can Not Delete Page" (~8 tests)
**Error**: `System.InvalidOperationException: Can not delete page because it has copies`

**Affected Tests**:
- PageTestsMemoryCache.GetStartpage
- PageTestsMemoryCache.GetGenericBySlug
- PageTestsMemoryCache.GetBaseClassBySlug
- PageTestsMemoryCache.CanNotUpdateCopyWithAnotherTypeIdOtherThanOriginalPageTypeId
- Multiple other page tests

**Root Cause**: Test cleanup tries to delete pages, but repository blocks deletion if page has copies (OriginalPageId references). Tests not properly cleaning up copies first.

**Solution**: 
1. Fix test cleanup to delete copies before original pages
2. OR fix repository to cascade delete copies
3. OR ensure tests don't leave orphaned copies

---

## Category 5: Comment Count Failures (~3 tests)
**Error**: `Assert.Equal() Failure: Values differ - Expected: 1, Actual: ?`

**Affected Tests**:
- CommentTests.AddPostComment
- CommentTests.AddPageComment
- CommentTestsDistributedCache.AddPostComment

**Root Cause**: Comments may be embedded differently in RavenDB model, or comment saving isn't working correctly.

**Solution**: Check how comments are stored/retrieved in NoSQL model.

---

## Category 6: Content Translation Failures (~9 tests)
**Error**: Multiple assertion failures (NotNull, Equal, False)

**Affected Tests**:
- ContentTests.GetById - String mismatch (translation returned instead of original)
- ContentTests.GetTranslatedStatus - Null
- ContentTests.GetUntranslatedStatus - Null  
- ContentTests.GetTranslationSummary - False expected
- (Same tests for MemoryCache and DistributedCache variants)

**Root Cause**: Content translation logic not working with embedded translations in RavenDB model.

**Solution**: Check ContentRepository translation queries and how translations are embedded in Content document.

---

## Category 7: DI Field Injection String Mismatches (~6 tests)
**Error**: `Assert.Equal() Failure: Strings differ` (position 2)

**Affected Tests**:
- PageTestsMemoryCache.GetDIDynamic
- PageTestsMemoryCache.CreateDIGeneric
- PageTestsMemoryCache.CreateDIDynamic
- PageTestsDistributedCache.GetDIDynamic
- PageTestsDistributedCache.CreateDIGeneric
- PostTests variants

**Root Cause**: DI-injected fields may have different string values after serialization/deserialization or model transformation.

**Solution**: Investigate DI field injection and serialization.

---

## Category 8: NullReferenceExceptions (~2 tests)
**Error**: `System.NullReferenceException`

**Affected Tests**:
- AliasTestsMemoryCache.IsCached
- AliasTestsMemoryCache.FixRedirectUrl

**Root Cause**: Alias object or related data is null when expected to have value.

**Solution**: Check alias initialization and caching logic.

---

## Category 9: NonUniqueObjectException (~1 test)
**Error**: `Attempted to associate a different object with id '...'`

**Affected Tests**:
- AliasTests.GetById

**Root Cause**: RavenDB session tracking conflict - trying to load different object with same ID.

**Solution**: Check session management and object tracking.

---

## Category 10: ArgumentNullException (~2 tests)
**Error**: `Value cannot be null. (Parameter 'key')`

**Affected Tests**:
- MediaTestsMemoryCache.DeleteById
- MediaTestsDistributedCache.DeleteById

**Root Cause**: Cache key is null when trying to delete from cache.

**Solution**: Check media cache key generation.

---

## Priority Order

1. **PageBase Deserialization** (Category 1) - Blocks many tests
2. **InvalidCastException** (Category 2) - ID collision is critical
3. **SaveDraft Bug** (Category 3) - Already identified, easy fix
4. **"Can Not Delete"** (Category 4) - Blocks many tests during cleanup
5. **Content Translations** (Category 6) - Multiple tests affected
6. **Comment Counts** (Category 5) - Check embedded model
7. **DI Field Injection** (Category 7) - Investigate serialization
8. **NullReferenceExceptions** (Category 8) - Debug specific tests
9. **NonUniqueObjectException** (Category 9) - Session management
10. **ArgumentNullException** (Category 10) - Cache key issue
