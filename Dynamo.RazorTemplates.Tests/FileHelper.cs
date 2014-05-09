using System;
using System.IO;

namespace Dynamo.RazorTemplates.Tests
{
	public static class FileHelper
	{
		private const String _templateFolder = @"..\..\Templates\";

		public static string GetTemplateSource(String filename)
		{
			var path = _templateFolder + filename;
			return File.ReadAllText(path);
		}
	}
}
