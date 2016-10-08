using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Noir.Util;

namespace Noir.Script
{
	public class ScriptDialogue : IScriptLine
	{
		private string sDialogue;

		public string Dialogue { get { return this.sDialogue; } }

		public ScriptDialogue(StringParser sStringParser)
		{

		}

		void IScriptLine.runScript()
		{
			throw new NotImplementedException();
		}
	}
}