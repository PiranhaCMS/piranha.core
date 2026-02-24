using Raven.Client.Documents.Session;

namespace Piranha.Tests;

public class TestDb : Db<TestDb>
{
    public TestDb(IAsyncDocumentSession session) : base(session) { }
}
