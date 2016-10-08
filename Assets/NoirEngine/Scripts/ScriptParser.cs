using UnityEngine;
using System.Collections;
using System.Text;
using Noir.Util;

namespace Noir.Script
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
		private static StringParser sStringParser;

		public void runScript(string sScriptFile)
		{
			var sAsset = Resources.Load<TextAsset>(sScriptFile);

			if(sAsset == null)
			{
				Noir.Error.Error.pushError("스크립트 파일을 찾을 수 없습니다.", sScriptFile, -1);
				return;
			}

			ScriptParser.sStringParser = new StringParser(sAsset.text);
			Resources.UnloadAsset(sAsset);
			
			
		}
	}
}