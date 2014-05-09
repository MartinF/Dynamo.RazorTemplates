using System.IO;
using System.Text;

namespace Dynamo.RazorTemplates
{
	public interface ITemplateCompiler
	{
		TextWriter Compile(TextReader input);
	}
}
