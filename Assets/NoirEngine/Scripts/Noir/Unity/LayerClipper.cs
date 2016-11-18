using UnityEngine;
using UnityEngine.UI;

namespace Noir.Unity
{
	public class LayerClipper
	{
		public static GameObject LayerClipperPrefab { set { LayerClipper.sLayerClipperPrefab = value; } }

		private static GameObject sLayerClipperPrefab;

		public RectTransform Transform { get { return this.sClipperTransform; } }
		public Rect Region { get { return this.sClipperTransform.rect; } }
		public bool Enabled { get { return this.sClipperMask.enabled; } set { this.sClipperMask.enabled = value; } }

		private GameObject sClipperObject;
		private RectTransform sClipperTransform;
		private RectMask2D sClipperMask;
		private Layer sClippingLayer;

		public LayerClipper(Layer sNewClippingLayer, bool bNewEnabled)
		{
			(this.sClippingLayer = sNewClippingLayer).Transform.SetParent(this.sClipperTransform = (this.sClipperObject = GameObject.Instantiate(LayerClipper.sLayerClipperPrefab)).GetComponent<RectTransform>(), true);
			(this.sClipperMask = this.sClipperObject.GetComponent<RectMask2D>()).enabled = bNewEnabled;
		}

		public void setRegion(Rect sNewRegion)
		{
			Vector3 sWorldPosition = this.sClippingLayer.Transform.position;

			this.sClipperTransform.position = sNewRegion.position;
			this.sClipperTransform.sizeDelta = sNewRegion.size;

			this.sClippingLayer.Transform.position = sWorldPosition;
		}

		public void deleteLayerClipper()
		{
			GameObject.Destroy(this.sClipperObject);
		}
	}
}