using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Noir.Util;

namespace Noir.Script
{
	public class ScriptTag : ScriptLine
	{
		private string sName;
		private Dictionary<string, string> sAttribute;

		public string Name { get { return this.sName; } }
		public Dictionary<string, string> Attribute { get { return this.sAttribute; } }

		public ScriptTag(string sScriptFilePath, StringParser sStringParser) : base(sScriptFilePath, sStringParser.Line)
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

				string sValue = sStringParser.mergeUntilWithEscape('"', '\\');

				sStringParser.skipWhile(1);

				this.sAttribute.Add(sKey, sValue);

				sStringParser.skipWhitespace();
			}

			sStringParser.skipWhile(1);
		}

		public override void runScript()
		{
			var sHandler = ScriptTagManager.getTagHandler(this.sName);

			if (sHandler == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, this.sName + "은(는) 정의되지 않은 명령어입니다.", this);
				return;
			}

			sHandler(this);
		}
	}
}