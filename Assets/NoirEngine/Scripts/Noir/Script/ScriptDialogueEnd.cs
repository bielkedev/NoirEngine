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
			if (!ScriptRuntime.CurrentScript.IsMacro && ScriptAutoInsertManager.LineEndScript != null)
				ScriptRuntime.callScript(ScriptAutoInsertManager.LineEndScript);
		}
	}
}