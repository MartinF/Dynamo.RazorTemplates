using System;

namespace Dynamo.RazorTemplates.Tests
{
	public class TestModel
	{
		public String String { get; set; }

		public String GetString()
		{
			return "magic";
		}
	}
}
