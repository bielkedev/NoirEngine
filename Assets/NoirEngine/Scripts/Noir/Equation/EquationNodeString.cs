using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noir.Equation
{
	public class EquationNodeString : IEquationNode
	{
		private string sValue;

		bool IEquationNode.buildNode(EquationTokenList sTokenList)
		{
			if (sTokenList.IsRemain && sTokenList.Token.eTokenType == EquationTokenType.String)
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
				sStack.Push(new EquationValueString(this.sValue));
			});
		}
	}
}