using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noir.Equation
{
	public class EquationNodeAddition : IEquationNode
	{
		private EquationTokenType eOperatorType = EquationTokenType.Error;
		private IEquationNode sLeft;
		private IEquationNode sRight;

		public EquationNodeAddition()
		{
			//Empty.
		}

		private EquationNodeAddition(EquationTokenType eNewOperatorType, IEquationNode sNewLeft, IEquationNode sNewRight)
		{
			this.eOperatorType = eNewOperatorType;
			this.sLeft = sNewLeft;
			this.sRight = sNewRight;
		}

		bool IEquationNode.buildNode(EquationTokenList sTokenList)
		{
			if (!sTokenList.IsRemain)
				return false;

			this.sLeft = new EquationNodeMultiplication();
			bool bResult = this.sLeft.buildNode(sTokenList);

			if (!sTokenList.IsRemain)
				return bResult;

			switch (sTokenList.Token.eTokenType)
			{
				case EquationTokenType.Plus:
				case EquationTokenType.Minus:
				{
					this.eOperatorType = sTokenList.Token.eTokenType;
				}
				break;

				default:
				return bResult;
			}

			sTokenList.skipToken();
			this.sRight = new EquationNodeMultiplication();
			this.sRight.buildNode(sTokenList);

			while (sTokenList.IsRemain)
			{
				if (sTokenList.Token.eTokenType == EquationTokenType.Plus ||
					sTokenList.Token.eTokenType == EquationTokenType.Minus)
				{
					this.sLeft = new EquationNodeAddition(this.eOperatorType, this.sLeft, this.sRight);
					this.sRight = new EquationNodeMultiplication();

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
				case EquationTokenType.Plus:
				{
					this.sRight.buildInstruction(sOperatorList);
					sOperatorList.Add((Stack<EquationValue> sStack) =>
					{
						EquationValue sRightValue = sStack.Pop();
						EquationValue sLeftValue = sStack.Pop();

						if (sLeftValue.IsNumeric == sRightValue.IsNumeric)
						{
							if (sLeftValue.IsNumeric)
								sStack.Push(new EquationValueNumber(((EquationValueNumber)sLeftValue).Value + ((EquationValueNumber)sRightValue).Value));
							else
								sStack.Push(new EquationValueString(((EquationValueString)sLeftValue).Value + ((EquationValueString)sRightValue).Value));
						}
						else if(sLeftValue.IsNumeric)
							sStack.Push(new EquationValueString(((EquationValueNumber)sLeftValue).Value + ((EquationValueString)sRightValue).Value));
						else
							sStack.Push(new EquationValueString(((EquationValueString)sLeftValue).Value + ((EquationValueNumber)sRightValue).Value));
					});
				}
				return;
				case EquationTokenType.Minus:
				{
					this.sRight.buildInstruction(sOperatorList);
					sOperatorList.Add((Stack<EquationValue> sStack) =>
					{
						EquationValueNumber sRightValue = (EquationValueNumber)sStack.Pop();
						EquationValueNumber sLeftValue = (EquationValueNumber)sStack.Pop();

						sStack.Push(new EquationValueNumber(sLeftValue.Value - sRightValue.Value));
					});
				}
				return;

				default:
				return;
			}
		}
	}
}