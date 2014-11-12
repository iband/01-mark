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
			CheckOutput("ab c d e", "ab c d e");
		}
		[Test]
		public void return_code_when_any_text_between_double_backtickes_on_input()
		{
			CheckOutput("ab `c d` ``e", "ab <code>c d</code> ``e");
		}
		[Test]
		public void return_symbol_after_escaped_one_on_input()
		{
			CheckOutput(@"ab \`c d\` \``e", "ab `c d` ``e");
		}
		[Test]
		public void return_text_in_paragraph_with_both_sides_double_newlines_on_input()
		{
			CheckOutput("text \n  \r\n paragraph text \n\n end", "text <p> paragraph text </p> end");
		}
		[Test]
		public void return_em_tags_with_two_underlines_from_sides_on_input()
		{
			CheckOutput("text _em_ text", "text <em>em</em> text");
		}
		[Test]
		public void return_underline_when_no_closing_underline_on_input()
		{
			CheckOutput("text _em`_ text", "text _em`_ text");
		}
		[Test]
		public void return_strong_tag_when_double_underline_on_input()
		{
			CheckOutput("text __strong__ text", "text <strong>strong</strong> text");
		}

		private void CheckOutput(string input, string expectedResult)
		{
			var output = TextProcessor.Tokenizer.Parse(input);
			Assert.AreEqual(expectedResult, output);
		}
	}
}
