using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace TextProcessor
{
	class Processor
	{
		static void Main(string[] args)
		{
			Console.WriteLine(@"&lt;");
		}
	}
	public class Tokenizer
	{
		static Regex _Text = new Regex(@"[^\r\n_`\\]+", RegexOptions.Compiled);
		static Regex _Space = new Regex(@"[\s]+", RegexOptions.Compiled);
		static Regex _NewLine = new Regex(@"[\r?\n]+", RegexOptions.Compiled);
		enum State
		{
			start,
			text,
			newLine
		}

		public static string BuildTokens(string text)
		{
			var tokens = new List<char>();
			var memory = new List<char>();
			State specCharState = State.start;
			State textState = State.start;
			for (int i = 0; i < text.Length; i++)
			{
				var c = text[i];
				if (_Text.IsMatch(c.ToString())) {
					if (!(_Space.IsMatch(c.ToString()) && specCharState == State.newLine))
					{
						specCharState = State.start;
						if (textState != State.text)
						{
							tokens.Add('{');
							textState = State.text;
						}
						tokens.Add(c);
					}
				}
				else if (_NewLine.IsMatch(c.ToString()))
				{
					if (specCharState == State.newLine)
					{
						tokens.Add('}');
						textState = State.start;
						tokens.Add('P');
						specCharState = State.start;
					} else
						specCharState = State.newLine;
				}
			}
			if (textState == State.text)
				tokens.Add('}');
			return new string(tokens.ToArray());
		}
	}
}
