using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Formatting;

namespace DateTimeNow
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DateTimeNowCodeFixProvider)), Shared]
    public class DateTimeNowCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds =>
            ImmutableArray.Create(DateTimeNowAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            // TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf()
                .OfType<MemberAccessExpressionSyntax>().First();
            var now = declaration.Name.ToString() == "Now" ? DateTime.Now : DateTime.UtcNow;

            // Register a code action that will invoke the fix.

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.CodeFixTitle,
                    createChangedDocument: c => ToObjectCreation(context.Document, declaration, now, c),
                    equivalenceKey: nameof(CodeFixResources.CodeFixTitle)),
                diagnostic);
        }

        private async Task<Document> ToObjectCreation(Document document, MemberAccessExpressionSyntax node,
            DateTime now, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var args = new List<ArgumentSyntax>()
            {
                SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                    SyntaxFactory.Literal(now.Year))),
                SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                    SyntaxFactory.Literal(now.Month))),
                SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                    SyntaxFactory.Literal(now.Day))),
                SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                    SyntaxFactory.Literal(now.Hour))),
                SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                    SyntaxFactory.Literal(now.Minute))),
                SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                    SyntaxFactory.Literal(now.Second))),
                SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                    SyntaxFactory.Literal(now.Millisecond)))
            };
            if (now.Kind == DateTimeKind.Utc)
            {
                args.Add(
                    SyntaxFactory.Argument(SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName(nameof(DateTimeKind)),
                        SyntaxFactory.IdentifierName(nameof(DateTimeKind.Utc)))));
            }

            var dateObjectCreatingSyntax = SyntaxFactory
                .ObjectCreationExpression(SyntaxFactory.ParseTypeName(nameof(DateTime)))
                .WithArgumentList(
                    SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList<ArgumentSyntax>(args))
                ).WithAdditionalAnnotations(Formatter.Annotation);

            var newRoot = root.ReplaceNode(node, dateObjectCreatingSyntax);


            return document.WithSyntaxRoot(newRoot);
        }
    }
}