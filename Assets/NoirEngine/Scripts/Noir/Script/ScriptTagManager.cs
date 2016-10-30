using System;
using System.Collections.Generic;
using Noir.Equation;
using Noir.UI;
using Noir.Unity;
using Noir.Util;

namespace Noir.Script
{
	public class ScriptTagManager
	{
		public delegate void ScriptTagHandler(ScriptTag sTag);

		private static Random sRandom = new Random();
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
			ScriptTagManager.sTagHandlerMap.Add("lyc", ScriptTagManager.lycHandler);
			ScriptTagManager.sTagHandlerMap.Add("lydel", ScriptTagManager.lydelHandler);
			ScriptTagManager.sTagHandlerMap.Add("lyprop", ScriptTagManager.lypropHandler);
			ScriptTagManager.sTagHandlerMap.Add("var", ScriptTagManager.varHandler);
			ScriptTagManager.sTagHandlerMap.Add("lytweendel", ScriptTagManager.lytweendelHandler);
			ScriptTagManager.sTagHandlerMap.Add("lytween", ScriptTagManager.lytweenHandler);

		}

		public static ScriptTagHandler getTagHandler(string sTagName)
		{
			ScriptTagHandler sHandler;

			ScriptTagManager.sTagHandlerMap.TryGetValue(sTagName, out sHandler);

			return sHandler;
		}

