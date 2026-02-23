using Aero.Identity.Models;
using Xunit;

namespace Aero.Identity.Tests.Models;

public class RavenUserTests
{
    [Fact]
    public void CanInitializeRavenUser()
    {
        // Arrange & Act
        var user = new RavenUser();

        // Assert
        Assert.NotNull(user);
        Assert.NotNull(user.Passkeys);
        Assert.Empty(user.Passkeys);
    }

    [Fact]
    public void CanAddPasskeyToRavenUser()
    {
        // Arrange
        var user = new RavenUser();
        var passkey = new PasskeyCredential
        {
            CredentialId = new byte[] { 1, 2, 3 },
            PublicKey = new byte[] { 4, 5, 6 },
            CreatedAt = DateTimeOffset.UtcNow,
            SignCount = 1,
            Name = "pk1"
        };

        // Act
        user.Passkeys.Add(passkey);

        // Assert
        Assert.Single(user.Passkeys);
        Assert.Equal(new byte[] { 1, 2, 3 }, user.Passkeys[0].CredentialId);
        Assert.Equal(new byte[] { 4, 5, 6 }, user.Passkeys[0].PublicKey);
        Assert.Equal(1u, user.Passkeys[0].SignCount);
        Assert.Equal("pk1", user.Passkeys[0].Name);
    }
}
