using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Document = Microsoft.CodeAnalysis.Document;

namespace DateTimeNow
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(DateTimeRefactoring)), Shared]
    internal class DateTimeRefactoring : CodeRefactoringProvider
    {
        public sealed override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root =
                await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var nodes = root.FindNode(context.Span).DescendantNodes().OfType<MemberAccessExpressionSyntax>();
            var now = DateTime.Now;

            foreach (var node in nodes)

            {
                if (node.ToString() == "DateTime.Now")
                {
                    context.RegisterRefactoring(CodeAction.Create("Create DateTime",
                        c => ReplaceWithCurrentDateAsync(context.Document, node, now, c)));

                    var str = node.ToString();
                }
            }
        }

        private async Task<Document> ReplaceWithCurrentDateAsync(Document document, MemberAccessExpressionSyntax node,
            DateTime now, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
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
            var dateObjectCreatingSyntax = SyntaxFactory.ObjectCreationExpression(SyntaxFactory.ParseTypeName(nameof(DateTime)))
                .WithArgumentList(
                    SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList<ArgumentSyntax>(args))
                );

            var newRoot = root.ReplaceNode(node, dateObjectCreatingSyntax);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}