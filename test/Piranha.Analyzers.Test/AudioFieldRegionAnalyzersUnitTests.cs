using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;
using Xunit;

namespace Piranha.Analyzers.Test
{
    public class AudioFieldRegionAnalyzersUnitTests : CodeFixVerifier
    {
        [Fact]
        public void NoDiagnosticIfNoCode()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void DiagnosticRegionAppliedToAudioFieldProperty()
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
            public AudioField Audio { get; set; }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "PA0001",
                Message = "AudioField should not be used as a single field region",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 10, 13)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Fact]
        public void NoDiagnosticAudioFieldInComplexRegion()
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
                public AudioField Audio { get; set; }
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
