using System.IO;
using System.Web.Razor;

namespace Dynamo.RazorTemplates
{
	internal class RazorParser : IRazorParser
	{
		public RazorParserLanguage Language { get; set; }

		public ParserResults Parse(TextReader source)
		{
			// Language
			RazorCodeLanguage language = CreateLanguage();
			// Host
			var host = new RazorEngineHost(language);
			// Engine
			var engine = new RazorTemplateEngine(host);
			// Parse
			var result = engine.ParseTemplate(source);

			return result;
		}

		private RazorCodeLanguage CreateLanguage()
		{
			switch (Language)
			{
				case RazorParserLanguage.VisualBasic:
					return new VBRazorCodeLanguage();
				case RazorParserLanguage.CSharp:
				default:
					return new CSharpRazorCodeLanguage();
			}
		}
	}
}
