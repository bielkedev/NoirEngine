using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noir.Equation
{
	public class EquationNodeSign : IEquationNode
	{
		private bool bMinus = false;
		private IEquationNode sSubNode;

		bool IEquationNode.buildNode(EquationTokenList sTokenList)
		{
			if (sTokenList.IsRemain)
			{
				if (sTokenList.Token.eTokenType == EquationTokenType.Plus)
				{
					sTokenList.skipToken();

					this.bMinus = false;
					this.sSubNode = new EquationNodeBracket();
					return this.sSubNode.buildNode(sTokenList);
				}
				else if (sTokenList.Token.eTokenType == EquationTokenType.Minus)
				{
					sTokenList.skipToken();

					this.bMinus = true;
					this.sSubNode = new EquationNodeBracket();
					return this.sSubNode.buildNode(sTokenList);
				}
			}

			this.bMinus = false;
			this.sSubNode = new EquationNodeBracket();
			return this.sSubNode.buildNode(sTokenList);
		}

		void IEquationNode.buildInstruction(List<NodeOperator> sOperatorList)
		{
			this.sSubNode.buildInstruction(sOperatorList);

			if (this.bMinus)
			{
				sOperatorList.Add((Stack<EquationValue> sStack) =>
				{
					if(sStack.Peek().IsNumeric)
					{
						sStack.Push(new EquationValueNumber(-((EquationValueNumber)sStack.Pop()).Value));
					}
				});
			}
		}
	}
}