using UnityEngine;

namespace Noir.Unity
{
	public class SpriteLayer : Layer
	{
		public static GameObject SpriteLayerPrefab { set { SpriteLayer.sSpriteLayerPrefab = value; } }

		private static GameObject sSpriteLayerPrefab;

		private Sprite sSprite;
		private bool bSpriteDirty;

		public SpriteLayer(string sLayerName) : base(sLayerName, SpriteLayer.sSpriteLayerPrefab)
		{
			//Empty.
		}

		public void setSprite(Sprite sNewSprite, bool bUpdateInstantly = false)
		{
			if (bUpdateInstantly)
			{
				this.sLayerImage.material.SetTexture("_MainTex", (this.sSprite = sNewSprite) == null ? null : this.sSprite.texture);
				this.sLayerImage.SetMaterialDirty();
			}
			else
			{
				this.sSprite = sNewSprite;
				this.bSpriteDirty = true;
			}
		}

		public override void applyLayerProperties()
		{
			base.applyLayerProperties();

			//Apply sprite
			if (this.bSpriteDirty)
			{
				this.sLayerImage.material.SetTexture("_MainTex", this.sSprite == null ? null : this.sSprite.texture);
				this.sLayerImage.SetMaterialDirty();
			}
		}
	}
}