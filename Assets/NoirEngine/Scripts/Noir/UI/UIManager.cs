using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Noir.Unity;

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
			UIManager.sUnityManager.addBacklogDialogueLog(sDialogueText);
		}
	}
}