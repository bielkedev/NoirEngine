using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Noir.Util;

namespace Noir.Script
{
	public class ScriptBlankLine : ScriptLine
	{
		public ScriptBlankLine(string sScriptFilePath, StringParser sStringParser) : base(sScriptFilePath, sStringParser.Line)
		{
			//Empty.
		}

		public override void runScript()
		{
			if (!ScriptRuntime.CurrentScript.IsMacro && ScriptAutoInsertManager.BlackLineScript != null)
				ScriptRuntime.callScript(ScriptAutoInsertManager.BlackLineScript);
		}
	}
}