using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Noir.Script;

namespace Noir.Equation
{
	public class EquationNodeIdentifier : IEquationNode
	{
		private string sValue;

		bool IEquationNode.buildNode(EquationTokenList sTokenList)
		{
			if (sTokenList.IsRemain && sTokenList.Token.eTokenType == EquationTokenType.Identifier)
			{
				this.sValue = sTokenList.Token.sTokenValue;
				sTokenList.skipToken();

				return true;
			}

			return false;
		}

		void IEquationNode.buildInstruction(List<NodeOperator> sOperatorList)
		{
			sOperatorList.Add((Stack<EquationValue> sStack) =>
			{
				string sVariableValue = null;

				if (!ScriptRuntime.CurrentScript.IsMacro || !Macro.getCurrrentVariable().TryGetValue(this.sValue, out sVariableValue))
					sVariableValue = null;

				if (sVariableValue == null)
					sVariableValue = EquationVariable.getVar(this.sValue);

				if (sVariableValue == null)
				{
					sStack.Push(new EquationValueString(""));
					return;
				}

				float nValue;

				if (float.TryParse(sVariableValue, out nValue))
				{
					sStack.Push(new EquationValueNumber(nValue));
					return;
				}

				sStack.Push(new EquationValueString(sVariableValue));
			});
		}
	}
}