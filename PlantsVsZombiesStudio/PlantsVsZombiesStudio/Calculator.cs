using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

using Jvav.CodeAnalysis;
using Jvav.CodeAnalysis.Syntax;

namespace PlantsVsZombiesStudio
{
    public class Calculator
    {
        private static Dictionary<VariableSymbol, object> _variable = new ();

        public static object Evaluate(string text)
        {
            SyntaxTree syntaxTree = SyntaxTree.Parse(text);
            Compilation compilation = new(syntaxTree);
            EvaluationResult result = compilation.Evaluate(_variable);
            ImmutableArray<Diagnostic> diagnostics = result.Diagnostics;
            if (diagnostics.Any())
            {
                StringBuilder builder = new();
                foreach(var diagnostic in diagnostics)
                {
                    builder.AppendLine(diagnostic.Message);
                }

                throw new Exception(builder.ToString());
            }

            return result.Value;
        }
    }
}
