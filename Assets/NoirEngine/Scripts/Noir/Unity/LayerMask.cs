using UnityEngine;
using UnityEngine.UI;

namespace Noir.Unity
{
	public class LayerMask
	{
		public Rect Region
		{
			get { return this.sLayerTransform.rect; }
			set
			{
				Vector3 sWorldPosition = this.sLayerTransform.position;

				this.sMaskTransform.localPosition = value.position;
				this.sMaskTransform.sizeDelta = value.size;

				this.sLayerTransform.position = sWorldPosition;
			}
		}
		public RectTransform Transform { get { return this.sMaskTransform; } }
		public bool Enabled { get { return this.sMask.enabled; } set { this.sMask.enabled = value; } }

		private GameObject sLayerMaskObject;
		private RectTransform sLayerTransform;
		private RectTransform sMaskTransform;
		private RectMask2D sMask;

		public LayerMask(GameObject sLayerObject, GameObject sMaskObject)
		{
			this.sLayerMaskObject = sMaskObject;
			this.sLayerTransform = sLayerObject.GetComponent<RectTransform>();
			this.sMaskTransform = sMaskObject.GetComponent<RectTransform>();
			this.sMask = sMaskObject.GetComponent<RectMask2D>();
		}

		public void deleteLayerMask()
		{
			GameObject.Destroy(this.sLayerMaskObject);
		}
	}
}