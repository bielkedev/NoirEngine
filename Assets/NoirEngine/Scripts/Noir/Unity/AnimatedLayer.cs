using System.Collections.Generic;
using UnityEngine;

namespace Noir.Unity
{
	public class AnimatedLayer : Layer
	{
		public static GameObject AnimatedLayerPrefab { set { AnimatedLayer.sAnimatedLayerPrefab = value; } }

		private static GameObject sAnimatedLayerPrefab;

		private AnimationController sAnimationController;
		private List<KeyValuePair<Sprite, float>> sMainSpriteList = new List<KeyValuePair<Sprite, float>>();

		public AnimatedLayer(AnimatedLayer sAnimatedLayer) : base(sAnimatedLayer)
		{
			this.sAnimationController = this.sLayerObject.GetComponent<AnimationController>();
			this.sAnimationController._SpriteList = this.sMainSpriteList;

			foreach (var sPair in sAnimatedLayer.sMainSpriteList)
				this.sMainSpriteList.Add(sPair);
		}

		public AnimatedLayer(string sLayerName) : base(sLayerName, AnimatedLayer.sAnimatedLayerPrefab)
		{
			(this.sAnimationController = this.sLayerObject.GetComponent<AnimationController>())._SpriteList = this.sMainSpriteList;
		}

		public void addLayerSprite(Sprite sNewMainSprite, float nTime)
		{
			this.sMainSpriteList.Add(new KeyValuePair<Sprite, float>(sNewMainSprite, nTime));
		}

		public void updateLayerSprite()
		{
			this.sAnimationController.StopAllCoroutines();
			this.sAnimationController._SpriteNum = this.sMainSpriteList.Count;
			this.sAnimationController.StartCoroutine(this.sAnimationController.startAnimation());
		}
	}
}