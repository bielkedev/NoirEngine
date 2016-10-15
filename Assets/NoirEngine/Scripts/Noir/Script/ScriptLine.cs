using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noir.Script
{
	public abstract class ScriptLine
	{
		private string sScriptFilePath;
		private int nScriptFileLine;

		public string ScriptFilePath { get { return this.sScriptFilePath; } }
		public int ScriptFileLine { get { return this.nScriptFileLine; } }

		public ScriptLine(string sNewScriptFilePath, int nNewScriptFileLine)
		{
			this.sScriptFilePath = sNewScriptFilePath;
			this.nScriptFileLine = nNewScriptFileLine;
		}

		public abstract void runScript();
	}
}