using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noir.Equation
{
	public class EquationValueString : EquationValue
	{
		public override bool IsNumeric { get { return false; } }
		public override bool IsString { get { return true; } }
		public string Value { get; set; }

		public EquationValueString(string sNewValue)
		{
			this.Value = sNewValue;
		}
	}
}