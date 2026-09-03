using System.Collections.Immutable;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query.Inference;
using VDS.RDF.Shacl;
using VDS.RDF.Writing.Formatting;
using Graph = VDS.RDF.Graph;

namespace Polyseme.Core
{
    public sealed class CoreGraph
    {
        public Graph Schema { get; } = new Graph();
        public Graph Data { get; } = new Graph();
        public Graph Rules { get; } = new Graph();
        public ImmutableList<Triple> RdfsInferredTriples { get; private set; } = [];
        public ImmutableList<Triple> RuleInferredTriples { get; private set; } = [];

        private static ITripleFormatter TripleFormatter { get; }
        private CoreGraph() { }

        static CoreGraph()
        {
            string rdf = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";
            string rdfs = "http://www.w3.org/2000/01/rdf-schema#";
            var namespaces = new NamespaceMapper();
            namespaces.AddNamespace("ex", new Uri("http://example.org/"));
            namespaces.AddNamespace("poly", new Uri("http://example.org/polyseme#"));
            namespaces.AddNamespace("prov", new Uri("http://www.w3.org/ns/prov#"));
            namespaces.AddNamespace("rdf", new Uri(rdf));
            namespaces.AddNamespace("rdfs", new Uri(rdfs));

            TripleFormatter = new TurtleFormatter(namespaces);
        }

        public static CoreGraph Build()
        {
            var core = new CoreGraph();
            core.InitSchemas();
            core.InitData();
            core.InitRules();
            core.ApplyRdfsInference();
            core.ValidateData();
            core.ApplyRules(true);
            return core;
        }

        public void PrintTriples(IEnumerable<Triple> triples, string? name = null)
        {
            if (name is not null)
                Util.ConsoleLog($"{name}:");
            Util.ConsoleLog(string.Join(Environment.NewLine, triples.Select(x => x.ToString(TripleFormatter))));
            Util.ConsoleLog(string.Empty);
        }

        void InitSchemas()
        {
            Schema.BaseUri = new Uri("http://www.w3.org/ns/prov#");
            LoadInternalOntology(Schema, "ext.prov");

            Schema.BaseUri = new Uri("http://example.org/polyseme#");
            LoadInternalOntology(Schema, "polyseme");
        }

        void InitData()
        {
            Data.BaseUri = new Uri("http://example.org/");
            LoadInternalOntology(Data, "ex");
        }

        void InitRules()
        {
            Rules.LoadFromEmbeddedResource($"{Util.AssemblyName}.ontology.polyseme.n3,{Util.AssemblyName}", new Notation3Parser());
        }

        void ApplyRdfsInference()
        {
            var graph = Data;
            var reasoner = new StaticRdfsReasoner();
            reasoner.Initialise(Schema);

            List<Triple> newTriples = [];
            void onAsserted(object _, TripleEventArgs e) => newTriples.Add(e.Triple);
            graph.TripleAsserted += onAsserted;

            try
            {
                reasoner.Apply(graph);
                RdfsInferredTriples = RdfsInferredTriples.AddRange(newTriples);
            }
            finally
            {
                graph.TripleAsserted -= onAsserted;
            }
        }

        static void LoadInternalOntology(Graph graph, string name)
            => graph.LoadFromEmbeddedResource($"{Util.AssemblyName}.ontology.{name}.ttl,{Util.AssemblyName}");

        IReadOnlyList<Triple> ApplyRules(bool iterateToFixPoint)
        {
            var graph = Data;

            List<Triple> newTriples = [];
            void onAsserted(object _, TripleEventArgs e) => newTriples.Add(e.Triple);
            graph.TripleAsserted += onAsserted;

            try
            {
                var reasoner = new SimpleN3RulesReasoner();
                reasoner.Initialise(Rules);

                while (true)
                {
                    var before = graph.Triples.Count;
                    reasoner.Apply(graph);

                    if (iterateToFixPoint && graph.Triples.Count != before)
                        continue;

                    RuleInferredTriples = RuleInferredTriples.AddRange(newTriples);
                    return newTriples;
                }
            }
            finally
            {
                graph.TripleAsserted -= onAsserted;
            }
        }

        void ValidateData()
        {
            var shapes = new ShapesGraph(Schema);

            var report = shapes.Validate(Data);
            if (!report.Conforms)
            {
                throw new Exception(
                    $"Data failed validation!{Environment.NewLine}" +
                    string.Join(Environment.NewLine, report.Normalised.Triples));
            }
        }
    }
}
