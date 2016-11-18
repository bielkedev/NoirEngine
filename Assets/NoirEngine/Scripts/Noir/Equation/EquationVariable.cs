using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noir.Equation
{
	public class EquationVariable
	{
		private static Dictionary<string, string> sVariableList = new Dictionary<string, string>();

		public static void initEquationVariable()
		{
			EquationVariable.sVariableList.Clear();

			EquationVariable.sVariableList.Add("s.engineversion", "0.0.1a");
			EquationVariable.sVariableList.Add("s.datapath", "Scenario");
			EquationVariable.sVariableList.Add("s.savepath", "Save");
			EquationVariable.sVariableList.Add("s.bgmvol", "1000");
			EquationVariable.sVariableList.Add("s.sevol", "1000");
		}

		public static string getVar(string sVariableName)
		{
			string sValue = null;

			return EquationVariable.sVariableList.TryGetValue(sVariableName, out sValue) ? sValue : null;
		}

		public static void setVar(string sVariableName, string sNewValue)
		{
			EquationVariable.sVariableList[sVariableName] = sNewValue;
		}

		public static void deleteVar(string sVariableName)
		{
			EquationVariable.sVariableList.Remove(sVariableName);
		}
	}
}