using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Piranha.Extend;
using Piranha.Extend.Fields;

namespace Piranha.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NonSingleFieldRegionAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "PA0001";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.NonSingleFieldRegionAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.NonSingleFieldRegionAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.NonSingleFieldRegionAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            // List of built-in field types that, per documentation, is primarily intended for complex regions.
            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode<AudioField>, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode<CheckBoxField>, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode<DateField>, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode<DocumentField>, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode<ImageField>, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode<MediaField>, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode<NumberField>, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode<PageField>, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode<PostField>, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode<StringField>, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode<VideoField>, SyntaxKind.PropertyDeclaration);
        }

        private static void AnalyzeSyntaxNode<T>(SyntaxNodeAnalysisContext context) where T : IField
        {
            if (!(context.Node is PropertyDeclarationSyntax pds))
            {
                return;
            }

            // Verify that the property is of type T.
            var fieldTypeSymbol = context.Compilation.GetTypeByMetadataName(typeof(T).FullName);
            var s = context.SemanticModel.GetTypeInfo(pds.Type, context.CancellationToken);
            
            if (!s.Type.Equals(fieldTypeSymbol))
            {
                return;
            }

            // Verify that the property is marked with RegionAttribute.
            var regionAttributeSymbol = context.Compilation.GetTypeByMetadataName(typeof(RegionAttribute).FullName);
            if (!pds.AttributeLists.Any(atts => atts.Attributes.Any(a => context.SemanticModel.GetTypeInfo(a, context.CancellationToken).Type.Equals(regionAttributeSymbol))))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, pds.GetLocation(), typeof(T).Name));
        }
    }
}
