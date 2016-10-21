using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noir.Equation
{
	public class EquationNodeNumber : IEquationNode
	{
		private float nValue;

		bool IEquationNode.buildNode(EquationTokenList sTokenList)
		{
			if(sTokenList.IsRemain && sTokenList.Token.eTokenType == EquationTokenType.Number)
			{
				this.nValue = float.Parse(sTokenList.Token.sTokenValue);
				sTokenList.skipToken();

				return true;
			}

			return false;
		}

		void IEquationNode.buildInstruction(List<NodeOperator> sOperatorList)
		{
			sOperatorList.Add((Stack<EquationValue> sStack) =>
			{
				sStack.Push(new EquationValueNumber(this.nValue));
			});
		}
	}
}