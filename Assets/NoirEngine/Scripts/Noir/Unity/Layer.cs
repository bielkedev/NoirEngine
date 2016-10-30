using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace Noir.Unity
{
	public struct LayerProperties
	{
		public Sprite sMainSprite;
		public Sprite sMaskSprite;
		public Vector2 sPosition;
		public Vector2 sAnchor;
		public Vector2 sScale;
		public float nAlpha;
		public float nRotation;
		public bool bReverseX;
		public bool bReverseY;
		public bool bVisible;

		public LayerProperties(Sprite sNewMainSprite, Sprite sNewMaskSprite)
		{
			this.sMainSprite = sNewMainSprite;
			this.sMaskSprite = sNewMaskSprite;
			this.sPosition = new Vector2(0f, 0f);
			this.sAnchor = new Vector2(.5f, .5f);
			this.sScale = new Vector2(1f, 1f);
			this.nAlpha = 1f;
			this.nRotation = 0f;
			this.bReverseX = false;
			this.bReverseY = false;
			this.bVisible = true;
		}
	}

	public struct LayerPropertiesDirty
	{
		public bool bSpriteDirty;
		public bool bPositionDirty;
		public bool bAnchorDirty;
		public bool bScaleDirty;
		public bool bAlphaDirty;
		public bool bRotationDirty;
		public bool bReverseDirty;
		public bool bVisibleDirty;
	}

	public class Layer
	{
		public static RectTransform LayerPanel { set { Layer.sLayerPanel = value; } }
		public static GameObject NamedLayerPrefab { set { Layer.sNamedLayerPrefab = value; } }
		public static Material NamedLayerMaterial { set { Layer.sNamedLayerMaterial = value; } }
		public static IEnumerable<KeyValuePair<string, Layer>> NamedLayerEnumerable { get { return Layer.sLayerMap; } }
		public static IEnumerable<Layer> NeedUpdateLayerEnumerable { get { return Layer.sNeedUpdateLayerSet; } }

		private static RectTransform sLayerPanel;
		private static GameObject sNamedLayerPrefab;
		private static Material sNamedLayerMaterial;
		private static Dictionary<string, Layer> sLayerMap = new Dictionary<string, Layer>();
		private static SortedList<string, Layer> sLayerList = new SortedList<string, Layer>();
		private static HashSet<Layer> sNeedUpdateLayerSet = new HashSet<Layer>();
		
		public string LayerName { get { return this.sNamedLayer.name; } }
		public GameObject NamedLayer { get { return this.sNamedLayer; } }
		public RawImage Image { get { return this.sNamedLayerImage; } }
		public RectTransform Transform { get { return this.sNamedLayerTransform; } }

		private GameObject sNamedLayer;
		private RawImage sNamedLayerImage;
		private RectTransform sNamedLayerTransform;
		private LayerProperties sLayerProperties = new LayerProperties(null, null);
		private LayerPropertiesDirty sLayerPropertiesDirty = new LayerPropertiesDirty();
		private List<LayerTween> sLayerTweenList = new List<LayerTween>();

		public static Layer getLayer(string sLayerName)
		{
			Layer sLayer;
			Layer.sLayerMap.TryGetValue(sLayerName, out sLayer);

			return sLayer;
		}

		public static void clearNeedUpdateLayerList()
		{
			Layer.sNeedUpdateLayerSet.Clear();
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
			this.sLayerProperties.sMainSprite = sMainSprite;
			this.sLayerProperties.sMaskSprite = sMaskSprite;
			this.sLayerPropertiesDirty.bSpriteDirty = true;
		}

		public void setPosX(float nNewX)
		{
			this.sLayerProperties.sPosition.x = nNewX;
			this.sLayerPropertiesDirty.bPositionDirty = true;
		}

		public void setPosY(float nNewY)
		{
			this.sLayerProperties.sPosition.y = nNewY;
			this.sLayerPropertiesDirty.bPositionDirty = true;
		}

		public void setAlpha(float nAlpha)
		{
			this.sLayerProperties.nAlpha = nAlpha / 255f;
			this.sLayerPropertiesDirty.bAlphaDirty = true;
		}

		public void setAnchorX(float nNewAnchorX)
		{
			this.sLayerProperties.sAnchor.x = nNewAnchorX;
			this.sLayerPropertiesDirty.bAnchorDirty = true;
		}

		public void setAnchorY(float nNewAnchorY)
		{
			this.sLayerProperties.sAnchor.y = nNewAnchorY;
			this.sLayerPropertiesDirty.bAnchorDirty = true;
		}

		public void setScaleX(float nNewScaleX)
		{
			this.sLayerProperties.sScale.x = nNewScaleX / 100f;
			this.sLayerPropertiesDirty.bScaleDirty = true;
		}

		public void setScaleY(float nNewScaleY)
		{
			this.sLayerProperties.sScale.y = nNewScaleY / 100f;
			this.sLayerPropertiesDirty.bScaleDirty = true;
		}

		public void setRotate(float nNewAngle)
		{
			this.sLayerProperties.nRotation = nNewAngle;
			this.sLayerPropertiesDirty.bRotationDirty = true;
		}

		public void setReverseX(float nNewReverseX)
		{
			this.sLayerProperties.bReverseX = nNewReverseX != 0.0f;
			this.sLayerPropertiesDirty.bReverseDirty = true;
		}

		public void setReverseY(float nNewReverseY)
		{
			this.sLayerProperties.bReverseY = nNewReverseY != 0.0f;
			this.sLayerPropertiesDirty.bReverseDirty = true;
		}

		public void setVisible(float nNewVisible)
		{
			this.sLayerProperties.bVisible = nNewVisible != 0f;
			this.sLayerPropertiesDirty.bVisibleDirty = true;
		}

		public void deleteLayer()
		{
			this.removeLayerTweenAll();

			Layer.sLayerMap.Remove(this.sNamedLayer.name);
			Layer.sLayerList.Remove(this.sNamedLayer.name);
			GameObject.Destroy(this.sNamedLayer);
		}

		public void applyLayerProperties()
		{
			//Apply sprite
			if(this.sLayerPropertiesDirty.bSpriteDirty)
			{
				Material sNewMaterial = GameObject.Instantiate(Layer.sNamedLayerMaterial);
				sNewMaterial.SetTexture("_MainTex", this.sLayerProperties.sMainSprite == null ? null : this.sLayerProperties.sMainSprite.texture);
				sNewMaterial.SetTexture("_MaskTex", this.sLayerProperties.sMaskSprite == null ? null : this.sLayerProperties.sMaskSprite.texture);

				if(this.sLayerProperties.sMainSprite != null)
				{
					Vector2 sSize;
					sSize.x = this.sLayerProperties.sMainSprite.texture.width;
					sSize.y = this.sLayerProperties.sMainSprite.texture.height;

					this.sNamedLayerTransform.sizeDelta = sSize;
				}

				this.sNamedLayerImage.material = sNewMaterial;
				this.sLayerPropertiesDirty.bSpriteDirty = false;
			}

			//Apply position
			if(this.sLayerPropertiesDirty.bPositionDirty)
			{
				this.sNamedLayerTransform.localPosition = this.sLayerProperties.sPosition;
				this.sLayerPropertiesDirty.bPositionDirty = false;
			}

			//Apply anchor
			if (this.sLayerPropertiesDirty.bAnchorDirty)
			{
				this.sNamedLayerTransform.pivot = this.sLayerProperties.sAnchor;
				this.sLayerPropertiesDirty.bAnchorDirty = false;
			}

			//Apply scale
			if (this.sLayerPropertiesDirty.bScaleDirty)
			{
				Vector3 sScale = this.sNamedLayerTransform.localScale;
				sScale.x = Mathf.Abs(this.sLayerProperties.sScale.x) * Mathf.Sign(sScale.x);
				sScale.y = Mathf.Abs(this.sLayerProperties.sScale.y) * Mathf.Sign(sScale.y);

				this.sNamedLayerTransform.localScale = sScale;
				this.sLayerPropertiesDirty.bScaleDirty = false;
			}

			//Apply alpha
			if(this.sLayerPropertiesDirty.bAlphaDirty)
			{
				this.sNamedLayerImage.material.SetFloat("_Alpha", this.sLayerProperties.nAlpha);
				this.sLayerPropertiesDirty.bAlphaDirty = false;
			}

			//Apply rotation
			if(this.sLayerPropertiesDirty.bRotationDirty)
			{
				this.sNamedLayerTransform.localRotation = Quaternion.Euler(0f, 0f, this.sLayerProperties.nRotation);
				this.sLayerPropertiesDirty.bRotationDirty = false;
			}
			
			//Apply reverse
			if(this.sLayerPropertiesDirty.bReverseDirty)
			{
				Vector3 sScale = this.sNamedLayerTransform.localScale;
				sScale.x = this.sLayerProperties.bReverseX ? Mathf.Abs(sScale.x) : -Mathf.Abs(sScale.x);
				sScale.y = this.sLayerProperties.bReverseY ? Mathf.Abs(sScale.y) : -Mathf.Abs(sScale.y);

				this.sNamedLayerTransform.localScale = sScale;
				this.sLayerPropertiesDirty.bReverseDirty = false;
			}

			//Apply visible
			if(this.sLayerPropertiesDirty.bVisibleDirty)
			{
				this.sNamedLayer.SetActive(this.sLayerProperties.bVisible);
				this.sLayerPropertiesDirty.bVisibleDirty = false;
			}
		}
		
		public void makeAsNeedUpdate()
		{
			Layer.sNeedUpdateLayerSet.Add(this);
		}
	}
}