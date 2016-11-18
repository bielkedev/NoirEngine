using Noir.Util;
using Noir.Unity.Live2D;
using UnityEngine;

namespace Noir.Unity
{
	public class CacheManager
	{
		public static Cache<string, Sprite> SpriteCache { get { return CacheManager.sSpriteCache; } }
		public static Cache<string, Live2DSharedData> Live2DSharedDataCache { get { return CacheManager.sLive2DSharedDataCache; } }

		private static Cache<string, Sprite> sSpriteCache = new Cache<string, Sprite>((sSprite) => { Resources.UnloadAsset(sSprite); });
		private static Cache<string, Live2DSharedData> sLive2DSharedDataCache = new Cache<string, Live2DSharedData>((sSharedData) => { sSharedData.releaseSharedData(); });

		public static Sprite loadSprite(string sSpritePath)
		{
			Sprite sSprite = CacheManager.SpriteCache[sSpritePath];

			if (sSprite != null)
				return sSprite;
			
			sSprite = Resources.Load<Sprite>(sSpritePath);

			if (sSprite != null)
				CacheManager.SpriteCache.addCache(sSpritePath, sSprite);

			return sSprite;
		}

		public static Live2DSharedData loadLive2DSharedData(string sLive2DPath)
		{
			Live2DSharedData sSharedData = CacheManager.Live2DSharedDataCache[sLive2DPath];

			if (sSharedData != null)
				return sSharedData;

			CacheManager.Live2DSharedDataCache.addCache(sLive2DPath, sSharedData = new Live2DSharedData(sLive2DPath));

			return sSharedData;
		}

		public static void releaseCacheAll()
		{
			CacheManager.sSpriteCache.releaseCacheAll();
			CacheManager.Live2DSharedDataCache.releaseCacheAll();
		}
	}
}