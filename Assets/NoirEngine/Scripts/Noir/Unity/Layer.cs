using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Noir.Unity
{
	public abstract class Layer : IWaitableObject
	{
		public static RectTransform LayerPanel { set { Layer.sLayerPanel = value; } }
		public static Material LayerMaterial { set { Layer.sLayerMaterial = value; } }
		public static IEnumerable<KeyValuePair<string, Layer>> LayerEnumerable { get { return Layer.sLayerMap; } }
		public static IEnumerable<Layer> NeedUpdateLayerEnumerable { get { return Layer.sNeedUpdateLayerSet; } }

		protected static RectTransform sLayerPanel;
		protected static Material sLayerMaterial;
		protected static Dictionary<string, Layer> sLayerMap = new Dictionary<string, Layer>();
		protected static SortedList<string, Layer> sLayerList = new SortedList<string, Layer>();
		protected static HashSet<Layer> sNeedUpdateLayerSet = new HashSet<Layer>();

		public Vector2 Position { get { return this.sLayerState.sPosition; } }
		public Vector2 Pivot { get { return this.sLayerState.sPivot; } }
		public Vector2 Scale { get { return this.sLayerState.sScale; } }
		public float Alpha { get { return this.sLayerState.nAlpha; } }
		public float Rotation { get { return this.sLayerState.nRotation; } }
		public bool ReverseX { get { return this.sLayerState.bReverseX; } }
		public bool ReverseY { get { return this.sLayerState.bReverseY; } }
		public bool Visiblility { get { return this.sLayerState.bVisible; } }
		public string LayerName { get { return this.sLayerObject.name; } }
		public RectTransform Transform { get { return this.sLayerTransform; } }
		public LayerClipper Clipper { get { return this.sLayerClipper; } }

		protected GameObject sLayerObject;
		protected RectTransform sLayerTransform;
		protected RawImage sLayerImage;
		protected LayerClipper sLayerClipper;
		protected LayerState sLayerState = LayerState.Identity;
		protected LayerStateDirty sLayerStateDirty;
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

		public static void setPosX(Layer sLayer, float nValue, bool bUpdateInstantly)
		{
			sLayer.setPosX(nValue, bUpdateInstantly);
		}

		public static void setPosY(Layer sLayer, float nValue, bool bUpdateInstantly)
		{
			sLayer.setPosY(nValue, bUpdateInstantly);
		}

		public static void setAlpha(Layer sLayer, float nValue, bool bUpdateInstantly)
		{
			sLayer.setAlpha(nValue, bUpdateInstantly);
		}

		public static void setScaleX(Layer sLayer, float nValue, bool bUpdateInstantly)
		{
			sLayer.setScaleX(nValue, bUpdateInstantly);
		}

		public static void setScaleY(Layer sLayer, float nValue, bool bUpdateInstantly)
		{
			sLayer.setScaleY(nValue, bUpdateInstantly);
		}

		public static void setRotate(Layer sLayer, float nValue, bool bUpdateInstantly)
		{
			sLayer.setRotate(nValue, bUpdateInstantly);
		}

		protected Layer(Layer sLayer)
		{
			GameObject sClipperObject = GameObject.Instantiate(sLayer.sLayerClipper.Object);

			foreach (Transform sTransform in sClipperObject.transform)
				this.sLayerObject = sTransform.gameObject;
			
			this.sLayerTransform = this.sLayerObject.GetComponent<RectTransform>();
			this.sLayerImage = this.sLayerObject.GetComponent<RawImage>();
			this.sLayerClipper = new LayerClipper(sClipperObject, this);

			foreach (LayerTween sLayerTween in this.sLayerObject.GetComponents<LayerTween>())
			{
				sLayerTween.StopAllCoroutines();
				GameObject.Destroy(sLayerTween);
			}

			Layer.sLayerMap.Add(this.sLayerObject.name = sLayer.LayerName + '\n', this);
			Layer.sLayerList.Add(this.sLayerObject.name, this);

			this.sLayerClipper.Transform.SetParent(Layer.sLayerPanel, false);
			this.sLayerClipper.Transform.SetSiblingIndex(Layer.sLayerList.IndexOfKey(this.sLayerObject.name));
		}

		protected Layer(string sLayerName, GameObject sLayerPrefab)
		{
			this.sLayerTransform = (this.sLayerObject = GameObject.Instantiate(sLayerPrefab)).GetComponent<RectTransform>();
			(this.sLayerImage = this.sLayerObject.GetComponent<RawImage>()).material = GameObject.Instantiate(Layer.sLayerMaterial);
			this.sLayerClipper = new LayerClipper(this, this.sLayerState.bClipping);

			Layer.sLayerMap.Add(this.sLayerObject.name = sLayerName, this);
			Layer.sLayerList.Add(this.sLayerObject.name, this);

			this.sLayerClipper.Transform.SetParent(Layer.sLayerPanel, false);
			this.sLayerClipper.Transform.SetSiblingIndex(Layer.sLayerList.IndexOfKey(this.sLayerObject.name));
		}

		public void addLayerTween(ref LayerTweenData sNewLayerTweenData, LayerStateModifier fNewStateModifier)
		{
			LayerTween sTween = this.sLayerObject.AddComponent<LayerTween>();
			sTween._Layer = this;
			sTween._LayerTweenData = sNewLayerTweenData;
			sTween._LayerStateModifier = fNewStateModifier;
			sTween.StartCoroutine(sTween.runTween());

			this.sLayerTweenList.Add(sTween);
		}

		public void removeLayerTween(LayerTween sLayerTween)
		{
			if(sLayerTween != null)
			{
				sLayerTween.StopAllCoroutines();
				this.sLayerTweenList.Remove(sLayerTween);
				GameObject.Destroy(sLayerTween);
			}
		}

		public void removeLayerTweenAll()
		{
			foreach (var sTween in this.sLayerTweenList)
				if (sTween != null)
				{
					sTween.StopAllCoroutines();
					GameObject.Destroy(sTween);
				}

			this.sLayerTweenList.Clear();
		}

		public void setMask(Sprite sNewMask, bool bUpdateInstantly = false)
		{
			if (bUpdateInstantly)
			{
				this.sLayerImage.material.SetTexture("_MaskTex", (this.sLayerState.sMask = sNewMask) == null ? null : this.sLayerState.sMask.texture);
				this.sLayerImage.SetMaterialDirty();
			}
			else
			{
				this.sLayerState.sMask = sNewMask;
				this.sLayerStateDirty.bMask = true;
			}
		}

		public void setPosX(float nNewX, bool bUpdateInstantly = false)
		{
			if (bUpdateInstantly)
			{
				Vector3 sPosition = this.sLayerTransform.localPosition;
				this.sLayerState.sPosition.x = sPosition.x = nNewX;
				this.sLayerTransform.localPosition = sPosition;
			}
			else
			{
				this.sLayerState.sPosition.x = nNewX;
				this.sLayerStateDirty.bPosition = true;
			}
		}

		public void setPosY(float nNewY, bool bUpdateInstantly = false)
		{
			if (bUpdateInstantly)
			{
				Vector3 sPosition = this.sLayerTransform.localPosition;
				this.sLayerState.sPosition.y = sPosition.y = nNewY;
				this.sLayerTransform.localPosition = sPosition;
			}
			else
			{
				this.sLayerState.sPosition.y = nNewY;
				this.sLayerStateDirty.bPosition = true;
			}
		}

		public void setAlpha(float nAlpha, bool bUpdateInstantly = false)
		{
			if (bUpdateInstantly)
				this.sLayerImage.material.SetFloat("_Alpha", this.sLayerState.nAlpha = nAlpha / 255f);
			else
			{
				this.sLayerState.nAlpha = nAlpha / 255f;
				this.sLayerStateDirty.bAlpha = true;
			}
		}

		public void setAnchorX(float nNewAnchorX, bool bUpdateInstantly = false)
		{
			if (bUpdateInstantly)
			{
				Vector2 sPivot = this.sLayerTransform.pivot;
				this.sLayerState.sPivot.x = sPivot.x = nNewAnchorX;
				this.sLayerTransform.pivot = sPivot;
			}
			else
			{
				this.sLayerState.sPivot.x = nNewAnchorX;
				this.sLayerStateDirty.bPivot = true;
			}
		}

		public void setAnchorY(float nNewAnchorY, bool bUpdateInstantly = false)
		{
			if (bUpdateInstantly)
			{
				Vector2 sPivot = this.sLayerTransform.pivot;
				this.sLayerState.sPivot.y = sPivot.y = nNewAnchorY;
				this.sLayerTransform.pivot = sPivot;
			}
			else
			{
				this.sLayerState.sPivot.y = nNewAnchorY;
				this.sLayerStateDirty.bPivot = true;
			}
		}

		public void setScaleX(float nNewScaleX, bool bUpdateInstantly = false)
		{
			if (bUpdateInstantly)
			{
				Vector3 sScale = this.sLayerTransform.localScale;
				this.sLayerState.sScale.x = sScale.x = Mathf.Sign(sScale.x) * Mathf.Abs(nNewScaleX / 100f);
				this.sLayerTransform.localScale = sScale;
			}
			else
			{
				this.sLayerState.sScale.x = nNewScaleX / 100f;
				this.sLayerStateDirty.bScale = true;
			}
		}

		public void setScaleY(float nNewScaleY, bool bUpdateInstantly = false)
		{
			if (bUpdateInstantly)
			{
				Vector3 sScale = this.sLayerTransform.localScale;
				this.sLayerState.sScale.y = sScale.y = Mathf.Sign(sScale.y) * Mathf.Abs(nNewScaleY / 100f);
				this.sLayerTransform.localScale = sScale;
			}
			else
			{
				this.sLayerState.sScale.y = nNewScaleY / 100f;
				this.sLayerStateDirty.bScale = true;
			}
		}

		public void setRotate(float nNewAngle, bool bUpdateInstantly = false)
		{
			if (bUpdateInstantly)
				this.sLayerTransform.localRotation = Quaternion.Euler(0f, 0f, this.sLayerState.nRotation = nNewAngle);
			else
			{
				this.sLayerState.nRotation = nNewAngle;
				this.sLayerStateDirty.bRotation = true;
			}
		}

		public void setReverseX(float nNewReverseX, bool bUpdateInstantly = false)
		{
			if (bUpdateInstantly)
			{
				Vector3 sScale = this.sLayerTransform.localScale;
				sScale.x = (this.sLayerState.bReverseX = nNewReverseX != 0.0f) ? -Mathf.Abs(sScale.x) : Mathf.Abs(sScale.x);
				this.sLayerTransform.localScale = sScale;
			}
			else
			{
				this.sLayerState.bReverseX = nNewReverseX != 0.0f;
				this.sLayerStateDirty.bReverse = true;
			}
		}

		public void setReverseY(float nNewReverseY, bool bUpdateInstantly = false)
		{
			if (bUpdateInstantly)
			{
				Vector3 sScale = this.sLayerTransform.localScale;
				sScale.y = (this.sLayerState.bReverseY = nNewReverseY != 0.0f) ? -Mathf.Abs(sScale.y) : Mathf.Abs(sScale.y);
				this.sLayerTransform.localScale = sScale;
			}
			else
			{
				this.sLayerState.bReverseY = nNewReverseY != 0.0f;
				this.sLayerStateDirty.bReverse = true;
			}
		}

		public void setClipping(bool bNewClipping, bool bUpdateInstantly = false)
		{
			if (bUpdateInstantly)
				this.sLayerState.bClipping = this.sLayerClipper.Enabled = bNewClipping;
			else
			{
				this.sLayerState.bClipping = bNewClipping;
				this.sLayerStateDirty.bClipping = true;
			}
		}

		public void setClipper(Rect sNewClipper, bool bUpdateInstantly = false)
		{
			if (bUpdateInstantly)
				this.sLayerClipper.setRegion(this.sLayerState.sClipper = sNewClipper);
			else
			{
				this.sLayerState.sClipper = sNewClipper;
				this.sLayerStateDirty.bClipping = true;
			}
		}

		public void setVisible(float nNewVisible, bool bUpdateInstantly = false)
		{
			if (bUpdateInstantly)
				this.sLayerObject.SetActive(this.sLayerState.bVisible = nNewVisible != 0f);
			else
			{
				this.sLayerState.bVisible = nNewVisible != 0f;
				this.sLayerStateDirty.bVisible = true;
			}
		}

		public void deleteLayer()
		{
			this.removeLayerTweenAll();

			Layer.sLayerMap.Remove(this.sLayerObject.name);
			Layer.sLayerList.Remove(this.sLayerObject.name);

			this.sLayerClipper.deleteLayerClipper();
		}

		public virtual void applyLayerProperties()
		{
			//Apply mask
			if (this.sLayerStateDirty.bMask)
			{
				this.sLayerImage.material.SetTexture("_MaskTex", this.sLayerState.sMask == null ? null : this.sLayerState.sMask.texture);
				this.sLayerImage.SetMaterialDirty();
				this.sLayerStateDirty.bMask = false;
			}

			//Apply position
			if (this.sLayerStateDirty.bPosition)
			{
				this.sLayerTransform.localPosition = this.sLayerState.sPosition;
				this.sLayerStateDirty.bPosition = false;
			}

			//Apply anchor
			if (this.sLayerStateDirty.bPivot)
			{
				this.sLayerTransform.pivot = this.sLayerState.sPivot;
				this.sLayerStateDirty.bPivot = false;
			}

			//Apply scale
			if (this.sLayerStateDirty.bScale)
			{
				Vector3 sScale = this.sLayerTransform.localScale;
				sScale.x = Mathf.Sign(sScale.x) * Mathf.Abs(this.sLayerState.sScale.x);
				sScale.y = Mathf.Sign(sScale.y) * Mathf.Abs(this.sLayerState.sScale.y);

				this.sLayerTransform.localScale = sScale;
				this.sLayerStateDirty.bScale = false;
			}

			//Apply alpha
			if (this.sLayerStateDirty.bAlpha)
			{
				this.sLayerImage.material.SetFloat("_Alpha", this.sLayerState.nAlpha);
				this.sLayerStateDirty.bAlpha = false;
			}

			//Apply rotation
			if (this.sLayerStateDirty.bRotation)
			{
				this.sLayerTransform.localRotation = Quaternion.Euler(0f, 0f, this.sLayerState.nRotation);
				this.sLayerStateDirty.bRotation = false;
			}

			//Apply reverse
			if (this.sLayerStateDirty.bReverse)
			{
				Vector3 sScale = this.sLayerTransform.localScale;
				sScale.x = this.sLayerState.bReverseX ? Mathf.Abs(sScale.x) : -Mathf.Abs(sScale.x);
				sScale.y = this.sLayerState.bReverseY ? Mathf.Abs(sScale.y) : -Mathf.Abs(sScale.y);

				this.sLayerTransform.localScale = sScale;
				this.sLayerStateDirty.bReverse = false;
			}

			//Apply clipping
			if (this.sLayerStateDirty.bClipping)
			{
				this.sLayerClipper.Enabled = this.sLayerState.bClipping;
				this.sLayerClipper.setRegion(this.sLayerState.sClipper);
				this.sLayerStateDirty.bClipping = false;
			}

			//Apply visible
			if (this.sLayerStateDirty.bVisible)
			{
				this.sLayerObject.SetActive(this.sLayerState.bVisible);
				this.sLayerStateDirty.bVisible = false;
			}
		}

		public void markAsNeedUpdate()
		{
			Layer.sNeedUpdateLayerSet.Add(this);
		}

		bool IWaitableObject.isComplete()
		{
			return this.sLayerTweenList.Count == 0;
		}
	}
}