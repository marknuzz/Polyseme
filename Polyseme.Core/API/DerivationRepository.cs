using Polyseme.Core.Derivations;
using VDS.RDF;

namespace Polyseme.Core
{
    public sealed class DerivationRepository
    {
        private readonly ICoreGraph _core;

        public DerivationRepository(ICoreGraph core)
        {
            _core = core;
        }

        public bool IsLosslesslyDerivedFrom(EntityRef result, EntityRef source)
        {
            var predicate = _core.Data.CreateUriNode("poly:losslesslyDerivedFrom");
            return _core.Data.ContainsTriple(new Triple(Node(result), predicate, Node(source)));
        }

        public IReadOnlyList<EntityRef> GetLosslessSources(EntityRef result)
        {
            var predicate = _core.Data.CreateUriNode("poly:losslesslyDerivedFrom");

            return _core.Data
                .GetTriplesWithSubjectPredicate(Node(result), predicate)
                .Select(x => x.Object)
                .OfType<IUriNode>()
                .Select(x => new EntityRef(x.Uri))
                .ToList();
        }

        private IUriNode Node(EntityRef entity) => _core.Data.CreateUriNode(entity.Uri);


        public LosslessPath? FindLosslessPath(EntityRef result, EntityRef source)
        {
            var visited = new HashSet<Uri>();
            var steps = new List<LosslessDerivationStep>();

            if (!FindLosslessPath(result, source, visited, steps))
                return null;

            return new LosslessPath(result, source, steps);
        }

        private bool FindLosslessPath(
            EntityRef current,
            EntityRef target,
            HashSet<Uri> visited,
            List<LosslessDerivationStep> steps)
        {
            if (current == target)
                return true;

            if (!visited.Add(current.Uri))
                return false;

            foreach (var step in GetDirectLosslessDerivations(current))
            {
                steps.Add(step);

                if (FindLosslessPath(step.Source, target, visited, steps))
                    return true;

                steps.RemoveAt(steps.Count - 1);
            }

            return false;
        }

        private IEnumerable<LosslessDerivationStep> GetDirectLosslessDerivations(EntityRef result)
        {
            var qualifiedDerivation = _core.Data.CreateUriNode("prov:qualifiedDerivation");
            var entity = _core.Data.CreateUriNode("prov:entity");
            var preservationStatus = _core.Data.CreateUriNode("poly:preservationStatus");
            var lossless = _core.Data.CreateUriNode("poly:Lossless");

            var resultNode = _core.Data.CreateUriNode(result.Uri);

            foreach (var qualifiedTriple in _core.Data.GetTriplesWithSubjectPredicate(resultNode, qualifiedDerivation))
            {
                if (qualifiedTriple.Object is not IUriNode derivation)
                    continue;

                if (!_core.Data.ContainsTriple(new Triple(derivation, preservationStatus, lossless)))
                    continue;

                foreach (var sourceTriple in _core.Data.GetTriplesWithSubjectPredicate(derivation, entity))
                {
                    if (sourceTriple.Object is not IUriNode source)
                        continue;

                    yield return new LosslessDerivationStep(
                        result,
                        new EntityRef(source.Uri),
                        new EntityRef(derivation.Uri));
                }
            }
        }
    }
}
