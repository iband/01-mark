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
			CheckTokensOutput("ab `c d` e", "ab <code>c d</code> e");
		}

		private void CheckTokensOutput(string input, string expectedResult)
		{
			var output = TextProcessor.Tokenizer.Parse(input);
			Assert.AreEqual(expectedResult, output);
		}
	}
}
