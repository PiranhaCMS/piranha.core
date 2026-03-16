# Draft: RavenDB Test Remediation Plan

## Current Situation Analysis

### Test Failure Pattern
Running tests shows **ALL failures are RavenDB Raft consensus timeouts**, not test logic failures:
- `System.TimeoutException: Waited for 00:00:15 but didn't get an index notification`
- `The server at /admin/databases responded with status code: RequestTimeout`
- Tests timing out during database creation/deletion
- Index creation timing out

### Root Cause
**NOT a code logic problem** - This is a **test infrastructure performance problem**:
1. Each test creates a new RavenDB database
2. Embedded RavenDB server cannot keep up with rapid database creation/deletion
3. Raft consensus operations (database creation, index deployment) exceed 15s timeout
4. Tests run in parallel, overwhelming the embedded server

### Key Insight
The AGENTS.md says "DO NOT modify the unit tests", but the failures aren't in test logic - they're in test infrastructure setup/teardown timing.

## Two Possible Interpretations

### Interpretation A: Infrastructure Problem Only
- All 68 "failures" are actually timeouts
- No actual test logic is broken
- Need to fix test infrastructure (increase timeouts, reduce parallelism, optimize RavenDB setup)

### Interpretation B: Mixed Problem
- Some failures are timeouts (masking real failures)
- Some failures are actual logic issues (hidden by timeouts)
- Need to fix infrastructure first to see real failures

## Questions for User

1. **Are these timeout failures new?** Did tests pass before with the same infrastructure?
2. **What changed recently?** Did you modify RavenDB setup, indexes, or test base?
3. **Do you want me to:**
   - Option A: Fix test infrastructure to eliminate timeouts, then see if any real failures remain
   - Option B: Assume timeouts are masking real issues and plan for both infrastructure + logic fixes
4. **Test parallelism:** Are you running tests in parallel? Should we reduce parallelism?

## Technical Options for Infrastructure Fix

### Option 1: Increase Timeouts
- Modify `RavenTestBase.cs` to use longer timeouts (30s instead of 15s)
- Pros: Simple, quick
- Cons: Slower tests, may not fully solve the problem

### Option 2: Reuse Database Across Tests
- Create database once per test class instead of per test
- Pros: Faster, less Raft overhead
- Cons: Tests must be isolated (cleanup between tests)

### Option 3: Reduce Test Parallelism
- Run tests sequentially or with limited parallelism
- Pros: Less load on embedded server
- Cons: Slower overall test run

### Option 4: Optimize Index Creation
- Defer index creation until first use
- Use async index deployment with proper waiting
- Pros: Less upfront overhead
- Cons: More complex test setup

### Option 5: Use In-Memory RavenDB
- Switch to pure in-memory mode if available
- Pros: Fastest, no disk I/O
- Cons: May not match production behavior

## Recommended Approach

**Phase 1: Infrastructure Stabilization** (if user confirms this is the issue)
1. Increase default timeout to 30s in RavenTestBase
2. Add retry logic for database creation failures
3. Consider database reuse within test classes
4. Verify all tests can run without timeouts

**Phase 2: Logic Verification** (after infrastructure is stable)
1. Run full test suite with stable infrastructure
2. Identify any actual test logic failures
3. Fix logic issues related to RavenDB modeling
4. Achieve 100% pass rate

## Waiting For
- User clarification on timeout vs logic issue nature
- User preference on infrastructure fix approach
- Background agent analysis of test failure patterns
