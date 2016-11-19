using Noir.Unity;
using UnityEngine;

namespace Noir.UI
{
	public class UIManager
	{
		public static UnityManager UnityManagerObject { set { UIManager.sUnityManager = value; } }

		private static UnityManager sUnityManager;

		public static void clearNameText()
		{
			UIManager.sUnityManager._NameText.text = string.Empty;
		}

		public static void setNameText(string sNewName)
		{
			UIManager.sUnityManager._NameText.text = sNewName;
		}

		public static void clearDialogueText()
		{
			UIManager.sUnityManager._DialogueText.text = string.Empty;
		}

		public static void appendDialogueText(string sDialogueText)
		{
			UIManager.sUnityManager._DialogueText.text += sDialogueText;
		}
		
		public static void appendBacklogDialogueLog(string sBacklogText)
		{
			UIManager.sUnityManager.addBacklogDialogueLog(sBacklogText);
		}

		public static void forceUpdateScreen()
		{
			Camera.current.Render();
		}

		public static void waitForObject(int nInputType, IWaitableObject sWaitableObject)
		{
			UIManager.sUnityManager.waitForObject(nInputType, sWaitableObject);
		}
	}
}