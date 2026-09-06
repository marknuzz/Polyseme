using Polyseme.Core;

var core = CoreGraph.Build();
core.PrintTriples(core.RdfsInferredTriples, "Schema Inferred Triples");
core.PrintTriples(core.RuleInferredTriples, "Rule Inferred Triples");

