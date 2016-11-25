using System;

namespace Noir.Unity
{
	public class WaitFuncObject : IWaitableObject
	{
		Func<bool> fWaitFunc;

		public WaitFuncObject(Func<bool> fNewWaitFunc)
		{
			this.fWaitFunc = fNewWaitFunc;
		}

		bool IWaitableObject.isComplete()
		{
			return this.fWaitFunc();
		}
	}
}