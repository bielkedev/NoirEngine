using Noir.Unity;
using UnityEngine;

namespace Noir.UI
{
	public class UIManager
	{
		public static UnityManager UnityManagerObject { set { UIManager.sUnityManager = value; } }
		public static TextLayer MainTextLayer { set { UIManager.sMainTextLayer = value; } }

		private static UnityManager sUnityManager;
		private static TextLayer sMainTextLayer;

		public static void initUIManager()
		{
			UIManager.sMainTextLayer = null;
		}

		public static void clearDialogueText()
		{
			if (UIManager.sMainTextLayer != null)
				UIManager.sMainTextLayer.LayerText.text = string.Empty;
		}

		public static void appendDialogueText(string sDialogueText)
		{
			if (UIManager.sMainTextLayer != null)
				UIManager.sMainTextLayer.LayerText.text += sDialogueText;
		}
		
		public static void appendBacklogDialogueLog(string sBacklogText)
		{
			UIManager.sUnityManager.addBacklogDialogueLog(sBacklogText);
		}

		public static void forceUpdateScreen()
		{
			Camera.main.Render();
		}

		public static void waitForObject(int nInputType, IWaitableObject sWaitableObject)
		{
			UIManager.sUnityManager.waitForObject(nInputType, sWaitableObject);
		}
	}
}