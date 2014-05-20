using System;

namespace Dynamo.RazorTemplates
{
	public static class StringHelper
	{
		public static String FixDoubleQuotes(String str)
		{
			return str.Replace("\"", "\\\"");
		}

		public static String RemoveControlCharacters(String content)
		{
			// Could be multiple whitespace in a row which all need to be filtered ?

			var filteredContent = "";
			for (int i = 0, length = content.Length; i < length; i++)
			{
				char c = content[i];

				// Do not include Control characters in filtered content
				if (!Char.IsControl(c))
				{
					filteredContent += c;
				}
			}

			return filteredContent;
		}

		public static String ConvertFirstLetterToLowerCase(String str)
		{
			return str[0].ToString().ToLowerInvariant() + str.Substring(1);
		}
	}
}
