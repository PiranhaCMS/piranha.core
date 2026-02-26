# Implementation Plan: Align Test Infrastructure with Flow Map

## Phase 1: Audit and Analysis [checkpoint: 686133e]
- [x] Task: Audit \`BaseTestsAsync.cs\` against the \`flow.md\` initialization map 686133e
    - [x] Verify \`CreateServiceCollection\` registers all necessary services
    - [x] Verify \`CreateApi\` correctly composes all 14 repositories
- [x] Task: Audit \`App.Init\` usage in tests 686133e
    - [x] Ensure \`App.Init\` is called correctly in \`InitializeAsync\`
    - [x] Verify thread-safety or reset mechanisms if tests run in parallel
- [x] Task: Conductor - User Manual Verification 'Phase 1: Audit and Analysis' (Protocol in workflow.md) 686133e

## Phase 2: Infrastructure Realignment
- [~] Task: Implement missing DI registrations in \`BaseTestsAsync\`
    - [ ] Write failing test case that demonstrates a missing service or incorrect composition
    - [ ] Update \`CreateServiceCollection\` to pass the test
- [ ] Task: Refactor \`CreateApi\` to match the composition root pattern in \`flow.md\`
    - [ ] Update repository instantiation to match the diagram
    - [ ] Ensure all optional services (Cache, Storage, Processor) are handled correctly
- [ ] Task: Fix Test Lifecycle and Cleanup
    - [ ] Ensure \`DisposeAsync\` correctly cleans up the RavenDB TestDriver and resets static state
- [ ] Task: Conductor - User Manual Verification 'Phase 2: Infrastructure Realignment' (Protocol in workflow.md)

## Phase 3: Verification and Finalization
- [ ] Task: Execute the core test suite (\`Piranha.Tests\`) and resolve behavioral failures
    - [ ] Run \`dotnet test test/Piranha.Tests\`
    - [ ] Fix any remaining failures caused by infrastructure mismatch
- [ ] Task: Conductor - User Manual Verification 'Phase 3: Verification and Finalization' (Protocol in workflow.md)
