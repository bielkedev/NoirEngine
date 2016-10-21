﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noir.Equation
{
	public class EquationNodeAnd : IEquationNode
	{
		private IEquationNode sLeft;
		private IEquationNode sRight = null;

		public EquationNodeAnd()
		{
			//Empty.
		}

		private EquationNodeAnd(IEquationNode sNewLeft, IEquationNode sNewRight)
		{
			this.sLeft = sNewLeft;
			this.sRight = sNewRight;
		}

		bool IEquationNode.buildNode(EquationTokenList sTokenList)
		{
			if (!sTokenList.IsRemain)
				return false;

			this.sLeft = new EquationNodeComparison();
			bool bResult = this.sLeft.buildNode(sTokenList);

			if (!sTokenList.IsRemain || sTokenList.Token.eTokenType != EquationTokenType.And)
				return bResult;

			sTokenList.skipToken();
			this.sRight = new EquationNodeComparison();
			this.sRight.buildNode(sTokenList);

			while (sTokenList.IsRemain)
			{
				if (sTokenList.Token.eTokenType == EquationTokenType.And)
				{
					this.sLeft = new EquationNodeAnd(this.sLeft, this.sRight);
					this.sRight = new EquationNodeComparison();
					
					sTokenList.skipToken();
					this.sRight.buildNode(sTokenList);
				}
				else
					break;
			}

			return bResult;
		}

		void IEquationNode.buildInstruction(List<NodeOperator> sOperatorList)
		{
			this.sLeft.buildInstruction(sOperatorList);

			if(this.sRight != null)
			{
				this.sRight.buildInstruction(sOperatorList);
				sOperatorList.Add((Stack<EquationValue> sStack) =>
				{
					EquationValueNumber sRightValue = (EquationValueNumber)sStack.Pop();
					EquationValueNumber sLeftValue = (EquationValueNumber)sStack.Pop();

					sStack.Push(new EquationValueNumber(sLeftValue.Value != 0f && sRightValue.Value != 0f ? 1f : 0f));
				});
			}
		}
	}
}