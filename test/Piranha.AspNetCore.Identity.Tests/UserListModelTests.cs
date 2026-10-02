using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Piranha.AspNetCore.Identity.Data;
using Piranha.AspNetCore.Identity.Models;
using Piranha.AspNetCore.Identity.SQLite;
using Xunit;

namespace Piranha.AspNetCore.Identity.Tests;

public class UserListModelTests
{
    [Fact]
    public void Get_ReturnsUsersWithTheirRoles()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<IdentitySQLiteDb>()
            .UseSqlite(connection)
            .Options;

        using var db = new IdentitySQLiteDb(options);

        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = "Editor",
            NormalizedName = "EDITOR"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = "testuser",
            NormalizedUserName = "TESTUSER",
            Email = "test@example.com",
            NormalizedEmail = "TEST@EXAMPLE.COM"
        };

        db.Roles.Add(role);
        db.Users.Add(user);

        db.UserRoles.Add(new IdentityUserRole<Guid>
        {
            UserId = user.Id,
            RoleId = role.Id
        });

        db.SaveChanges();

        var model = UserListModel.Get(db);

        var testUser = model.Users
            .Single(u => u.UserName == "testuser");

        Assert.Contains("Editor", testUser.Roles);
    }
}