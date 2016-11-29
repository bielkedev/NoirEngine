using Noir.Unity;
using Noir.Unity.Live2D;

namespace Noir.Script
{
	public class ScriptTagHelper
	{
		public static SpriteLayer getSpriteLayer(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (sID == null)
				return null;

			Layer sLayer = Layer.getLayer(sID);

			if(sLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) 없는 레이어입니다.", sTag);
				return null;
			}

			SpriteLayer sSpriteLayer = sLayer as SpriteLayer;

			if(sSpriteLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) Sprite 레이어가 아닙니다.", sTag);
				return null;
			}

			return sSpriteLayer;
		}

		public static Live2DLayer getLive2DLayer(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (sID == null)
				return null;

			Layer sLayer = Layer.getLayer(sID);

			if (sLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) 없는 레이어입니다.", sTag);
				return null;
			}

			Live2DLayer sLive2DLayer = sLayer as Live2DLayer;

			if (sLive2DLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) Live2D 레이어가 아닙니다.", sTag);
				return null;
			}

			return sLive2DLayer;
		}

		public static AnimatedLayer getAnimatedLayer(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (sID == null)
				return null;

			Layer sLayer = Layer.getLayer(sID);

			if (sLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) 없는 레이어입니다.", sTag);
				return null;
			}

			AnimatedLayer sAnimatedLayer = sLayer as AnimatedLayer;

			if (sAnimatedLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) Animated 레이어가 아닙니다.", sTag);
				return null;
			}

			return sAnimatedLayer;
		}

		public static TextLayer getTextLayer(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (sID == null)
				return null;

			Layer sLayer = Layer.getLayer(sID);

			if (sLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) 없는 레이어입니다.", sTag);
				return null;
			}

			TextLayer sTextLayer = sLayer as TextLayer;

			if (sTextLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) Text 레이어가 아닙니다.", sTag);
				return null;
			}

			return sTextLayer;
		}

		public static bool getFloat(ScriptTag sTag, string sAttr, out float nFloat, bool bPushError = true)
		{
			string sFloat = sTag.getAttribute(sAttr, bPushError);

			if (sFloat == null)
			{
				nFloat = default(float);
				return false;
			}

			if(!float.TryParse(sFloat, out nFloat))
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sFloat + "'은(는) 숫자가 아닙니다.", sTag);
				return false;
			}

			return true;
		}
	}
}