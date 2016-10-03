using UnityEngine;
using System.Collections;
using System.Text;

namespace Noir
{
	public class ScriptParser
	{
		#region Definition of enums and structs

		public struct ParsingData
		{
			public string ScriptFile;
			public string ScriptText;
			public int CurrentLine;
			public int CurrentPosition;
		};

		public enum TokenType : uint
		{
			Error,
			Tag,        //명령어 ([] @)
			Section,    //구역 지정 (*)
			Speech,     //대사
		}

		public struct TokenData
		{
			public TokenType Type;
			public object Data;
		}

		public struct TagData
		{
			public string TagName;
			public Hashtable TagAttribute;
		}

		public struct SpeechData
		{
			public string SpeechText;
		}

		public struct SectionData
		{
			public string SectionName;
		}

		#endregion

		private static ParsingData sParsingData;

		/// <summary>
		/// 새로운 스크립트 파일을 준비합니다.
		/// </summary>
		/// <param name="sNewScriptFilePath">스크립트 파일 경로입니다. Resources 폴더 내에 있어야 합니다.</param>
		public static void setScriptFile(string sNewScriptFilePath)
		{
			ScriptParser.sParsingData.ScriptFile = sNewScriptFilePath;
			ScriptParser.sParsingData.ScriptText = Resources.Load<TextAsset>(sNewScriptFilePath).text;
			ScriptParser.sParsingData.CurrentLine = 1;
			ScriptParser.sParsingData.CurrentPosition = 0;
		}

		/// <summary>
		/// 스크립트를 실행합니다.
		/// </summary>
		public static void runScript()
		{
			while (ScriptParser.isNotEndOfFile())
			{
				//한 라인 추출
				string sLine = ScriptParser.mergeUntil("\n");
				ScriptParser.skipWhile();

				//CR + LF일 경우 CR 삭제
				if (sLine.EndsWith("\r"))
					sLine = sLine.Substring(0, sLine.Length - 1);

				//라인의 첫 글자를 확인
				switch (ScriptParser.getCurrentCharacterUnsafe())
				{
					case ';':
					{
						//주석, 현재 라인 무시
					}
					break;
					case '*':
					{
						//영역 선언
						
					}
					break;
					case '@':
					{
						//커맨드라인
						ScriptParser.parseCommandTokenAndRun(sLine);
					}
					break;
					default:
					{
						ScriptParser.parseSpeechTokenAndRun();
					}
					break;
				}
			}
		}

		public static void parseTagAndSpeechAndRun(string sLine)
		{

		}

		public static void parseTagTokenAndRun(string sLine)
		{

		}

		public static void parseCommandTokenAndRun(string sLine)
		{
			int nPosition = 0;
			sLine = sLine.Substring(1);

			while(nPosition < sLine.Length)
			{
				
			}
		}

		public static void parseSpeechTokenAndRun()
		{

		}

		public static void runToken()
		{

		}
		
		/// <summary>
		/// 새로운 에러를 에러 목록에 넣습니다.
		/// </summary>
		/// <param name="sErrorMessage">자세한 에러 메세지입니다.</param>
		public static void pushError(string sErrorMessage)
		{
			Error.pushError(sErrorMessage, ScriptParser.sParsingData.ScriptFile, ScriptParser.sParsingData.CurrentLine);
		}

		/// <summary>
		/// 현재 스크립트가 파싱이 끝났는지 확인합니다.
		/// </summary>
		/// <returns>스크립트가 남아있다면 false, 그렇지 않다면 true를 반환합니다.</returns>
		public static bool isEndOfFile()
		{
			return ScriptParser.sParsingData.ScriptText.Length <= ScriptParser.sParsingData.CurrentPosition;
		}

		/// <summary>
		/// 파싱중인 스크립트의 남은 문자 개수가 기준보다 적은지 확인합니다.
		/// </summary>
		/// <param name="nCount">기준 문자 개수입니다.</param>
		/// <returns>스크립트의 남은 문자 개수가 기준 개수보다 적다면 true를, 아니라면 false를 반환합니다.</returns>
		public static bool isEndOfFile(int nCount)
		{
			return ScriptParser.sParsingData.ScriptText.Length + 1 <= ScriptParser.sParsingData.CurrentPosition + nCount;
		}
		
		/// <summary>
		/// 현재 파싱할 스크립트가 남아있는지 확인합니다.
		/// </summary>
		/// <returns>스크립트가 남아있다면 true, 그렇지 않다면 false를 반환합니다.</returns>
		public static bool isNotEndOfFile()
		{
			return ScriptParser.sParsingData.ScriptText.Length > ScriptParser.sParsingData.CurrentPosition;
		}

