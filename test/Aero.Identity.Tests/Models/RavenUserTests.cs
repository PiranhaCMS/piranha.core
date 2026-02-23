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
            CredentialId = "id1",
            PublicKey = new byte[] { 1, 2, 3 },
            SignCount = 1,
            UserHandle = "handle1"
        };

        // Act
        user.Passkeys.Add(passkey);

        // Assert
        Assert.Single(user.Passkeys);
        Assert.Equal("id1", user.Passkeys[0].CredentialId);
        Assert.Equal(new byte[] { 1, 2, 3 }, user.Passkeys[0].PublicKey);
        Assert.Equal(1, user.Passkeys[0].SignCount);
        Assert.Equal("handle1", user.Passkeys[0].UserHandle);
    }
}
