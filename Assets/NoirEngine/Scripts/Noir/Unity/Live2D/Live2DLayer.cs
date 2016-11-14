using Noir.Unity.Live2D;
using UnityEngine;

namespace Noir.Unity.Live2D
{
	public class Live2DLayer : Layer
	{
		public static GameObject NamedLive2DLayerPrefab { set { Live2DLayer.sNamedLive2DLayerPrefab = value; } }
		public static Camera RenderCamera { set { Live2DLayer.sRenderCamera = value; } }

		private static GameObject sNamedLive2DLayerPrefab;
		private static Camera sRenderCamera;

		public Live2DController Controller { get { return this.sLive2DController; } }

		private Live2DController sLive2DController;

		public Live2DLayer(string sLayerName) : base(sLayerName, Live2DLayer.sNamedLive2DLayerPrefab)
		{
			(this.sLive2DController = this.sNamedLayer.GetComponent<Live2DController>())._RenderCamera = Live2DLayer.sRenderCamera;
			this.sLive2DController._RawImageMaterial = Layer.sNamedLayerMaterial;
		}

		public override void setLayerSprite(Sprite sMainSprite, Sprite sMaskSprite, bool bUpdateInstantly = false)
		{
			return;
		}

		public void setLayerSprite(string sNewLive2DModelJsonPath, string sNewIdleMotionName, Sprite sMaskSprite, bool bUpdateInstantly = false)
		{
			this.sLive2DController._Live2DModelJsonPath = sNewLive2DModelJsonPath;
			this.sLive2DController._IdleMotionName = sNewIdleMotionName;
			this.sLive2DController.initializeController();

			if (bUpdateInstantly)
			{
				this.sNamedLayerImage.material.SetTexture("_MaskTex", sMaskSprite == null ? null : sMaskSprite.texture);
				this.sNamedLayerImage.SetMaterialDirty();
			}
			else
			{
				this.sLayerProperties.sMaskSprite = sMaskSprite;
				this.sLayerPropertiesDirty.bSpriteDirty = true;
			}
		}

		public override void applyLayerProperties()
		{
			//Apply sprite
			if (this.sLayerPropertiesDirty.bSpriteDirty)
			{
				this.sNamedLayerImage.material.SetTexture("_MaskTex", this.sLayerProperties.sMaskSprite == null ? null : this.sLayerProperties.sMaskSprite.texture);
				this.sNamedLayerImage.SetMaterialDirty();
				
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
	}
}