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
		static Regex _PTag = new Regex(@"\A(\r?\n +\r?\n)", RegexOptions.Compiled);
		static void Main(string[] args)
		{
			//var scope = "abc".Substring(1, 3);
			var result = _PTag.Match(@"
 text 
      
text");
			Console.WriteLine("{0}", result.Length);
		}
	}
	public class Tokenizer
	{
		static Regex _Text = new Regex(@"[^\r\n_`\\]", RegexOptions.Compiled);
		static Regex _Space = new Regex(@"[\s]", RegexOptions.Compiled);
		static Regex _NewLine = new Regex(@"[\r?\n]", RegexOptions.Compiled);
		static Regex _PTag = new Regex(@"\A(\r?\n +\r?\n)", RegexOptions.Compiled);
		static Regex _Backticks = new Regex(@"[`]", RegexOptions.Compiled);
		static Regex _Underscore = new Regex(@"[_]", RegexOptions.Compiled);
		static Regex _Escape = new Regex(@"\\[`_\\]", RegexOptions.Compiled);
		enum State
		{
			start,
			text,
			code
		}

		public static string BuildTokens(string text)
		{
			text = " " + text + "  ";
			var tokens = new List<char>();
			var memory = new List<char>();
			State specCharState = State.start;
			State textState = State.start;
			for (int i = 1; i < text.Length - 2; i++)
			{
				var c = text[i];
				var s = c.ToString();
				if (_Escape.IsMatch(text.Substring(i, 2)))
				{
					if (textState != State.text && specCharState != State.code)
					{
						tokens.Add('{');
						textState = State.text;
					}
					if (specCharState == State.code)
						memory.Add(text[i + 1]);
					else
						tokens.Add(text[i + 1]);
					i += 1;
				}
				else if (specCharState == State.code && !_Backticks.IsMatch(s))
				{
					memory.Add(c);
				}
				else if (_Text.IsMatch(s))
				{
					specCharState = State.start;
					if (textState != State.text)
					{
						tokens.Add('{');
						textState = State.text;
					}
					tokens.Add(c);
				}
				else if (_NewLine.IsMatch(s))
				{
					var length = _PTag.Match(text.Substring(i)).Length;
					if (length != 0)
					{
						tokens.Add('}');
						textState = State.start;
						tokens.Add('P');
						i += length - 1;
					}
					else
					{
						if (textState != State.text)
						{
							tokens.Add('{');
							textState = State.text;
						}
						tokens.Add(c);
					}
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
				else if (_Underscore.IsMatch(s))
				{
					Regex strongOpens = new Regex(@"\s__[^_\s]", RegexOptions.Compiled);
					Regex strongCloses = new Regex(@"[^_\s]__\s", RegexOptions.Compiled);
					Regex emOpens = new Regex(@"\s_[^_\s]+", RegexOptions.Compiled);
					Regex emCloses = new Regex(@"[^_\s]+_\s", RegexOptions.Compiled);

					var scope = text.Substring(i - 1, 4);
					char toAdd = '\0';

					if (emOpens.IsMatch(scope))
					{
						toAdd = 'E';
					}
					else if (emCloses.IsMatch(scope))
					{
						toAdd = 'e';
					}
					else if (strongOpens.IsMatch(scope))
					{
						toAdd = 'S';
						i++;
					}
					else if (strongCloses.IsMatch(scope))
					{
						toAdd = 's';
						i++;
					}

					if (toAdd != '\0')
					{
						if (textState == State.text)
							tokens.Add('}');
						textState = State.start;
						tokens.Add(toAdd);
					}
					else
					{
						if (textState != State.text)
							tokens.Add('{');
						textState = State.text;
						tokens.Add(c);
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
