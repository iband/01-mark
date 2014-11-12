﻿using System;
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
			var result = Tokenizer.Parse("text _em`_ text");
			Console.WriteLine("{0}", result);
		}
	}
	public class Tokenizer
	{
		static Regex _Text = new Regex(@"[^\r\n_`\\]", RegexOptions.Compiled);
		static Regex _NewLine = new Regex(@"\n", RegexOptions.Compiled);
		static Regex _PTag = new Regex(@"\A(\n[ \r]*\n)", RegexOptions.Compiled);
		static Regex _Backticks = new Regex(@"`", RegexOptions.Compiled);
		static Regex _Code = new Regex(@"\A`([^`]+[^\\])`", RegexOptions.Compiled);
		static Regex _Underscore = new Regex(@"_", RegexOptions.Compiled);
		static Regex _Escape = new Regex(@"\\", RegexOptions.Compiled);

		static Regex _StrongOpens = new Regex(@"\s__[^_\s]", RegexOptions.Compiled);
		static Regex _StrongCloses = new Regex(@"[^_\s]__\s", RegexOptions.Compiled);
		static Regex _EmOpens = new Regex(@"\s_[^_\s]", RegexOptions.Compiled);
		//static Regex _EmCloses = new Regex(@"[^_\s]_\s", RegexOptions.Compiled);
		static Regex _EmCloses = new Regex(@"(?s)^((?!\n[ ]*\r?\n|`).)*[^_\s`]_\s", RegexOptions.Compiled);

		enum State
		{
			start,
			text,
			code,
			newline,
			undeline,
			em,
			strong,
			escape
		}

		public static string Parse(string text)
		{
			text = " " + text + "  ";
			State state = State.start;
			State prevState = State.start;
			bool isParagraph = false;
			bool isEm = false;
			var output = "";
			var memory = "";
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
								state = State.undeline;
							else
								break;
							continue;
						case State.text:
							output += s;
							state = prevState;
							break;
						case State.code:
							var code = _Code.Match(text.Substring(i)).Groups[1];
							if (code.Length > 0)
							{
								output += "<code>" + code + "</code>";
								i += code.Length + 1;
								state = prevState;
								break;
							}
							else
								state = State.text;
							continue;
						case State.newline:
							var paraLength = _PTag.Match(text.Substring(i)).Length;
							if (paraLength > 0)
							{
								if (isParagraph)
								{
									output += "</p>";
									isParagraph = false;
								}
								else
								{
									output += "<p>";
									isParagraph = true;
								}
								i += paraLength - 1;
								break;
							}
							else
							{
								state = State.text;
								continue;
							}
						case State.undeline:
							var scope = text.Substring(i - 1, 3);
							if (isEm)
							{
								if (_EmCloses.IsMatch(scope))
								{
									output += "</em>";
									isEm = false;
								}
							}
							else
							{
								var a = _EmCloses.IsMatch(text.Substring(i + 1));
								if (_EmOpens.IsMatch(scope) && _EmCloses.IsMatch(text.Substring(i + 1)))
								{
									output += "<em>";
									isEm = true;
								}
								else
								{
									state = State.text;
									continue;
								}
							}
							state = State.text;
							break;
						case State.em:

						case State.strong:

						case State.escape:
							state = State.text;
							break;
						default:
							break;
					}
					break;
				}
			}
			if (isParagraph)
				output += "</p>";
			return output;
		}
	}
}
