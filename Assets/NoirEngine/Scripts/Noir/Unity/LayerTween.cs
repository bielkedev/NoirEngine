using Noir.Util;
using UnityEngine;
using System.Collections;

namespace Noir.Unity
{
	public delegate void LayerPropertiesModifier(float nValue);

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
		public LayerPropertiesModifier _LayerPropertiesModifier { set { this.fPropModifier = value; } }

		private Layer sLayer;
		private LayerTweenData sTweenData;
		private LayerPropertiesModifier fPropModifier;
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
						this.fPropModifier(this.sTweenData.nValueBegin);
						yield return null;

						while ((this.nRunningTime += Time.deltaTime) <= this.sTweenData.nDuration)
						{
							this.fPropModifier(this.sTweenData.fEasingFunc(this.sTweenData.nValueBegin, this.sTweenData.nValueEnd, this.nRunningTime / this.sTweenData.nDuration));
							yield return null;
						}

						this.fPropModifier(this.sTweenData.nValueEnd);
						yield return null;
					}
				}
				else
				{
					for (; this.sTweenData.nLoop > 0; --this.sTweenData.nLoop)
					{
						this.nRunningTime = 0f;
						this.fPropModifier(this.sTweenData.nValueBegin);
						yield return null;

						while ((this.nRunningTime += Time.deltaTime) <= this.sTweenData.nDuration)
						{
							this.fPropModifier(this.sTweenData.fEasingFunc(this.sTweenData.nValueBegin, this.sTweenData.nValueEnd, this.nRunningTime / this.sTweenData.nDuration));
							yield return null;
						}

						this.fPropModifier(this.sTweenData.nValueEnd);
						yield return null;
					}
				}
			}
			else if (this.sTweenData.nYoyo == -1)
			{
				for (;;)
				{
					this.fPropModifier(this.sTweenData.nValueBegin);
					yield return null;

					while ((this.nRunningTime += Time.deltaTime) <= this.sTweenData.nDuration)
					{
						this.fPropModifier(this.sTweenData.fEasingFunc(this.sTweenData.nValueBegin, this.sTweenData.nValueEnd, this.nRunningTime / this.sTweenData.nDuration));
						yield return null;
					}

					this.fPropModifier(this.sTweenData.nValueEnd);
					yield return null;

					while ((this.nRunningTime -= Time.deltaTime) >= 0)
					{
						this.fPropModifier(this.sTweenData.fEasingFunc(this.sTweenData.nValueBegin, this.sTweenData.nValueEnd, this.nRunningTime / this.sTweenData.nDuration));
						yield return null;
					}

					this.fPropModifier(this.sTweenData.nValueBegin);
					yield return null;
				}
			}
			else if (this.sTweenData.nYoyo > 0)
			{
				for (; this.sTweenData.nYoyo > 0; --this.sTweenData.nYoyo)
				{
					this.fPropModifier(this.sTweenData.nValueBegin);
					yield return null;

					while ((this.nRunningTime += Time.deltaTime) <= this.sTweenData.nDuration)
					{
						this.fPropModifier(this.sTweenData.fEasingFunc(this.sTweenData.nValueBegin, this.sTweenData.nValueEnd, this.nRunningTime / this.sTweenData.nDuration));
						yield return null;
					}

					this.fPropModifier(this.sTweenData.nValueEnd);
					yield return null;

					while ((this.nRunningTime -= Time.deltaTime) >= 0)
					{
						this.fPropModifier(this.sTweenData.fEasingFunc(this.sTweenData.nValueBegin, this.sTweenData.nValueEnd, this.nRunningTime / this.sTweenData.nDuration));
						yield return null;
					}

					this.fPropModifier(this.sTweenData.nValueBegin);
					yield return null;
				}
			}
			else
			{
				this.fPropModifier(this.sTweenData.nValueBegin);
				yield return null;

				while ((this.nRunningTime += Time.deltaTime) <= this.sTweenData.nDuration)
				{
					this.fPropModifier(this.sTweenData.fEasingFunc(this.sTweenData.nValueBegin, this.sTweenData.nValueEnd, this.nRunningTime / this.sTweenData.nDuration));
					yield return null;
				}

				this.fPropModifier(this.sTweenData.nValueEnd);
				yield return null;
			}

			if (this.sTweenData.bDelete)
				this.sLayer.deleteLayer();
			else
				GameObject.Destroy(this);
		}
	}
}