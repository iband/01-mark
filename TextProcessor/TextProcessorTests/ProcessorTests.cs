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
		// Testing error cases
		[Test]
		public void not_return_tags_when_unbalanced_modifiers_on_input()
		{
			CheckOutput("__not strong_ _text__ _another_text__", "__not strong_ _text__ _another_text__");
		}
		[Test]
		public void not_return_lower_priority_tags_when_overlapse_on_input()
		{
			CheckOutput("text _not em because `code starts_ inside`", "text _not em because <code>code starts_ inside</code>");
		}
		[Test]
		public void not_allow_strong_tag_around_em_tag()
		{
			CheckOutput("__not strong _but em_ because of the priority__", "__not strong <em>but em</em> because of the priority__");
		}
		[Test]
		public void allow_em_tag_around_strong_tag()
		{
			CheckOutput("_em around __strong__ tag_", "<em>em around <strong>strong</strong> tag</em>");
		}
		[Test]
		public void respect_priority_strong_in_em_in_p_tags()
		{
			CheckOutput("\n\n new para with _em around __strong__ tag_ works correct\n\r\n", "<p> new para with <em>em around <strong>strong</strong> tag</em> works correct</p>");
		}
		[Test]
		public void ignore_escaped_symbols_when_checking_overlapses()
		{
			CheckOutput(@"_em because \`code_ __is \_mas\_ked__", "<em>em because `code</em> <strong>is _mas_ked</strong>");
		}
		[Test]
		public void wrap_existing_html_tags_into_pre_tags()
		{
			CheckOutput(@"<p>...</p> <code>example</code>", "&lt;p>...&lt;/p> &lt;code>example&lt;/code>");
		}

		private void CheckOutput(string input, string expectedResult)
		{
			var output = TextProcessor.Tokenizer.Parse(input);
			Assert.AreEqual(expectedResult, output);
		}
	}
}
