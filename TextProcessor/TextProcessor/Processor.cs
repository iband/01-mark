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
			//var scope = "abc".Substring(1, 3);
			var result = Tokenizer.Parse("a bc");
			Console.WriteLine("{0}", result);
		}
	}
	public class Tokenizer
	{
		static Regex _Text = new Regex(@"[^\r\n_`\\]", RegexOptions.Compiled);
		static Regex _Space = new Regex(@"\s", RegexOptions.Compiled);
		static Regex _NewLine = new Regex(@"[\r?\n]", RegexOptions.Compiled);
		static Regex _PTag = new Regex(@"\A(\r?\n +\r?\n)", RegexOptions.Compiled);
		static Regex _Backticks = new Regex(@"`", RegexOptions.Compiled);
		static Regex _Code = new Regex(@"\A`([^`]+[^\\])`", RegexOptions.Compiled);
		static Regex _Underscore = new Regex(@"_", RegexOptions.Compiled);
		static Regex _Escape = new Regex(@"\\[`_\\]", RegexOptions.Compiled);
		enum State
		{
			start,
			text,
			code,
			newline,
			underscore,
			escape
		}

		public static string Parse(string text)
		{
			text = " " + text + "  ";
			var tokens = new List<char>();
			var memory = new List<char>();
			State specCharState = State.start;
			State textState = State.start;
			State state = State.start;
			State prevState = State.start;
			var output = "";
			for (int i = 1; i < text.Length - 2; i++)
			{
				var c = text[i];
				var s = c.ToString();
				while (true)
				{
					switch (state)
					{
						case State.start:
							if (_Escape.IsMatch(s))
								state = State.escape;
							else if (_Text.IsMatch(s))
								state = State.text;
							else if (_Backticks.IsMatch(s))
								state = State.code;
							else if (_NewLine.IsMatch(s))
								state = State.newline;
							else if (_Underscore.IsMatch(s))
								state = State.underscore;
							continue;
						case State.text:
							output += s;
							state = prevState;
							break;
						
						default:
							break;
					}
					break;
				}
			}
			return output;
		}
	}
}
