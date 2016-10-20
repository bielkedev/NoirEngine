using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Noir.Equation;
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
		public RectTransform _BacklogViewport;
		public RectTransform _BacklogContent;
		public GameObject _BacklogDialogueLog;

		private Vector2 sBacklogContentSize;

		public void addBacklogDialogueLog(string sDialogueText)
		{
			if (string.IsNullOrEmpty(sDialogueText.Trim()))
				return;

			GameObject sDialogueLog = GameObject.Instantiate<GameObject>(this._BacklogDialogueLog);
			RectTransform sRectTransform = sDialogueLog.GetComponent<RectTransform>();
			Text sText = sDialogueLog.GetComponent<Text>();
			float nHeight = sText.cachedTextGenerator.GetPreferredHeight(sDialogueText, sText.GetGenerationSettings(sBacklogContentSize));

			sRectTransform.sizeDelta = new Vector2(sRectTransform.sizeDelta.x, nHeight);
			sRectTransform.SetParent(this._BacklogContent, false);
			sRectTransform.Translate(0.0f, -this._BacklogContent.sizeDelta.y, 0.0f, Space.Self);

			sText.text = sDialogueText;

			this._BacklogContent.sizeDelta = new Vector2(this._BacklogContent.sizeDelta.x, this._BacklogContent.sizeDelta.y + nHeight);
			this._BacklogScroll.verticalNormalizedPosition = 0.0f;
		}

		private void Start()
		{
			UIManager.UnityManagerObject = this;

			//에러 핸들러 등록
			ScriptError.ErrorEvent += this.OnError;

			//모듈 초기화
			EquationVariable.initEquationVariable();
			ScriptTagManager.initTagHandler();

			//유니티 오브젝트 초기화
			{
				this._MenuPanel.SetActive(false);
				this._BacklogPanel.SetActive(false);

				this._ProceedButton.onClick.AddListener(this.OnProceed);
				this._MenuButton.onClick.AddListener(this.OnMenu);
				this._BacklogButton.onClick.AddListener(this.OnBacklog);
				this._MenuReturnButton.onClick.AddListener(this.OnReturn);
				this._BacklogReturnButton.onClick.AddListener(this.OnReturn);

				this.sBacklogContentSize.x = Screen.width +
					this._BacklogScroll.GetComponent<RectTransform>().sizeDelta.x +
					this._BacklogViewport.sizeDelta.x +
					this._BacklogContent.sizeDelta.x;
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