using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noir.Equation
{
	public class EquationNodeComparison : IEquationNode
	{
		private EquationTokenType eOperatorType = EquationTokenType.Error;
		private IEquationNode sLeft;
		private IEquationNode sRight;

		public EquationNodeComparison()
		{
			//Empty.
		}

		private EquationNodeComparison(EquationTokenType eNewOperatorType, IEquationNode sNewLeft, IEquationNode sNewRight)
		{
			this.eOperatorType = eNewOperatorType;
			this.sLeft = sNewLeft;
			this.sRight = sNewRight;
		}

		bool IEquationNode.buildNode(EquationTokenList sTokenList)
		{
			if (!sTokenList.IsRemain)
				return false;

			this.sLeft = new EquationNodeAmount();
			bool bResult = this.sLeft.buildNode(sTokenList);

			if (!sTokenList.IsRemain)
				return bResult;

			switch (sTokenList.Token.eTokenType)
			{
				case EquationTokenType.Equal:
				case EquationTokenType.NotEqual:
				{
					this.eOperatorType = sTokenList.Token.eTokenType;
				}
				break;

				default:
				return bResult;
			}

			sTokenList.skipToken();
			this.sRight = new EquationNodeAmount();
			this.sRight.buildNode(sTokenList);

			while (sTokenList.IsRemain)
			{
				if (sTokenList.Token.eTokenType == EquationTokenType.Equal ||
					sTokenList.Token.eTokenType == EquationTokenType.NotEqual)
				{
					this.sLeft = new EquationNodeComparison(this.eOperatorType, this.sLeft, this.sRight);
					this.sRight = new EquationNodeAmount();

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
				case EquationTokenType.Equal:
				{
					this.sRight.buildInstruction(sOperatorList);
					sOperatorList.Add((Stack<EquationValue> sStack) =>
					{
						EquationValue sRightValue = sStack.Pop();
						EquationValue sLeftValue = sStack.Pop();

						if (sLeftValue.IsNumeric == sRightValue.IsNumeric)
							if(sLeftValue.IsNumeric)
								sStack.Push(new EquationValueNumber(((EquationValueNumber)sLeftValue).Value == ((EquationValueNumber)sRightValue).Value ? 1f : 0f));
							else
								sStack.Push(new EquationValueNumber(((EquationValueString)sLeftValue).Value == ((EquationValueString)sRightValue).Value ? 1f : 0f));
						else
							sStack.Push(new EquationValueNumber(0.0f));
					});
				}
				return;
				case EquationTokenType.NotEqual:
				{
					this.sRight.buildInstruction(sOperatorList);
					sOperatorList.Add((Stack<EquationValue> sStack) =>
					{
						EquationValue sRightValue = sStack.Pop();
						EquationValue sLeftValue = sStack.Pop();

						if (sLeftValue.IsNumeric == sRightValue.IsNumeric)
							if (sLeftValue.IsNumeric)
								sStack.Push(new EquationValueNumber(((EquationValueNumber)sLeftValue).Value != ((EquationValueNumber)sRightValue).Value ? 1f : 0f));
							else
								sStack.Push(new EquationValueNumber(((EquationValueString)sLeftValue).Value != ((EquationValueString)sRightValue).Value ? 1f : 0f));
						else
							sStack.Push(new EquationValueNumber(1.0f));
					});
				}
				return;

				default:
				return;
			}
		}
	}
}