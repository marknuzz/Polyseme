using VDS.RDF;

namespace Polyseme.Core.Tests;

public sealed class DerivationRepositoryTests : GraphTest
{
    private readonly DerivationRepository _sut;

    public DerivationRepositoryTests() : base()
    {
        _sut = new DerivationRepository(Core);
    }


    [Fact]
    public void IsLosslesslyDerivedFrom_ReturnsTrue_WhenTripleExists()
    {
        var wrapped = Core.Data.CreateUriNode("ex:wrappedFile");
        var original = Core.Data.CreateUriNode("ex:originalFile");
        var predicate = Core.Data.CreateUriNode("poly:losslesslyDerivedFrom");

        Core.Data.Assert(new Triple(wrapped, predicate, original));

        var result = _sut.IsLosslesslyDerivedFrom(Entity("ex:wrappedFile"), Entity("ex:originalFile"));

        Assert.True(result);
    }

    [Fact]
    public void IsLosslesslyDerivedFrom_ReturnsFalse_WhenTripleDoesNotExist()
    {
        var result = _sut.IsLosslesslyDerivedFrom(
            Entity("ex:compressedHash"),
            Entity("ex:originalFile"));

        Assert.False(result);
    }

    [Fact]
    public void FindLosslessPath_ReturnsQualifiedDerivationChain()
    {
        var repository = new DerivationRepository(Core);

        var path = repository.FindLosslessPath(
            Entity("ex:wrappedFile"),
            Entity("ex:originalFile"));

        Assert.NotNull(path);
        Assert.Collection(
            path.Steps,
            x => AssertEntity("ex:wrapDerivation", x.Derivation),
            x => AssertEntity("ex:base64Derivation", x.Derivation),
            x => AssertEntity("ex:gzipDerivation", x.Derivation));
    }
}
