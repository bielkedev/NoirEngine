using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Noir.Unity;
using Noir.Util;

namespace Noir.Script
{
	public class ScriptWait : ScriptLine
	{
		public ScriptWait(string sScriptFilePath, StringParser sStringParser) : base(sScriptFilePath, sStringParser.Line)
		{
			sStringParser.skipWhile(1);
		}

		public override void runScript()
		{
			foreach (Layer sLayer in Layer.NeedUpdateLayerEnumerable)
				sLayer.applyLayerProperties();

			Layer.clearNeedUpdateLayerList();
			ScriptRuntime.suspendScript();
		}
	}
}