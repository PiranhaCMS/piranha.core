# Fix 403 Forbidden Error on CMS Manager Login

## TL;DR

> **Quick Summary**: The 403 Forbidden error occurs because the `AeroAdmin` claim is not present in the user's ClaimsPrincipal during authorization. The fix syncs role claims to the user document during login (denormalized RavenDB approach).
>
> **Deliverables**:
> - Modified `IdentitySecurity.cs` with role claim sync on login
> - Verified login flow with proper claims
>
> **Estimated Effort**: Quick
> **Parallel Execution**: NO - sequential
> **Critical Path**: IdentitySecurity → Test

---

## Context

### Original Request
User receives 403 Forbidden error when logging into `/manage` CMS admin backend after authentication succeeds.

### Interview Summary
**Key Discussions**:
- Authentication succeeds but authorization fails
- The `AeroAdmin` claim is required by all Manager authorization policies
- User's RavenDB user document has `Roles: ["SYSADMIN", "ADMIN", "AeroAdmin"]` but `Claims: []` (empty)
- Role claims need to flow to user's ClaimsPrincipal

**Research Findings**:
- ASP.NET Identity's `UserClaimsPrincipalFactory` builds ClaimsPrincipal from:
  1. `UserManager.GetClaimsAsync(user)` → Returns `user.Claims`
  2. `UserManager.GetRolesAsync(user)` → Returns role names
  3. For each role, calls `RoleManager.GetClaimsAsync(role)` → Returns `role.Claims`
- Current implementation stores role claims in `RavenRole.Claims` (denormalized)
- The seed properly stores claims in the role document

### Root Cause
The user's `Claims` array is empty. While the `RavenRoleStore.GetClaimsAsync` correctly reads from `role.Claims`, the claims don't appear to be flowing through the ClaimsPrincipal creation. The most reliable RavenDB approach is to denormalize - copy role claims directly to the user's Claims collection.

---

## Work Objectives

### Core Objective
Ensure the `AeroAdmin` claim (and all role claims) are present in the user's ClaimsPrincipal during login.

### Concrete Deliverables
- `src/Aero.Cms.AspNetCore.Identity/IdentitySecurity.cs` - Add role claim sync before sign-in

### Definition of Done
- [ ] User can log in without 403 error
- [ ] User's ClaimsPrincipal contains `AeroAdmin` claim
- [ ] Manager pages are accessible after login

### Must Have
- Role claims synced to user document on login

### Must NOT Have (Guardrails)
- Don't modify the seed method
- Don't change authorization policy definitions
- Don't break existing test suite

---

## Verification Strategy

### Test Decision
- **Infrastructure exists**: YES
- **Automated tests**: NO (manual testing)
- **Framework**: N/A
- **Agent-Executed QA**: YES

### QA Policy
Manual verification with browser testing.

---

## Execution Strategy

### Parallel Execution Waves

```
Wave 1 (Sequential):
└── Task 1: Modify IdentitySecurity.cs to sync role claims [quick]
```

---

## TODOs

