using VDS.RDF;

namespace Polyseme.Core
{
    public readonly record struct EntityRef(Uri Uri)
    {
        public static EntityRef From(string uri) => new(new Uri(uri));

        public static EntityRef FromQName(IGraph graph, string qname)
            => new(graph.CreateUriNode(qname).Uri);
    }
}
