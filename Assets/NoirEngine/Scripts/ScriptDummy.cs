using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Noir.Util;
using UnityEngine;

namespace Noir.Script
{
	public class ScriptDummy : IScriptLine
	{
		public ScriptDummy(StringParser sStringParser)
		{
			sStringParser.skipWhile(1);
		}

		void IScriptLine.runScript()
		{
			Debug.Log("입력대기");
		}
	}
}