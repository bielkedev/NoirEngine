using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace Noir.Unity
{
	public class Layer
	{
		public static RectTransform LayerPanel { set { Layer.sLayerPanel = value; } }
		public static GameObject NamedLayerPrefab { set { Layer.sNamedLayerPrefab = value; } }
		public static Material NamedLayerMaterial { set { Layer.sNamedLayerMaterial = value; } }
		public static IEnumerable<KeyValuePair<string, Layer>> NamedLayerEnumerable { get { return Layer.sLayerMap; } }

		private static RectTransform sLayerPanel;
		private static GameObject sNamedLayerPrefab;
		private static Material sNamedLayerMaterial;
		private static Dictionary<string, Layer> sLayerMap = new Dictionary<string, Layer>();
		private static SortedList<string, Layer> sLayerList = new SortedList<string, Layer>();

		public bool IsUpdated { get { return this.bUpdated; } }
		public string LayerName { get { return this.sNamedLayer.name; } }
		public GameObject NamedLayer { get { return this.sNamedLayer; } }
		public RawImage Image { get { return this.sNamedLayerImage; } }
		public RectTransform Transform { get { return this.sNamedLayerTransform; } }

		private bool bUpdated = false;
		private GameObject sNamedLayer;
		private RawImage sNamedLayerImage;
		private RectTransform sNamedLayerTransform;
		private List<LayerTween> sLayerTweenList = new List<LayerTween>();

		public static Layer getLayer(string sLayerName)
		{
			Layer sLayer;
			Layer.sLayerMap.TryGetValue(sLayerName, out sLayer);

			return sLayer;
		}

		public Layer(string sLayerName)
		{
			Layer.sLayerMap.Add((this.sNamedLayer = GameObject.Instantiate(Layer.sNamedLayerPrefab)).name = sLayerName, this);
			Layer.sLayerList.Add(sLayerName, this);

			this.sNamedLayerImage = this.sNamedLayer.GetComponent<RawImage>();
			(this.sNamedLayerTransform = this.sNamedLayer.GetComponent<RectTransform>()).SetParent(Layer.sLayerPanel, false);
			this.sNamedLayerTransform.SetSiblingIndex(Layer.sLayerList.IndexOfKey(sLayerName));
		}

		public void addLayerTween(ref LayerTweenData sNewLayerTweenData, LayerPropertiesModifier fNewPropertiesModifier)
		{
			LayerTween sTween = this.sNamedLayer.AddComponent<LayerTween>();
			sTween._Layer = this;
			sTween._LayerTweenData = sNewLayerTweenData;
			sTween._LayerPropertiesModifier = fNewPropertiesModifier;
			sTween.StartCoroutine(sTween.runTween());

			this.sLayerTweenList.Add(sTween);
		}

		public void removeLayerTweenAll()
		{
			foreach (var sTween in this.sLayerTweenList)
				if (sTween != null)
					sTween.StopAllCoroutines();

			this.sLayerTweenList.Clear();
		}

		public void setLayerSprite(Sprite sMainSprite, Sprite sMaskSprite)
		{
			this.bUpdated = false;

			Material sMaterial = GameObject.Instantiate(Layer.sNamedLayerMaterial);

			sMaterial.SetTexture("_MainTex", sMainSprite == null ? null : sMainSprite.texture);
			sMaterial.SetTexture("_MaskTex", sMaskSprite == null ? null : sMaskSprite.texture);

			if (sMainSprite != null)
			{
				Vector2 sSize;
				sSize.x = sMainSprite.texture.width;
				sSize.y = sMainSprite.texture.height;

				this.sNamedLayerTransform.sizeDelta = sSize;
			}

			this.sNamedLayerImage.material = sMaterial;
		}

		public void setPosX(float nNewX)
		{
			this.bUpdated = false;

			Vector3 sPosition = this.sNamedLayerTransform.localPosition;
			sPosition.x = nNewX;

			this.sNamedLayerTransform.localPosition = sPosition;
		}

		public void setPosY(float nNewY)
		{
			this.bUpdated = false;

			Vector3 sPosition = this.sNamedLayerTransform.localPosition;
			sPosition.y = nNewY;

			this.sNamedLayerTransform.localPosition = sPosition;
		}

		public void setAlpha(float nAlpha)
		{
			this.bUpdated = false;
			this.sNamedLayerImage.material.SetFloat("_Alpha", nAlpha / 255f);
		}

		public void setAnchorX(float nNewAnchorX)
		{
			this.bUpdated = false;

			Vector2 sPivot = this.sNamedLayerTransform.pivot;
			sPivot.x = nNewAnchorX;

			this.sNamedLayerTransform.pivot = sPivot;
		}

		public void setAnchorY(float nNewAnchorY)
		{
			Vector2 sPivot = this.sNamedLayerTransform.pivot;
			sPivot.y = nNewAnchorY;

			this.sNamedLayerTransform.pivot = sPivot;
		}

		public void setScaleX(float nNewScaleX)
		{
			this.bUpdated = false;

			Vector3 sScale = this.sNamedLayerTransform.localScale;
			sScale.x = Mathf.Abs(nNewScaleX) / 100f * Mathf.Sign(sScale.x);

			this.sNamedLayerTransform.localScale = sScale;
		}

		public void setScaleY(float nNewScaleY)
		{
			this.bUpdated = false;

			Vector3 sScale = this.sNamedLayerTransform.localScale;
			sScale.y = Mathf.Abs(nNewScaleY) / 100f * Mathf.Sign(sScale.y);

			this.sNamedLayerTransform.localScale = sScale;
		}

		public void setRotate(float nNewAngle)
		{
			this.bUpdated = false;

			this.sNamedLayerTransform.rotation = Quaternion.Euler(0f, 0f, nNewAngle);
		}

		public void setReverseX(float nNewReverseX)
		{
			this.bUpdated = false;

			Vector3 sScale = this.sNamedLayerTransform.localScale;
			sScale.x = nNewReverseX == 0f ? Mathf.Abs(sScale.x) : -Mathf.Abs(sScale.x);

			this.sNamedLayerTransform.localScale = sScale;
		}

		public void setReverseY(float nNewReverseY)
		{
			this.bUpdated = false;

			Vector3 sScale = this.sNamedLayerTransform.localScale;
			sScale.y = nNewReverseY == 0f ? Mathf.Abs(sScale.y) : -Mathf.Abs(sScale.y);

			this.sNamedLayerTransform.localScale = sScale;
		}

		public void setVisible(float nNewVisible)
		{
			this.bUpdated = false;
			this.sNamedLayer.SetActive(nNewVisible != 0f);
		}

		public void deleteLayer()
		{
			this.removeLayerTweenAll();

			Layer.sLayerMap.Remove(this.sNamedLayer.name);
			Layer.sLayerList.Remove(this.sNamedLayer.name);
			GameObject.Destroy(this.sNamedLayer);
		}

		public void markUpdate()
		{
			this.bUpdated = true;
		}
	}
}