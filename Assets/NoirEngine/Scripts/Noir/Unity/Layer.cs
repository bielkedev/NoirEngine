using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Noir.Unity
{
	public abstract class Layer
	{
		public static GameObject LayerMaskPrefab { set { Layer.sLayerMaskPrefab = value; } }
		public static RectTransform LayerPanel { set { Layer.sLayerPanel = value; } }
		public static Material NamedLayerMaterial { set { Layer.sNamedLayerMaterial = value; } }
		public static IEnumerable<KeyValuePair<string, Layer>> NamedLayerEnumerable { get { return Layer.sLayerMap; } }
		public static IEnumerable<Layer> NeedUpdateLayerEnumerable { get { return Layer.sNeedUpdateLayerSet; } }

		protected static GameObject sLayerMaskPrefab;
		protected static RectTransform sLayerPanel;
		protected static Material sNamedLayerMaterial;
		protected static Dictionary<string, Layer> sLayerMap = new Dictionary<string, Layer>();
		protected static SortedList<string, Layer> sLayerList = new SortedList<string, Layer>();
		protected static HashSet<Layer> sNeedUpdateLayerSet = new HashSet<Layer>();

		public string LayerName { get { return this.sNamedLayer.name; } }
		public GameObject NamedLayer { get { return this.sNamedLayer; } }
		public RawImage Image { get { return this.sNamedLayerImage; } }
		public RectTransform Transform { get { return this.sNamedLayerTransform; } }
		public LayerMask Mask { get { return this.sMask; } }

		protected GameObject sNamedLayer;
		protected RawImage sNamedLayerImage;
		protected RectTransform sNamedLayerTransform;
		protected LayerMask sMask;
		protected LayerState sLayerState = LayerState.Identity;
		protected List<LayerTween> sLayerTweenList = new List<LayerTween>();

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

		protected Layer(string sLayerName, GameObject sNamedLayerPrefab)
		{
			GameObject sMaskObject = GameObject.Instantiate(Layer.sLayerMaskPrefab);
			this.sNamedLayer = GameObject.Instantiate(sNamedLayerPrefab);

			sMaskObject.transform.SetParent(Layer.sLayerPanel);
			this.sNamedLayer.transform.SetParent(sMaskObject.transform);

			Layer.sLayerMap.Add(this.sNamedLayer.name = sLayerName, this);
			Layer.sLayerList.Add(sLayerName, this);

			(this.sNamedLayerImage = this.sNamedLayer.GetComponent<RawImage>()).material = GameObject.Instantiate<Material>(Layer.sNamedLayerMaterial);
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

		public void setPosX(float nNewX, bool bUpdateInstantly = false)
		{
			if (bUpdateInstantly)
			{
				Vector3 sPosition = this.sNamedLayerTransform.localPosition;
				sPosition.x = nNewX;
				this.sNamedLayerTransform.localPosition = sPosition;
			}
			else
			{
				this.sLayerProperties.sPosition.x = nNewX;
				this.sLayerPropertiesDirty.bPositionDirty = true;
			}
		}

		public void setPosY(float nNewY, bool bUpdateInstantly = false)
		{
			if (bUpdateInstantly)
			{
				Vector3 sPosition = this.sNamedLayerTransform.localPosition;
				sPosition.y = nNewY;
				this.sNamedLayerTransform.localPosition = sPosition;
			}
			else
			{
				this.sLayerProperties.sPosition.y = nNewY;
				this.sLayerPropertiesDirty.bPositionDirty = true;
			}
		}

		public void setAlpha(float nAlpha, bool bUpdateInstantly = false)
		{
			if (bUpdateInstantly)
				this.sNamedLayerImage.material.SetFloat("_Alpha", nAlpha / 255f);
			else
			{
				this.sLayerProperties.nAlpha = nAlpha / 255f;
				this.sLayerPropertiesDirty.bAlphaDirty = true;
			}
		}

		public void setAnchorX(float nNewAnchorX, bool bUpdateInstantly = false)
		{
			if (bUpdateInstantly)
			{
				Vector2 sPivot = this.sNamedLayerTransform.pivot;
				sPivot.x = nNewAnchorX;
				this.sNamedLayerTransform.pivot = sPivot;
			}
			else
			{
				this.sLayerProperties.sAnchor.x = nNewAnchorX;
				this.sLayerPropertiesDirty.bAnchorDirty = true;
			}
		}

		public void setAnchorY(float nNewAnchorY, bool bUpdateInstantly = false)
		{
			if (bUpdateInstantly)
			{
				Vector2 sPivot = this.sNamedLayerTransform.pivot;
				sPivot.y = nNewAnchorY;
				this.sNamedLayerTransform.pivot = sPivot;
			}
			else
			{
				this.sLayerProperties.sAnchor.y = nNewAnchorY;
				this.sLayerPropertiesDirty.bAnchorDirty = true;
			}
		}

		public void setScaleX(float nNewScaleX, bool bUpdateInstantly = false)
		{
			if (bUpdateInstantly)
			{
				Vector3 sScale = this.sNamedLayerTransform.localScale;
				sScale.x = Mathf.Sign(sScale.x) * Mathf.Abs(nNewScaleX / 100f);
				this.sNamedLayerTransform.localScale = sScale;
			}
			else
			{
				this.sLayerProperties.sScale.x = nNewScaleX / 100f;
				this.sLayerPropertiesDirty.bScaleDirty = true;
			}
		}

		public void setScaleY(float nNewScaleY, bool bUpdateInstantly = false)
		{
			if (bUpdateInstantly)
			{
				Vector3 sScale = this.sNamedLayerTransform.localScale;
				sScale.y = Mathf.Sign(sScale.y) * Mathf.Abs(nNewScaleY / 100f);
				this.sNamedLayerTransform.localScale = sScale;
			}
			else
			{
				this.sLayerProperties.sScale.y = nNewScaleY / 100f;
				this.sLayerPropertiesDirty.bScaleDirty = true;
			}
		}

		public void setRotate(float nNewAngle, bool bUpdateInstantly = false)
		{
			if (bUpdateInstantly)
				this.sNamedLayerTransform.localRotation = Quaternion.Euler(0f, 0f, nNewAngle);
			else
			{
				this.sLayerProperties.nRotation = nNewAngle;
				this.sLayerPropertiesDirty.bRotationDirty = true;
			}
		}

		public void setReverseX(float nNewReverseX, bool bUpdateInstantly = false)
		{
			if (bUpdateInstantly)
			{
				Vector3 sScale = this.sNamedLayerTransform.localScale;
				sScale.x = nNewReverseX != 0.0f ? -Mathf.Abs(sScale.x) : Mathf.Abs(sScale.x);
				this.sNamedLayerTransform.localScale = sScale;
			}
			else
			{
				this.sLayerProperties.bReverseX = nNewReverseX != 0.0f;
				this.sLayerPropertiesDirty.bReverseDirty = true;
			}
		}

		public void setReverseY(float nNewReverseY, bool bUpdateInstantly = false)
		{
			if (bUpdateInstantly)
			{
				Vector3 sScale = this.sNamedLayerTransform.localScale;
				sScale.y = nNewReverseY != 0.0f ? -Mathf.Abs(sScale.y) : Mathf.Abs(sScale.y);
				this.sNamedLayerTransform.localScale = sScale;
			}
			else
			{
				this.sLayerProperties.bReverseY = nNewReverseY != 0.0f;
				this.sLayerPropertiesDirty.bReverseDirty = true;
			}
		}

		public void setVisible(float nNewVisible, bool bUpdateInstantly = false)
		{
			if (bUpdateInstantly)
				this.sNamedLayer.SetActive(nNewVisible != 0f);
			else
			{
				this.sLayerProperties.bVisible = nNewVisible != 0f;
				this.sLayerPropertiesDirty.bVisibleDirty = true;
			}
		}

		public void deleteLayer()
		{
			this.removeLayerTweenAll();

			Layer.sLayerMap.Remove(this.sNamedLayer.name);
			Layer.sLayerList.Remove(this.sNamedLayer.name);
			GameObject.Destroy(this.sNamedLayer);
		}

		public virtual void applyLayerProperties()
		{
			//Apply sprite
			if (this.sLayerPropertiesDirty.bSpriteDirty)
			{
				this.sNamedLayerImage.material.SetTexture("_MainTex", this.sLayerProperties.sMainSprite == null ? null : this.sLayerProperties.sMainSprite.texture);
				this.sNamedLayerImage.material.SetTexture("_MaskTex", this.sLayerProperties.sMaskSprite == null ? null : this.sLayerProperties.sMaskSprite.texture);
				this.sNamedLayerImage.SetMaterialDirty();

				if (this.sLayerProperties.sMainSprite != null)
				{
					Vector2 sSize;
					sSize.x = this.sLayerProperties.sMainSprite.texture.width;
					sSize.y = this.sLayerProperties.sMainSprite.texture.height;

					this.sNamedLayerTransform.sizeDelta = sSize;
				}
				
				this.sLayerPropertiesDirty.bSpriteDirty = false;
			}

			//Apply position
			if (this.sLayerPropertiesDirty.bPositionDirty)
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
				sScale.x = Mathf.Sign(sScale.x) * Mathf.Abs(this.sLayerProperties.sScale.x);
				sScale.y = Mathf.Sign(sScale.y) * Mathf.Abs(this.sLayerProperties.sScale.y);

				this.sNamedLayerTransform.localScale = sScale;
				this.sLayerPropertiesDirty.bScaleDirty = false;
			}

			//Apply alpha
			if (this.sLayerPropertiesDirty.bAlphaDirty)
			{
				this.sNamedLayerImage.material.SetFloat("_Alpha", this.sLayerProperties.nAlpha);
				this.sLayerPropertiesDirty.bAlphaDirty = false;
			}

			//Apply rotation
			if (this.sLayerPropertiesDirty.bRotationDirty)
			{
				this.sNamedLayerTransform.localRotation = Quaternion.Euler(0f, 0f, this.sLayerProperties.nRotation);
				this.sLayerPropertiesDirty.bRotationDirty = false;
			}

			//Apply reverse
			if (this.sLayerPropertiesDirty.bReverseDirty)
			{
				Vector3 sScale = this.sNamedLayerTransform.localScale;
				sScale.x = this.sLayerProperties.bReverseX ? Mathf.Abs(sScale.x) : -Mathf.Abs(sScale.x);
				sScale.y = this.sLayerProperties.bReverseY ? Mathf.Abs(sScale.y) : -Mathf.Abs(sScale.y);

				this.sNamedLayerTransform.localScale = sScale;
				this.sLayerPropertiesDirty.bReverseDirty = false;
			}

			//Apply visible
			if (this.sLayerPropertiesDirty.bVisibleDirty)
			{
				this.sNamedLayer.SetActive(this.sLayerProperties.bVisible);
				this.sLayerPropertiesDirty.bVisibleDirty = false;
			}
		}

		public void markAsNeedUpdate()
		{
			Layer.sNeedUpdateLayerSet.Add(this);
		}
	}
}