namespace Aero.Cms.Tests;

/// <summary>
/// This is only here to show how to use multiple fixtures in a collection.
/// There were tests that already had a collection definition attached to it
/// and this would break athe [Collection("Marten")] inheritance chain
/// </summary>
[CollectionDefinition("Integration tests")]
public class IntegrationTestCollection :
    ICollectionFixture<MartenFixture>
//ICollectionFixture<SomeOtherFixture>
{
}