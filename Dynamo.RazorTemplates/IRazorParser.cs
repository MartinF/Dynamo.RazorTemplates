using System.IO;
using System.Web.Razor;

namespace Dynamo.RazorTemplates
{
	public interface IRazorParser
	{
		RazorParserLanguage Language { get; set; }

		ParserResults Parse(TextReader source);
	}
}
