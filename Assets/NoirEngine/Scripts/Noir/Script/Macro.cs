using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Noir.Equation;

namespace Noir.Script
{
	public struct MacroRegion
	{
		public int nMacroLine;
		public Script sMacroScript;
	}

	public class Macro
	{
		private static Dictionary<string, MacroRegion> sMacroList = new Dictionary<string, MacroRegion>();
		private static Stack<Dictionary<string, string>> sMacroVariableStack = new Stack<Dictionary<string, string>>();

		public static void addMacroScript(string sMacroScriptFilePath)
		{
			MacroRegion sRegion = new MacroRegion();
			Script sScript = new Script(sMacroScriptFilePath, true);

			sRegion.sMacroScript = sScript;

			foreach (var sPair in sScript.ScriptRegionList)
			{
				sRegion.nMacroLine = sPair.Value;
				Macro.sMacroList.Add(sPair.Key, sRegion);
			}
		}

		public static bool callMacro(ScriptTag sTag)
		{
			MacroRegion sMacroRegion;

			if(sMacroList.TryGetValue(sTag.Name, out sMacroRegion))
			{
				var sVarList = Macro.pushMacroVariable();

				foreach (var sPair in sTag.Attribute)
					sVarList.Add(sPair.Key, sPair.Value);

				foreach (var sPair in sTag.EquationAttribute)
					sVarList.Add(sPair.Key, sPair.Value.evaluateEquation());

				ScriptRuntime.callScript(sMacroRegion.sMacroScript, sMacroRegion.nMacroLine);

				return true;
			}

			return false;
		}

		public static Dictionary<string, string> pushMacroVariable()
		{
			Dictionary<string, string> sStack = new Dictionary<string, string>();

			Macro.sMacroVariableStack.Push(sStack);

			return sStack;
		}

		public static void popMacroVariable()
		{
			if (Macro.sMacroVariableStack.Count > 0)
				Macro.sMacroVariableStack.Pop();
		}

		public static Dictionary<string, string> getCurrrentVariable()
		{
			return Macro.sMacroVariableStack.Count > 0 ? Macro.sMacroVariableStack.Peek() : null;
		}
	}
}