using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Noir.Script;

namespace Noir.Equation
{
	public class EquationLine
	{
		private Stack<EquationValue> sStack = new Stack<EquationValue>();
		private List<NodeOperator> sOperatorList = new List<NodeOperator>();
		
		public EquationLine(string sEquation)
		{
			if (!sEquation.StartsWith("$"))
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "수식이 아닙니다.", "EMPTY", -1);
				return;
			}

			EquationTokenList sTokenList = new EquationTokenList(sEquation.Substring(1));
			IEquationNode sNode = new EquationNode();

			if (!sNode.buildNode(sTokenList))
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "수식이 잘못되었습니다.", "EMPTY", -1);
				return;
			}

			sNode.buildInstruction(this.sOperatorList);
		}

		public string evaluateEquation()
		{
			for (int nIndex = 0; nIndex < this.sOperatorList.Count; ++nIndex)
				this.sOperatorList[nIndex](this.sStack);

			if(this.sStack.Count != 1)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "계산 결과에 문제가 있습니다.", "EMPTY", -1);
				return "";
			}

			return this.sStack.Peek().IsString ? ((EquationValueString)this.sStack.Pop()).Value : ((EquationValueNumber)this.sStack.Pop()).Value.ToString();
		}
	}
}