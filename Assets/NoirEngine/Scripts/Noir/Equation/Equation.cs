using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Noir.Equation
{
	public class Equation
	{
		//private static Regex sNumberMatcher = new Regex("^[\\d]+(?:\\.\\d+)?(?:[eE][+-]?\\d+)?", RegexOptions.Compiled);
		//private static Regex sOperatorMatcher = new Regex("^[\\+\\-\\*\\/\\%\\<\\>]|\\&{2}|\\|{2}|\\={2}|(?:\\!\\=)|(?:\\<\\=)|(?:\\>\\=)", RegexOptions.Compiled);

		private Stack<EquationValue> sStack = new Stack<EquationValue>();
		private List<IEquationOperator> sOperationList = new List<IEquationOperator>();
		
		public Equation(string sEquation)
		{

		}
	}
}