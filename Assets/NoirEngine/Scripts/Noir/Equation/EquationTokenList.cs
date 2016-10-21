using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Noir.Equation
{
	public enum EquationTokenType : byte
	{
		Error,
		Plus,
		Minus,
		Asterisk,
		Division,
		Modulo,
		And,
		Or,
		Equal,
		NotEqual,
		Less,
		LessEq,
		Greater,
		GreaterEq,
		BracketL,
		BracketR,
		Number,
		String,
		Identifier
	}

	public struct EquationToken
	{
		public EquationTokenType eTokenType;
		public string sTokenValue;
	}

	public class EquationTokenList
	{
		private static string sNonIdentifier = "()*/%+-<>=!&|";
		private static Regex sNumberMatcher = new Regex("[\\d]+(?:\\.\\d+)?(?:[eE][+-]?\\d+)?", RegexOptions.Compiled);
		private static Regex sStringMatcher = new Regex("'[^']*'", RegexOptions.Compiled);
		private static Regex sOperatorMatcher = new Regex("[\\+\\-\\*\\/\\%\\<\\>\\(\\)]|\\&{2}|\\|{2}|\\={2}|(?:\\!\\=)|(?:\\<\\=)|(?:\\>\\=)", RegexOptions.Compiled);

		public bool IsRemain { get { return this.nIndex < sTokenList.Count; } }
		public EquationToken Token { get { return this.sTokenList[this.nIndex]; } }

		private int nIndex = 0;
		private List<EquationToken> sTokenList = new List<EquationToken>();

		public EquationTokenList(string sEquation)
		{
			Match sMatch;
			EquationToken sToken = new EquationToken();

			while(this.nIndex < sEquation.Length)
			{
				if (char.IsWhiteSpace(sEquation[this.nIndex]))
				{
					++this.nIndex;
					continue;
				}
				
				if ((sMatch = EquationTokenList.sOperatorMatcher.Match(sEquation, this.nIndex)).Success && sMatch.Index == this.nIndex)
				{
					switch (sToken.sTokenValue = sMatch.Value)
					{
						case "(":
						sToken.eTokenType = EquationTokenType.BracketL;
						break;

						case ")":
						sToken.eTokenType = EquationTokenType.BracketR;
						break;

						case "*":
						sToken.eTokenType = EquationTokenType.Asterisk;
						break;

						case "/":
						sToken.eTokenType = EquationTokenType.Division;
						break;

						case "%":
						sToken.eTokenType = EquationTokenType.Modulo;
						break;

						case "+":
						sToken.eTokenType = EquationTokenType.Plus;
						break;

						case "-":
						sToken.eTokenType = EquationTokenType.Minus;
						break;

						case "<":
						sToken.eTokenType = EquationTokenType.Less;
						break;

						case "<=":
						sToken.eTokenType = EquationTokenType.LessEq;
						break;

						case ">":
						sToken.eTokenType = EquationTokenType.Greater;
						break;

						case ">=":
						sToken.eTokenType = EquationTokenType.GreaterEq;
						break;

						case "==":
						sToken.eTokenType = EquationTokenType.Equal;
						break;
						
						case "!=":
						sToken.eTokenType = EquationTokenType.NotEqual;
						break;

						case "&&":
						sToken.eTokenType = EquationTokenType.And;
						break;

						case "||":
						sToken.eTokenType = EquationTokenType.Or;
						break;

						default:
						sToken.eTokenType = EquationTokenType.Error;
						break;
					}

					this.nIndex += sMatch.Value.Length;
					this.sTokenList.Add(sToken);
				}
				else if((sMatch = EquationTokenList.sNumberMatcher.Match(sEquation, this.nIndex)).Success && sMatch.Index == this.nIndex)
				{
					sToken.eTokenType = EquationTokenType.Number;
					sToken.sTokenValue = sMatch.Value;

					this.nIndex += sMatch.Value.Length;
					this.sTokenList.Add(sToken);
				}
				else if((sMatch = EquationTokenList.sStringMatcher.Match(sEquation, this.nIndex)).Success && sMatch.Index == this.nIndex)
				{
					sToken.eTokenType = EquationTokenType.String;
					sToken.sTokenValue = sMatch.Value.Substring(1, sMatch.Value.Length - 2);

					this.nIndex += sMatch.Value.Length;
					this.sTokenList.Add(sToken);
				}
				else
				{
					sToken.eTokenType = EquationTokenType.Identifier;
					sToken.sTokenValue = string.Empty;

					for(; this.nIndex < sEquation.Length; ++this.nIndex)
					{
						if (char.IsWhiteSpace(sEquation[this.nIndex]))
							break;

						int nCount = EquationTokenList.sNonIdentifier.Length - 1;
						for (; nCount >= 0; --nCount)
							if (sEquation[this.nIndex] == EquationTokenList.sNonIdentifier[nCount])
								break;

						if (nCount >= 0)
							break;

						sToken.sTokenValue += sEquation[this.nIndex];
					}

					this.sTokenList.Add(sToken);
				}
			}

			this.nIndex = 0;
		}

		public void skipToken(int nCount = 1)
		{
			this.nIndex += nCount;
		}
	}
}