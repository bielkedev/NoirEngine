using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Noir.UI;

namespace Noir.Script
{
	public class ScriptTagManager
	{
		public delegate void ScriptTagHandler(ScriptTag sTag);

		private static Dictionary<string, ScriptTagHandler> sTagHandlerMap = new Dictionary<string, ScriptTagHandler>();
		
		public static void initTagHandler()
		{
			ScriptTagManager.sTagHandlerMap.Add("&autoinsert", ScriptTagManager._autoinsertHandler);
			ScriptTagManager.sTagHandlerMap.Add("jump", ScriptTagManager.jumpHandler);
			ScriptTagManager.sTagHandlerMap.Add("call", ScriptTagManager.callHandler);
			ScriptTagManager.sTagHandlerMap.Add("return", ScriptTagManager.returnHandler);
			ScriptTagManager.sTagHandlerMap.Add("rt", ScriptTagManager.rtHandler);
			ScriptTagManager.sTagHandlerMap.Add("rp", ScriptTagManager.rpHandler);
			ScriptTagManager.sTagHandlerMap.Add("stop", ScriptTagManager.stopHandler);
		}

		public static ScriptTagHandler getTagHandler(string sTagName)
		{
			ScriptTagHandler sHandler;

			return ScriptTagManager.sTagHandlerMap.TryGetValue(sTagName, out sHandler) ? sHandler : null;
		}

		private static void _autoinsertHandler(ScriptTag sTag)
		{
			string sTarget;

			if(!sTag.Attribute.TryGetValue("target", out sTarget))
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'target' 인자가 필요합니다.", sTag);
				return;
			}

			string sCommand;

			if (!sTag.Attribute.TryGetValue("command", out sCommand))
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'command' 인자가 필요합니다.", sTag);
				return;
			}

			switch(sTarget)
			{
				case "blankline":
				{
					ScriptAutoInsertManager.setBlackLineTag(sCommand);
				}
				return;
				case "linehead":
				{
					ScriptAutoInsertManager.setLineHeadTagList(sCommand);
				}
				return;
				case "lineend":
				{
					ScriptAutoInsertManager.setLineEndTagList(sCommand);
				}
				return;
				default:
				{
					ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sTarget + "'은(는) 잘못된 값입니다. 'blankline', 'linehead', 'lineend' 중 하나여야합니다.", sTag);
				}
				return;
			}
		}

		private static void jumpHandler(ScriptTag sTag)
		{
			string sFile = null;

			if (!sTag.Attribute.TryGetValue("file", out sFile))
				sFile = null;

			string sLabel = null;

			if (!sTag.Attribute.TryGetValue("label", out sLabel))
				sLabel = null;

			ScriptRuntime.gotoScript(sFile, sLabel);
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

		private static void rtHandler(ScriptTag sTag)
		{
			UIManager.appendDialogueText("\n");
		}

		private static void rpHandler(ScriptTag sTag)
		{
			UIManager.clearDialogueText();
		}

		private static void stopHandler(ScriptTag sTag)
		{
			ScriptRuntime.suspendScript();
		}
	}
}