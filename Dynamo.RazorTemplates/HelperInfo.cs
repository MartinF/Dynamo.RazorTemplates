using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Razor.Parser.SyntaxTree;
using System.Web.Razor.Tokenizer.Symbols;

namespace Dynamo.RazorTemplates
{
	public class HelperInfo
	{
		public HelperInfo(Block helperBlock)
		{
			if (helperBlock.Type != BlockType.Helper)
				throw new ArgumentException("helperBlock is not of type helper", "helperBlock");

			Block = helperBlock;
			Name = GetHelperName();
		}

		public Block Block { get; private set; }
		public String Name { get; private set; }

		// Helper methods
		public IEnumerable<Block> GetStatementBlocks()
		{
			// TODO: Is there always only one or multiple - for now assume multiple just to be on the safe side.

			return Block.Children.Where(x => x.IsBlock)
				.Cast<Block>()
				.Where(x => x.Type == BlockType.Statement);
		}

		public IEnumerable<String> GetParameterNames()
		{
			var infoSpan = GetHelperInfoSpan();

			var sort1 = infoSpan.Symbols.Cast<CSharpSymbol>();

			// Get all of type Identifier, Keyword or Comma
			var sort2 = sort1.Where(x => x.Type == CSharpSymbolType.Identifier || x.Type == CSharpSymbolType.Keyword || x.Type == CSharpSymbolType.Comma);

			// Always skip the first one as it is the helper declaration
			var sort3 = sort2.Skip(1);

			// Keep selecting the last symbol seperated by comma
			String paramName = "";
			foreach (var symbol in sort3)
			{
				if (symbol.Type == CSharpSymbolType.Comma)
					yield return paramName;

				paramName = symbol.Content;
			}

			// When it has reached the end make sure to return the last found parameter
			yield return paramName;
		}

		private Span GetHelperInfoSpan()
		{
			// First Span of Kind == SpanKind.Code
			//return (Span)Block.Children.ToArray()[2];
			return Block.Children.Where(x => !x.IsBlock).Cast<Span>().First(x => x.Kind == SpanKind.Code);
		}

		private String GetHelperName()
		{
			var nameSpan = GetHelperInfoSpan();
			// First identifier is the name
			return nameSpan.Symbols.Cast<CSharpSymbol>().First(x => x.Type == CSharpSymbolType.Identifier).Content;
		}
	}
}
