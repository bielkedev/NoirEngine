using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noir.Equation
{
	public class EquationValueNumber : EquationValue
	{
		public override bool IsNumeric { get { return true; } }
		public override bool IsString { get { return false; } }
		public float Value { get; set; }

		public EquationValueNumber(float nNewNumber)
		{
			this.Value = nNewNumber;
		}
	}
}