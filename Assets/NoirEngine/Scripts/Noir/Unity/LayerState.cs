using UnityEngine;

namespace Noir.Unity
{
	public struct LayerState
	{
		public static LayerState Identity { get { return LayerState.sIdentity; } }

		private static LayerState sIdentity = new LayerState(
			Vector2.zero,
			new Vector2(.5f, .5f),
			Vector2.one,
			1f,
			0f,
			false,
			false,
			false,
			true,
			Rect.MinMaxRect(0f, 0f, 0f, 0f));

		public Vector2 sPosition;
		public Vector2 sPivot;
		public Vector2 sScale;
		public float nAlpha;
		public float nRotation;
		public bool bReverseX;
		public bool bReverseY;
		public bool bClipping;
		public bool bVisible;
		public Rect sClip;

		public LayerState(
			Vector2 sNewPosition,
			Vector2 sNewPivot,
			Vector2 sNewScale,
			float nNewAlpha,
			float nNewRotation,
			bool bNewReverseX,
			bool bNewReverseY,
			bool bNewClipping,
			bool bNewVisible,
			Rect sNewClip)
		{
			this.sPosition = sNewPosition;
			this.sPivot = sNewPivot;
			this.sScale = sNewScale;
			this.nAlpha = nNewAlpha;
			this.nRotation = nNewRotation;
			this.bReverseX = bNewReverseX;
			this.bReverseY = bNewReverseY;
			this.bClipping = bNewClipping;
			this.bVisible = bNewVisible;
			this.sClip = sNewClip;
		}
	}
}