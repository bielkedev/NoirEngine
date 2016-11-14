using UnityEngine;
using System.Collections;

namespace Noir.Unity
{
	public struct LayerTransitionData
	{
		public bool bFadeIn;
		public float nDuration;
		public Sprite sRuleSprite;
	}

	public class LayerTransition : MonoBehaviour
	{
		public Layer _Layer { set { this.sLayer = value; } }
		public LayerTransitionData _LayerTransitionData { set { this.sTransitionData = value; } }

		private Layer sLayer;
		private LayerTransitionData sTransitionData;
		private float nRunningTime;

		public IEnumerator runTransition()
		{
			Material sMaterial = this.sLayer.Image.material;
			this.nRunningTime = 0f;

			if (this.sTransitionData.bFadeIn)
			{
				//Fade in
				while ((this.nRunningTime += Time.deltaTime) <= this.sTransitionData.nDuration)
				{
					sMaterial.SetFloat("_Alpha", this.nRunningTime / this.sTransitionData.nDuration);
					yield return null;
				}
			}
			else
			{
				//Fade out
				while ((this.nRunningTime += Time.deltaTime) <= this.sTransitionData.nDuration)
				{
					sMaterial.SetFloat("_Alpha", 1f - this.nRunningTime / this.sTransitionData.nDuration);
					yield return null;
				}
			}

			if (this.sTransitionData.bFadeIn)
				GameObject.Destroy(this);
			else
				sLayer.deleteLayer();
		}
	}
}