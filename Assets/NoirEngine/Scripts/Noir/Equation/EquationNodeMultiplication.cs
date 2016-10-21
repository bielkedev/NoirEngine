using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noir.Equation
{
	public class EquationNodeMultiplication : IEquationNode
	{
		private EquationTokenType eOperatorType = EquationTokenType.Error;
		private IEquationNode sLeft;
		private IEquationNode sRight;

		public EquationNodeMultiplication()
		{
			//Empty.
		}

		private EquationNodeMultiplication(EquationTokenType eNewOperatorType, IEquationNode sNewLeft, IEquationNode sNewRight)
		{
			this.eOperatorType = eNewOperatorType;
			this.sLeft = sNewLeft;
			this.sRight = sNewRight;
		}

		bool IEquationNode.buildNode(EquationTokenList sTokenList)
		{
			if (!sTokenList.IsRemain)
				return false;

			this.sLeft = new EquationNodeSign();
			bool bResult = this.sLeft.buildNode(sTokenList);

			if (!sTokenList.IsRemain)
				return bResult;

			switch(sTokenList.Token.eTokenType)
			{
				case EquationTokenType.Asterisk:
				case EquationTokenType.Division:
				case EquationTokenType.Modulo:
				{
					this.eOperatorType = sTokenList.Token.eTokenType;
				}
				break;

				default:
				return bResult;
			}

			sTokenList.skipToken();
			this.sRight = new EquationNodeSign();
			this.sRight.buildNode(sTokenList);

			while (sTokenList.IsRemain)
			{
				if (sTokenList.Token.eTokenType == EquationTokenType.Asterisk ||
					sTokenList.Token.eTokenType == EquationTokenType.Division ||
					sTokenList.Token.eTokenType == EquationTokenType.Modulo)
				{
					this.sLeft = new EquationNodeMultiplication(this.eOperatorType, this.sLeft, this.sRight);
					this.sRight = new EquationNodeSign();

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
				case EquationTokenType.Asterisk:
				{
					this.sRight.buildInstruction(sOperatorList);
					sOperatorList.Add((Stack<EquationValue> sStack) =>
					{
						EquationValueNumber sRightValue = (EquationValueNumber)sStack.Pop();
						EquationValueNumber sLeftValue = (EquationValueNumber)sStack.Pop();

						sStack.Push(new EquationValueNumber(sLeftValue.Value * sRightValue.Value));
					});
				}
				return;
				case EquationTokenType.Division:
				{
					this.sRight.buildInstruction(sOperatorList);
					sOperatorList.Add((Stack<EquationValue> sStack) =>
					{
						EquationValueNumber sRightValue = (EquationValueNumber)sStack.Pop();
						EquationValueNumber sLeftValue = (EquationValueNumber)sStack.Pop();

						sStack.Push(new EquationValueNumber(sLeftValue.Value / sRightValue.Value));
					});
				}
				return;
				case EquationTokenType.Modulo:
				{
					this.sRight.buildInstruction(sOperatorList);
					sOperatorList.Add((Stack<EquationValue> sStack) =>
					{
						EquationValueNumber sRightValue = (EquationValueNumber)sStack.Pop();
						EquationValueNumber sLeftValue = (EquationValueNumber)sStack.Pop();

						sStack.Push(new EquationValueNumber(sLeftValue.Value % sRightValue.Value));
					});
				}
				return;

				default:
				return;
			}
		}
	}
}
