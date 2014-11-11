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
			var output = TextProcessor.Tokenizer.BuildTokens("abc_d e");
			Assert.AreEqual("{abcd e}", output);
		}
		[Test]
		public void return_paragraph_tokens_with_two_newlines_on_input()
		{
			var output = TextProcessor.Tokenizer.BuildTokens("a b \nd\n   \r\ntxt");
			Assert.AreEqual("{a b d}P{txt}", output);
		}
	}
}
