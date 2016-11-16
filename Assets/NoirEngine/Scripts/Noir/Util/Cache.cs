using System.Collections.Generic;

namespace Noir.Unity
{
	public class Cache<K, V>
	{
		public delegate void DisposeValue(V sValue);

		private DisposeValue fDisposeValue;
		private Dictionary<K, V> sCacheMap;

		public Cache(DisposeValue fNewDisposeValue)
		{
			this.fDisposeValue = fNewDisposeValue;
			this.sCacheMap = new Dictionary<K, V>();
		}

		public V this[K sKey]
		{
			get
			{
				V sValue;
				this.sCacheMap.TryGetValue(sKey, out sValue);
				return sValue;
			}
			set { this.sCacheMap.Add(sKey, value); }
		}

		public void addCache(K sKey, V sValue)
		{
			this.sCacheMap.Add(sKey, sValue);
		}

		public void releaseCache(K sKey)
		{
			this.fDisposeValue(this[sKey]);
		}

		public void releaseCacheAll()
		{
			foreach (var sPair in this.sCacheMap)
				this.fDisposeValue(sPair.Value);

			this.sCacheMap.Clear();
		}
	}
}