		/// <summary>
		/// 파싱중인 스크립트의 남은 문자 개수가 기준 이상인지 확인합니다.
		/// </summary>
		/// <param name="nCount">기준 문자 개수입니다.</param>
		/// <returns>스크립트의 남은 문자 개수가 기준 개수 이상이라면 true를, 아니라면 false를 반환합니다.</returns>
		public static bool isNotEndOfFile(int nCount)
		{
			return ScriptParser.sParsingData.ScriptText.Length + 1 < ScriptParser.sParsingData.CurrentPosition + nCount;
		}

		/// <summary>
		/// 현재 파싱중인 문자가 특수문자인지 검사합니다. 특수문자란 대사를 중지시킬 수 있는 문자를 말합니다.
		/// </summary>
		/// <returns>현재 문자가 '[', '*', '@', ';', '\t'라면 true를 반환합니다. 그렇지 않거나 "[["라면, 혹은 스크립트가 남아있지 않다면 false를 반환합니다.</returns>
		public static bool isSpecialCharacter()
		{
			if (ScriptParser.isEndOfFile())
				return false;

			switch (ScriptParser.getCurrentCharacterUnsafe())
			{
				case '[':
					if (ScriptParser.isNotEndOfFile(2) && ScriptParser.sParsingData.ScriptText[ScriptParser.sParsingData.CurrentPosition + 1] == '[')
						return false;
				return true;

				case '*':
				case '@':
				case ';':
				case '\t':
				return true;

				default:
				return false;
			}
		}

		/// <summary>
		/// 현재 파싱할 문자를 기준 문자와 비교합니다.
		/// </summary>
		/// <param name="nChar">기준 문자입니다.</param>
		/// <returns>두 문자가 같으면 true, 아니면 false를 반환합니다.</returns>
		public static bool tryMatchChar(char nChar)
		{
			return ScriptParser.isNotEndOfFile() && ScriptParser.getCurrentCharacterUnsafe() == nChar;
		}

		/// <summary>
		/// 현재 파싱할 문자를 기준 문자들과 비교합니다.
		/// </summary>
		/// <param name="sChar">기준 문자들입니다.</param>
		/// <returns>현재 문자와 기준 문자들이 하나라도 같으면 true, 아니면 false를 반환합니다.</returns>
		public static bool tryMatchChar(string sChar)
		{
			if (ScriptParser.isEndOfFile())
				return false;

			foreach (char nChar in sChar)
				if (ScriptParser.getCurrentCharacterUnsafe() == nChar)
					return true;

			return false;
		}

		/// <summary>
		/// 현재 파싱할 문자열을 기준 문자열과 비교합니다.
		/// </summary>
		/// <param name="sString">기준 문자열입니다.</param>
		/// <returns>현재 문자열과 기준 문자열이 같으면 true, 아니면 false를 반환합니다.</returns>
		public static bool tryMatchStr(string sString)
		{
			if (ScriptParser.isEndOfFile(sString.Length))
				return false;

			for(int nFirst = ScriptParser.sParsingData.CurrentPosition, nSecond = 0; nSecond < sString.Length; ++nFirst, ++nSecond)
				if (ScriptParser.sParsingData.ScriptText[nFirst] != sString[nSecond])
					return false;

			return true;
		}

		/// <summary>
		/// 현재 파싱할 문자를 기준 문자와 비교합니다.
		/// </summary>
		/// <param name="nChar">기준 문자입니다.</param>
		/// <returns>두 문자가 다르면 true, 아니면 false를 반환합니다.</returns>
		public static bool tryNotMatchChar(char nChar)
		{
			return ScriptParser.isNotEndOfFile() && ScriptParser.getCurrentCharacterUnsafe() != nChar;
		}

		/// <summary>
		/// 현재 파싱할 문자를 기준 문자들과 비교합니다.
		/// </summary>
		/// <param name="sChar">기준 문자들입니다.</param>
		/// <returns>현재 문자와 기준 문자들이 모두 다르면 true, 아니면 false를 반환합니다.</returns>
		public static bool tryNotMatchChar(string sChar)
		{
			if (ScriptParser.isEndOfFile())
				return false;

			foreach (char nChar in sChar)
				if (ScriptParser.getCurrentCharacterUnsafe() == nChar)
					return false;

			return true;
		}

		/// <summary>
		/// 파싱중인 문자를 반환합니다.
		/// </summary>
		/// <returns>파싱할 스크립트가 남아있다면 그 문자를, 아니라면 -1을 반환합니다.</returns>
		public static int getCurrentCharacter()
		{
			return ScriptParser.isEndOfFile() ? -1 : ScriptParser.sParsingData.ScriptText[ScriptParser.sParsingData.CurrentPosition];
		}

