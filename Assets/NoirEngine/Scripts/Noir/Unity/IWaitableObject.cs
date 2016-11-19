using System.Collections;
using UnityEngine;

namespace Noir.Unity
{
	public interface IWaitableObject
	{
		bool isComplete();
	}
}