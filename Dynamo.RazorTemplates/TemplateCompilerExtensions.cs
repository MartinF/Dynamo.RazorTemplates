using System;
using System.IO;
using System.Text;

namespace Dynamo.RazorTemplates
{
	public static class TemplateCompilerExtensions
	{
		public static TextWriter Compile(this ITemplateCompiler templateCompiler, String source)
		{
			using (TextReader reader = new StringReader(source))
			{
				return templateCompiler.Compile(reader);
			}
		}
	}
}
