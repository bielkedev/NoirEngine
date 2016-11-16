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

		public static void releaseCacheAll()
		{
			CacheManager.sSpriteCache.releaseCacheAll();
			CacheManager.Live2DSharedDataCache.releaseCacheAll();
		}
	}
}