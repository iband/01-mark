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
			var output = TextProcessor.Tokenizer.BuildTokens("abcd e");
			Assert.AreEqual(output, "{abcde}");
		}
	}
}
