using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Noir.Util;

namespace Noir.Script
{
	public class ScriptDialogueHead : ScriptLine
	{
		public ScriptDialogueHead(string sScriptFilePath, StringParser sStringParser) : base(sScriptFilePath, sStringParser.Line)
		{
			//Empty.
		}

		public override void runScript()
		{
			if(ScriptAutoInsertManager.LineHeadScript != null)
				ScriptRuntime.callScript(ScriptAutoInsertManager.LineHeadScript, null);
		}
	}
}