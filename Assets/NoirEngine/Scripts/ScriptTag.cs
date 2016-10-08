using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Noir.Util;
using UnityEngine;

namespace Noir.Script
{
	public class ScriptTag : IScriptLine
	{
		private string sName;
		private Dictionary<string, string> sAttribute;

		public string Name { get { return this.sName; } }
		public Dictionary<string, string> Attribute { get { return this.sAttribute; } }

		public ScriptTag(StringParser sStringParser)
		{
			this.sAttribute = new Dictionary<string, string>();

			if(!sStringParser.tryMatchChar('['))
				throw new FormatException("태그가 시작되지 않았습니다.");
			
			sStringParser.skipWhile(1);

			this.sName = sStringParser.mergeBlackspace("]");

			sStringParser.skipWhitespace();

			while(sStringParser.tryNotMatchChar(']'))
			{
				string sKey = sStringParser.mergeBlackspace("=");
				sStringParser.skipWhitespace();

				if (!sStringParser.tryMatchChar('='))
					throw new FormatException("'='가 없습니다.");

				sStringParser.skipWhile(1);
				sStringParser.skipWhitespace();

				if (!sStringParser.tryMatchChar('"'))
					throw new FormatException("'\"'가 없습니다.");

				sStringParser.skipWhile(1);

				string sValue = sStringParser.mergeUntil('"');

				sStringParser.skipWhile(1);

				this.sAttribute.Add(sKey, sValue);

				sStringParser.skipWhitespace();
			}

			sStringParser.skipWhile(1);
		}

		void IScriptLine.runScript()
		{
			Debug.Log("명령 : " + this.sName);
		}
	}
}