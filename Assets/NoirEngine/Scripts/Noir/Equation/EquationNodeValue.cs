using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noir.Equation
{
	public class EquationNodeValue : IEquationNode
	{
		private IEquationNode sValueNode;

		bool IEquationNode.buildNode(EquationTokenList sTokenList)
		{
			if (!sTokenList.IsRemain)
				return false;

			switch(sTokenList.Token.eTokenType)
			{
				case EquationTokenType.Number:
				{
					this.sValueNode = new EquationNodeNumber();
					this.sValueNode.buildNode(sTokenList);
				}
				return true;
				case EquationTokenType.String:
				{
					this.sValueNode = new EquationNodeString();
					this.sValueNode.buildNode(sTokenList);
				}
				return true;
				case EquationTokenType.Identifier:
				{
					this.sValueNode = new EquationNodeIdentifier();
					this.sValueNode.buildNode(sTokenList);
				}
				return true;

				default:
				return false;
			}
		}

		void IEquationNode.buildInstruction(List<NodeOperator> sOperatorList)
		{
			this.sValueNode.buildInstruction(sOperatorList);
		}
	}
}