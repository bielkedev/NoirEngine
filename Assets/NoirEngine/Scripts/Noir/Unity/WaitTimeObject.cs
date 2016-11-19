using UnityEngine;

namespace Noir.Unity
{
	public class WaitTimeObject : IWaitableObject
	{
		private float nTime;

		public WaitTimeObject(float nDuration)
		{
			this.nTime = Time.time + nDuration;
		}

		bool IWaitableObject.isComplete()
		{
			return Time.time >= this.nTime;
		}
	}
}