using System;
using TextProcessor;
using NUnit.Framework;

namespace TextProcessorTests
{
	[TestFixture]
	public class Processor_should
	{
		[Test]
		public void return_text_on_plain_text_input()
		{
			CheckTokensOutput("ab c d e", "ab c d e");
		}
		[Test]
		public void return_code_on_any_text_between_double_backtickes_input()
		{
			CheckTokensOutput("ab `c d` ``e", "ab <code>c d</code> ``e");
		}
		[Test]
		public void return_text_on_escaped_symbol_input()
		{
			CheckTokensOutput(@"ab \`c d\` \``e", "ab `c d` ``e");
		}
		[Test]
		public void return_text_in_paragraph_on_both_sides_double_returns_input()
		{
			CheckTokensOutput("text \n  \r\n paragraph text \n\n end", "text <p> paragraph text </p> end");
		}

		private void CheckTokensOutput(string input, string expectedResult)
		{
			var output = TextProcessor.Tokenizer.Parse(input);
			Assert.AreEqual(expectedResult, output);
		}
	}
}
