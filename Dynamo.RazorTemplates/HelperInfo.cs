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
			// All identifiers except the first are parameters ?
			return infoSpan.Symbols.Cast<CSharpSymbol>().Where(x => x.Type == CSharpSymbolType.Identifier).Skip(1).Select(x => x.Content);
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
