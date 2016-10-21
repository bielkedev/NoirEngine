using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noir.Equation
{
	public class EquationNode : IEquationNode
	{
		private IEquationNode sNode;

		bool IEquationNode.buildNode(EquationTokenList sTokenList)
		{
			this.sNode = new EquationNodeOr();
			return this.sNode.buildNode(sTokenList);
		}

		void IEquationNode.buildInstruction(List<NodeOperator> sOperatorList)
		{
			this.sNode.buildInstruction(sOperatorList);
		}
	}
}