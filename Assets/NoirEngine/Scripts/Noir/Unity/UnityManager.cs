using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Noir.Script;
using Noir.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Noir.Unity
{
	public class UnityManager : MonoBehaviour
	{
		public enum ErrorReportType
		{
			Log,
			Print,
			Conceal
		}

		[Header("Debug")]
		public ErrorReportType _ErrorReportType;

		[Header("Script")]
		public string _ScriptFilePath;

		[Header("Front UI")]
		public GameObject _FrontPanel;
		public Text _NameText;
		public Text _DialogueText;
		public Button _ProceedButton;
		public Button _MenuButton;

		[Header("Menu UI")]
		public GameObject _MenuPanel;
		public Button _BacklogButton;
		public Button _MenuReturnButton;

		[Header("Backlog UI")]
		public GameObject _BacklogPanel;
		public Button _BacklogReturnButton;
		public ScrollRect _BacklogScroll;
		public GameObject _BacklogContent;
		public GameObject _BacklogDialogueLog;

		private Vector2 sBacklogContentSize;
		private RectTransform sBacklogContentRectTransform;

		public void addBacklogDialogueLog(string sDialogueText)
		{
			if (string.IsNullOrEmpty(sDialogueText.Trim()))
				return;

			GameObject sDialogueLog = GameObject.Instantiate<GameObject>(this._BacklogDialogueLog);
			RectTransform sRectTransform = sDialogueLog.GetComponent<RectTransform>();
			Text sText = sDialogueLog.GetComponent<Text>();
			float nHeight = sText.cachedTextGenerator.GetPreferredHeight(sDialogueText, sText.GetGenerationSettings(sBacklogContentSize));

			sRectTransform.sizeDelta = new Vector2(sRectTransform.sizeDelta.x, nHeight);
			sRectTransform.SetParent(this.sBacklogContentRectTransform, false);
			sRectTransform.Translate(0.0f, -this.sBacklogContentRectTransform.sizeDelta.y, 0.0f, Space.Self);

			sText.text = sDialogueText;

			this.sBacklogContentRectTransform.sizeDelta = new Vector2(this.sBacklogContentRectTransform.sizeDelta.x, this.sBacklogContentRectTransform.sizeDelta.y + nHeight);
			this._BacklogScroll.verticalNormalizedPosition = 0.0f;
		}

		private void Start()
		{
			ScriptError.ErrorEvent += this.OnError;
			ScriptTagManager.initTagHandler();

			UIManager.UnityManagerObject = this;

			/*
				여기에 각종 초기화 삽입
			*/
			{
				this._MenuPanel.SetActive(false);
				this._BacklogPanel.SetActive(false);

				this._ProceedButton.onClick.AddListener(this.OnProceed);
				this._MenuButton.onClick.AddListener(this.OnMenu);
				this._BacklogButton.onClick.AddListener(this.OnBacklog);
				this._MenuReturnButton.onClick.AddListener(this.OnReturn);
				this._BacklogReturnButton.onClick.AddListener(this.OnReturn);
				
				this.sBacklogContentRectTransform = this._BacklogContent.GetComponent<RectTransform>();

				Vector3[] vCorner = new Vector3[4];
				this.sBacklogContentRectTransform.GetWorldCorners(vCorner);
				this.sBacklogContentSize.x = Math.Abs(vCorner[0].x - vCorner[2].x);
				this.sBacklogContentSize.y = 0;
			}

			UIManager.clearNameText();
			UIManager.clearDialogueText();

			ScriptRuntime.gotoScript(this._ScriptFilePath, null);
			ScriptRuntime.runScript();
		}

		private void OnError(ref ScriptError.Error sError)
		{
			switch (this._ErrorReportType)
			{
				case ErrorReportType.Log:
				{
					Debug.LogError(sError.sErrorMessage + " - " + sError.sErrorFilePath + " : " + sError.nErrorLineNumber);
				}
				break;
				case ErrorReportType.Print:
				{
					Debug.LogError(sError.sErrorMessage + " - " + sError.sErrorFilePath + " : " + sError.nErrorLineNumber);
				}
				break;
				default:
				break;
			}
		}

		private void OnProceed()
		{
			ScriptRuntime.runScript();
		}

		private void OnMenu()
		{
			this._MenuPanel.SetActive(true);
		}

		private void OnBacklog()
		{
			this._MenuPanel.SetActive(false);
			this._BacklogPanel.SetActive(true);
		}

		private void OnReturn()
		{
			this._MenuPanel.SetActive(false);
			this._BacklogPanel.SetActive(false);
		}
	}
}