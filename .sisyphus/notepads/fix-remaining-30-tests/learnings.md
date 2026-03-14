# Learnings

## [2026-03-06 14:51:15.212Z]

## Content Translation Issue Analysis (Task 1)

### Root Cause Identified
**File**: `ContentRepository.cs` line 64-76
**Issue**: `GetById` passes `languageId` to `TransformAsync`, which applies the translation to returns translated content instead of original.

### Current Code Flow
```csharp
public async Task<T> GetById<T>(string id, string languageId)
{
    var content = await GetQuery().FirstOrDefaultAsync(c => c.Id == id);
  // Loads content with embedded Translations
    
    if (content != null)
    {
        return await _service.TransformAsync<T>(content, ..., languageId);
        // ⚠ PROBLEM: languageId passed here
    }
}
```

### ContentService Transform Logic (ContentService.cs)
**Lines 93-106**: `TransformAsync` method
1. Sets `content.SelectedLanguageId = languageId` (line 50)
2. Calls `SetTranslation` at line 95-100:
3. `SetTranslation` **replaces** `content.Title` with translated title

4. Lines 202-204 attempt to **restore original** but fail

### Expected Behavior (from test)
**Test**: `ContentTests.GetById` (line 136-145)
1. Creates content with English title "My first content" (default language)
2. Saves Swedish translation "Mitt första innehåll" for that content
3. Calls `GetById(id, ID_1)` without languageId
4. **Expects**: content.Title == "My first content" (original)
5. **Actual**: content.Title == "Mitt första innehåll" (translated)

### Solution
The `languageId` parameter in `GetById` should be **optional**:
- **NULL**: Return original content (default language)
- **Non-NULL**: Return translated content for that specific language

### Implementation Approach
Modify `ContentService.TransformAsync`:
```csharp
// Lines 93-106 in ContentService.cs
if (content is ITranslatable translatableContent && !string.IsNullOrEmpty(languageId))
{
    var translation = translatableContent.GetTranslation(languageId);
    if (translation != null)
    {
        _mapper.Map(translation, model);  // This overwrites original content!
    }
}
```

**Should become**:
```csharp
if (content is ITranslatable translatableContent)
{
    // Save original values before applying translation
    string originalTitle = null;
    string originalExcerpt = null;
    
    if (!string.IsNullOrEmpty(languageId))
    {
        var translation = translatableContent.GetTranslation(languageId);
        if (translation != null)
        {
            originalTitle = content.Title;
            if (content is IExcerpt contentWithExcerpt)
            {
                originalExcerpt = contentWithExcerpt.Excerpt;
            }
            
            // Apply translation
            _mapper.Map(translation, model);
            
            // Restore original values
            if (originalTitle != null)
                content.Title = originalTitle;
            if (content is IExcerpt contentWithExcerpt && originalExcerpt != null)
            {
                contentWithExcerpt.Excerpt = originalExcerpt;
            }
        }
    }
}
```

### Why This Works
1. Translation fields (Title, Excerpt) are in both `Content` and `ContentTranslation`
2. AutoMapper maps translation → content (overwrites original)
3. We need to preserve original content's Title/Excerpt
4. Solution: Save originals before mapping, then restore them after
5. Only applies when languageId is provided (translation requested)
6. When languageId is null, original content is returned as-is

### Test Commands
```bash
dotnet test test/Piranha.Tests/Piranha.Tests.csproj --filter "FullyQualifiedName~ContentTests.GetById" --no-build
```

### Next Steps
1. Implement fix in ContentService.cs
2. Run test to verify
3. Move to Task 2 (Translation Status Queries)
## Fix for ContentRepository.GetById Translation Logic

### Problem
When retrieving content without specifying a languageId, the system was returning translated content instead of the original content. This was because the core ContentService was defaulting the languageId to the default language before passing it to the repository.

### Root Cause
In , the  method was automatically converting a null languageId to the default language ID:



This caused the repository to apply translations when the user explicitly requested original content by not providing a languageId.

### Solution
Modified  to track the originally requested languageId separately from the languageId used for caching:

1. Store the original requested languageId: 
2. Still default to the default language for caching purposes
3. Pass the original  to the repository, which will be null when the user wants original content
4. The repository then passes null to TransformAsync, which skips translation mapping

### Key Changes
- File: 
- Method: 
- Added:  variable to preserve user's intent
- Changed: Repository call uses  instead of defaulted 

### Test Results
- GetById without languageId now returns original content (My first content) ✓
- GetById with languageId returns translated content (Mitt första innehåll) ✓
- GetTranslationById works correctly ✓

### RavenDB Idiom
This fix follows RavenDB's denormalized document model where:
- Original content is stored in the main document
- Translations are embedded as a collection within the same document
- When languageId is null, we get the original content without translation overlay
- When languageId is specified, we overlay the translation on top of the base content

