using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
		private static Regex sCommentMatcher = new Regex("(?:\\/\\/.*)|(?:\\/\\*(?:(?!\\*\\/)[\\s\\S])*\\*\\/)", RegexOptions.Compiled);

		private List<ScriptLine> sScriptLineList = new List<ScriptLine>();
		private Dictionary<string, int> sRegionList = new Dictionary<string, int>();

		public List<ScriptLine> ScriptLineList { get { return this.sScriptLineList; } }
		public Dictionary<string, int> RegionList { get { return this.sRegionList; } }

		public Script(string sScriptFilePath)
		{
			var sAsset = Resources.Load<TextAsset>(sScriptFilePath);

			if (sAsset == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.ParsingError, "스크립트 파일을 찾을 수 없습니다.", sScriptFilePath, -1);
				return;
			}

			string sText = sAsset.text;
			Resources.UnloadAsset(sAsset);
			sAsset = null;

			//주석을 제거합니다.
			for (Match sMatch = Script.sCommentMatcher.Match(sText); sMatch.Success; sMatch = Script.sCommentMatcher.Match(sText))
			{
				int nCount = 0;

				foreach (char nChar in sMatch.Value)
					if (nChar == '\n')
						++nCount;

				sText = sText.Remove(sMatch.Index, sMatch.Length);
				sText = sText.Insert(sMatch.Index, new string('\n', nCount));
			}

			//들여쓰기를 제거합니다.
			sText = Regex.Replace(sText, "^(?:(?!\\n)\\s)+", "", RegexOptions.Multiline);

			//CR-LF식 새줄 문자를 LF식으로 바꿉니다.
			sText = Regex.Replace(sText, "\\r\\n", "\n");

			//스크립트를 처리합니다.
			StringParser sParser = new StringParser(sText);

			while(sParser.IsRemain)
			{
				sParser.skipWhitespace();

				if(sParser.tryMatchChar('*'))	//구역 지정자입니다.
				{
					sParser.skipWhile(1);
					this.sRegionList.Add(sParser.mergeUntil("\n"), this.sScriptLineList.Count);
					sParser.skipWhile(1);
				}
				else if(sParser.tryMatchChar('['))	//커맨드입니다.
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
				else if(sParser.tryMatchChar('@'))	//입력 대기 명령입니다.
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
				else	//대사입니다.
				{
					try
					{
						this.sScriptLineList.Add(new ScriptDialogue(sScriptFilePath, sParser));
					}
					catch (Exception sException)
					{
						ScriptError.pushError(ScriptError.ErrorType.ParsingError, sException.Message, sScriptFilePath, sParser.Line);
					}
				}
			}
		}
	}
}