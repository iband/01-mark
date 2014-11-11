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
			CheckTokensOutput("abcd e", "{abcd e}");
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
			CheckTokensOutput("_some text _some code _and text", "E{some text }E{some code }E{and text}");
		}
		[Test]
		public void return_em_closing_tokens_with_underscore_and_space_on_input()
		{
			CheckTokensOutput("some text some_ code and_ text", "{some text some}e{ code and}e{ text}");
		}
		[Test]
		public void return_strong_opening_tokens_with_space_and_two_underscore_on_input()
		{
			CheckTokensOutput("__some text __some code __and text", "S{some text }S{some code }S{and text}");
		}
		[Test]
		public void return_strong_closing_tokens_with_two_underscore_and_space_on_input()
		{
			CheckTokensOutput("some text some__ code and__ text", "{some text some}s{ code and}s{ text}");
		}
		[Test]
		public void return_text_tokens_when_wrong_number_of_underscores_on_input()
		{
			CheckTokensOutput("___some_text some___ code__and___ text", "{___some_text some___ code__and___ text}");
		}

		private void CheckTokensOutput(string input, string expectedResult)
		{
			var output = TextProcessor.Tokenizer.BuildTokens(input);
			Assert.AreEqual(expectedResult, output);
		}
	}
}
