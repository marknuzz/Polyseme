namespace Polyseme.Core.Tests;

public sealed class OntologyInferenceTests : GraphTest
{
    [Fact]
    public void DirectLosslessDerivation_IsInferred()
    {
        AssertTriple("ex:compressedFile", "poly:losslesslyDerivedFrom", "ex:originalFile");
    }

    [Fact]
    public void LosslessDerivations_ComposeTransitively()
    {
        AssertTriple("ex:wrappedFile", "poly:losslesslyDerivedFrom", "ex:base64File");
        AssertTriple("ex:wrappedFile", "poly:losslesslyDerivedFrom", "ex:compressedFile");
        AssertTriple("ex:wrappedFile", "poly:losslesslyDerivedFrom", "ex:originalFile");
    }

    [Fact]
    public void LossyDerivation_DoesNotProduceLosslessRelation()
    {
        AssertNoTriple("ex:compressedHash", "poly:losslesslyDerivedFrom", "ex:compressedFile");
    }

    [Fact]
    public void LossyDerivation_BlocksLosslessComposition()
    {
        AssertTriple("ex:encodedHash", "poly:losslesslyDerivedFrom", "ex:compressedHash");
        AssertNoTriple("ex:encodedHash", "poly:losslesslyDerivedFrom", "ex:compressedFile");
        AssertNoTriple("ex:encodedHash", "poly:losslesslyDerivedFrom", "ex:originalFile");
    }

    [Fact]
    public void LosslessRelation_IsDirectional()
    {
        AssertTriple("ex:compressedFile", "poly:losslesslyDerivedFrom", "ex:originalFile");
        AssertNoTriple("ex:originalFile", "poly:losslesslyDerivedFrom", "ex:compressedFile");
    }

    [Fact]
    public void TransitiveTriple_IsActuallyRuleInferred()
    {
        var triple = Triple("ex:wrappedFile", "poly:losslesslyDerivedFrom", "ex:originalFile");

        Assert.Contains(triple, Core.RuleInferredTriples);
    }

    [Fact]
    public void DirectLosslessTriple_IsActuallyRuleInferred()
    {
        var triple = Triple("ex:compressedFile", "poly:losslesslyDerivedFrom", "ex:originalFile");

        Assert.Contains(triple, Core.RuleInferredTriples);
    }

    [Fact]
    public void ProvDomainAndRange_InferEntityTypes()
    {
        AssertTriple("ex:originalFile", "rdf:type", "prov:Entity");
        AssertTriple("ex:compressedFile", "rdf:type", "prov:Entity");
    }

    [Fact]
    public void QualifiedDerivation_IsRecognizedAsProvDerivation()
    {
        AssertTriple("ex:gzipDerivation", "rdf:type", "prov:Derivation");
    }

    [Fact]
    public void WrappedFile_HasExpectedQualifiedDerivation()
    {
        AssertTriple("ex:wrappedFile", "prov:qualifiedDerivation", "ex:wrapDerivation");
        AssertTriple("ex:wrapDerivation", "prov:entity", "ex:base64File");
        AssertTriple("ex:wrapDerivation", "poly:preservationStatus", "poly:Lossless");

        AssertTriple("ex:base64File", "prov:qualifiedDerivation", "ex:base64Derivation");
        AssertTriple("ex:base64Derivation", "prov:entity", "ex:compressedFile");
        AssertTriple("ex:base64Derivation", "poly:preservationStatus", "poly:Lossless");

        AssertTriple("ex:compressedFile", "prov:qualifiedDerivation", "ex:gzipDerivation");
        AssertTriple("ex:gzipDerivation", "prov:entity", "ex:originalFile");
        AssertTriple("ex:gzipDerivation", "poly:preservationStatus", "poly:Lossless");
    }
}
