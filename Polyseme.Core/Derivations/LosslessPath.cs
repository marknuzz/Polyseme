namespace Polyseme.Core.Derivations
{
    public sealed record LosslessDerivationStep(
        EntityRef Result,
        EntityRef Source,
        EntityRef Derivation);

    public sealed record LosslessPath(
        EntityRef Result,
        EntityRef Source,
        IReadOnlyList<LosslessDerivationStep> Steps);
}
