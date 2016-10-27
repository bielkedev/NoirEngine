using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Noir.Unity
{
	public class SpriteManager
	{
		private static Dictionary<string, Sprite> sSpriteMap = new Dictionary<string, Sprite>();

		public static Sprite loadSprite(string sSpriteFilePath)
		{
			Sprite sSprite;

			if (SpriteManager.sSpriteMap.TryGetValue(sSpriteFilePath, out sSprite))
				return sSprite;

			if ((sSprite = Resources.Load<Sprite>(sSpriteFilePath)) == null)
				return null;

			SpriteManager.sSpriteMap.Add(sSpriteFilePath, sSprite);

			return sSprite;
		}

		public static void unloadSprite(string sSpriteFilePath)
		{
			Sprite sSprite;

			if (SpriteManager.sSpriteMap.TryGetValue(sSpriteFilePath, out sSprite))
			{
				SpriteManager.sSpriteMap.Remove(sSpriteFilePath);
				Resources.UnloadAsset(sSprite);
			}
		}

		public static void unloadSpriteAll()
		{
			foreach(var sPair in SpriteManager.sSpriteMap)
				Resources.UnloadAsset(sPair.Value);

			SpriteManager.sSpriteMap.Clear();
		}
	}
}