- [x} 1. Modify IdentitySecurity to Sync Role Claims on Login

  **What to do**:
  - Add `UserManager<User>` and `RoleManager<Role>` as dependencies
  - Add `SyncRoleClaimsToUserAsync` method that:
    1. Gets all role names for the user
    2. For each role, loads the role document
    3. Copies any missing claims from `role.Claims` to `user.Claims`
    4. Saves the user if claims were added
  - Call this sync method in `SignIn` before `PasswordSignInAsync`

  **Implementation Details**:

  Update `IdentitySecurity.cs`:

  ```csharp
  // Add using statements
  using Aero.Identity.Models;

  // Add new dependencies
  private readonly UserManager<User> _userManager;
  private readonly RoleManager<Role> _roleManager;
  private readonly IdentityOptions _options;

  // Update constructor
  public IdentitySecurity(
      UserManager<User> userManager,
      RoleManager<Role> roleManager,
      SignInManager<User> signInManager,
      IOptions<IdentityOptions> identityOptions,
      IIdentitySeed seed = null)
  {
      _userManager = userManager;
      _roleManager = roleManager;
      _signInManager = signInManager;
      _options = identityOptions.Value;
      _seed = seed;
  }

  // Update SignIn method
  public async Task<LoginResult> SignIn(object context, string username, string password)
  {
      if (_seed != null)
      {
          await _seed.CreateAsync();
      }

      // Sync role claims to user before sign-in (RavenDB denormalized approach)
      var user = await _userManager.FindByNameAsync(username);
      if (user != null)
      {
          await SyncRoleClaimsToUserAsync(user);
      }

      var result = await _signInManager.PasswordSignInAsync(username, password, false,
          _options.Lockout.MaxFailedAccessAttempts > 0 ? true : false);

      if (result.Succeeded)
      {
          return LoginResult.Succeeded;
      }
      else if (result.IsLockedOut)
      {
          return LoginResult.Locked;
      }
      return LoginResult.Failed;
  }

  // Add new method
  private async Task SyncRoleClaimsToUserAsync(User user)
  {
      var userRoleNames = await _userManager.GetRolesAsync(user);
      var existingClaims = user.Claims.Select(c => (c.ClaimType, c.ClaimValue)).ToHashSet();
      var claimsAdded = false;

      foreach (var roleName in userRoleNames)
      {
          var role = await _roleManager.FindByNameAsync(roleName);
          if (role?.Claims != null)
          {
              foreach (var roleClaim in role.Claims)
              {
                  if (!string.IsNullOrEmpty(roleClaim.ClaimType) &&
                      !string.IsNullOrEmpty(roleClaim.ClaimValue) &&
                      !existingClaims.Contains((roleClaim.ClaimType, roleClaim.ClaimValue)))
                  {
                      user.Claims.Add(new RavenUserClaim
                      {
                          ClaimType = roleClaim.ClaimType,
                          ClaimValue = roleClaim.ClaimValue
                      });
                      existingClaims.Add((roleClaim.ClaimType, roleClaim.ClaimValue));
                      claimsAdded = true;
                  }
              }
          }
      }

      if (claimsAdded)
      {
          await _userManager.UpdateAsync(user);
      }
  }
  ```

  **Recommended Agent Profile**:
  - **Category**: `quick`
  - **Skills**: []

  **Parallelization**:
  - **Can Run In Parallel**: NO
  - **Parallel Group**: Sequential
  - **Blocks**: None
  - **Blocked By**: None (can start immediately)

  **References**:
  - `src/Aero.Cms.AspNetCore.Identity/IdentitySecurity.cs` - File to modify
  - `src/Aero.Identity/Models/RavenUser.cs` - RavenUserClaim class definition
  - `src/Aero.Identity/Models/RavenRole.cs` - RavenRoleClaim class definition
  - `src/Aero.Cms.AspNetCore.Identity/Data/Role.cs` - Role class

  - [x} Code compiles without errors
  - [ ] Code compiles without errors
  - [x] UserManager and RoleManager injected properly
  - [x] SyncRoleClaimsToUserAsync method implemented
  - [x] Method called before PasswordSignInAsync

  **QA Scenarios**:
  ```
  Scenario: Login with existing user who has SysAdmin role
    Tool: Browser (Playwright)
    Preconditions:
      - User "admin" exists with role "SysAdmin"
      - SysAdmin role has AeroAdmin claim in role.Claims
      - User's Claims array is empty or missing AeroAdmin
    Steps:
      1. Navigate to https://localhost:64472/manager/login
      2. Enter username "admin"
      3. Enter password (valid password)
      4. Click login button
      5. Verify redirect to /manager (not 403)
    Expected Result: User is logged in and redirected to /manager
    Failure Indicators: 403 Forbidden response
    Evidence: .sisyphus/evidence/task-1-login-success.png
  - [x] Evidence: .sisyphus/evidence/task-1-login-success.png

  **Commit**: YES
  - Message: `fix(identity): sync role claims to user on login for denormalized auth`
  - Files: `src/Aero.Cms.AspNetCore.Identity/IdentitySecurity.cs`

---

## Final Verification Wave

- [ ] F1. **Manual QA** — Test login and manager access
  Start application. Login as admin. Verify access to /manager without 403 error.
  Output: `Login [SUCCESS/FAIL] | Manager Access [SUCCESS/FAIL] | VERDICT`

---

## Commit Strategy

- **1**: `fix(identity): sync role claims to user on login for denormalized auth` — IdentitySecurity.cs

---

## Success Criteria

### Verification Commands
```bash
dotnet build src/Aero.Cms.AspNetCore.Identity/Aero.Cms.AspNetCore.Identity.csproj
# Expected: Build succeeded. 0 Error(s)
```

### Final Checklist
- [ ] All "Must Have" present
- [ ] All "Must NOT Have" absent
- [ ] Login succeeds without 403 error
- [ ] Manager pages accessible
