using System;
using System.Web.Razor.Tokenizer.Symbols;

namespace Dynamo.RazorTemplates
{
	public static class SymbolHelper
	{
		public static int GetClosingParenthesisIndex(CSharpSymbol[] symbols, int startIndex)
		{
			var leftParenthesisCount = 0;

			for (int i = startIndex; i < symbols.Length; i++)
			{
				var symbol = symbols[i];

				if (symbol.Type == CSharpSymbolType.LeftParenthesis)
				{
					leftParenthesisCount++;
					continue;
				}

				if (symbol.Type == CSharpSymbolType.RightParenthesis)
				{
					if (leftParenthesisCount > 0)
						leftParenthesisCount--;
					else
						return i;	// found the closing one!

					continue;
				}
			}

			return -1;
		}

		public static Boolean IsPropertyCall(CSharpSymbol[] symbols, int targetIndex)
		{
			// Not possible to be a property as it must be preceeded by "." and "Identifier
			if (targetIndex < 2)
				return false;

			// Make sure it is an identifier
			if (symbols[targetIndex].Type != CSharpSymbolType.Identifier)
				return false;

			// Make sure that the symbol before it is a dot
			if (symbols[targetIndex - 1].Type != CSharpSymbolType.Dot)
				return false;

			// Make sure before the dot there was another identifier
			if (symbols[targetIndex - 2].Type != CSharpSymbolType.Identifier)
				return false;

			// Make sure that if there is a next symbol it is either a dot, semicolon or whitespace
			if (targetIndex + 1 < symbols.Length)
			{
				var nextSymbol = symbols[targetIndex + 1];
				if (!(symbols[targetIndex + 1].Type == CSharpSymbolType.Dot || symbols[targetIndex + 1].Type == CSharpSymbolType.Semicolon || nextSymbol.Type == CSharpSymbolType.WhiteSpace))
					return false;
			}

			return true;
		}

		public static Boolean IsMethodCall(CSharpSymbol[] symbols, int targetIndex)
		{
			var targetSymbol = symbols[targetIndex];

			// Make sure it is an identifier
			if (targetSymbol.Type != CSharpSymbolType.Identifier)
				return false;

			// target symbol is an identifier
			// Check that it is followed by a LeftParenthesis

			var parenthesesIndex = targetIndex + 1;

			// make sure that the index doesnt go out of bounds
			if (parenthesesIndex >= symbols.Length)
				return false;	// no left LeftParenthesis

			if (symbols[parenthesesIndex].Type == CSharpSymbolType.WhiteSpace)
				parenthesesIndex++; // adjust

			// If not a left parenthesis it is not a method call
			if (parenthesesIndex >= symbols.Length || symbols[parenthesesIndex].Type != CSharpSymbolType.LeftParenthesis)
				return false;

			return true;
		}

		public static Boolean IsHelperMethodCall(CSharpSymbol[] symbols, int targetIndex, String helperName, String methodCallName)
		{
			var targetSymbol = symbols[targetIndex];

			// Make sure it is an identifier
			if (targetSymbol.Type != CSharpSymbolType.Identifier)
				return false;

			// Check the name
			if (targetSymbol.Content != helperName)
				return false;

			// Make sure it is preceeded with a Dot ("Html.") 
			if (targetIndex + 1 >= symbols.Length || symbols[targetIndex + 1].Type != CSharpSymbolType.Dot)
				return false;

			if (targetIndex + 2 >= symbols.Length || !IsMethodCall(symbols, targetIndex + 2))
				return false;

			// Check name of method called
			if (symbols[targetIndex + 2].Content != methodCallName)
				return false;

			return true;
		}
	}
}
