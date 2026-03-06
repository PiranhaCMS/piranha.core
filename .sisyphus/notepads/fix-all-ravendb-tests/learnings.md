# Fix PageBase Deserialization Error - Learnings

## Problem
- Error: "Deserialization of interface or abstract types is not supported. Type 'Piranha.Models.PageBase'"
- Occurs when calling `GetAllAsync<PageBase>()` where PageBase is abstract

## Root Cause
- System.Text.Json cannot deserialize to abstract types directly
- The test passes PageBase (abstract) as generic type parameter
- Utils.DeepClone and caching try to deserialize to the abstract type

## Solution Applied
1. **PageService.cs - GetByIdsAsync** (line 329):
   - Added check: `!typeof(T).IsAbstract` to skip cache for abstract types
   
2. **PageService.cs - MapOriginalAsync** (line 942):
   - Added check: `typeof(T).IsAbstract` to skip DeepClone for abstract types
   - When abstract, just use the original object directly

## Key Insight
- When generic type T is abstract, must bypass serialization entirely
- Cannot use JsonDerivedType on abstract classes with open generics (GenericPage<>, Page<>)
- The workaround is to skip serialization/deserialization for abstract types

## Files Modified
- `core/Piranha/Services/Internal/PageService.cs` - 2 locations
  - Line 329: Skip cache when T is abstract
  - Line 942: Skip DeepClone when T is abstract