		private static void _autoinsertHandler(ScriptTag sTag)
		{
			string sTarget = null;

			if ((sTarget = sTag.getAttribute("target")) == null)
				return;

			string sCommand = null;

			if ((sCommand = sTag.getAttribute("command")) == null)
				return;

			switch (sTarget)
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
			if (!ScriptRuntime.gotoScript(sTag.getAttribute("file", false), sTag.getAttribute("label", false)))
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "점프에 실패했습니다. 존재하지 않는 파일 또는 레이블입니다.", sTag);
		}

		private static void callHandler(ScriptTag sTag)
		{
			if (!ScriptRuntime.callScript(sTag.getAttribute("file", false), sTag.getAttribute("label", false)))
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "호출에 실패했습니다. 존재하지 않는 파일 또는 레이블입니다.", sTag);
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

		private static void lycHandler(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (string.IsNullOrEmpty(sID))
				return;

			Layer sLayer = Layer.getLayer(sID);

			if (sLayer == null)
				sLayer = new Layer(sID);

			string sFile = sTag.getAttribute("file");

			if (string.IsNullOrEmpty(sFile))
				return;

			UnityEngine.Sprite sMainSprite = SpriteManager.loadSprite(sFile);

			if (sMainSprite == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sFile + "'에 스프라이트가 없습니다.", sTag);
				return;
			}

			UnityEngine.Sprite sMaskSprite = null;

			if (!string.IsNullOrEmpty(sFile = sTag.getAttribute("mask", false)) && (sMaskSprite = SpriteManager.loadSprite(sFile)) == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sFile + "'에 스프라이트가 없습니다.", sTag);
				return;
			}

			sLayer.setLayerSprite(sMainSprite, sMaskSprite);
		}

		private static void lydelHandler(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (!string.IsNullOrEmpty(sID))
			{
				Layer sLayer = Layer.getLayer(sID);

				if (sLayer == null)
				{
					ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) 없는 레이어입니다.", sTag);
					return;
				}

				sLayer.deleteLayer();
			}
		}

		private static void varHandler(ScriptTag sTag)
		{
			string sVariableName = sTag.getAttribute("name");
			string sSystem = sTag.getAttribute("system", false);

			if (string.IsNullOrEmpty(sSystem))
			{
				string sData = sTag.getAttribute("data");

				EquationVariable.setVar(sVariableName, sData == null ? "" : sData);
				return;
			}

			switch (sSystem)
			{
				case "var_exist":
				{
					string sTarget = sTag.getAttribute("target");
					string sLocal = sTag.getAttribute("local");

					if (string.IsNullOrEmpty(sTarget) || string.IsNullOrEmpty(sLocal))
						return;

					if (sLocal == "0")
						EquationVariable.setVar(sVariableName, EquationVariable.getVar(sTarget) == null ? "0" : "1");
					else if (ScriptRuntime.CurrentScript.IsMacro)
						EquationVariable.setVar(sVariableName, Macro.getCurrrentVariable().ContainsKey(sTarget) ? "1" : "0");
				}
				return;
				case "random":
				{
					string sMin = sTag.getAttribute("min");
					string sMax = sTag.getAttribute("max");

					int nMin;
					int nMax;

					if (!string.IsNullOrEmpty(sMin) && !string.IsNullOrEmpty(sMax) && int.TryParse(sMin, out nMin) && int.TryParse(sMax, out nMax))
						EquationVariable.setVar(sVariableName, ScriptTagManager.sRandom.Next(nMin, nMax).ToString());
				}
				return;
				case "length":
				{
					string sSource = sTag.getAttribute("source");

					if (!string.IsNullOrEmpty(sSource))
					{
						string sValue = EquationVariable.getVar(sSource);
						EquationVariable.setVar(sVariableName, string.IsNullOrEmpty(sValue) ? "0" : sValue.Length.ToString());
					}
				}
				return;
				default:
				return;
			}
		}

		private static void lypropHandler(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (string.IsNullOrEmpty(sID))
				return;

			Layer sLayer = Layer.getLayer(sID);

			if (sLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) 없는 레이어입니다.", sTag);
				return;
			}

			string sValue;
			float nValue;

			if (!string.IsNullOrEmpty(sValue = sTag.getAttribute("left", false)))
				if (float.TryParse(sValue, out nValue))
					sLayer.setPosX(nValue);
				else
					ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sValue + "'(은)는 숫자가 아닙니다.", sTag);

			if (!string.IsNullOrEmpty(sValue = sTag.getAttribute("top", false)))
				if (float.TryParse(sValue, out nValue))
					sLayer.setPosY(nValue);
				else
					ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sValue + "'(은)는 숫자가 아닙니다.", sTag);

			if (!string.IsNullOrEmpty(sValue = sTag.getAttribute("alpha", false)))
				if (float.TryParse(sValue, out nValue))
					sLayer.setAlpha(nValue);
				else
					ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sValue + "'(은)는 숫자가 아닙니다.", sTag);

			if (!string.IsNullOrEmpty(sValue = sTag.getAttribute("anchorx", false)))
				if (float.TryParse(sValue, out nValue))
					sLayer.setAnchorX(nValue);
				else
					ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sValue + "'(은)는 숫자가 아닙니다.", sTag);

			if (!string.IsNullOrEmpty(sValue = sTag.getAttribute("anchory", false)))
				if (float.TryParse(sValue, out nValue))
					sLayer.setAnchorY(nValue);
				else
					ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sValue + "'(은)는 숫자가 아닙니다.", sTag);

			if (!string.IsNullOrEmpty(sValue = sTag.getAttribute("xscale", false)))
				if (float.TryParse(sValue, out nValue))
					sLayer.setScaleX(nValue);
				else
					ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sValue + "'(은)는 숫자가 아닙니다.", sTag);

			if (!string.IsNullOrEmpty(sValue = sTag.getAttribute("yscale", false)))
				if (float.TryParse(sValue, out nValue))
					sLayer.setScaleY(nValue);
				else
					ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sValue + "'(은)는 숫자가 아닙니다.", sTag);

			if (!string.IsNullOrEmpty(sValue = sTag.getAttribute("rotate", false)))
				if (float.TryParse(sValue, out nValue))
					sLayer.setRotate(nValue);
				else
					ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sValue + "'(은)는 숫자가 아닙니다.", sTag);

			if (!string.IsNullOrEmpty(sValue = sTag.getAttribute("reversex", false)))
				if (float.TryParse(sValue, out nValue))
					sLayer.setReverseX(nValue);
				else
					ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sValue + "'(은)는 숫자가 아닙니다.", sTag);

			if (!string.IsNullOrEmpty(sValue = sTag.getAttribute("reversey", false)))
				if (float.TryParse(sValue, out nValue))
					sLayer.setReverseY(nValue);
				else
					ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sValue + "'(은)는 숫자가 아닙니다.", sTag);

			if (!string.IsNullOrEmpty(sValue = sTag.getAttribute("visible", false)))
				if (float.TryParse(sValue, out nValue))
					sLayer.setVisible(nValue);
				else
					ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sValue + "'(은)는 숫자가 아닙니다.", sTag);
		}

		private static void lytweendelHandler(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (string.IsNullOrEmpty(sID))
				return;

			Layer sLayer = Layer.getLayer(sID);

			if (sLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) 없는 레이어입니다.", sTag);
				return;
			}

			sLayer.removeLayerTweenAll();
		}

		private static void lytweenHandler(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (string.IsNullOrEmpty(sID))
				return;

			Layer sLayer = Layer.getLayer(sID);

			if (sLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) 없는 레이어입니다.", sTag);
				return;
			}

			LayerPropertiesModifier fModifier;

			string sValue = sTag.getAttribute("param");

			if (string.IsNullOrEmpty(sValue))
				return;

			switch (sValue)
			{
				case "left":
				fModifier = sLayer.setPosX;
				break;

				case "top":
				fModifier = sLayer.setPosY;
				break;

				case "alpha":
				fModifier = sLayer.setAlpha;
				break;

				case "xscale":
				fModifier = sLayer.setScaleX;
				break;

				case "yscale":
				fModifier = sLayer.setScaleY;
				break;

				case "rotate":
				fModifier = sLayer.setRotate;
				break;

				default:
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sValue + "'은(는) 잘못된 속성입니다. 'left', 'top', 'alpha', 'xscale', 'yscale', 'rotate' 중 하나여야합니다.", sTag);
				return;
			}

			LayerTweenData sTweenData = new LayerTweenData();

			if (string.IsNullOrEmpty(sValue = sTag.getAttribute("ease", false)))
				sTweenData.fEasingFunc = EasingFunction.Linear;
			else
				switch (sValue)
				{
					//Quadratic
					case "easein_quad":
					sTweenData.fEasingFunc = EasingFunction.Quadratic.In;
					break;

					case "easeout_quad":
					sTweenData.fEasingFunc = EasingFunction.Quadratic.Out;
					break;

					case "easeinout_quad":
					sTweenData.fEasingFunc = EasingFunction.Quadratic.InOut;
					break;

					//Cubic
					case "easein_cubic":
					sTweenData.fEasingFunc = EasingFunction.Cubic.In;
					break;

					case "easeout_cubic":
					sTweenData.fEasingFunc = EasingFunction.Cubic.Out;
					break;

					case "easeinout_cubic":
					sTweenData.fEasingFunc = EasingFunction.Cubic.InOut;
					break;

					//Quartic
					case "easein_quart":
					sTweenData.fEasingFunc = EasingFunction.Quartic.In;
					break;

					case "easeout_quart":
					sTweenData.fEasingFunc = EasingFunction.Quartic.Out;
					break;

					case "easeinout_quart":
					sTweenData.fEasingFunc = EasingFunction.Quartic.InOut;
					break;

					//Quintic
					case "easein_quint":
					sTweenData.fEasingFunc = EasingFunction.Quintic.In;
					break;

					case "easeout_quint":
					sTweenData.fEasingFunc = EasingFunction.Quintic.Out;
					break;

					case "easeinout_quint":
					sTweenData.fEasingFunc = EasingFunction.Quintic.InOut;
					break;

					//Exponential
					case "easein_expo":
					sTweenData.fEasingFunc = EasingFunction.Exponential.In;
					break;

					case "easeout_expo":
					sTweenData.fEasingFunc = EasingFunction.Exponential.Out;
					break;

					case "easeinout_expo":
					sTweenData.fEasingFunc = EasingFunction.Exponential.InOut;
					break;

					//Circular
					case "easein_circ":
					sTweenData.fEasingFunc = EasingFunction.Circular.In;
					break;

					case "easeout_circ":
					sTweenData.fEasingFunc = EasingFunction.Circular.Out;
					break;

					case "easeinout_circ":
					sTweenData.fEasingFunc = EasingFunction.Circular.InOut;
					break;

					//Sinusoidal
					case "easein_sine":
					sTweenData.fEasingFunc = EasingFunction.Sinusoidal.In;
					break;

					case "easeout_sine":
					sTweenData.fEasingFunc = EasingFunction.Sinusoidal.Out;
					break;

					case "easeinout_sine":
					sTweenData.fEasingFunc = EasingFunction.Sinusoidal.InOut;
					break;

					//Back
					case "easein_back":
					sTweenData.fEasingFunc = EasingFunction.Back.In;
					break;

					case "easeout_back":
					sTweenData.fEasingFunc = EasingFunction.Back.Out;
					break;

					case "easeinout_back":
					sTweenData.fEasingFunc = EasingFunction.Back.InOut;
					break;

					//Elastic
					case "easein_elastic":
					sTweenData.fEasingFunc = EasingFunction.Elastic.In;
					break;

					case "easeout_elastic":
					sTweenData.fEasingFunc = EasingFunction.Elastic.Out;
					break;

					case "easeinout_elastic":
					sTweenData.fEasingFunc = EasingFunction.Elastic.InOut;
					break;

					//Bounce
					case "easein_bounce":
					sTweenData.fEasingFunc = EasingFunction.Bounce.In;
					break;

					case "easeout_bounce":
					sTweenData.fEasingFunc = EasingFunction.Bounce.Out;
					break;

					case "easeinout_bounce":
					sTweenData.fEasingFunc = EasingFunction.Bounce.InOut;
					break;

					default:
					sTweenData.fEasingFunc = EasingFunction.Linear;
					break;
				}

			if (string.IsNullOrEmpty(sValue = sTag.getAttribute("from")))
				return;

			if (!float.TryParse(sValue, out sTweenData.nValueBegin))
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sValue + "'(은)는 숫자가 아닙니다.", sTag);
				return;
			}

			if (string.IsNullOrEmpty(sValue = sTag.getAttribute("to")))
				return;

			if (!float.TryParse(sValue, out sTweenData.nValueEnd))
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sValue + "'(은)는 숫자가 아닙니다.", sTag);
				return;
			}

			if (string.IsNullOrEmpty(sValue = sTag.getAttribute("time")))
				return;

			if (!float.TryParse(sValue, out sTweenData.nDuration))
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sValue + "'(은)는 숫자가 아닙니다.", sTag);
				return;
			}
			else
				sTweenData.nDuration /= 1000f;

			if (!string.IsNullOrEmpty(sValue = sTag.getAttribute("delay", false)))
			{
				if (!float.TryParse(sValue, out sTweenData.nDelay))
				{
					ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sValue + "'(은)는 숫자가 아닙니다.", sTag);
					return;
				}
				else
					sTweenData.nDelay /= 1000f;
			}

			if (!string.IsNullOrEmpty(sValue = sTag.getAttribute("loop", false)))
			{
				if (!int.TryParse(sValue, out sTweenData.nLoop))
				{
					ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sValue + "'(은)는 숫자가 아닙니다.", sTag);
					return;
				}
			}

			if (!string.IsNullOrEmpty(sValue = sTag.getAttribute("yoyo", false)))
			{
				if (!int.TryParse(sValue, out sTweenData.nYoyo))
				{
					ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sValue + "'(은)는 숫자가 아닙니다.", sTag);
					return;
				}
			}

			sTweenData.bDelete = string.IsNullOrEmpty(sValue = sTag.getAttribute("delete", false)) ? false : sValue != "0";
			sLayer.addLayerTween(ref sTweenData, fModifier);
		}

		private static void transHandler(ScriptTag sTag)
		{
			string sType = sTag.getAttribute("type");

			if (string.IsNullOrEmpty(sType))
				return;

			switch (sType)
			{
				case "0":
				{
					foreach (Layer sLayer in Layer.NeedUpdateLayerEnumerable)
						sLayer.applyLayerProperties();

					Layer.clearNeedUpdateLayerList();
					UIManager.forceUpdateScreen();
				}
				return;
				case "1":
				{
					foreach (Layer sLayer in Layer.NeedUpdateLayerEnumerable) ;

					Layer.clearNeedUpdateLayerList();
				}
				break;
				case "2":
				{
					foreach (Layer sLayer in Layer.NeedUpdateLayerEnumerable) ;

					Layer.clearNeedUpdateLayerList();
				}
				break;
				default:
				{
					ScriptError.pushError(ScriptError.ErrorType.RuntimeError, sType + "은(는) 잘못된 속성입니다. '0', '1', '2' 중 하나여야합니다.", sTag);
				}
				return;
			}
		}
	}
}