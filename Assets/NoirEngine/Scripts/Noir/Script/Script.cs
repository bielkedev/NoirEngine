using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Noir.Util;

namespace Noir.Script
{
	/// <summary>
	/// 스크립트 객체입니다.
	/// </summary>
	public class Script
	{
		private static Regex sCRLFMatcher = new Regex("\\r\\n", RegexOptions.Compiled);
		private static Regex sIndentationMatcher = new Regex("^(?:(?!\\n)\\s)+", RegexOptions.Compiled | RegexOptions.Multiline);
		private static Regex sCommentMatcher = new Regex("(?:\\/\\/.*)|(?:\\/\\*(?:(?!\\*\\/)[\\s\\S])*\\*\\/)", RegexOptions.Compiled);
		
		public bool IsMacro { get { return this.bMacro; } }
		public Script ParentScript { get { return this.sParentScript; } }
		public List<ScriptLine> ScriptLineList { get { return this.sScriptLineList; } }
		public Dictionary<string, int> ScriptRegionList { get { return this.sScriptRegionList; } }

		private bool bMacro;
		private Script sParentScript;
		private List<ScriptLine> sScriptLineList;
		private Dictionary<string, int> sScriptRegionList = new Dictionary<string, int>();

		/// <summary>
		/// 특정 스크립트 객체를 부모로 하는 파생 스크립트 객체를 생성합니다.
		/// </summary>
		/// <param name="sNewParentScript">부모 스크립트 객체입니다.</param>
		/// <param name="sNewScriptLineList">파생 스크립트의 명령줄 리스트입니다.</param>
		public Script(Script sNewParentScript, List<ScriptLine> sNewScriptLineList)
		{
			this.bMacro = false;
			this.sParentScript = sNewParentScript;
			this.sScriptLineList = sNewScriptLineList;
		}

		/// <summary>
		/// 스크립트 객체를 스크립트 파일로부터 생성합니다.
		/// </summary>
		/// <param name="sScriptFilePath">대상 스크립트 파일의 경로입니다.</param>
		/// <param name="bIsMacro">매크로 여부입니다.</param>
		public Script(string sScriptFilePath, bool bIsMacro = false)
		{
			this.bMacro = bIsMacro;
			this.sParentScript = this;
			this.sScriptLineList = new List<ScriptLine>();

			var sAsset = Resources.Load<TextAsset>(sScriptFilePath);

			if (sAsset == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.ParsingError, "스크립트 파일을 찾을 수 없습니다.", sScriptFilePath, -1);
				return;
			}

			StringParser sParser = new StringParser(Script.preprocessScript(sAsset.text));
			Resources.UnloadAsset(sAsset);
			
			//스크립트를 처리합니다.
			while(sParser.IsRemain)
			{
				sParser.skipWhitespace();

				if(sParser.tryMatchChar('*')) //구역 지정자입니다.
				{
					sParser.skipWhile(1);
					this.sScriptRegionList.Add(sParser.mergeUntil("\n").Trim(), this.sScriptLineList.Count);
					sParser.skipWhile(1);
				}
				else if(sParser.tryMatchChar('[')) //커맨드입니다.
				{
					try
					{
						this.sScriptLineList.Add(new ScriptTag(sScriptFilePath, sParser));
					}
					catch(Exception sException)
					{
						ScriptError.pushError(ScriptError.ErrorType.ParsingError, sException.Message, sScriptFilePath, sParser.Line);
					}
				}
				else if(sParser.tryMatchChar('@')) //입력 대기 명령입니다.
				{
					try
					{
						this.sScriptLineList.Add(new ScriptWait(sScriptFilePath, sParser));
					}
					catch (Exception sException)
					{
						ScriptError.pushError(ScriptError.ErrorType.ParsingError, sException.Message, sScriptFilePath, sParser.Line);
					}
				}
				else //대사입니다.
				{
					try
					{
						this.sScriptLineList.Add(new ScriptDialogueHead(sScriptFilePath, sParser));
						this.sScriptLineList.Add(new ScriptDialogue(sScriptFilePath, sParser));
						this.sScriptLineList.Add(new ScriptDialogueEnd(sScriptFilePath, sParser));
					}
					catch (Exception sException)
					{
						ScriptError.pushError(ScriptError.ErrorType.ParsingError, sException.Message, sScriptFilePath, sParser.Line);
					}
				}
			}
		}

		public void releaseScript()
		{
			this.sParentScript = null;
			this.sScriptLineList = null;
			this.sScriptRegionList = null;
		}

		/// <summary>
		/// 스크립트를 전처리합니다.
		/// </summary>
		/// <param name="sScript">전처리할 스크립트입니다.</param>
		/// <returns>전처리된 스크립트입니다.</returns>
		public static string preprocessScript(string sScript)
		{
			//주석을 제거합니다.
			for (Match sMatch = Script.sCommentMatcher.Match(sScript); sMatch.Success; sMatch = Script.sCommentMatcher.Match(sScript))
			{
				int nCount = 0;

				foreach (char nChar in sMatch.Value)
					if (nChar == '\n')
						++nCount;

				sScript = sScript.Remove(sMatch.Index, sMatch.Length);
				sScript = sScript.Insert(sMatch.Index, new string('\n', nCount));
			}

			//들여쓰기를 제거합니다.
			sScript = Script.sIndentationMatcher.Replace(sScript, "");

			//CR-LF식 새줄 문자를 LF식으로 바꿉니다.
			return Script.sCRLFMatcher.Replace(sScript, "\n");
		}

		/// <summary>
		/// 스크립트 라인을 파싱합니다. autoinsert에서 쓰입니다.
		/// </summary>
		/// <param name="sStringParser">스크립트 라인입니다.</param>
		/// <returns>파싱된 명령입니다.</returns>
		public static ScriptLine parseLine(StringParser sStringParser)
		{
			while(sStringParser.IsRemain)
			{
				sStringParser.skipWhitespace("\n");

				if (sStringParser.tryMatchChar('*')) //구역 지정자입니다.
				{
					ScriptError.pushError(ScriptError.ErrorType.ParsingError, "구역 지정자가 올 수 없습니다.", "EMPTY", -1);
					sStringParser.skipWhile('\n');
					sStringParser.skipWhile(1);
				}
				else if (sStringParser.tryMatchChar('[')) //커맨드입니다.
				{
					try
					{
						return new ScriptTag("EMPTY", sStringParser);
					}
					catch (Exception sException)
					{
						ScriptError.pushError(ScriptError.ErrorType.ParsingError, sException.Message, "EMPTY", -1);
					}
				}
				else if (sStringParser.tryMatchChar('@')) //입력 대기 명령입니다.
				{
					try
					{
						return new ScriptWait("EMPTY", sStringParser);
					}
					catch (Exception sException)
					{
						ScriptError.pushError(ScriptError.ErrorType.ParsingError, sException.Message, "EMPTY", -1);
					}
				}
				else //대사입니다.
				{
					ScriptError.pushError(ScriptError.ErrorType.ParsingError, "대사가 올 수 없습니다.", "EMPTY", -1);
					sStringParser.skipUntil("[@*");
				}
			}

			return null;
		}
	}
}