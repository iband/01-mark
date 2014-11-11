using System;
using TextProcessor;
using NUnit.Framework;

namespace TextProcessorTests
{
	[TestFixture]
	public class Processor_should
	{
		[Test]
		public void return_text_tokens_with_text_on_input()
		{
			CheckTokensOutput("abc_d e", "{abcd e}");
		}
		[Test]
		public void return_paragraph_tokens_with_two_newlines_on_input()
		{
			CheckTokensOutput("a b \n       d\n   \r\ntxt", "{a b d}P{txt}");
		}
		[Test]
		public void return_code_tokens_with_backticks_on_input()
		{
			CheckTokensOutput("some text `some _code` and text", "{some text }[some _code]{ and text}");
		}
		[Test]
		public void return_text_tokens_with_unpaired_backticks_on_input()
		{
			CheckTokensOutput("some text `some code and text", "{some text `some code and text}");
		}
		[Test]
		public void return_em_opening_tokens_with_space_and_underscore_on_input()
		{
			CheckTokensOutput("some text _some code _and text", "{some text }E{some code }E{and text}");
		}
		[Test]
		public void return_em_closing_tokens_with_underscoree_and_space_on_input()
		{
			CheckTokensOutput("some text some_ code and_ text", "{some text some}e{ code and}e{ text}");
		}

		private void CheckTokensOutput(string input, string expectedResult)
		{
			var output = TextProcessor.Tokenizer.BuildTokens(input);
			Assert.AreEqual(expectedResult, output);
		}
	}
}
