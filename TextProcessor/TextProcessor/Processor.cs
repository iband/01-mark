﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Security;
using System.IO;

namespace TextProcessor
{
	class Processor
	{
		static void Main(string[] args)
		{
			if (args.Length != 1)
			{
				Console.WriteLine("Using: {0} [inputFile]", AppDomain.CurrentDomain.FriendlyName);
				return;
			}
			try
			{
				var text = File.ReadAllText(args[0], Encoding.UTF8);
				text = SecurityElement.Escape(text);
				var output = Tokenizer.Parse(text);
				var header = "<head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"></head>";
				var html = "<!DOCTYPE html><html>" + header + "<body>" + output + "</body></html>";
				
				var path = @"output.html";
				File.WriteAllText(path, html, Encoding.UTF8);
			}
			catch (Exception e)
			{
				Console.WriteLine("Program exception: {0}", e.Message);
			}
		}
	}
	public class Tokenizer
	{
		static Regex _Text = new Regex(@"[^\r\n_`\\]", RegexOptions.Compiled);
		static Regex _NewLine = new Regex(@"\n", RegexOptions.Compiled);
		static Regex _PTag = new Regex(@"\A(\n[ \r]*\n)", RegexOptions.Compiled);
		static Regex _Backticks = new Regex(@"`", RegexOptions.Compiled);
		static Regex _Code = new Regex(@"\A`([^`]+[^\\])`", RegexOptions.Compiled);
		static Regex _Underline = new Regex(@"_", RegexOptions.Compiled);
		static Regex _Escape = new Regex(@"\\", RegexOptions.Compiled);

		static Regex _StrongOpens = new Regex(@"[^\w]__[^_]", RegexOptions.Compiled);
		static Regex _StrongCloses = new Regex(@"(?s)^((?!\n[ ]*\r?\n|`|_[^\w\\]|[^\w\\]_[^_\r?\n`\\]).|(\\[`\n]))*[^_\s`]__[^\w]", RegexOptions.Compiled);
		static Regex _EmOpens = new Regex(@"[^\w]_[^_]", RegexOptions.Compiled);
		static Regex _EmCloses = new Regex(@"(?s)^((?!\n[ ]*\r?\n|`|[^\w\\]_[^_\r?\n`\\]).|(\\[`\n]))*[^_\s`]_[^\w]", RegexOptions.Compiled);

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
			State state = State.start;
			bool isParagraph = false;
			bool isEm = false;
			bool isStrong = false;
			text = " " + text + "  ";
			var output = "";
			for (int i = 1; i < text.Length - 2; i++)
			{
				var s = text[i].ToString();
				while (true)
				{
					switch (state)
					{
						case State.start:
							if (isParagraph == false)
							{
								output += "<p>";
								isParagraph = true;
							}
							if (_Escape.IsMatch(s))
								state = State.escape;
							else if (_Text.IsMatch(s))
								state = State.text;
							else if (_Backticks.IsMatch(s))
								state = State.code;
							else if (_NewLine.IsMatch(s))
								state = State.newline;
							else if (_Underline.IsMatch(s))
								state = State.undeline;
							else
								break;
							continue;
						case State.text:
							output += s;
							state = State.start;
							break;
						case State.code:
							var code = _Code.Match(text.Substring(i)).Groups[1];
							if (code.Length > 0)
							{
								output += "<code>" + code + "</code>";
								i += code.Length + 1;
								state = State.start;
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
								state = State.start;
								break;
							}
							else
							{
								state = State.text;
								continue;
							}
						case State.undeline:
							if (text[i + 1] == '_')
								state = State.strong;
							else
								state = State.em;
							continue;
						case State.em:
							var emRange = text.Substring(i - 1, 3);
							if (isEm)
							{
								if (_EmCloses.IsMatch(emRange))
								{
									output += "</em>";
									isEm = false;
								}
							}
							else
							{
								if (_EmOpens.IsMatch(emRange) && _EmCloses.IsMatch(text.Substring(i + 1)))
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
						case State.strong:
							var strongRange = text.Substring(i - 1, 4);
							if (isStrong)
							{
								if (_StrongCloses.IsMatch(strongRange))
								{
									output += "</strong>";
									isStrong = false;
									i++;
								}
							}
							else
							{
								if (_StrongOpens.IsMatch(strongRange) && _StrongCloses.IsMatch(text.Substring(i + 2)))
								{
									output += "<strong>";
									isStrong = true;
									i++;
								}
								else
								{
									state = State.text;
									continue;
								}
							}
							state = State.text;
							break;
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
