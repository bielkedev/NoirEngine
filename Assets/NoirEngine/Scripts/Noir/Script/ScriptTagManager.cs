using System;
using System.Collections.Generic;
using Noir.Equation;
using Noir.UI;
using Noir.Unity;
using Noir.Unity.Live2D;
using Noir.Util;

namespace Noir.Script
{
	public class ScriptTagManager
	{
		public delegate void ScriptTagHandler(ScriptTag sTag);

		private static Random sRandom = new Random();
		private static Dictionary<string, ScriptTagHandler> sTagHandlerMap = new Dictionary<string, ScriptTagHandler>();
		//private static char[] vClipSeparator = new char[] { ',' };
		private static string[] vIfDelimitTagName = new string[] { "if", "elseif", "else", "/if" };
		private static string[] vLoopDelimitTagName = new string[] { "loop", "/loop" };

		public static void initTagHandler()
		{
			ScriptTagManager.sTagHandlerMap.Clear();

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
			//ScriptTagManager.sTagHandlerMap.Add("trans", ScriptTagManager.transHandler);
			ScriptTagManager.sTagHandlerMap.Add("if", ScriptTagManager.ifHandler);
			ScriptTagManager.sTagHandlerMap.Add("elseif", ScriptTagManager.elseifHandler);
			ScriptTagManager.sTagHandlerMap.Add("else", ScriptTagManager.elseHandler);
			ScriptTagManager.sTagHandlerMap.Add("/if", ScriptTagManager._ifHandler);
			ScriptTagManager.sTagHandlerMap.Add("lycl2d", ScriptTagManager.lycl2dHandler);
			ScriptTagManager.sTagHandlerMap.Add("lymotionl2d", ScriptTagManager.lymotionl2dHandler);
			ScriptTagManager.sTagHandlerMap.Add("lymotionstopl2d", ScriptTagManager.lymotionstopl2dHandler);
			ScriptTagManager.sTagHandlerMap.Add("print", ScriptTagManager.printHandler);
			ScriptTagManager.sTagHandlerMap.Add("loop", ScriptTagManager.loopHandler);
			ScriptTagManager.sTagHandlerMap.Add("/loop", ScriptTagManager._loopHandler);
			ScriptTagManager.sTagHandlerMap.Add("lycanim", ScriptTagManager.lycanimHandler);
			ScriptTagManager.sTagHandlerMap.Add("lyaddanim", ScriptTagManager.lyaddanimHandler);
			ScriptTagManager.sTagHandlerMap.Add("lyupdateanim", ScriptTagManager.lyupdateanimHandler);
			ScriptTagManager.sTagHandlerMap.Add("waittime", ScriptTagManager.waittimeHandler);
			ScriptTagManager.sTagHandlerMap.Add("waittween", ScriptTagManager.waittweenHandler);
			ScriptTagManager.sTagHandlerMap.Add("waitmotion", ScriptTagManager.waitmotionHandler);
			ScriptTagManager.sTagHandlerMap.Add("getlayerx", ScriptTagManager.getlayerxHandler);
			ScriptTagManager.sTagHandlerMap.Add("getlayery", ScriptTagManager.getlayeryHandler);
			ScriptTagManager.sTagHandlerMap.Add("getlayerlalpha", ScriptTagManager.getlayeralphaHandler);
			ScriptTagManager.sTagHandlerMap.Add("getlayeranchorx", ScriptTagManager.getlayeranchorxHandler);
			ScriptTagManager.sTagHandlerMap.Add("getlayeranchory", ScriptTagManager.getlayeranchoryHandler);
			ScriptTagManager.sTagHandlerMap.Add("getlayerxscale", ScriptTagManager.getlayerxscaleHandler);
			ScriptTagManager.sTagHandlerMap.Add("getlayeryscale", ScriptTagManager.getlayeryscaleHandler);
			ScriptTagManager.sTagHandlerMap.Add("getlayerrotate", ScriptTagManager.getlayerrotateHandler);
			ScriptTagManager.sTagHandlerMap.Add("getlayerreversex", ScriptTagManager.getlayerreversexHandler);
			ScriptTagManager.sTagHandlerMap.Add("getlayerreversey", ScriptTagManager.getlayerreverseyHandler);
			ScriptTagManager.sTagHandlerMap.Add("getlayervisible", ScriptTagManager.getlayervisibleHandler);
			ScriptTagManager.sTagHandlerMap.Add("lyctxt", ScriptTagManager.lyctxtHandler);
			ScriptTagManager.sTagHandlerMap.Add("lycleartxt", ScriptTagManager.lycleartxtHandler);
			ScriptTagManager.sTagHandlerMap.Add("lyfonttxt", ScriptTagManager.lyfonttxtHandler);
			ScriptTagManager.sTagHandlerMap.Add("lyfontsizetxt", ScriptTagManager.lyfontsizetxtHandler);
			ScriptTagManager.sTagHandlerMap.Add("lyfontcolortxt", ScriptTagManager.lyfontcolortxtHandler);
			ScriptTagManager.sTagHandlerMap.Add("lyalignhtxt", ScriptTagManager.lyalignhtxtHandler);
			ScriptTagManager.sTagHandlerMap.Add("lyalignvtxt", ScriptTagManager.lyalignvtxtHandler);
			ScriptTagManager.sTagHandlerMap.Add("lysetmaintxt", ScriptTagManager.lysetmaintxtHandler);
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
					ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sTarget + "'은(는) 잘못된 값입니다. 'linehead', 'lineend' 중 하나여야합니다.", sTag);
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
			SpriteLayer sSpriteLayer;

