using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noir.Script
{
	public class ScriptTagManager
	{
		public delegate void ScriptTagHandler(ScriptTag sTag);

		private static Dictionary<string, ScriptTagHandler> sTagHandlerMap = new Dictionary<string, ScriptTagHandler>();
		
		public static void initTagHandler()
		{
			ScriptTagManager.sTagHandlerMap.Add("call", ScriptTagManager.callHandler);
			ScriptTagManager.sTagHandlerMap.Add("return", ScriptTagManager.returnHandler);
		}

		public static ScriptTagHandler getTagHandler(string sTagName)
		{
			ScriptTagHandler sHandler;

			return ScriptTagManager.sTagHandlerMap.TryGetValue(sTagName, out sHandler) ? sHandler : null;
		}

		private static void callHandler(ScriptTag sTag)
		{
			string sFile = null;

			if (!sTag.Attribute.TryGetValue("file", out sFile))
				sFile = null;

			string sLabel = null;

			if (!sTag.Attribute.TryGetValue("label", out sLabel))
				sLabel = null;

			ScriptRuntime.callScript(sFile, sLabel);
		}

		private static void returnHandler(ScriptTag sTag)
		{
			ScriptRuntime.returnScript();
		}

		private static void debugHandler(ScriptTag sTag)
		{

		}
	}
}