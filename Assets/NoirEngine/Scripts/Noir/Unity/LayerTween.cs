using Noir.Util;
using UnityEngine;
using System.Collections;

namespace Noir.Unity
{
	public delegate void LayerStateModifier(Layer sLayer, float nValue, bool bUpdateInstantly);

	public struct LayerTweenData
	{
		public bool bDelete;
		public int nLoop;
		public int nYoyo;
		public float nDelay;
		public float nDuration;
		public float nValueBegin;
		public float nValueEnd;
		public EasingFunc fEasingFunc;
	}

	public class LayerTween : MonoBehaviour
	{
		public Layer _Layer { set { this.sLayer = value; } }
		public LayerTweenData _LayerTweenData { set { this.sTweenData = value; } }
		public LayerStateModifier _LayerStateModifier { set { this.fStateModifier = value; } }

		private Layer sLayer;
		private LayerTweenData sTweenData;
		private LayerStateModifier fStateModifier;
		private float nRunningTime;

		public IEnumerator runTween()
		{
			this.nRunningTime = 0f;

			yield return new WaitForSeconds(this.sTweenData.nDelay);

			if (this.sTweenData.nLoop == -1 || this.sTweenData.nLoop > 0)
			{
				if (this.sTweenData.nLoop == -1)
				{
					for (;;)
					{
						this.nRunningTime = 0f;

						while ((this.nRunningTime += Time.deltaTime) <= this.sTweenData.nDuration)
						{
							this.fStateModifier(this.sLayer, this.sTweenData.fEasingFunc(this.sTweenData.nValueBegin, this.sTweenData.nValueEnd, this.nRunningTime / this.sTweenData.nDuration), true);
							yield return null;
						}
					}
				}
				else
				{
					for (; this.sTweenData.nLoop > 0; --this.sTweenData.nLoop)
					{
						this.nRunningTime = 0f;

						while ((this.nRunningTime += Time.deltaTime) <= this.sTweenData.nDuration)
						{
							this.fStateModifier(this.sLayer, this.sTweenData.fEasingFunc(this.sTweenData.nValueBegin, this.sTweenData.nValueEnd, this.nRunningTime / this.sTweenData.nDuration), true);
							yield return null;
						}
					}

					this.fStateModifier(this.sLayer, this.sTweenData.nValueEnd, true);
				}
			}
			else if (this.sTweenData.nYoyo == -1)
			{
				for (;;)
				{
					while ((this.nRunningTime += Time.deltaTime) <= this.sTweenData.nDuration)
					{
						this.fStateModifier(this.sLayer, this.sTweenData.fEasingFunc(this.sTweenData.nValueBegin, this.sTweenData.nValueEnd, this.nRunningTime / this.sTweenData.nDuration), true);
						yield return null;
					}

					while ((this.nRunningTime -= Time.deltaTime) >= 0)
					{
						this.fStateModifier(this.sLayer, this.sTweenData.fEasingFunc(this.sTweenData.nValueBegin, this.sTweenData.nValueEnd, this.nRunningTime / this.sTweenData.nDuration), true);
						yield return null;
					}
				}
			}
			else if (this.sTweenData.nYoyo > 0)
			{
				for (; this.sTweenData.nYoyo > 0; --this.sTweenData.nYoyo)
				{
					while ((this.nRunningTime += Time.deltaTime) <= this.sTweenData.nDuration)
					{
						this.fStateModifier(this.sLayer, this.sTweenData.fEasingFunc(this.sTweenData.nValueBegin, this.sTweenData.nValueEnd, this.nRunningTime / this.sTweenData.nDuration), true);
						yield return null;
					}
					
					while ((this.nRunningTime -= Time.deltaTime) >= 0)
					{
						this.fStateModifier(this.sLayer, this.sTweenData.fEasingFunc(this.sTweenData.nValueBegin, this.sTweenData.nValueEnd, this.nRunningTime / this.sTweenData.nDuration), true);
						yield return null;
					}
				}

				this.fStateModifier(this.sLayer, this.sTweenData.nValueBegin, true);
			}
			else
			{
				while ((this.nRunningTime += Time.deltaTime) <= this.sTweenData.nDuration)
				{
					this.fStateModifier(this.sLayer, this.sTweenData.fEasingFunc(this.sTweenData.nValueBegin, this.sTweenData.nValueEnd, this.nRunningTime / this.sTweenData.nDuration), true);
					yield return null;
				}

				this.fStateModifier(this.sLayer, this.sTweenData.nValueEnd, true);
			}

			if (this.sTweenData.bDelete)
				this.sLayer.deleteLayer();
			else
			{
				this.sLayer.removeLayerTween(this);
				GameObject.Destroy(this);
			}
		}
	}
}