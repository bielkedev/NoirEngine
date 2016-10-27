using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Noir.Equation;
using Noir.Util;

namespace Noir.Script
{
	public class ScriptTag : ScriptLine
	{
		private string sName;
		private Dictionary<string, string> sAttribute;
		private Dictionary<string, EquationLine> sAttributeEquationLine;

		public string Name { get { return this.sName; } }
		public Dictionary<string, string> Attribute { get { return this.sAttribute; } }
		public Dictionary<string, EquationLine> EquationAttribute { get { return this.sAttributeEquationLine; } }

		public ScriptTag(string sTagName) : base("", -1)
		{
			this.sName = sTagName;
			this.sAttribute = new Dictionary<string, string>();
			this.sAttributeEquationLine = new Dictionary<string, EquationLine>();
		}

		public ScriptTag(string sScriptFilePath, StringParser sStringParser) : base(sScriptFilePath, sStringParser.Line)
		{
			this.sAttribute = new Dictionary<string, string>();
			this.sAttributeEquationLine = new Dictionary<string, EquationLine>();

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

				if (sValue.StartsWith("$"))
					this.sAttributeEquationLine.Add(sKey, new EquationLine(sValue));
				else
					this.sAttribute.Add(sKey, sValue);

				sStringParser.skipWhitespace();
			}

			sStringParser.skipWhile(1);
		}

		public override void runScript()
		{
			var sHandler = ScriptTagManager.getTagHandler(this.sName);

			if(sHandler != null)
			{
				sHandler(this);
				return;
			}

			if (!Macro.callMacro(this))
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, this.sName + "은(는) 정의되지 않은 명령어 또는 매크로입니다.", this);
		}

		public string getAttribute(string sAttributeName, bool bPushError = true)
		{
			EquationLine sEquationLine = null;

			if (this.sAttributeEquationLine.TryGetValue(sAttributeName, out sEquationLine))
				return sEquationLine.evaluateEquation();

			string sValue;

			if (this.sAttribute.TryGetValue(sAttributeName, out sValue) && !string.IsNullOrEmpty(sValue))
				return sValue;

			if(bPushError)
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "매개변수 '" + sAttributeName + "'가 필요합니다.", this);

			return null;
		}
	}
}