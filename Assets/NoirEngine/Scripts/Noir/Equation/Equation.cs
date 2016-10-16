using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Noir.Equation
{
	public class Equation
	{
		private static Regex sNumberMatcher = new Regex("^[\\d]+(?:\\.\\d+)?(?:[eE][+-]?\\d+)?", RegexOptions.Compiled);
		private static Regex sOperatorMatcher = new Regex("^[\\+\\-\\*\\/\\%\\<\\>]|\\&{2}|\\|{2}|\\={2}|(?:\\!\\=)|(?:\\<\\=)|(?:\\>\\=)", RegexOptions.Compiled);
		
		public static bool isNumeric(char nCharacter)
		{
			return nCharacter >= '0' && nCharacter <= '9';
		}

		public static bool isOperator(char nCharacter)
		{
			switch (nCharacter)
			{
				case '+':
				case '-':
				case '*':
				case '/':
				case '%':
				case '&':
				case '|':
				case '=':
				case '!':
				case '<':
				case '>':
				return true;

				default:
				return false;
			}
		}

		public static bool isVariable(char nCharacter)
		{
			return !Equation.isNumeric(nCharacter) && !Equation.isOperator(nCharacter);
		}

		public static string calcEquation(string sEquation)
		{
			Match sMatch;
			List<string> sTokenList = new List<string>();
			
			sEquation = sEquation.Trim();

			for(int nIndex = 0; nIndex < sEquation.Length; )
			{
				if((sMatch = Equation.sNumberMatcher.Match(sEquation, nIndex)).Success)
				{
					nIndex += sMatch.Length;
					sTokenList.Add(sMatch.Value);

					continue;
				}
				else if((sMatch = Equation.sOperatorMatcher.Match(sEquation, nIndex)).Success)
				{
					nIndex += sMatch.Length;
					sTokenList.Add(sMatch.Value);

					continue;
				}
				else
				{
					StringBuilder sVariableBuilder = new StringBuilder();

					for (; Equation.isVariable(sEquation[nIndex]); ++nIndex)
						sVariableBuilder.Append(sEquation[nIndex]);

					sTokenList.Add(sVariableBuilder.ToString().Trim());
				}
			}



			return null;
		}
	}
}