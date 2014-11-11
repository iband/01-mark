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
			var result = Tokenizer.BuildTokens("a `cc` b");
		}
	}
	public class Tokenizer
	{
		static Regex _Text = new Regex(@"[^\r\n_`\\]+", RegexOptions.Compiled);
		static Regex _Space = new Regex(@"[\s]+", RegexOptions.Compiled);
		static Regex _NewLine = new Regex(@"[\r?\n]+", RegexOptions.Compiled);
		static Regex _Backticks = new Regex(@"[`]+", RegexOptions.Compiled);
		enum State
		{
			start,
			text,
			newLine,
			code
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
				var s = c.ToString();
				if (specCharState == State.code && !_Backticks.IsMatch(s)) 
				{
					memory.Add(c);
				}
				else if (_Text.IsMatch(s)) {
					if (!(_Space.IsMatch(s) && specCharState == State.newLine))
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
				else if (_NewLine.IsMatch(s))
				{
					if (specCharState == State.newLine)
					{
						tokens.Add('}');
						textState = State.start;
						tokens.Add('P');
						specCharState = State.start;
					}
					else
						specCharState = State.newLine;
				}
				else if (_Backticks.IsMatch(s))
				{
					if (specCharState == State.code)
					{
						tokens.AddRange(memory);
						memory.Clear();
						tokens.Add(']');
						specCharState = State.start;
						textState = State.start;
					}
					else
					{
						if (textState == State.text)
							memory.Add('}');
						specCharState = State.code;
						memory.Add('[');
					}
				}
			}
			if (textState == State.text)
			{
				if (memory.Any())
				{
					if (specCharState == State.code)
					{
						memory.RemoveAt(0);
						memory[0] = '`';
					}
					tokens.AddRange(memory);
				}
				tokens.Add('}');
			}
			return new string(tokens.ToArray());
		}
	}
}
