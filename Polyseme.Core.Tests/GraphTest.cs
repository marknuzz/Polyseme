using VDS.RDF;

namespace Polyseme.Core.Tests;

public abstract class GraphTestBase
{
    protected ICoreGraph Core { get; }

    protected GraphTestBase(ICoreGraph core)
    {
        Core = core;
    }

    protected void AssertTriple(string subject, string predicate, string obj)
        => TestHelpers.AssertTriple(Core.Data, subject, predicate, obj);

    protected void AssertNoTriple(string subject, string predicate, string obj)
        => TestHelpers.AssertNoTriple(Core.Data, subject, predicate, obj);

    protected Triple Triple(string subject, string predicate, string obj)
        => TestHelpers.Triple(Core.Data, subject, predicate, obj);

    protected void AssertEntity(string qname, EntityRef actual)
        => Assert.Equal(Core.Data.CreateUriNode(qname).Uri, actual.Uri);

    protected EntityRef Entity(string qname) => EntityRef.FromQName(Core.Data, qname);
}

public abstract class GraphTest : GraphTestBase
{
    protected GraphTest() : base(CoreGraph.Build())
    {
    }
}

public abstract class GraphTestThin : GraphTestBase
{
    protected GraphTestThin() : base(new FakeCoreGraph())
    {
    }
}
