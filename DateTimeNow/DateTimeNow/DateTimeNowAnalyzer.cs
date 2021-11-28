using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace DateTimeNow
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DateTimeNowAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "DateTimeNow";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(
            nameof(Resources.AnalyzerDescription),
            Resources.ResourceManager,
            typeof(Resources));
        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Info,
            true,
            Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.SimpleMemberAccessExpression);
        }
        private static readonly string[] InvalidTokens =
        {
            "System.DateTime.Now",
            "System.DateTime.UtcNow"
        };
        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var name = (context.Node as MemberAccessExpressionSyntax)?.Name.ToString();
            if (name == "Now" || name == "UtcNow")
            {
                var symbol = context.SemanticModel.GetSymbolInfo(context.Node).Symbol as IPropertySymbol;
                var symbolText = symbol?.ToString();
                if (InvalidTokens.Contains(symbolText))
                {
                    var diagnostic = Diagnostic.Create(Rule, context.Node.GetLocation(), Description);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }


    }
}
