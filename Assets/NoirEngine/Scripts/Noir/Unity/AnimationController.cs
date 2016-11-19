using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Noir.Unity
{
	[RequireComponent(typeof(RectTransform), typeof(RawImage))]
	public class AnimationController : MonoBehaviour
	{
		public int _SpriteNum;
		public List<KeyValuePair<Sprite, float>> _SpriteList;

		private RectTransform sTransform;
		private RawImage sRawImage;

		private void Awake()
		{
			this.sTransform = this.gameObject.GetComponent<RectTransform>();
			this.sRawImage = this.gameObject.GetComponent<RawImage>();
		}

		public IEnumerator startAnimation() 
		{
			if (this._SpriteNum <= 0)
				yield break;

			for(int nFrame = 0; ;)
			{
				var sPair = this._SpriteList[nFrame];

				this.sRawImage.texture = sPair.Key.texture;
				this.sTransform.sizeDelta = sPair.Key.textureRect.size;

				yield return new WaitForSeconds(sPair.Value);

				if (++nFrame >= this._SpriteNum)
					nFrame = 0;
			}
		}
	}
}