using Xunit;

namespace web.Tests.FunctionTests;

public class GroupsIndexTests : IClassFixture<TestDatabaseFixture>
{
    public GroupsIndexTests(TestDatabaseFixture fixture) => Fixture = fixture;

    public TestDatabaseFixture Fixture { get; }
}
