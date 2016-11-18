using System.Collections.Generic;
using UnityEngine;

namespace Noir.Unity
{
	public class AnimatedLayer : Layer
	{
		public static GameObject AnimatedLayerPrefab { set { AnimatedLayer.sAnimatedLayerPrefab = value; } }

		private static GameObject sAnimatedLayerPrefab;

		private List<KeyValuePair<Sprite, float>> sMainSpriteList = new List<KeyValuePair<Sprite, float>>();

		public AnimatedLayer(string sLayerName) : base(sLayerName, AnimatedLayer.sAnimatedLayerPrefab)
		{
			//Empty.
		}

		public void addLayerSprite(Sprite sNewMainSprite, float nTime)
		{
			this.sMainSpriteList.Add(new KeyValuePair<Sprite, float>(sNewMainSprite, nTime));
		}
	}
}