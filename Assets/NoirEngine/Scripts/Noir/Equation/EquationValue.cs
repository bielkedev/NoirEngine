using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noir.Equation
{
	public abstract class EquationValue
	{
		public abstract bool IsNumeric { get; }
		public abstract bool IsString { get; }
		

	}
}