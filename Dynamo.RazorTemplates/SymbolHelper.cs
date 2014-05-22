using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Razor.Tokenizer.Symbols;

// Make a SplitBySymbol method ?
// GetSymbolBeforeType or GetSymbolTypeBeforeType method?

namespace Dynamo.RazorTemplates
{
	public static class SymbolHelper
	{
		// Fields
		private static int _varNameCounter = 0;
		
		// Methods
		public static StringBuilder ProcessSymbols(CSharpSymbol[] symbols)
		{
			var output = new StringBuilder();

			for (int i = 0; i < symbols.Length; i++)
			{
				var symbol = symbols[i];

				// Ignore NewLine's and comments
				if (symbol.Type == CSharpSymbolType.NewLine ||
					symbol.Type == CSharpSymbolType.Comment ||
					symbol.Type == CSharpSymbolType.RazorComment)
				{
					continue;
				}

				// Remove Control Characters
				if (symbol.Type == CSharpSymbolType.WhiteSpace)
				{
					var filteredContent = StringHelper.RemoveControlCharacters(symbol.Content);

					output.Append(filteredContent);
					continue;
				}

				// If Html.Raw method is called remove it
				if (SymbolHelper.IsHelperMethodCall(symbols, i, "Html", "Raw"))
				{
					// Get symbols within parenthsis
					int closingIndex;
					var methodSymbols = GetSymbolsWithinNextParenthesis(symbols, i, out closingIndex).ToArray();

					// Process method symbols
					var builder = ProcessSymbols(methodSymbols);
					output.Append(builder);

					// continue from closing parenthesis
					i = closingIndex;

					continue;
				}

				if (SymbolHelper.IsMethodCall(symbols, i) || SymbolHelper.IsPropertyCall(symbols, i))
				{
					// fix first letter to lowercase
					var name = symbol.Content;
					var fixedName = StringHelper.ConvertFirstLetterToLowerCase(name);

					output.Append(fixedName);
					continue;
				}

				// Support Foreach
				if (SymbolHelper.IsForeachLoop(symbols, i))
				{
					// 3 Options
					// enumerableVar.forEach(function(innerVar) { ... - requires it is possible to wrap foreach inner content and end it with an right parenthesis ")"
					// for (i = 0; i < enumerableVar.length; i++) { innerVar = variableIn[i]; ....
					// for (var key in enumerableVar) { innerVar = enumerableVar[key]; ....

					String innerVar;
					String enumerableVar;
					int closingIndex;
					SymbolHelper.GetForeachInfo(symbols, i, out innerVar, out enumerableVar, out closingIndex);

					var iVar = GetUniqueVarName();
					var result = "for(var " + iVar + "=0;" + iVar + "<" + enumerableVar + ".length;" + iVar + "++){ var " + innerVar + "=" + enumerableVar + "[" + iVar + "];";
					output.Append(result);

					var startBrace = GetIndexOfNext(symbols, closingIndex, CSharpSymbolType.LeftBrace);

					// Continue from the starting brace
					i = startBrace;

					continue;
				}

				//Any other just append content
				output.Append(symbol.Content);
			}

			return output;
		}

		private static String GetUniqueVarName()
		{
			var name = "__i" + _varNameCounter;
			_varNameCounter ++;
			return name;
		}

		public static int GetIndexOfClosingParenthesis(CSharpSymbol[] symbols, int leftParenthesisIndex)
		{
			if (symbols[leftParenthesisIndex].Type != CSharpSymbolType.LeftParenthesis)
				throw new ArgumentException("leftParenthesisIndex doesn't refer to a LeftParenthesis", "leftParenthesisIndex");
			
			var leftParenthesisCount = 0;

			for (int i = leftParenthesisIndex + 1; i < symbols.Length; i++)
			{
				var symbol = symbols[i];

				if (symbol.Type == CSharpSymbolType.LeftParenthesis)
				{
					// an extra left parenthesis was opened
					leftParenthesisCount++;
					continue;
				}

				if (symbol.Type == CSharpSymbolType.RightParenthesis)
				{
					// if no additional left parenthesis is opend this must be the closing of the first one
					if (leftParenthesisCount == 0)
						return i;

					// Count down as it must have closed a left parenthesis
					leftParenthesisCount--;
				}
			}

			return -1;
		}
		
		public static int GetIndexOfNext(CSharpSymbol[] symbols, int startIndex, CSharpSymbolType type, String content = null)
		{
			for (var i = startIndex; i < symbols.Length; i++)
			{
				if (symbols[i].Type == type && (content == null || symbols[i].Content == content))
					return i;
			}

			return -1;
		}

		public static IEnumerable<CSharpSymbol> GetSymbolsWithinNextParenthesis(CSharpSymbol[] symbols, int startIndex, out int closingParenthesisIndex)
		{
			var leftParenthesis = GetIndexOfNext(symbols, startIndex, CSharpSymbolType.LeftParenthesis);
			var rightParenthesis = GetIndexOfClosingParenthesis(symbols, leftParenthesis);

			closingParenthesisIndex = rightParenthesis;

			return symbols.Skip(leftParenthesis + 1).Take(rightParenthesis - (leftParenthesis + 1));
		}

		public static Boolean IsForeachLoop(CSharpSymbol[] symbols, int index)
		{
			return symbols[index].Type == CSharpSymbolType.Keyword && symbols[index].Content == "foreach";
		}

		public static void GetForeachInfo(CSharpSymbol[] symbols, int index, out String innerVar, out String enumerableVar, out int closingIndex)
		{
			// Make sure symbols[index] is foreach ? IsForeachLoop() ?

			// Get all the symbols within the method parenthesis
			int closingParenthesisIndex;
			var foreachSymbols = GetSymbolsWithinNextParenthesis(symbols, index, out closingParenthesisIndex);

			// Remove Whitespace from the symbols
			var foreachSymbolsFiltered = foreachSymbols.Where(x => x.Type != CSharpSymbolType.WhiteSpace).ToArray();
			
			// Find the index of the "in" keyword
			var inKeywordIndex = GetIndexOfNext(foreachSymbolsFiltered, 0, CSharpSymbolType.Keyword, "in");
			
			// Symbol left of "in" keyword is the variable name
			innerVar = foreachSymbolsFiltered[inKeywordIndex - 1].Content;

			// Get all symbols on the right side of the "in" keyword
			var rightSideSymbols = foreachSymbolsFiltered.Skip(inKeywordIndex + 1).ToArray();
			
			// Process all the symbols on the right side - eg. if varName.Model.Something it needs to be converted to varName.model.something
			var builder = ProcessSymbols(rightSideSymbols);
			enumerableVar = builder.ToString();

			// TODO: What to call this !? It is the closing parenthesis of the foreach() - before the foreach inner ?
			closingIndex = closingParenthesisIndex;
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
