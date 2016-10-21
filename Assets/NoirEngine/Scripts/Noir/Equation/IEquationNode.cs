using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noir.Equation
{
	public delegate void NodeOperator(Stack<EquationValue> sStack);

	public interface IEquationNode
	{
		bool buildNode(EquationTokenList sTokenList);
		void buildInstruction(List<NodeOperator> sOperatorList);
	}
}