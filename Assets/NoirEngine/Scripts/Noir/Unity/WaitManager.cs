using Noir.Script;
using System.Collections;
using UnityEngine;

namespace Noir.Unity
{
	public class WaitManager : MonoBehaviour
	{
		public bool IsWaiting { get { return this.bWaiting; } }

		private bool bWaiting = false;
		private int nWaitInputType = 0;
		private IWaitableObject sWaitableObject = null;

		public bool isCanPassWithInput()
		{
			return !this.bWaiting || this.nWaitInputType != 0;
		}

		public void passWithInput()
		{
			if (this.bWaiting)
			{
				this.bWaiting = false;
				this.StopAllCoroutines();
			}
		}

		public void startWait(int nNewWaitInputType, IWaitableObject sNewWaitableObject)
		{
			if (this.bWaiting)
				this.StopAllCoroutines();

			this.bWaiting = true;
			this.nWaitInputType = nNewWaitInputType;
			this.sWaitableObject = sNewWaitableObject;

			if (this.sWaitableObject.isComplete())
				this.bWaiting = false;
			else
			{
				ScriptRuntime.suspendScript();
				this.StartCoroutine(this.doWait());
			}
		}

		private IEnumerator doWait()
		{
			while (this.sWaitableObject != null && !this.sWaitableObject.isComplete())
				yield return null;

			this.bWaiting = false;
			ScriptRuntime.runScript();
		}
	}
}