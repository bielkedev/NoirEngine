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
		public Live2DModelUnity Model { get { return this.sLive2DModelUnity; } }
		public IEnumerable ExpressionEnumerable { get { return this.sExpressionMap; } }
		public IEnumerable MotionEnumerable { get { return this.sMotionMap; } }
		public MotionQueueManager ExpressionManager { get { return this.sExpressionManager; } }
		public MotionQueueManager MotionManager { get { return this.sMotionManager; } }

		private Live2DModelUnity sLive2DModelUnity;
		private Dictionary<string, AMotion> sExpressionMap = new Dictionary<string, AMotion>();
		private Dictionary<string, KeyValuePair<AMotion, AudioClip>[]> sMotionMap = new Dictionary<string, KeyValuePair<AMotion, AudioClip>[]>();
		private MotionQueueManager sExpressionManager = new MotionQueueManager();
		private MotionQueueManager sMotionManager = new MotionQueueManager();
		private L2DEyeBlink sEyeBlink = new L2DEyeBlink();
		private L2DPhysics sPhysics = new L2DPhysics();
		private L2DPose sPose = new L2DPose();

		public bool loadLiveCharacter(string sModelJsonFilePath)
		{
			string sDirectory = Path.GetDirectoryName(sModelJsonFilePath);

			ModelSettingJson sModelSetting;

			{
				TextAsset sModelJsonAsset = Resources.Load<TextAsset>(Path.Combine(sDirectory, Path.GetFileNameWithoutExtension(sModelJsonFilePath)));

				if (sModelJsonAsset == null)
					return false;

				sModelSetting = new ModelSettingJson(sModelJsonAsset.text);
				Resources.UnloadAsset(sModelJsonAsset);
			}

			string sValue;

			if (string.IsNullOrEmpty(sValue = sModelSetting.GetModelFile()))
				return false;

			{
				TextAsset sModelAsset = Resources.Load<TextAsset>(
					Path.Combine(
						sDirectory,
						Path.Combine(
							Path.GetDirectoryName(sValue),
							Path.GetFileName(sValue))));

				if (sModelAsset == null)
					return false;

				this.sLive2DModelUnity = Live2DModelUnity.loadModel(sModelAsset.bytes);
				Resources.UnloadAsset(sModelAsset);
			}

			for (int nIndex = 0, nTexLen = sModelSetting.GetTextureNum(); nIndex < nTexLen; ++nIndex)
			{
				Texture2D sTexture = Resources.Load<Texture2D>(
					Path.Combine(
						sDirectory,
						Path.Combine(
							Path.GetDirectoryName(sValue = sModelSetting.GetTextureFile(nIndex)),
							Path.GetFileNameWithoutExtension(sValue))));

				if (sTexture == null)
					return false;

				this.sLive2DModelUnity.setTexture(nIndex, sTexture);
			}

			for (int nIndex = 0, nExpLen = sModelSetting.GetExpressionNum(); nIndex < nExpLen; ++nIndex)
			{
				TextAsset sExpressionAsset = Resources.Load<TextAsset>(
					Path.Combine(
						sDirectory,
						Path.Combine(
							Path.GetDirectoryName(sValue = sModelSetting.GetExpressionFile(nIndex)),
							Path.GetFileNameWithoutExtension(sValue))));

				if (sExpressionAsset == null)
					return false;

				this.sExpressionMap.Add(sModelSetting.GetExpressionName(nIndex), L2DExpressionMotion.loadJson(sExpressionAsset.text));
				Resources.UnloadAsset(sExpressionAsset);
			}

			{
				string[] vMotionGroup = sModelSetting.GetMotionGroupNames();

				foreach (string sMotion in vMotionGroup)
				{
					int nMotionLen = sModelSetting.GetMotionNum(sMotion);
					KeyValuePair<AMotion, AudioClip>[] vMotionPair = new KeyValuePair<AMotion, AudioClip>[nMotionLen];

					for (int nIndex = 0; nIndex < nMotionLen; ++nIndex)
					{
						TextAsset sMotionAsset = Resources.Load<TextAsset>(
							Path.Combine(
								sDirectory,
								Path.Combine(
									Path.GetDirectoryName((sValue = sModelSetting.GetMotionFile(sMotion, nIndex))),
									Path.GetFileName(sValue))));

						if (sMotionAsset == null)
							return false;

						AudioClip sMotionAudioClipAsset = null;

						if (!string.IsNullOrEmpty(sValue = sModelSetting.GetMotionSound(sMotion, nIndex)))
						{
							sMotionAudioClipAsset = Resources.Load<AudioClip>(
								Path.Combine(
									sDirectory,
									Path.Combine(
										Path.GetDirectoryName(sValue),
										Path.GetFileNameWithoutExtension(sValue))));

							if (sMotionAudioClipAsset == null)
								return false;
						}

						vMotionPair[nIndex] = new KeyValuePair<AMotion, AudioClip>(Live2DMotion.loadMotion(sMotionAsset.bytes), sMotionAudioClipAsset);

						if(sModelSetting.ExistMotionFadeIn(sMotion, nIndex))
							vMotionPair[nIndex].Key.setFadeIn(sModelSetting.GetMotionFadeIn(sMotion, nIndex));

						if (sModelSetting.ExistMotionFadeOut(sMotion, nIndex))
							vMotionPair[nIndex].Key.setFadeOut(sModelSetting.GetMotionFadeIn(sMotion, nIndex));

						Resources.UnloadAsset(sMotionAsset);
					}

					this.sMotionMap.Add(sMotion, vMotionPair);
				}
			}

			if (!string.IsNullOrEmpty(sValue = sModelSetting.GetPhysicsFile()))
			{
				TextAsset sPhysicsAsset = Resources.Load<TextAsset>(Path.Combine(sDirectory, Path.GetFileNameWithoutExtension(sValue)));

				if (sPhysicsAsset == null)
					return false;

				this.sPhysics = L2DPhysics.load(sPhysicsAsset.text);
				Resources.UnloadAsset(sPhysicsAsset);
			}

			if (!string.IsNullOrEmpty(sValue = sModelSetting.GetPoseFile()))
			{
				TextAsset sPoseAsset = Resources.Load<TextAsset>(Path.Combine(sDirectory, Path.GetFileNameWithoutExtension(sValue)));

				if (sPoseAsset == null)
					return false;

				this.sPose = L2DPose.load(sPoseAsset.text);
				Resources.UnloadAsset(sPoseAsset);
			}

			for (int nIndex = 0, nParamLen = sModelSetting.GetInitParamNum(); nIndex < nParamLen; ++nIndex)
				this.sLive2DModelUnity.setParamFloat(sModelSetting.GetInitParamID(nIndex), sModelSetting.GetInitParamValue(nIndex));

			for (int nIndex = 0, nVisibleLen = sModelSetting.GetInitPartsVisibleNum(); nIndex < nVisibleLen; ++nIndex)
				this.sLive2DModelUnity.setPartsOpacity(sModelSetting.GetInitPartsVisibleID(nIndex), sModelSetting.GetInitPartsVisibleValue(nIndex));

			return true;
		}
		
		public void idleExpression()
		{
			this.sExpressionManager.stopAllMotions();
		}

		public bool startExpression(string sExpressionName)
		{
			AMotion sExpression;

			if (this.sExpressionMap.TryGetValue(sExpressionName, out sExpression))
			{
				this.sExpressionManager.startMotion(sExpression);
				return true;
			}

			return false;
		}

		public bool startMotion(string sMotionName, out AudioClip sAudioClip)
		{
			KeyValuePair<AMotion, AudioClip>[] vMotionPair;

			if (this.sMotionMap.TryGetValue(sMotionName, out vMotionPair) && vMotionPair.Length >= 1)
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