		/// <summary>
		/// 파싱중인 문자를 반환합니다. 파싱할 스크립트가 남아있지 않으면 문제가 생길 수 있습니다. Noir.ScriptParser.getCurrentCharacter보다 빠릅니다.
		/// </summary>
		/// <returns>파싱중인 문자를 반환합니다.</returns>
		public static int getCurrentCharacterUnsafe()
		{
			return ScriptParser.sParsingData.ScriptText[ScriptParser.sParsingData.CurrentPosition];
		}
		
		/// <summary>
		/// 특정 문자를 모두 넘깁니다.
		/// </summary>
		/// <param name="nChar">넘길 문자입니다.</param>
		public static void skipWhile(char nChar)
		{
			while (ScriptParser.isNotEndOfFile() && ScriptParser.tryMatchChar(nChar))
			{
				if (ScriptParser.getCurrentCharacterUnsafe() == '\n')
					++ScriptParser.sParsingData.CurrentLine;

				++ScriptParser.sParsingData.CurrentPosition;
			}
		}

		/// <summary>
		/// 특정 문자들을 모두 넘깁니다.
		/// </summary>
		/// <param name="sChar">넘길 문자입니다.</param>
		public static void skipWhile(string sChar)
		{
			while (ScriptParser.isNotEndOfFile() && ScriptParser.tryMatchChar(sChar))
			{
				if (ScriptParser.getCurrentCharacterUnsafe() == '\n')
					++ScriptParser.sParsingData.CurrentLine;

				++ScriptParser.sParsingData.CurrentPosition;
			}
		}

		/// <summary>
		/// 일정 개수만큼 문자를 넘깁니다.
		/// </summary>
		/// <param name="nCount">넘길 문자 개수입니다.</param>
		public static void skipWhile(int nCount = 1)
		{
			for (int nIndex = 0; ScriptParser.isNotEndOfFile() && nIndex < nCount; ++nIndex)
			{
				if (ScriptParser.getCurrentCharacterUnsafe() == '\n')
					++ScriptParser.sParsingData.CurrentLine;

				++ScriptParser.sParsingData.CurrentPosition;
			}
		}

		/// <summary>
		/// 특정 문자가 나타날 때 까지 문자를 모두 넘깁니다.
		/// </summary>
		/// <param name="nChar">대상 문자입니다.</param>
		public static void skipUntil(char nChar)
		{
			while(ScriptParser.isNotEndOfFile() && ScriptParser.tryNotMatchChar(nChar))
			{
				if (ScriptParser.getCurrentCharacterUnsafe() == '\n')
					++ScriptParser.sParsingData.CurrentLine;

				++ScriptParser.sParsingData.CurrentPosition;
			}
		}

		/// <summary>
		/// 특정 문자들이 나타날 때 까지 문자를 모두 넘깁니다.
		/// </summary>
		/// <param name="sChar">대상 문자들입니다.</param>
		public static void skipUntil(string sChar)
		{
			while (ScriptParser.isNotEndOfFile() && ScriptParser.tryNotMatchChar(sChar))
			{
				if (ScriptParser.getCurrentCharacterUnsafe() == '\n')
					++ScriptParser.sParsingData.CurrentLine;

				++ScriptParser.sParsingData.CurrentPosition;
			}
		}

		/// <summary>
		/// 특정 문자가 나타날 때 까지 문자를 모두 병합합니다.
		/// </summary>
		/// <param name="nChar">대상 문자입니다.</param>
		/// <returns>병합된 문자열입니다.</returns>
		public static string mergeUntil(char nChar)
		{
			StringBuilder sBuilder = new StringBuilder();

			while (ScriptParser.tryNotMatchChar(nChar))
			{
				if (ScriptParser.tryMatchChar('\n'))
					++ScriptParser.sParsingData.CurrentLine;

				sBuilder.Append((char)ScriptParser.getCurrentCharacterUnsafe());
				++ScriptParser.sParsingData.CurrentPosition;
			}

			return sBuilder.ToString();
		}

		/// <summary>
		/// 특정 문자들이 나타날 때 까지 문자를 모두 병합합니다.
		/// </summary>
		/// <param name="sChar">대상 문자들입니다.</param>
		/// <returns>병합된 문자열입니다.</returns>
		public static string mergeUntil(string sChar)
		{
			StringBuilder sBuilder = new StringBuilder();

			while (ScriptParser.tryNotMatchChar(sChar))
			{
				if (ScriptParser.tryMatchChar('\n'))
					++ScriptParser.sParsingData.CurrentLine;

				sBuilder.Append((char)ScriptParser.getCurrentCharacterUnsafe());
				++ScriptParser.sParsingData.CurrentPosition;
			}

			return sBuilder.ToString();
		}
	}
}