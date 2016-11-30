using Noir.Unity;
using Noir.Unity.Live2D;
using System.Collections.Generic;
using System.Linq;

namespace Noir.Script
{
	public class ScriptTagHelper
	{
		public static IEnumerable<Layer> getLayer(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (sID == null)
				return null;

			if (sID.StartsWith("!"))
				return from sPair in Layer.LayerEnumerable
					   select sPair.Value;

			if (sID.EndsWith("."))
				return Layer.getLayerFamily(sID);

			Layer sLayer = Layer.getLayer(sID);

			if (sLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) 없는 레이어입니다.", sTag);
				return null;
			}

			return new Layer[] { sLayer };
		}

		public static Layer getLayerExact(ScriptTag sTag)
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

			return sLayer;
		}

		public static IEnumerable<SpriteLayer> getSpriteLayer(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (sID == null)
				return null;

			if(sID.StartsWith("!"))
			{
				SpriteLayer sTemp = null;

				return from sPair in Layer.LayerEnumerable
					   where (sTemp = sPair.Value as SpriteLayer) != null
					   select sTemp;
			}

			if(sID.EndsWith("."))
			{
				SpriteLayer sTemporary = null;

				return from sPair in Layer.LayerEnumerable
					   where sPair.Key.StartsWith(sID) && (sTemporary = sPair.Value as SpriteLayer) != null
					   select sTemporary;
			}

			Layer sLayer = Layer.getLayer(sID);

			if (sLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) 없는 레이어입니다.", sTag);
				return null;
			}

			SpriteLayer sSpriteLayer = sLayer as SpriteLayer;

			if (sSpriteLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) Sprite 레이어가 아닙니다.", sTag);
				return null;
			}

			return new SpriteLayer[] { sSpriteLayer };
		}

		public static SpriteLayer getSpriteLayerExact(ScriptTag sTag)
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

			SpriteLayer sSpriteLayer = sLayer as SpriteLayer;

			if(sSpriteLayer == null)
			{
				ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "'" + sID + "'은(는) Sprite 레이어가 아닙니다.", sTag);
				return null;
			}

			return sSpriteLayer;
		}

		public static IEnumerable<Live2DLayer> getLive2DLayer(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (sID == null)
				return null;

			if (sID.StartsWith("!"))
			{
				Live2DLayer sTemp = null;

				return from sPair in Layer.LayerEnumerable
					   where (sTemp = sPair.Value as Live2DLayer) != null
					   select sTemp;
			}

			if (sID.EndsWith("."))
			{
				Live2DLayer sTemporary = null;

				return from sPair in Layer.LayerEnumerable
					   where sPair.Key.StartsWith(sID) && (sTemporary = sPair.Value as Live2DLayer) != null
					   select sTemporary;
			}

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

			return new Live2DLayer[] { sLive2DLayer };
		}

		public static Live2DLayer getLive2DLayerExact(ScriptTag sTag)
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

		public static IEnumerable<AnimatedLayer> getAnimatedLayer(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (sID == null)
				return null;

			if (sID.StartsWith("!"))
			{
				AnimatedLayer sTemp = null;

				return from sPair in Layer.LayerEnumerable
					   where (sTemp = sPair.Value as AnimatedLayer) != null
					   select sTemp;
			}

			if (sID.EndsWith("."))
			{
				AnimatedLayer sTemporary = null;

				return from sPair in Layer.LayerEnumerable
					   where sPair.Key.StartsWith(sID) && (sTemporary = sPair.Value as AnimatedLayer) != null
					   select sTemporary;
			}

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

			return new AnimatedLayer[] { sAnimatedLayer };
		}

		public static AnimatedLayer getAnimatedLayerExact(ScriptTag sTag)
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

		public static IEnumerable<TextLayer> getTextLayer(ScriptTag sTag)
		{
			string sID = sTag.getAttribute("id");

			if (sID == null)
				return null;

			if (sID.StartsWith("!"))
			{
				TextLayer sTemp = null;

				return from sPair in Layer.LayerEnumerable
					   where (sTemp = sPair.Value as TextLayer) != null
					   select sTemp;
			}

			if (sID.EndsWith("."))
			{
				TextLayer sTemporary = null;

				return from sPair in Layer.LayerEnumerable
					   where sPair.Key.StartsWith(sID) && (sTemporary = sPair.Value as TextLayer) != null
					   select sTemporary;
			}

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

			return new TextLayer[] { sTextLayer };
		}

		public static TextLayer getTextLayerExact(ScriptTag sTag)
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