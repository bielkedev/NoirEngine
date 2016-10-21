using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noir.Equation
{
	public class EquationNodeBracket : IEquationNode
	{
		private IEquationNode sSubNode;

		bool IEquationNode.buildNode(EquationTokenList sTokenList)
		{
			if(sTokenList.IsRemain && sTokenList.Token.eTokenType == EquationTokenType.BracketL)
			{
				sTokenList.skipToken();

				this.sSubNode = new EquationNode();
				this.sSubNode.buildNode(sTokenList);

				if(sTokenList.IsRemain && sTokenList.Token.eTokenType == EquationTokenType.BracketR)
					sTokenList.skipToken();

				return true;
			}

			this.sSubNode = new EquationNodeValue();
			return this.sSubNode.buildNode(sTokenList);
		}

		void IEquationNode.buildInstruction(List<NodeOperator> sOperatorList)
		{
			this.sSubNode.buildInstruction(sOperatorList);
		}
	}
}