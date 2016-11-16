using UnityEngine;
using UnityEngine.UI;

namespace Noir.Unity.Live2D
{
	[RequireComponent(typeof(AudioSource), typeof(RawImage))]
	public class Live2DController : MonoBehaviour
	{
		private static Live2DController sCurrentRenderingController = null;

		[Header("Live2D Model")]
		public string _Live2DModelJsonPath;
		public string _IdleMotionName;

		[Header("Render")]
		public int _ResolutionWidth;
		public int _ResolutionHeight;
		public Camera _RenderCamera;
		public Material _RawImageMaterial;

		public string CurrentExpressionName { get { return this.sCurrentExpressionName; } }
		public string CurrentMotionName { get { return this.sCurrentMotionName; } }
		public bool IsCurrentExpressionLoop { get { return this.bExpressionLoop; } }
		public bool IsCurrentMotionLoop { get { return this.bMotionLoop; } }

		private string sCurrentExpressionName;
		private string sCurrentMotionName;
		private bool bExpressionLoop;
		private bool bMotionLoop;
		private AudioSource sAudioSource;
		private RawImage sRawImage;
		private RenderTexture sRenderTexture;
		private Live2DCharacter sLive2DCharacter = new Live2DCharacter();

		private void Awake()
		{
			this.sAudioSource = this.gameObject.GetComponent<AudioSource>();
			this.sRawImage = this.gameObject.GetComponent<RawImage>();
			this.sRenderTexture = RenderTexture.GetTemporary(this._ResolutionWidth, this._ResolutionHeight);
		}

		public bool initializeController()
		{
			if (!this.sLive2DCharacter.loadLiveCharacter(this._Live2DModelJsonPath))
				return false;

			this.sRawImage.material = GameObject.Instantiate<Material>(_RawImageMaterial);

			this.startIdleExpression();
			this.startIdleMotion();

			return true;
		}

		private void Update()
		{
			if (this.sLive2DCharacter.Model == null)
				return;

			if (this.sLive2DCharacter.ExpressionManager.isFinished())
			{
				if (this.bExpressionLoop)
					this.startExpression(this.sCurrentExpressionName, true);
				else
					this.startIdleExpression();
			}

			if (this.sLive2DCharacter.MotionManager.isFinished())
			{
				if (this.bMotionLoop)
					this.startMotion(this.sCurrentMotionName, true);
				else
					this.startIdleMotion();
			}

			this.sLive2DCharacter.updateCharacter();
		}

		private void OnRenderObject()
		{
			if (this.sLive2DCharacter.Model == null)
				return;

			if (Camera.current == this._RenderCamera)
			{
				if (sCurrentRenderingController != this)
					return;

				float nCameraHeight = this._RenderCamera.orthographicSize * 2f;
				float nCanvasHeight = this.sLive2DCharacter.Model.getCanvasHeight();

				Vector3 sSize = Vector3.one;
				sSize.x = nCameraHeight / nCanvasHeight * this._RenderCamera.aspect;
				sSize.y = -nCameraHeight / nCanvasHeight;

				Vector3 sLocation = Vector3.zero;
				sLocation.x = -this._RenderCamera.orthographicSize * this._RenderCamera.aspect * this.sLive2DCharacter.Model.getCanvasWidth() / nCanvasHeight;
				sLocation.y = this._RenderCamera.orthographicSize;

				this.sLive2DCharacter.Model.setMatrix(Matrix4x4.TRS(sLocation, Quaternion.identity, sSize));
				this.sLive2DCharacter.Model.draw();
			}
			else
			{
				sCurrentRenderingController = this;

				this._RenderCamera.targetTexture = this.sRenderTexture;
				this._RenderCamera.Render();

				sCurrentRenderingController = null;

				this.sRawImage.material.mainTexture = this.sRenderTexture;
				this.sRawImage.SetMaterialDirty();
			}
		}

		private void OnDestroy()
		{
			RenderTexture.ReleaseTemporary(this.sRenderTexture);
		}

		public void startIdleExpression()
		{
			this.startExpression(null, true);
		}

		public void startIdleMotion()
		{
			this.startMotion(this._IdleMotionName, true);
		}

		public bool startExpression(string sExpressionName, bool bLoop)
		{
			this.bExpressionLoop = bLoop;

			if ((this.sCurrentExpressionName = sExpressionName) == null)
			{
				this.sLive2DCharacter.idleExpression();
				return true;
			}

			return this.sLive2DCharacter.startExpression(this.sCurrentExpressionName);
		}

		public bool startMotion(string sMotionName, bool bLoop)
		{
			AudioClip sAudioClip;
			bool bResult = this.sLive2DCharacter.startMotion(this.sCurrentMotionName = sMotionName, out sAudioClip);

			if (sAudioClip != null)
			{
				this.sAudioSource.Stop();
				this.sAudioSource.PlayOneShot(sAudioClip);
			}

			this.bMotionLoop = bLoop;

			return bResult;
		}
	}
}