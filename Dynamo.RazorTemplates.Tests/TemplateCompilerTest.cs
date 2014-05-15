using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dynamo.RazorTemplates.Tests
{
	[TestClass]
	public class TemplateCompilerTest
	{
		[TestMethod]
		public void CanCompileSimpleTemplate()
		{
			var source = FileHelper.GetTemplateSource("Simple.cshtml");

			var compiler = new TemplateCompiler();
			var output = compiler.Compile(source);

			var outputResult = output.ToString();
			
			var expectedResult = "function Simple_Tmpl(){ var t=\"\"; t+=\"<h1>Hello - Simple</h1>\"; return t; }";

			Assert.AreEqual(expectedResult, outputResult);
		}

		[TestMethod]
		public void CanCompileSimpleStatementTemplate()
		{
			var source = FileHelper.GetTemplateSource("Simple-Statement.cshtml");

			var compiler = new TemplateCompiler();
			var output = compiler.Compile(source);

			var outputResult = output.ToString();

			var expectedResult = "function SimpleStatement_Tmpl(){ var t=\"\"; t+=\"<h1>Hello - \";t+=true ? \"Yes\" : \"No\";t+=\"</h1>\"; return t; }";

			Assert.AreEqual(expectedResult, outputResult);
		}

		[TestMethod]
		public void CanCompileParametersTemplate()
		{
			var source = FileHelper.GetTemplateSource("Parameters.cshtml");

			var compiler = new TemplateCompiler();
			var output = compiler.Compile(source);

			var outputResult = output.ToString();

			var expectedResult = "function Parameters_Tmpl(test,what){ var t=\"\"; t+=\"<h1>Extremly Simple - \";t+=test;t+=\" is \";t+=what;t+=\"</h1>\"; return t; }";

			Assert.AreEqual(expectedResult, outputResult);
		}

		[TestMethod]
		public void CanCompileLoopTemplate()
		{
			var source = FileHelper.GetTemplateSource("Loop.cshtml");

			var compiler = new TemplateCompiler();
			var output = compiler.Compile(source);

			var outputResult = output.ToString();

			var expectedResult = "function Loop_Tmpl(count){ var t=\"\"; for (var i = 0; i < count; i++){t+=\"<h1>Extremly Simple - \";t+=i;t+=\"</h1>\";} return t; }";

			Assert.AreEqual(expectedResult, outputResult);
		}

		[TestMethod]
		public void CanCompileMultipleTemplates()
		{
			var source = FileHelper.GetTemplateSource("Multiple.cshtml");

			var compiler = new TemplateCompiler();
			var output = compiler.Compile(source);

			var outputResult = output.ToString();

			var expectedResult = "function Multiple1_Tmpl(){ var t=\"\"; t+=\"<h1>Hello - 1</h1>\"; return t; }function Multiple2_Tmpl(){ var t=\"\"; t+=\"<h1>Hello - 2</h1>\"; return t; }";

			Assert.AreEqual(expectedResult, outputResult);
		}

		[TestMethod]
		public void CanCompileAdvancedTemplate()
		{
			var source = FileHelper.GetTemplateSource("Advanced.cshtml");

			var compiler = new TemplateCompiler();
			var output = compiler.Compile(source);

			var outputResult = output.ToString();

			var expectedResult = "function Advanced_Tmpl(count){ var t=\"\"; for (var i = 0; i < count; i++){t+=\"<h1>Extremly Simple - \";t+=i;t+=\"</h1>\";t+=\"<div>\";t+=i + 100 * 0.5;t+=\"</div>\";t+=i == 3 ? \"equals 3\" : \"not equal 3\" ;} return t; }";

			Assert.AreEqual(expectedResult, outputResult);
		}

		[TestMethod]
		public void RemovesNewLinesAndComments()
		{
			var source = FileHelper.GetTemplateSource("NewlineAndComments.cshtml");

			var compiler = new TemplateCompiler();
			var output = compiler.Compile(source);

			var outputResult = output.ToString();

			var expectedResult = "function NewLineAndComments_Tmpl(number){ var t=\"\"; t+=\"<h1>\";t+=number;t+=\"</h1>\"; return t; }";

			Assert.AreEqual(expectedResult, outputResult);
		}

		[TestMethod]
		public void FullTypeDeclarationsAreCompiledCorrectly()
		{
			var source = FileHelper.GetTemplateSource("FullTypeDeclaration.cshtml");

			var compiler = new TemplateCompiler();
			var output = compiler.Compile(source);

			var outputResult = output.ToString();

			var expectedResult = "function FullTypeDeclaration_Tmpl(test,what){ var t=\"\"; t+=\"<h1>Extremly Simple - \";t+=test;t+=\" is \";t+=what;t+=\"</h1>\"; return t; }";

			Assert.AreEqual(expectedResult, outputResult);
		}

		[TestMethod]
		public void MultipleAdvancedCompilesCorrectly()
		{
			var source = FileHelper.GetTemplateSource("MultipleAdvanced.cshtml");

			var compiler = new TemplateCompiler();
			var output = compiler.Compile(source);

			var outputResult = output.ToString();

			var expectedResult = "function Pager_Tmpl(pages,currentPage){ var t=\"\"; t+=\"<ol\";t+=\" class=\\\"pager\\\"\";t+=\">\";t+=PagerItem_Tmpl(\"<\", (currentPage > 1 ? \"?page=\" + (currentPage - 1) : null));for (var i = 1; i <= pages; i++){t+=PagerItem_Tmpl(i.toString(), (i != currentPage ? \"?page=\" + i : null));}t+=PagerItem_Tmpl(\">\", (currentPage < pages ? \"?page=\" + (currentPage + 1) : null));t+=\"</ol>\"; return t; }function PagerItem_Tmpl(text,url){ var t=\"\"; t+=\"<li><a \";t+=url == null ? \"\" : \"href=\\\"\"+ url + \"\\\"\";t+=\">\";t+=text;t+=\"</a></li>\"; return t; }";

			Assert.AreEqual(expectedResult, outputResult);
		}
	}
}
