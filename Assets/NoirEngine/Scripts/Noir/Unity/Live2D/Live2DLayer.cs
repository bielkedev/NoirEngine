﻿using UnityEngine;

namespace Noir.Unity.Live2D
{
	public class Live2DLayer : Layer
	{
		public static GameObject Live2DLayerPrefab { set { Live2DLayer.sLive2DLayerPrefab = value; } }
		public static Camera RenderCamera { set { Live2DLayer.sRenderCamera = value; } }

		private static GameObject sLive2DLayerPrefab;
		private static Camera sRenderCamera;

		public Live2DController Controller { get { return this.sLive2DController; } }
		
		private Live2DController sLive2DController;

		public Live2DLayer(string sLayerName) : base(sLayerName, Live2DLayer.sLive2DLayerPrefab)
		{
			(this.sLive2DController = this.sLayerObject.GetComponent<Live2DController>())._RenderCamera = Live2DLayer.sRenderCamera;
		}

		public bool setModel(string sNewLive2DModelJsonPath, string sNewIdleMotionName)
		{
			this.sLive2DController._Live2DModelJsonPath = sNewLive2DModelJsonPath;
			this.sLive2DController._IdleMotionName = sNewIdleMotionName;

			return this.sLive2DController.initializeController();
		}
	}
}