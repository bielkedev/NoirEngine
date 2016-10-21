using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noir.Equation
{
	public class EquationNodeAmount : IEquationNode
	{
		private EquationTokenType eOperatorType = EquationTokenType.Error;
		private IEquationNode sLeft;
		private IEquationNode sRight;

		public EquationNodeAmount()
		{
			//Empty.
		}

		private EquationNodeAmount(EquationTokenType eNewOperatorType, IEquationNode sNewLeft, IEquationNode sNewRight)
		{
			this.eOperatorType = eNewOperatorType;
			this.sLeft = sNewLeft;
			this.sRight = sNewRight;
		}

		bool IEquationNode.buildNode(EquationTokenList sTokenList)
		{
			if (!sTokenList.IsRemain)
				return false;

			this.sLeft = new EquationNodeAddition();
			bool bResult = this.sLeft.buildNode(sTokenList);

			if (!sTokenList.IsRemain)
				return bResult;

			switch (sTokenList.Token.eTokenType)
			{
				case EquationTokenType.Less:
				case EquationTokenType.LessEq:
				case EquationTokenType.Greater:
				case EquationTokenType.GreaterEq:
				{
					this.eOperatorType = sTokenList.Token.eTokenType;
				}
				break;

				default:
				return bResult;
			}

			sTokenList.skipToken();
			this.sRight = new EquationNodeAddition();
			this.sRight.buildNode(sTokenList);

			while (sTokenList.IsRemain)
			{
				if (sTokenList.Token.eTokenType == EquationTokenType.Less ||
					sTokenList.Token.eTokenType == EquationTokenType.LessEq ||
					sTokenList.Token.eTokenType == EquationTokenType.Greater ||
					sTokenList.Token.eTokenType == EquationTokenType.GreaterEq)
				{
					this.sLeft = new EquationNodeAmount(this.eOperatorType, this.sLeft, this.sRight);
					this.sRight = new EquationNodeAddition();

					this.eOperatorType = sTokenList.Token.eTokenType;
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

			switch (this.eOperatorType)
			{
				case EquationTokenType.Less:
				{
					this.sRight.buildInstruction(sOperatorList);
					sOperatorList.Add((Stack<EquationValue> sStack) =>
					{
						EquationValueNumber sRightValue = (EquationValueNumber)sStack.Pop();
						EquationValueNumber sLeftValue = (EquationValueNumber)sStack.Pop();

						sStack.Push(new EquationValueNumber(sLeftValue.Value < sRightValue.Value ? 1f : 0f));
					});
				}
				return;
				case EquationTokenType.LessEq:
				{
					this.sRight.buildInstruction(sOperatorList);
					sOperatorList.Add((Stack<EquationValue> sStack) =>
					{
						EquationValueNumber sRightValue = (EquationValueNumber)sStack.Pop();
						EquationValueNumber sLeftValue = (EquationValueNumber)sStack.Pop();

						sStack.Push(new EquationValueNumber(sLeftValue.Value <= sRightValue.Value ? 1f : 0f));
					});
				}
				return;
				case EquationTokenType.Greater:
				{
					this.sRight.buildInstruction(sOperatorList);
					sOperatorList.Add((Stack<EquationValue> sStack) =>
					{
						EquationValueNumber sRightValue = (EquationValueNumber)sStack.Pop();
						EquationValueNumber sLeftValue = (EquationValueNumber)sStack.Pop();

						sStack.Push(new EquationValueNumber(sLeftValue.Value > sRightValue.Value ? 1f : 0f));
					});
				}
				return;
				case EquationTokenType.GreaterEq:
				{
					this.sRight.buildInstruction(sOperatorList);
					sOperatorList.Add((Stack<EquationValue> sStack) =>
					{
						EquationValueNumber sRightValue = (EquationValueNumber)sStack.Pop();
						EquationValueNumber sLeftValue = (EquationValueNumber)sStack.Pop();

						sStack.Push(new EquationValueNumber(sLeftValue.Value >= sRightValue.Value ? 1f : 0f));
					});
				}
				return;

				default:
				return;
			}
		}
	}
}