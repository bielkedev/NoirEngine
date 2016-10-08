using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noir.Script
{
	public class ScriptRuntime
	{
		private static int nCurrentIndex;
		private static Script sCurrentScript;

		public static void runScript(string sNewScriptPath)
		{
			ScriptRuntime.sCurrentScript = new Script(sNewScriptPath);

			for (ScriptRuntime.nCurrentIndex = 0; ScriptRuntime.nCurrentIndex < ScriptRuntime.sCurrentScript.ScriptLineList.Count; )
				ScriptRuntime.sCurrentScript.ScriptLineList[ScriptRuntime.nCurrentIndex++].runScript();
		}

		public static void gotoScript(string sScriptPath, string sLabel)
		{
			if(string.IsNullOrEmpty(sScriptPath))
			{
				int nNewIndex = 0;

				if(ScriptRuntime.sCurrentScript.RegionList.TryGetValue(sLabel, out nNewIndex))
					ScriptRuntime.nCurrentIndex = nNewIndex;
				else
					ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "레이블을 찾을 수 없습니다.", sLabel, -1);
			}
		}
	}
}