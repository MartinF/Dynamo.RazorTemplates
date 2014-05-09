using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dynamo.RazorTemplates.Tests
{
	[TestClass]
	public class RazorParserTest
	{
		[TestMethod]
		public void CanParseSimpleTemplate()
		{
			var source = FileHelper.GetTemplateSource("Simple.cshtml");

			var parser = new RazorParser();

			var result = parser.Parse(source);

			Assert.IsNotNull(result);
			Assert.IsTrue(result.Success);
		}

		[TestMethod]
		public void CanParseSimpleStatementTemplate()
		{
			var source = FileHelper.GetTemplateSource("Simple-Statement.cshtml");

			var parser = new RazorParser();

			var result = parser.Parse(source);

			Assert.IsNotNull(result);
			Assert.IsTrue(result.Success);
		}
	}
}
