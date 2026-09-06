using VDS.RDF;

namespace Polyseme.Core.Tests
{
    internal sealed class FakeCoreGraph : ICoreGraph
    {
        public IGraph Schema { get; } = new PolysemeGraph();
        public IGraph Data { get; } = new PolysemeGraph();
        public IGraph Rules { get; } = new PolysemeGraph();

        public IReadOnlyList<Triple> RdfsInferredTriples { get; } = [];
        public IReadOnlyList<Triple> RuleInferredTriples { get; } = [];
    }
}