			if (sLayer == null)
				sSpriteLayer = new SpriteLayer(sID);
			else if ((sSpriteLayer = sLayer as SpriteLayer) == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) 이미 존재하는 Live2D/Animated 레이어입니다.", sTag);
				return;
			}

			string sFile = sTag.getAttribute("file");

			if (string.IsNullOrEmpty(sFile))
				return;

			UnityEngine.Sprite sMainSprite = CacheManager.loadSprite(sFile);

			if (sMainSprite == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sFile + "'에 스프라이트가 없습니다.", sTag);
				return;
			}

			sSpriteLayer.setSprite(sMainSprite);
			sSpriteLayer.markAsNeedUpdate();
		}

		private static void lydelHandler(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (!string.IsNullOrEmpty(sID))
			{
				Layer sLayer = Layer.getLayer(sID);

				if (sLayer != null)
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

			if ((sValue = sTag.getAttribute("mask", false)) != null)
			{
				if (sValue == "!")
					sLayer.setMask(null);
				else
				{
					UnityEngine.Sprite sMask = CacheManager.loadSprite(sValue);

					if (sMask == null)
						ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sValue + "'에 스프라이트가 없습니다.", sTag);
					else
						sLayer.setMask(sMask);
				}
			}

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
			
			/*
			if (!string.IsNullOrEmpty(sValue = sTag.getAttribute("clip", false)))
			{
				string[] vAttr = sValue.Split(ScriptTagManager.vClipSeparator, StringSplitOptions.RemoveEmptyEntries);

				if (vAttr.Length != 4)
					sLayer.setClipping(false);
				else
				{
					sLayer.setClipping(true);

					if (vAttr[0].StartsWith("$"))
						vAttr[0] = new EquationLine(vAttr[0]).evaluateEquation();
					if (vAttr[1].StartsWith("$"))
						vAttr[1] = new EquationLine(vAttr[1]).evaluateEquation();
					if (vAttr[2].StartsWith("$"))
						vAttr[2] = new EquationLine(vAttr[2]).evaluateEquation();
					if (vAttr[3].StartsWith("$"))
						vAttr[3] = new EquationLine(vAttr[3]).evaluateEquation();

					float nX;
					float nY;
					float nW;
					float nH;

					if (!float.TryParse(vAttr[0], out nX))
						ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + vAttr[0] + "'은(는) 숫자가 아닙니다.", sTag);
					if (!float.TryParse(vAttr[1], out nY))
						ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + vAttr[1] + "'은(는) 숫자가 아닙니다.", sTag);
					if (!float.TryParse(vAttr[2], out nW))
						ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + vAttr[2] + "'은(는) 숫자가 아닙니다.", sTag);
					if (!float.TryParse(vAttr[3], out nH))
						ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + vAttr[3] + "'은(는) 숫자가 아닙니다.", sTag);

					sLayer.setClipper(UnityEngine.Rect.MinMaxRect(nX, nY, nX + nW, nY + nH));
				}
			}
			*/

			if (!string.IsNullOrEmpty(sValue = sTag.getAttribute("visible", false)))
				if (float.TryParse(sValue, out nValue))
					sLayer.setVisible(nValue);
				else
					ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sValue + "'(은)는 숫자가 아닙니다.", sTag);

			sLayer.markAsNeedUpdate();
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

			LayerStateModifier fModifier;

			string sValue = sTag.getAttribute("param");

			if (string.IsNullOrEmpty(sValue))
				return;

			switch (sValue)
			{
				case "left":
				fModifier = Layer.setPosX;
				break;

				case "top":
				fModifier = Layer.setPosY;
				break;

				case "alpha":
				fModifier = Layer.setAlpha;
				break;

				case "xscale":
				fModifier = Layer.setScaleX;
				break;

				case "yscale":
				fModifier = Layer.setScaleY;
				break;

				case "rotate":
				fModifier = Layer.setRotate;
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
					foreach (Layer sLayer in Layer.NeedUpdateLayerEnumerable)
					{

					}

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

		private static void ifHandler(ScriptTag sTag)
		{
			string sEstimate = sTag.getAttribute("estimate");

			if (string.IsNullOrEmpty(sEstimate))
				return;

			float nValue;

			if (!float.TryParse(sEstimate, out nValue))
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, sEstimate + "은(는) 숫자가 아닙니다.", sTag);
				return;
			}

			ScriptBranch.pushBranch();

			if (nValue != 0f)
				ScriptBranch.IsCurrentBranching = true;
			else
			{
				int nCount = 0;

				for (string sTagName = ScriptRuntime.skipScript(ScriptTagManager.vIfDelimitTagName); sTagName != null; sTagName = ScriptRuntime.skipScript(ScriptTagManager.vIfDelimitTagName))
				{
					if (sTagName == "if")
						++nCount;
					else if (sTagName == "/if")
					{
						if (nCount <= 0)
							break;

						--nCount;
					}
					else if (nCount <= 0)
						break;

					ScriptRuntime.skipScript(1);
				}
			}
		}

		private static void elseifHandler(ScriptTag sTag)
		{
			if (ScriptBranch.IsCurrentBranching)
			{
				int nCount = 0;

				for (string sTagName = ScriptRuntime.skipScript(ScriptTagManager.vIfDelimitTagName); sTagName != null; sTagName = ScriptRuntime.skipScript(ScriptTagManager.vIfDelimitTagName))
				{
					if (sTagName == "if")
						++nCount;
					else if (sTagName == "/if")
					{
						if (nCount <= 0)
							break;

						--nCount;
					}
					else if (nCount <= 0)
						break;

					ScriptRuntime.skipScript(1);
				}

				return;
			}

			string sEstimate = sTag.getAttribute("estimate");

			if (string.IsNullOrEmpty(sEstimate))
				return;

			float nValue;

			if (!float.TryParse(sEstimate, out nValue))
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, sEstimate + "은(는) 숫자가 아닙니다.", sTag);
				return;
			}

			if (nValue != 0f)
				ScriptBranch.IsCurrentBranching = true;
			else
			{
				int nCount = 0;

				for (string sTagName = ScriptRuntime.skipScript(ScriptTagManager.vIfDelimitTagName); sTagName != null; sTagName = ScriptRuntime.skipScript(ScriptTagManager.vIfDelimitTagName))
				{
					if (sTagName == "if")
						++nCount;
					else if (sTagName == "/if")
					{
						if (nCount <= 0)
							break;

						--nCount;
					}
					else if (nCount <= 0)
						break;

					ScriptRuntime.skipScript(1);

				}
			}
		}

		private static void elseHandler(ScriptTag sTag)
		{
			if (ScriptBranch.IsCurrentBranching)
			{
				int nCount = 0;

				for (string sTagName = ScriptRuntime.skipScript(ScriptTagManager.vIfDelimitTagName); sTagName != null; sTagName = ScriptRuntime.skipScript(ScriptTagManager.vIfDelimitTagName))
				{
					if (sTagName == "if")
						++nCount;
					else if (sTagName == "/if")
					{
						if (nCount <= 0)
							break;

						--nCount;
					}
					else if (nCount <= 0)
						break;

					ScriptRuntime.skipScript(1);
				}

				return;
			}

			ScriptBranch.IsCurrentBranching = true;
		}

		private static void _ifHandler(ScriptTag sTag)
		{
			ScriptBranch.popBranch();
		}

		private static void lycl2dHandler(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (string.IsNullOrEmpty(sID))
				return;

			Layer sLayer = Layer.getLayer(sID);

			if (sLayer != null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) 이미 존재하는 레이어입니다.", sTag);
				return;
			}

			Live2DLayer sLive2DLayer = new Live2DLayer(sID);
			string sFile = sTag.getAttribute("file");

			if (string.IsNullOrEmpty(sFile))
				return;

			string sIdle = sTag.getAttribute("idle");

			if (string.IsNullOrEmpty(sIdle))
				return;
			
			if (!sLive2DLayer.setModel(sFile, sIdle))
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sFile + "'에 Live2D 모델이 없거나 손상되었습니다.", sTag);
			
			sLive2DLayer.markAsNeedUpdate();
		}

		private static void lymotionl2dHandler(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (string.IsNullOrEmpty(sID))
				return;

			Live2DLayer sLayer = Layer.getLayer(sID) as Live2DLayer;

			if (sLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) 없는 레이어이거나 Live2D 레이어가 아닙니다.", sTag);
				return;
			}

			string sMotion = sTag.getAttribute("motion");

			if (string.IsNullOrEmpty(sMotion))
				return;

			bool bLoop = false;
			string sLoop = sTag.getAttribute("loop", false);

			if (!string.IsNullOrEmpty(sLoop))
			{
				float nLoop;
				bLoop = float.TryParse(sLoop, out nLoop) && nLoop != 0f;
			}

			if (!sLayer.Controller.startMotion(sMotion, bLoop))
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sMotion + "'은(는) 존재하지 않는 모션입니다.", sTag);
		}

		private static void lymotionstopl2dHandler(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (string.IsNullOrEmpty(sID))
				return;

			Live2DLayer sLayer = Layer.getLayer(sID) as Live2DLayer;

			if (sLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) 없는 레이어이거나 Live2D 레이어가 아닙니다.", sTag);
				return;
			}

			sLayer.Controller.startIdleMotion();
		}

		private static void printHandler(ScriptTag sTag)
		{
			string sData = sTag.getAttribute("data");

			if (string.IsNullOrEmpty(sData))
				return;

			UIManager.appendDialogueText(sData);
		}

		private static void loopHandler(ScriptTag sTag)
		{
			string sEstimate = sTag.getAttribute("estimate");

			if (string.IsNullOrEmpty(sEstimate))
				return;

			float nEstimate;

			if (!float.TryParse(sEstimate, out nEstimate))
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sEstimate + "'은(는) 숫자가 아닙니다.", sTag);
				return;
			}

			ScriptLoop.pushLoop();

			if (nEstimate == 0f)
			{
				int nCount = 0;

				for (string sTagName = ScriptRuntime.skipScript(ScriptTagManager.vLoopDelimitTagName); sTag != null; sTagName = ScriptRuntime.skipScript(ScriptTagManager.vLoopDelimitTagName))
				{
					if (sTagName == "loop")
						++nCount;
					else
					{
						if (nCount <= 0)
							break;

						--nCount;
					}

					ScriptRuntime.skipScript(1);
				}
			}
			else
				ScriptLoop.IsCurrentLooping = true;
		}

		private static void _loopHandler(ScriptTag sTag)
		{
			if (ScriptLoop.IsCurrentLooping)
			{
				int nCount = 0;

				ScriptRuntime.skipScriptBack("/loop");
				ScriptRuntime.skipScriptBack(1);

				for (string sTagName = ScriptRuntime.skipScriptBack(ScriptTagManager.vLoopDelimitTagName); sTag != null; sTagName = ScriptRuntime.skipScriptBack(ScriptTagManager.vLoopDelimitTagName))
				{
					if (sTagName == "/loop")
						++nCount;
					else
					{
						if (nCount <= 0)
							break;

						--nCount;
					}

					ScriptRuntime.skipScriptBack(1);
				}
			}

			ScriptLoop.popLoop();
		}

		private static void lycanimHandler(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (string.IsNullOrEmpty(sID))
				return;

			Layer sLayer = Layer.getLayer(sID);

			if (sLayer != null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) 이미 있는 레이어입니다.", sTag);
				return;
			}

			new AnimatedLayer(sID);
		}

		private static void lyaddanimHandler(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (string.IsNullOrEmpty(sID))
				return;

			AnimatedLayer sAnimatedLayer = Layer.getLayer(sID) as AnimatedLayer;

			if (sAnimatedLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) Animated 레이어가 아닙니다.", sTag);
				return;
			}

			string sFile = sTag.getAttribute("file");

			if (string.IsNullOrEmpty(sFile))
				return;

			UnityEngine.Sprite sMainSprite = CacheManager.loadSprite(sFile);

			if (sMainSprite == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sFile + "'에 스프라이트가 없습니다.", sTag);
				return;
			}

			string sTime = sTag.getAttribute("time");

			if (string.IsNullOrEmpty(sTime))
				return;

			float nTime;

			if (!float.TryParse(sTime, out nTime))
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sTime + "'은(는) 숫자가 아닙니다.", sTag);
				return;
			}

			sAnimatedLayer.addLayerSprite(sMainSprite, nTime * .001f);
		}

		private static void lyupdateanimHandler(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (string.IsNullOrEmpty(sID))
				return;

			AnimatedLayer sAnimatedLayer = Layer.getLayer(sID) as AnimatedLayer;

			if (sAnimatedLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) Animated 레이어가 아닙니다.", sTag);
				return;
			}

			sAnimatedLayer.updateLayerSprite();
		}

		private static void waittimeHandler(ScriptTag sTag)
		{
			string sTime = sTag.getAttribute("time");

			if (sTime == null)
				return;

			float nTime;

			if (!float.TryParse(sTime, out nTime))
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sTime + "'은(는) 숫자가 아닙니다.", sTag);
				return;
			}

			string sInput = sTag.getAttribute("input");

			if (sInput == null)
				return;

			float nInput;

			if (!float.TryParse(sInput, out nInput))
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sInput + "'은(는) 숫자가 아닙니다.", sTag);
				return;
			}

			UIManager.waitForObject((int)nInput, new WaitTimeObject(nTime * .001f));
		}

		private static void waittweenHandler(ScriptTag sTag)
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

			string sInput = sTag.getAttribute("input");

			if (sInput == null)
				return;

			float nInput;

			if (!float.TryParse(sInput, out nInput))
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sInput + "'은(는) 숫자가 아닙니다.", sTag);
				return;
			}

			UIManager.waitForObject((int)nInput, sLayer);
		}

		private static void waitmotionHandler(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (string.IsNullOrEmpty(sID))
				return;

			Live2DLayer sLive2DLayer = Layer.getLayer(sID) as Live2DLayer;

			if (sLive2DLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) 없는 레이어이거나 Live2D 레이어가 아닙니다.", sTag);
				return;
			}

			string sInput = sTag.getAttribute("input");

			if (sInput == null)
				return;

			float nInput;

			if (!float.TryParse(sInput, out nInput))
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sInput + "'은(는) 숫자가 아닙니다.", sTag);
				return;
			}

			UIManager.waitForObject((int)nInput, sLive2DLayer.Controller);
		}
		
		private static void getlayerxHandler(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (sID == null)
				return;

			Layer sLayer = Layer.getLayer(sID);

			if (sLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) 없는 레이어입니다.", sTag);
				return;
			}

			string sName = sTag.getAttribute("name");

			if (sName == null)
				return;

			EquationVariable.setVar(sName, sLayer.Position.x.ToString());
		}

		private static void getlayeryHandler(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (sID == null)
				return;

			Layer sLayer = Layer.getLayer(sID);

			if (sLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) 없는 레이어입니다.", sTag);
				return;
			}

			string sName = sTag.getAttribute("name");

			if (sName == null)
				return;

			EquationVariable.setVar(sName, sLayer.Position.y.ToString());
		}

		private static void getlayeralphaHandler(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (sID == null)
				return;

			Layer sLayer = Layer.getLayer(sID);

			if (sLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) 없는 레이어입니다.", sTag);
				return;
			}

			string sName = sTag.getAttribute("name");

			if (sName == null)
				return;

			EquationVariable.setVar(sName, sLayer.Alpha.ToString());
		}

		private static void getlayeranchorxHandler(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (sID == null)
				return;

			Layer sLayer = Layer.getLayer(sID);

			if (sLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) 없는 레이어입니다.", sTag);
				return;
			}

			string sName = sTag.getAttribute("name");

			if (sName == null)
				return;

			EquationVariable.setVar(sName, sLayer.Pivot.x.ToString());
		}

		private static void getlayeranchoryHandler(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (sID == null)
				return;

			Layer sLayer = Layer.getLayer(sID);

			if (sLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) 없는 레이어입니다.", sTag);
				return;
			}

			string sName = sTag.getAttribute("name");

			if (sName == null)
				return;

			EquationVariable.setVar(sName, sLayer.Pivot.y.ToString());
		}

		private static void getlayerxscaleHandler(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (sID == null)
				return;

			Layer sLayer = Layer.getLayer(sID);

			if (sLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) 없는 레이어입니다.", sTag);
				return;
			}

			string sName = sTag.getAttribute("name");

			if (sName == null)
				return;

			EquationVariable.setVar(sName, sLayer.Scale.x.ToString());
		}

		private static void getlayeryscaleHandler(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (sID == null)
				return;

			Layer sLayer = Layer.getLayer(sID);

			if (sLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) 없는 레이어입니다.", sTag);
				return;
			}

			string sName = sTag.getAttribute("name");

			if (sName == null)
				return;

			EquationVariable.setVar(sName, sLayer.Scale.y.ToString());
		}

		private static void getlayerrotateHandler(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (sID == null)
				return;

			Layer sLayer = Layer.getLayer(sID);

			if (sLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) 없는 레이어입니다.", sTag);
				return;
			}

			string sName = sTag.getAttribute("name");

			if (sName == null)
				return;

			EquationVariable.setVar(sName, sLayer.Rotation.ToString());
		}

		private static void getlayerreversexHandler(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (sID == null)
				return;

			Layer sLayer = Layer.getLayer(sID);

			if (sLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) 없는 레이어입니다.", sTag);
				return;
			}

			string sName = sTag.getAttribute("name");

			if (sName == null)
				return;

			EquationVariable.setVar(sName, sLayer.ReverseX ? "1" : "0");
		}

		private static void getlayerreverseyHandler(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (sID == null)
				return;

			Layer sLayer = Layer.getLayer(sID);

			if (sLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) 없는 레이어입니다.", sTag);
				return;
			}

			string sName = sTag.getAttribute("name");

			if (sName == null)
				return;

			EquationVariable.setVar(sName, sLayer.ReverseY ? "1" : "0");
		}

		private static void getlayervisibleHandler(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (sID == null)
				return;

			Layer sLayer = Layer.getLayer(sID);

			if (sLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) 없는 레이어입니다.", sTag);
				return;
			}

			string sName = sTag.getAttribute("name");

			if (sName == null)
				return;

			EquationVariable.setVar(sName, sLayer.Visiblility ? "1" : "0");
		}

		public static void lyctxtHandler(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (sID == null)
				return;

			Layer sLayer = Layer.getLayer(sID);

			if(sLayer != null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) 이미 있는 레이어입니다.", sTag);
				return;
			}

			new TextLayer(sID);
		}

		public static void lycleartxtHandler(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (sID == null)
				return;

			TextLayer sLayer = Layer.getLayer(sID) as TextLayer;

			if(sLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) 없는 레이어거나 Text 레이어가 아닙니다.", sTag);
				return;
			}

			sLayer.LayerText.text = string.Empty;
		}

		public static void lynewlinetxtHandler(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (sID == null)
				return;

			TextLayer sLayer = Layer.getLayer(sID) as TextLayer;

			if (sLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) 없는 레이어거나 Text 레이어가 아닙니다.", sTag);
				return;
			}

			sLayer.LayerText.text += '\n';
		}

		public static void lyapptexttxtHandler(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (sID == null)
				return;

			TextLayer sLayer = Layer.getLayer(sID) as TextLayer;

			if (sLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) 없는 레이어거나 Text 레이어가 아닙니다.", sTag);
				return;
			}

			string sText = sTag.getAttribute("text");

			if (sText == null)
				return;

			sLayer.LayerText.text += sText;
		}

		public static void lyfonttxtHandler(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (sID == null)
				return;

			TextLayer sLayer = Layer.getLayer(sID) as TextLayer;

			if (sLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) 없는 레이어거나 Text 레이어가 아닙니다.", sTag);
				return;
			}

			string sFontPath = sTag.getAttribute("font");

			if (sFontPath == null)
				return;

			UnityEngine.Font sFont = CacheManager.loadFont(sFontPath);

			if(sFont == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sFontPath + "'에 폰트가 없습니다.", sTag);
				return;
			}

			sLayer.LayerText.font = sFont;
		}

		public static void lyfontsizetxtHandler(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (sID == null)
				return;

			TextLayer sLayer = Layer.getLayer(sID) as TextLayer;

			if (sLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) 없는 레이어거나 Text 레이어가 아닙니다.", sTag);
				return;
			}

			string sFontSize = sTag.getAttribute("size");
			float nFontSize;

			if (!float.TryParse(sFontSize, out nFontSize))
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sFontSize + "'은(는) 숫자가 아닙니다.", sTag);
				return;
			}

			sLayer.LayerText.fontSize = (int)nFontSize;
		}

		public static void lyfontcolortxtHandler(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (sID == null)
				return;

			TextLayer sLayer = Layer.getLayer(sID) as TextLayer;

			if (sLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) 없는 레이어거나 Text 레이어가 아닙니다.", sTag);
				return;
			}

			string sR = sTag.getAttribute("r");
			string sG = sTag.getAttribute("g");
			string sB = sTag.getAttribute("b");
			string sA = sTag.getAttribute("a");
			float nR;
			float nG;
			float nB;
			float nA;

			if (!float.TryParse(sR, out nR))
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sR + "'은(는) 숫자가 아닙니다.", sTag);
				return;
			}

			if (!float.TryParse(sG, out nG))
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sG + "'은(는) 숫자가 아닙니다.", sTag);
				return;
			}

			if (!float.TryParse(sB, out nB))
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sB + "'은(는) 숫자가 아닙니다.", sTag);
				return;
			}

			if (!float.TryParse(sA, out nA))
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sA + "'은(는) 숫자가 아닙니다.", sTag);
				return;
			}

			sLayer.LayerText.color = new UnityEngine.Color(nR / 255f, nG / 255f, nB / 255f, nA / 255f);
		}

		public static void lyalignhtxtHandler(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (sID == null)
				return;

			TextLayer sLayer = Layer.getLayer(sID) as TextLayer;

			if (sLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) 없는 레이어거나 Text 레이어가 아닙니다.", sTag);
				return;
			}

			string sAlign = sTag.getAttribute("align");

			if (sAlign == null)
				return;

			float nAlign;

			if (!float.TryParse(sAlign, out nAlign))
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sAlign + "'은(는) 숫자가 아닙니다.", sTag);
				return;
			}
			
			switch(sLayer.LayerText.alignment)
			{
				case UnityEngine.TextAnchor.LowerCenter:
				case UnityEngine.TextAnchor.LowerLeft:
				case UnityEngine.TextAnchor.LowerRight:
				if (nAlign < 0f)
					sLayer.LayerText.alignment = UnityEngine.TextAnchor.LowerLeft;
				else if (nAlign > 0f)
					sLayer.LayerText.alignment = UnityEngine.TextAnchor.LowerRight;
				else
					sLayer.LayerText.alignment = UnityEngine.TextAnchor.LowerCenter;
				break;

				default:
				case UnityEngine.TextAnchor.MiddleCenter:
				case UnityEngine.TextAnchor.MiddleLeft:
				case UnityEngine.TextAnchor.MiddleRight:
				if (nAlign < 0f)
					sLayer.LayerText.alignment = UnityEngine.TextAnchor.MiddleLeft;
				else if (nAlign > 0f)
					sLayer.LayerText.alignment = UnityEngine.TextAnchor.MiddleRight;
				else
					sLayer.LayerText.alignment = UnityEngine.TextAnchor.MiddleCenter;
				break;

				case UnityEngine.TextAnchor.UpperCenter:
				case UnityEngine.TextAnchor.UpperLeft:
				case UnityEngine.TextAnchor.UpperRight:
				if (nAlign < 0f)
					sLayer.LayerText.alignment = UnityEngine.TextAnchor.UpperLeft;
				else if (nAlign > 0f)
					sLayer.LayerText.alignment = UnityEngine.TextAnchor.UpperRight;
				else
					sLayer.LayerText.alignment = UnityEngine.TextAnchor.UpperCenter;
				break;
			}
		}

		public static void lyalignvtxtHandler(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (sID == null)
				return;

			TextLayer sLayer = Layer.getLayer(sID) as TextLayer;

			if (sLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) 없는 레이어거나 Text 레이어가 아닙니다.", sTag);
				return;
			}

			string sAlign = sTag.getAttribute("align");

			if (sAlign == null)
				return;

			float nAlign;

			if (!float.TryParse(sAlign, out nAlign))
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sAlign + "'은(는) 숫자가 아닙니다.", sTag);
				return;
			}

			switch (sLayer.LayerText.alignment)
			{
				default:
				case UnityEngine.TextAnchor.LowerCenter:
				case UnityEngine.TextAnchor.MiddleCenter:
				case UnityEngine.TextAnchor.UpperCenter:
				if (nAlign < 0f)
					sLayer.LayerText.alignment = UnityEngine.TextAnchor.UpperCenter;
				else if (nAlign > 0f)
					sLayer.LayerText.alignment = UnityEngine.TextAnchor.LowerCenter;
				else
					sLayer.LayerText.alignment = UnityEngine.TextAnchor.MiddleCenter;
				break;

				case UnityEngine.TextAnchor.LowerLeft:
				case UnityEngine.TextAnchor.MiddleLeft:
				case UnityEngine.TextAnchor.UpperLeft:
				if (nAlign < 0f)
					sLayer.LayerText.alignment = UnityEngine.TextAnchor.UpperLeft;
				else if (nAlign > 0f)
					sLayer.LayerText.alignment = UnityEngine.TextAnchor.LowerLeft;
				else
					sLayer.LayerText.alignment = UnityEngine.TextAnchor.MiddleLeft;
				break;

				case UnityEngine.TextAnchor.LowerRight:
				case UnityEngine.TextAnchor.MiddleRight:
				case UnityEngine.TextAnchor.UpperRight:
				if (nAlign < 0f)
					sLayer.LayerText.alignment = UnityEngine.TextAnchor.UpperRight;
				else if (nAlign > 0f)
					sLayer.LayerText.alignment = UnityEngine.TextAnchor.LowerRight;
				else
					sLayer.LayerText.alignment = UnityEngine.TextAnchor.MiddleRight;
				break;
			}
		}

		public static void lysetmaintxtHandler(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (sID == null)
				return;

			TextLayer sLayer = Layer.getLayer(sID) as TextLayer;

			if (sLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) 없는 레이어거나 Text 레이어가 아닙니다.", sTag);
				return;
			}

			UIManager.MainTextLayer = sLayer;
		}
	}
}