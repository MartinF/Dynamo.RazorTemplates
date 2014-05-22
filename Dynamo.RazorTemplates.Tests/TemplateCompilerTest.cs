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
			
			var expectedResult = "function simple_Tmpl(){ var t=\"\"; t+=\"<h1>Hello - Simple</h1>\"; return t; }";

			Assert.AreEqual(expectedResult, outputResult);
		}

		[TestMethod]
		public void CanCompileSimpleStatementTemplate()
		{
			var source = FileHelper.GetTemplateSource("Simple-Statement.cshtml");

			var compiler = new TemplateCompiler();
			var output = compiler.Compile(source);

			var outputResult = output.ToString();

			var expectedResult = "function simpleStatement_Tmpl(){ var t=\"\"; t+=\"<h1>Hello - \";t+=true ? \"Yes\" : \"No\";t+=\"</h1>\"; return t; }";

			Assert.AreEqual(expectedResult, outputResult);
		}

		[TestMethod]
		public void CanCompileParametersTemplate()
		{
			var source = FileHelper.GetTemplateSource("Parameters.cshtml");

			var compiler = new TemplateCompiler();
			var output = compiler.Compile(source);

			var outputResult = output.ToString();

			var expectedResult = "function parameters_Tmpl(test,what){ var t=\"\"; t+=\"<h1>Extremly Simple - \";t+=test;t+=\" is \";t+=what;t+=\"</h1>\"; return t; }";

			Assert.AreEqual(expectedResult, outputResult);
		}

		[TestMethod]
		public void CanCompileMultipleTemplates()
		{
			var source = FileHelper.GetTemplateSource("Multiple.cshtml");

			var compiler = new TemplateCompiler();
			var output = compiler.Compile(source);

			var outputResult = output.ToString();

			var expectedResult = "function multiple1_Tmpl(){ var t=\"\"; t+=\"<h1>Hello - 1</h1>\"; return t; }function multiple2_Tmpl(){ var t=\"\"; t+=\"<h1>Hello - 2</h1>\"; return t; }";

			Assert.AreEqual(expectedResult, outputResult);
		}

		[TestMethod]
		public void CanCompileAdvancedTemplate()
		{
			var source = FileHelper.GetTemplateSource("Advanced.cshtml");

			var compiler = new TemplateCompiler();
			var output = compiler.Compile(source);

			var outputResult = output.ToString();

			var expectedResult = "function advanced_Tmpl(count){ var t=\"\"; for (var i = 0; i < count; i++){t+=\"<h1>Extremly Simple - \";t+=i;t+=\"</h1>\";t+=\"<div>\";t+=i + 100 * 0.5;t+=\"</div>\";t+=i == 3 ? \"equals 3\" : \"not equal 3\" ;} return t; }";

			Assert.AreEqual(expectedResult, outputResult);
		}

		[TestMethod]
		public void RemovesNewLinesAndComments()
		{
			var source = FileHelper.GetTemplateSource("NewlineAndComments.cshtml");

			var compiler = new TemplateCompiler();
			var output = compiler.Compile(source);

			var outputResult = output.ToString();

			var expectedResult = "function newLineAndComments_Tmpl(number){ var t=\"\"; t+=\"<h1>\";t+=number;t+=\"</h1>\"; return t; }";

			Assert.AreEqual(expectedResult, outputResult);
		}

		[TestMethod]
		public void FullTypeDeclarationsAreCompiledCorrectly()
		{
			var source = FileHelper.GetTemplateSource("FullTypeDeclaration.cshtml");

			var compiler = new TemplateCompiler();
			var output = compiler.Compile(source);

			var outputResult = output.ToString();

			var expectedResult = "function fullTypeDeclaration_Tmpl(test,what){ var t=\"\"; t+=\"<h1>Extremly Simple - \";t+=test;t+=\" is \";t+=what;t+=\"</h1>\"; return t; }";

			Assert.AreEqual(expectedResult, outputResult);
		}

		[TestMethod]
		public void MultipleAdvancedCompilesCorrectly()
		{
			var source = FileHelper.GetTemplateSource("MultipleAdvanced.cshtml");

			var compiler = new TemplateCompiler();
			var output = compiler.Compile(source);

			var outputResult = output.ToString();

			var expectedResult = "function pager_Tmpl(pages,currentPage){ var t=\"\"; t+=\"<ol\";t+=\" class=\\\"pager\\\"\";t+=\">\";t+=pagerItem_Tmpl(\"<\", (currentPage > 1 ? \"?page=\" + (currentPage - 1) : null));for (var i = 1; i <= pages; i++){t+=pagerItem_Tmpl(i.toString(), (i != currentPage ? \"?page=\" + i : null));}t+=pagerItem_Tmpl(\">\", (currentPage < pages ? \"?page=\" + (currentPage + 1) : null));t+=\"</ol>\"; return t; }function pagerItem_Tmpl(text,url){ var t=\"\"; t+=\"<li><a \";t+=url == null ? \"\" : \"href=\\\"\"+ url + \"\\\"\";t+=\">\";t+=text;t+=\"</a></li>\"; return t; }";

			Assert.AreEqual(expectedResult, outputResult);
		}

		[TestMethod]
		public void SimpleHtmlRawHelperCompilesCorrectly()
		{
			var source = FileHelper.GetTemplateSource("Helper-HtmlRaw.cshtml");

			var compiler = new TemplateCompiler();
			var output = compiler.Compile(source);

			var outputResult = output.ToString();

			var expectedResult = "function simple_Tmpl(){ var t=\"\"; t+=\"<h1 \";t+=\"class=\\\"Whatever\\\"\";t+=\">Hello - </h1>\"; return t; }";

			Assert.AreEqual(expectedResult, outputResult);
		}

		[TestMethod]
		public void SimpleHtmlRawHelperStatementCompilesCorrectly()
		{
			var source = FileHelper.GetTemplateSource("Helper-HtmlRaw-Statement.cshtml");

			var compiler = new TemplateCompiler();
			var output = compiler.Compile(source);

			var outputResult = output.ToString();

			var expectedResult = "function simpleStatement_Tmpl(){ var t=\"\"; t+=\"<h1 \";t+=true ? \"class=\\\"true\\\"\" : \"class=\\\"false\\\"\";t+=\">Hello</h1>\"; return t; }";

			Assert.AreEqual(expectedResult, outputResult);
		}

		[TestMethod]
		public void ModelTemplateCompilesCorrectly()
		{
			// Tests both method and property name is converted to first letter lowercase
			var source = FileHelper.GetTemplateSource("Model.cshtml");

			var compiler = new TemplateCompiler();
			var output = compiler.Compile(source);

			var outputResult = output.ToString();

			var expectedResult = "function model_Tmpl(model){ var t=\"\"; t+=\"<h1>Model - \";t+=model.getString();t+=\" is \";t+=model.string;t+=\"</h1>\"; return t; }";

			Assert.AreEqual(expectedResult, outputResult);
		}

		[TestMethod]
		public void CanCompileForLoopTemplate()
		{
			var source = FileHelper.GetTemplateSource("ForLoop.cshtml");

			var compiler = new TemplateCompiler();
			var output = compiler.Compile(source);

			var outputResult = output.ToString();

			var expectedResult = "function loop_Tmpl(count){ var t=\"\"; for (var i = 0; i < count; i++){t+=\"<h1>Extremly Simple - \";t+=i;t+=\"</h1>\";} return t; }";

			Assert.AreEqual(expectedResult, outputResult);
		}

		[TestMethod]
		public void CanCompileForLoopAdvancedTemplate()
		{
			var source = FileHelper.GetTemplateSource("ForLoop-Advanced.cshtml");

			var compiler = new TemplateCompiler();
			var output = compiler.Compile(source);

			var outputResult = output.ToString();

			var expectedResult = "function loop_Tmpl(array){ var t=\"\"; for (var i = 0; i < array.length; i++){t+=\"<h1>Extremly Simple - \";t+=i;t+=\"</h1>\";} return t; }";

			Assert.AreEqual(expectedResult, outputResult);
		}

		[TestMethod]
		public void CanCompileGenericParametersTemplate()
		{
			var source = FileHelper.GetTemplateSource("Parameters-Generics.cshtml");

			var compiler = new TemplateCompiler();
			var output = compiler.Compile(source);

			var outputResult = output.ToString();

			var expectedResult = "function parameters_Tmpl(tuple,model){ var t=\"\"; t+=\"<h1>Extremly Simple - \";t+=tuple.item1;t+=\" - \";t+=model.string;t+=\"</h1>\"; return t; }";

			Assert.AreEqual(expectedResult, outputResult);
		}

		[TestMethod]
		public void CanCompileMultipleTemplatesWithModels()
		{
			var source = FileHelper.GetTemplateSource("MultipleWithModel.cshtml");

			var compiler = new TemplateCompiler();
			var output = compiler.Compile(source);

			var outputResult = output.ToString();

			var expectedResult = "function pager_Tmpl(int,model){ var t=\"\"; t+=pagerItem_Tmpl(\"<\", (model.item2 > 1 ? \"?page=\" + (model.item2 - 1) : null));for (var i = 1; i <= model.item1; i++){t+=pagerItem_Tmpl(i.toString(), (i != model.item2 ? \"?page=\" + i : null));}t+=pagerItem_Tmpl(\">\", (model.item2 < model.item1 ? \"?page=\" + (model.item2 + 1) : null)); return t; }function pagerItem_Tmpl(text,url){ var t=\"\"; t+=\"<li><a \";t+=url == null ? \"\" : \"href=\\\"\" + url + \"\\\"\";t+=\">\";t+=text;t+=\"</a></li>\"; return t; }";

			Assert.AreEqual(expectedResult, outputResult);
		}

		[TestMethod]
		public void CanCompileForEachLoopTemplate()
		{
			var source = FileHelper.GetTemplateSource("ForEachLoop.cshtml");

			var compiler = new TemplateCompiler();
			var output = compiler.Compile(source);

			var outputResult = output.ToString();

			var expectedResult = "function forEachLoop_Tmpl(model){ var t=\"\"; for(var __i0=0;__i0<model.models.length;__i0++){ var item=model.models[__i0];t+=\"<h1>Model output - \";t+=item.string;t+=\"</h1>\";} return t; }";

			Assert.AreEqual(expectedResult, outputResult);
		}
	}
}
