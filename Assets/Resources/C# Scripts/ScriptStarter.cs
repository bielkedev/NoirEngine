using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Noir.Script;

namespace Assets.Resources.C__Scripts
{
	public class ScriptStarter : MonoBehaviour
	{
		public string ScriptFilePath;

		private void Start()
		{
			ScriptTagManager.initTagHandler();

			ScriptRuntime.callScript(this.ScriptFilePath, null);
			ScriptRuntime.runScript();
		}
	}
}