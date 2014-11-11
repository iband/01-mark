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
			
		}
	}
	public class Tokenizer
	{
		static Regex _Text = new Regex(@"[^\s\r\n_`\\]+", RegexOptions.Compiled);
		enum State
		{
			start,
			text
		}

		public static string BuildTokens(string text)
		{
			var tokens = new List<char>();
			State cur_state = State.start;
			foreach (char c in text)
			{
				if (_Text.IsMatch(c.ToString())) {
					if (cur_state != State.text)
					{
						tokens.Add('{');
						cur_state = State.text;
					}
					tokens.Add(c);
				}
			}
			if (cur_state == State.text)
				tokens.Add('}');
			return new string(tokens.ToArray());
		}
	}
}
