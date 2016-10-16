﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Noir.Util;

namespace Noir.Script
{
	public class ScriptDialogueEnd : ScriptLine
	{
		public ScriptDialogueEnd(string sScriptFilePath, StringParser sStringParser) : base(sScriptFilePath, sStringParser.Line)
		{
			//Empty.
		}

		public override void runScript()
		{
			if (ScriptAutoInsertManager.LineEndScript != null)
				ScriptRuntime.callScript(ScriptAutoInsertManager.LineEndScript, null);
		}
	}
}