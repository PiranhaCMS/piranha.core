# Specification: `spec.md`

## Overview
This track involves implementing an integration test suite for the Piranha Manager login functionality. The tests will utilize the **Alba** library to simulate HTTP requests to the manager application. The focus is on verifying that the default administrative user, created during the database seeding process, can successfully log in and access authorized areas with the correct roles and claims.

## Functional Requirements
1. **Integration Test Setup:**
    - Integrate **Alba** for ASP.NET Core integration testing.
    - Configure the test environment to use an in-memory RavenDB (Raven.TestDriver).
2. **Database Seeding:**
    - Ensure the `Seed()` method (from `IdentityDb` or `DefaultIdentitySeed`) is executed to populate the database with the default admin account and roles.
3. **Login Verification:**
    - Test a successful login POST request to the manager login page using the seeded admin credentials.
    - Verify that the login result is successful (e.g., redirection to the authentication cookie endpoint).
4. **Permissions and Authorization Check:**
    - Verify that the logged-in admin user has the correct roles (e.g., `SysAdmin`, `Admin`).
    - Verify that the user possesses the expected claims.
    - Test that the user can successfully access a protected manager endpoint (e.g., `/manager`) with a 200 OK status code.

## Acceptance Criteria
1. A new integration test project or file is created using Alba.
2. Tests successfully execute the seeding process.
3. A test case demonstrates successful login with the admin user.
4. A test case verifies that the admin user can access the manager area with a 200 status code.
5. All new integration tests pass in the build environment.
