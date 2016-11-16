using live2d;
using live2d.framework;
using live2d.util;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Noir.Unity.Live2D
{
	public class Live2DCharacter
	{
		public Live2DSharedData SharedData { get { return this.sLive2DSharedData; } }
		public Live2DModelUnity Model { get { return this.sLive2DModelUnity; } }
		public MotionQueueManager ExpressionManager { get { return this.sExpressionManager; } }
		public MotionQueueManager MotionManager { get { return this.sMotionManager; } }

		private Live2DSharedData sLive2DSharedData;
		private Live2DModelUnity sLive2DModelUnity;
		private MotionQueueManager sExpressionManager = new MotionQueueManager();
		private MotionQueueManager sMotionManager = new MotionQueueManager();
		private L2DEyeBlink sEyeBlink = new L2DEyeBlink();
		private L2DPhysics sPhysics;
		private L2DPose sPose;

		public bool loadLiveCharacter(Live2DSharedData sNewLive2DSharedData)
		{
			if (sNewLive2DSharedData.ModelSettingJson == null || sNewLive2DSharedData.ModelAsset == null)
				return false;

			this.sLive2DSharedData = sNewLive2DSharedData;
			this.sLive2DModelUnity = Live2DModelUnity.loadModel(this.sLive2DSharedData.ModelAsset.bytes);

			{
				int nIndex = 0;

				foreach (var sTexture in this.sLive2DSharedData.Texture)
				{
					if (sTexture != null)
						this.sLive2DModelUnity.setTexture(nIndex, sTexture);

					++nIndex;
				}
			}

			this.sPhysics = this.sLive2DSharedData.PhysicsAsset == null ? new L2DPhysics() : L2DPhysics.load(this.sLive2DSharedData.PhysicsAsset.text);
			this.sPose = this.sLive2DSharedData.PoseAsset == null ? new L2DPose() : L2DPose.load(this.sLive2DSharedData.PoseAsset.text);

			for (int nIndex = 0, nParamLen = this.sLive2DSharedData.ModelSettingJson.GetInitParamNum(); nIndex < nParamLen; ++nIndex)
				this.sLive2DModelUnity.setParamFloat(this.sLive2DSharedData.ModelSettingJson.GetInitParamID(nIndex), this.sLive2DSharedData.ModelSettingJson.GetInitParamValue(nIndex));

			for (int nIndex = 0, nVisibleLen = this.sLive2DSharedData.ModelSettingJson.GetInitPartsVisibleNum(); nIndex < nVisibleLen; ++nIndex)
				this.sLive2DModelUnity.setPartsOpacity(this.sLive2DSharedData.ModelSettingJson.GetInitPartsVisibleID(nIndex), this.sLive2DSharedData.ModelSettingJson.GetInitPartsVisibleValue(nIndex));

			return true;
		}
		
		public void idleExpression()
		{
			this.sExpressionManager.stopAllMotions();
		}

		public bool startExpression(string sExpressionName)
		{
			AMotion sExpression;

			if (this.sLive2DSharedData.Expression.TryGetValue(sExpressionName, out sExpression))
			{
				this.sExpressionManager.startMotion(sExpression);
				return true;
			}

			return false;
		}

		public bool startMotion(string sMotionName, out AudioClip sAudioClip)
		{
			KeyValuePair<AMotion, AudioClip>[] vMotionPair;

			if (this.sLive2DSharedData.Motion.TryGetValue(sMotionName, out vMotionPair) && vMotionPair.Length >= 1)
			{
				KeyValuePair<AMotion, AudioClip> sMotionPair = vMotionPair[Random.Range(0, vMotionPair.Length)];
				sAudioClip = sMotionPair.Value;

				this.sMotionManager.startMotion(sMotionPair.Key);

				return true;
			}

			sAudioClip = null;

			return false;
		}

		public void updateCharacter()
		{
			this.sExpressionManager.updateParam(this.sLive2DModelUnity);
			this.sMotionManager.updateParam(this.sLive2DModelUnity);
			this.sEyeBlink.updateParam(this.sLive2DModelUnity);
			this.sPhysics.updateParam(this.sLive2DModelUnity);
			this.sPose.updateParam(this.sLive2DModelUnity);

			this.sLive2DModelUnity.update();
		}
	}
}