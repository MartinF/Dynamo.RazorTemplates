using System;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Web.Razor.Parser;
using System.Web.Razor.Parser.SyntaxTree;
using System.Web.Razor.Tokenizer.Symbols;

// Uses the visitor pattern to compile
// Not thread-safe

// TODO: Make it possible to configure whether it should use global or scoped function declarations - function name() {} vs. var name = function() {}
// TODO: Make it possible to create a "namespace" for the functions

namespace Dynamo.RazorTemplates
{
	internal class HelperVisitor
	{
		// Constructor
		public HelperVisitor(HelperInfo helperInfo, TextWriter writer = null)
		{
			if (helperInfo == null)
				throw new ArgumentNullException("helperInfo");

			HelperInfo = helperInfo;
			Output = writer ?? new StringWriter();
		}

		// Properties
		public HelperInfo HelperInfo { get; private set; }
		public TextWriter Output { get; private set; }

		// Methods
		public void Visit()
		{
			// Write Start
			var functionName = StringHelper.ConvertFirstLetterToLowerCase(HelperInfo.Name);
			Write("function " + functionName + "(");
			var parameters = String.Join(",", HelperInfo.GetParameterNames());
			Write(parameters);
			Write("){ var t=\"\"; ");

			// Write All the content
			foreach (var statementBlock in HelperInfo.GetStatementBlocks())
			{
				Visit(statementBlock);
			}

			// Write End
			Write(" return t; }");
		}

		private void Visit(Block block)
		{
			// Visit all children
			foreach (var child in block.Children)
			{
				Visit(child);
			}
		}

		private void Visit(SyntaxTreeNode node)
		{
			if (node.IsBlock)
			{
				Visit((Block)node);
			}
			else
			{
				// If not block must be span
				Visit((Span)node);
			}
		}

		private void Visit(Span span)
		{
			switch (span.Kind)
			{
				case SpanKind.Code:
					WriteCode(span);
					break;
				case SpanKind.Comment:
					WriteComment(span);
					break;
				case SpanKind.Markup:
					WriteMarkup(span);
					break;
				case SpanKind.MetaCode:
					WriteMetaCode(span);
					break;
				case SpanKind.Transition:
					WriteTransition(span);
					break;
				default:
					throw new NotSupportedException();	//	new NotImplementedException()
			}
		}

		private void WriteComment(Span span)
		{
			// Ignore
		}

		private void WriteMetaCode(Span span)
		{
			// Ignore 
			// helper in @helper etc. 
		}

		private void WriteTransition(Span span)
		{
			// Ignore
			// @
		}

		private void WriteCode(Span span)
		{
			var symbols = span.Symbols.Cast<CSharpSymbol>().ToArray();

			var output = SymbolHelper.ProcessSymbols(symbols);

			if (span.Parent.Type == BlockType.Statement) // eg. for loop
			{
				Write(output.ToString());
			}
			else
			{
				// All other - eg. span.Parent.Type == BlockType.Expression
				WriteToVariabel(output.ToString());				
			}
		}

		private void WriteMarkup(Span span)
		{
			var content = new StringBuilder();

			foreach (var symbol in span.Symbols.Cast<HtmlSymbol>())
			{
				// Ignore NewLine's
				if (symbol.Type == HtmlSymbolType.NewLine)
				{
					continue;
				}

				// Remove Control Characters
				if (symbol.Type == HtmlSymbolType.WhiteSpace)
				{
					var filteredContent = StringHelper.RemoveControlCharacters(symbol.Content);
					content.Append(filteredContent);

					continue;
				}

				// Fix Double quotes
				if (symbol.Type == HtmlSymbolType.DoubleQuote)
				{
					var fixedContent = StringHelper.FixDoubleQuotes(symbol.Content);
					content.Append(fixedContent);

					continue;
				}

				content.Append(symbol.Content);
			}

			if (content.Length > 0)
			{
				// Wrap in quotes
				content.Insert(0, "\"");
				content.Append("\"");

				WriteToVariabel(content.ToString());			
			}
		}

		private void Write(String content)
		{
			Output.Write(content);
		}

		private void WriteToVariabel(String content)
		{
			if (content.Length > 0)
				Write("t+=" + content + ";");
		}
	}
}
