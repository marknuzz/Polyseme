using VDS.RDF;

namespace Polyseme.Core.Tests;

public static class TestHelpers
{
    public static void AssertTriple(IGraph graph, string subject, string predicate, string obj)
    {
        var triple = Triple(graph, subject, predicate, obj);
        Assert.True(graph.ContainsTriple(triple), $"Expected triple: {triple}");
    }

    public static void AssertNoTriple(IGraph graph, string subject, string predicate, string obj)
    {
        var triple = Triple(graph, subject, predicate, obj);
        Assert.False(graph.ContainsTriple(triple), $"Unexpected triple: {triple}");
    }

    public static Triple Triple(IGraph graph, string subject, string predicate, string obj)
    {
        return new Triple(
            graph.CreateUriNode(subject),
            graph.CreateUriNode(predicate),
            graph.CreateUriNode(obj));
    }
}