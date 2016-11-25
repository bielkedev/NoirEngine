using UnityEngine;
using UnityEngine.UI;

namespace Noir.Unity
{
	public class TextLayer : Layer
	{
		public static GameObject TextLayerPrefab { set { TextLayer.sTextLayerPrefab = value; } }
		public static Texture2D TransparentTexture { set { TextLayer.sTransparentTexture = value; } }

		private static GameObject sTextLayerPrefab;
		private static Texture2D sTransparentTexture;
		
		public Text LayerText { get { return this.sText; } }

		private Text sText;

		public TextLayer(string sLayerName) : base(sLayerName, TextLayer.sTextLayerPrefab)
		{
			this.sText = this.sLayerObject.GetComponentInChildren<Text>();
			this.sLayerImage.material.mainTexture = TextLayer.sTransparentTexture;
		}
	}
}