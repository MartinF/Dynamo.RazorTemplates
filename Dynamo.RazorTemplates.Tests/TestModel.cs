using System;
using System.Collections;
using System.Collections.Generic;

namespace Dynamo.RazorTemplates.Tests
{
	public class TestModel
	{
		public String String { get; set; }
		public IEnumerable<TestModel> Models { get; set; }

		public String GetString()
		{
			return "magic";
		}
	}
}
