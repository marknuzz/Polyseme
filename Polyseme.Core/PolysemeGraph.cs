using VDS.RDF;

namespace Polyseme.Core
{
    public class PolysemeGraph : Graph
    {
        public NamespaceMapper Namespaces { get; } = new NamespaceMapper();

        public PolysemeGraph()
        {
            PopulateNamespaces(NamespaceMap);
        }

        public static void PopulateNamespaces(INamespaceMapper mapper)
        {
            mapper.AddNamespace("ex", new Uri("http://example.org/"));
            mapper.AddNamespace("poly", new Uri("http://example.org/polyseme#"));
            mapper.AddNamespace("prov", new Uri("http://www.w3.org/ns/prov#"));
            mapper.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            mapper.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
        }
    }
}
