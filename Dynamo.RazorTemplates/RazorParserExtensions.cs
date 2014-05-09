using System;
using System.IO;
using System.Web.Razor;

namespace Dynamo.RazorTemplates
{
	public static class RazorParserExtensions
	{
		public static ParserResults Parse(this IRazorParser parser, String source)
		{
			using (TextReader reader = new StringReader(source))
			{
				return parser.Parse(reader);
			}
		}
	}
}
