using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Erni.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ParameterNullGuardAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "ERNI0001";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Erni.Design";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCodeBlockAction(AnalyzeBlock);
        }

        private static void AnalyzeBlock(CodeBlockAnalysisContext context)
        {
            if (context.OwningSymbol.Kind == SymbolKind.Method)
            {
                var methodBlock = context.CodeBlock as MethodDeclarationSyntax;
                if (methodBlock != null)
                {
                    AnalyzeStatements(context, methodBlock.Body.Statements);
                }
                System.Diagnostics.Debug.WriteLine("Main");
            }
        }

        private static void AnalyzeStatements(CodeBlockAnalysisContext context, SyntaxList<StatementSyntax> statements)
        {
            foreach (var statement in statements)
            {
                var ifStatement = statement as IfStatementSyntax;
                if (ifStatement == null)
                {
                    break;
                }

                AnalyzeIfStatement(context, ifStatement);
            }

        }

        private static void AnalyzeIfStatement(CodeBlockAnalysisContext context, IfStatementSyntax ifStatement)
        {
            var condition = ifStatement.Condition as BinaryExpressionSyntax;
            if (condition != null)
            {
                var diagnostic = Diagnostic.Create(Rule, ifStatement.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
