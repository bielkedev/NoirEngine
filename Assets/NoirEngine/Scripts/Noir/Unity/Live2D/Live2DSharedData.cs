using live2d;
using live2d.framework;
using live2d.util;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Noir.Unity.Live2D
{
	public class Live2DSharedData
	{
		public ModelSettingJson ModelSettingJson { get { return this.sModelSettingJson; } }
		public TextAsset ModelAsset { get { return this.sModelAsset; } }
		public TextAsset PhysicsAsset { get { return this.sPhysicsAsset; } }
		public TextAsset PoseAsset { get { return this.sPoseAsset; } }
		public Texture2D[] Texture { get { return this.vTexture; } }
		public Dictionary<string, AMotion> Expression { get { return this.sExpressionMap; } }
		public Dictionary<string, KeyValuePair<AMotion, AudioClip>[]> Motion { get { return this.sMotionMap; } }

		private ModelSettingJson sModelSettingJson;
		private TextAsset sModelAsset;
		private TextAsset sPhysicsAsset;
		private TextAsset sPoseAsset;
		private Texture2D[] vTexture;
		private Dictionary<string, AMotion> sExpressionMap = new Dictionary<string, AMotion>();
		private Dictionary<string, KeyValuePair<AMotion, AudioClip>[]> sMotionMap = new Dictionary<string, KeyValuePair<AMotion, AudioClip>[]>();

		public Live2DSharedData(string sModelJsonFilePath)
		{
			string sDirectory = Path.GetDirectoryName(sModelJsonFilePath);

			{
				TextAsset sModelJsonAsset = Resources.Load<TextAsset>(Path.Combine(sDirectory, Path.GetFileNameWithoutExtension(sModelJsonFilePath)));

				if (sModelJsonAsset == null)
					return;

				this.sModelSettingJson = new ModelSettingJson(sModelJsonAsset.text);
				Resources.UnloadAsset(sModelJsonAsset);
			}

			string sValue;

			if (string.IsNullOrEmpty(sValue = this.sModelSettingJson.GetModelFile()))
				return;

			this.sModelAsset = Resources.Load<TextAsset>(
					Path.Combine(
						sDirectory,
						Path.Combine(
							Path.GetDirectoryName(sValue),
							Path.GetFileName(sValue))));

			if (this.sModelAsset == null)
				return;

			this.vTexture = new Texture2D[this.sModelSettingJson.GetTextureNum()];

			for (int nIndex = 0; nIndex < this.vTexture.Length; ++nIndex)
			{
				Texture2D sTexture = Resources.Load<Texture2D>(
					Path.Combine(
						sDirectory,
						Path.Combine(
							Path.GetDirectoryName(sValue = this.sModelSettingJson.GetTextureFile(nIndex)),
							Path.GetFileNameWithoutExtension(sValue))));

				this.vTexture[nIndex] = sTexture;
			}

			for (int nIndex = 0, nExpLen = this.sModelSettingJson.GetExpressionNum(); nIndex < nExpLen; ++nIndex)
			{
				TextAsset sExpressionAsset = Resources.Load<TextAsset>(
					Path.Combine(
						sDirectory,
						Path.Combine(
							Path.GetDirectoryName(sValue = this.sModelSettingJson.GetExpressionFile(nIndex)),
							Path.GetFileNameWithoutExtension(sValue))));

				if (sExpressionAsset == null)
					continue;

				this.sExpressionMap.Add(this.sModelSettingJson.GetExpressionName(nIndex), L2DExpressionMotion.loadJson(sExpressionAsset.text));
				Resources.UnloadAsset(sExpressionAsset);
			}

			{
				string[] vMotionGroup = this.sModelSettingJson.GetMotionGroupNames();

				foreach (string sMotion in vMotionGroup)
				{
					int nMotionLen = this.sModelSettingJson.GetMotionNum(sMotion);
					KeyValuePair<AMotion, AudioClip>[] vMotionPair = new KeyValuePair<AMotion, AudioClip>[nMotionLen];

					for (int nIndex = 0; nIndex < nMotionLen; ++nIndex)
					{
						TextAsset sMotionAsset = Resources.Load<TextAsset>(
							Path.Combine(
								sDirectory,
								Path.Combine(
									Path.GetDirectoryName((sValue = this.sModelSettingJson.GetMotionFile(sMotion, nIndex))),
									Path.GetFileName(sValue))));

						if (sMotionAsset == null)
							continue;

						AudioClip sMotionAudioClipAsset = null;

						if (!string.IsNullOrEmpty(sValue = this.sModelSettingJson.GetMotionSound(sMotion, nIndex)))
							sMotionAudioClipAsset = Resources.Load<AudioClip>(
								Path.Combine(
									sDirectory,
									Path.Combine(
										Path.GetDirectoryName(sValue),
										Path.GetFileNameWithoutExtension(sValue))));

						vMotionPair[nIndex] = new KeyValuePair<AMotion, AudioClip>(Live2DMotion.loadMotion(sMotionAsset.bytes), sMotionAudioClipAsset);

						if (this.sModelSettingJson.ExistMotionFadeIn(sMotion, nIndex))
							vMotionPair[nIndex].Key.setFadeIn(this.sModelSettingJson.GetMotionFadeIn(sMotion, nIndex));

						if (this.sModelSettingJson.ExistMotionFadeOut(sMotion, nIndex))
							vMotionPair[nIndex].Key.setFadeOut(this.sModelSettingJson.GetMotionFadeIn(sMotion, nIndex));

						Resources.UnloadAsset(sMotionAsset);
					}

					this.sMotionMap.Add(sMotion, vMotionPair);
				}
			}

			if (!string.IsNullOrEmpty(sValue = this.sModelSettingJson.GetPhysicsFile()))
				this.sPhysicsAsset = Resources.Load<TextAsset>(Path.Combine(sDirectory, Path.GetFileNameWithoutExtension(sValue)));

			if (!string.IsNullOrEmpty(sValue = this.sModelSettingJson.GetPoseFile()))
				this.sPoseAsset = Resources.Load<TextAsset>(Path.Combine(sDirectory, Path.GetFileNameWithoutExtension(sValue)));
		}

		public void releaseSharedData()
		{
			this.sModelSettingJson = null;

			Resources.UnloadAsset(this.sModelAsset);
			Resources.UnloadAsset(this.sPhysicsAsset);
			Resources.UnloadAsset(this.sPoseAsset);

			this.sModelAsset = null;
			this.sPhysicsAsset = null;
			this.sPoseAsset = null;

			if (this.vTexture != null)
				foreach (var sTexture in this.vTexture)
					if (sTexture != null)
						Resources.UnloadAsset(sTexture);

			this.vTexture = null;

			foreach (var vPair in this.sMotionMap)
				foreach (var sPair in vPair.Value)
					if (sPair.Value != null)
						Resources.UnloadAsset(sPair.Value);

			this.sExpressionMap.Clear();
			this.sMotionMap.Clear();
		}
	}
}