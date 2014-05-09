using System.Collections.Generic;
using System.IO;
using System.Web.Razor.Parser.SyntaxTree;

// TODO: Add minifier (AjaxMin) - Wrapper MinfiedTemplateCompiler

// TODO: Add Configuration of the name convention of templates to include - Predicate func<bool, string>

// TODO: Add VB support at some point (needs changes in the visitor)

namespace Dynamo.RazorTemplates
{
	public class TemplateCompiler : ITemplateCompiler
	{
		public TemplateCompiler()
		{
			// TODO: Take configuration of naming convention
		}
		
		public TextWriter Compile(TextReader input)
		{
			// TODO: Take TextWriter in instead - then there is no reason to return it? or what? or should it be set in the constructor instead?
			// TODO: Return a result wrapper that includes both state(bool-success?), TextWriter, ParseErrors, Parsed document and more ? -see parseResult 

			// Parse
			var parser = new RazorParser();
			var parseResult = parser.Parse(input);

			TextWriter output = new StringWriter();
			
			if (parseResult.Success)
			{
				var helpers = SelectValidHelpers(parseResult.Document);

				// Visit and Write all helpers to combined output
				foreach (var helper in helpers)
				{
					var visitor = new HelperVisitor(helper, output);
					visitor.Visit();
				}
			}

			return output;
		}

		private IEnumerable<HelperInfo> SelectValidHelpers(Block document)
		{
			// Go through the document and select all the valid helpers

			var helpers = new List<HelperInfo>();

			foreach (var node in document.Children)
			{
				if (node.IsBlock)
				{
					var block = (Block)node;
					if (block.Type == BlockType.Helper)
					{
						var helper = new HelperInfo(block);

						// Make sure the name of the helper is valid according to the naming convention
						// TODO: Should be specified by the config using a Func<bool, String>
						if (helper.Name.ToLowerInvariant().EndsWith("_tmpl"))
						{
							helpers.Add(helper);
						}
					}
				}		
			}

			return helpers;
		} 
	}
}
