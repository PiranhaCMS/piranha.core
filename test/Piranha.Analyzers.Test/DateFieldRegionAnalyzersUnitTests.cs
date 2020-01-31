using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;
using Xunit;

namespace Piranha.Analyzers.Test
{
    public class DateFieldRegionAnalyzersUnitTests : CodeFixVerifier
    {
        [Fact]
        public void NoDiagnosticIfNoCode()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void DiagnosticRegionAppliedToDateFieldProperty()
        {
            var test = @"
    using Piranha.Extend;
    using Piranha.Extend.Fields;
    using Piranha.Models;

    namespace ConsoleApplication1
    {
        class TypeName : Post<TypeName>
        {
            [Region]
            public DateField Date { get; set; }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "PA0001",
                Message = "DateField should not be used as a single field region",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 10, 13)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Fact]
        public void NoDiagnosticDateFieldInComplexRegion()
        {
            var test = @"
    using Piranha.Extend;
    using Piranha.Extend.Fields;
    using Piranha.Models;

    namespace ConsoleApplication1
    {
        class TypeName : Post<TypeName>
        {
            [Region]
            public ContentRegion Content { get; set; }

            public class ContentRegion
            {
                [Field]
                public DateField Date { get; set; }
            }
        }
    }";

            VerifyCSharpDiagnostic(test);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return null;
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new NonSingleFieldRegionAnalyzer();
        }
    }
}
