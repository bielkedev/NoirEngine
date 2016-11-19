using Noir.Equation;
using Noir.Script;
using Noir.UI;
using Noir.Unity.Live2D;
using UnityEngine;
using UnityEngine.UI;

namespace Noir.Unity
{
	[RequireComponent(typeof(WaitManager))]
	public class UnityManager : MonoBehaviour
	{
		public enum ErrorReportType
		{
			Log,
			Print,
			Conceal
		}

		[Header("Optimization")]
		public int _TargetFPS;

		[Header("Debug")]
		public ErrorReportType _ErrorReportType;
		
		[Header("Script")]
		public string _ScriptFilePath;
		public string[] _MacroScriptFilePath;

		[Header("Layer")]
		public RectTransform _LayerPanel;
		public Material _LayerMaterial;
		public GameObject _LayerClipperPrefab;

		[Header("Sprite Layer")]
		public GameObject _SpriteLayerPrefab;

		[Header("Live2D Layer")]
		public GameObject _Live2DLayerPrefab;
		public Camera _RenderCamera;

		[Header("Animated Layer")]
		public GameObject _AnimatedLayerPrefab;

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
		public RectTransform _BacklogContent;
		public GameObject _DialogueLogPrefab;

		private WaitManager sWaitManager;
		private Vector2 sBacklogContentSize;

		public void waitForObject(int nInputType, IWaitableObject sWaitableObject)
		{
			this.sWaitManager.startWait(nInputType, sWaitableObject);
		}
		
		public void addBacklogDialogueLog(string sDialogueText)
		{
			if (string.IsNullOrEmpty(sDialogueText.Trim()))
				return;

			GameObject sDialogueLog = GameObject.Instantiate<GameObject>(this._DialogueLogPrefab);
			sDialogueLog.name = "Backlog element";

			RectTransform sRectTransform = sDialogueLog.GetComponent<RectTransform>();
			Text sText = sDialogueLog.GetComponent<Text>();
			float nHeight = sText.cachedTextGenerator.GetPreferredHeight(sDialogueText, sText.GetGenerationSettings(sBacklogContentSize));

			sRectTransform.sizeDelta = new Vector2(sRectTransform.sizeDelta.x, nHeight);
			sRectTransform.Translate(0.0f, -this._BacklogContent.sizeDelta.y, 0.0f, Space.Self);
			sRectTransform.SetParent(this._BacklogContent, false);
			
			sText.text = sDialogueText;

			this._BacklogContent.sizeDelta = new Vector2(this._BacklogContent.sizeDelta.x, this._BacklogContent.sizeDelta.y + nHeight);
			this._BacklogScroll.verticalNormalizedPosition = 0.0f;
		}
		
		private void Awake()
		{
			//에러 핸들러 등록
			ScriptError.ErrorEvent += this.OnError;

			//모듈 초기화
			Application.targetFrameRate = this._TargetFPS;
			UIManager.UnityManagerObject = this;

			EquationVariable.initEquationVariable();
			ScriptTagManager.initTagHandler();
			ScriptBranch.initBranch();
			ScriptLoop.initLoop();
			live2d.Live2D.init();

			//Layer
			Layer.LayerPanel = this._LayerPanel;
			Layer.LayerMaterial = this._LayerMaterial;
			LayerClipper.LayerClipperPrefab = this._LayerClipperPrefab;

			//Sprite Layer
			SpriteLayer.SpriteLayerPrefab = this._SpriteLayerPrefab;

			//Live2D Layer
			Live2DLayer.Live2DLayerPrefab = this._Live2DLayerPrefab;
			Live2DLayer.RenderCamera = this._RenderCamera;

			//Animated Layer
			AnimatedLayer.AnimatedLayerPrefab = this._AnimatedLayerPrefab;

			foreach (var sMacroScriptFilePath in this._MacroScriptFilePath)
				Macro.addMacroScript(sMacroScriptFilePath);

			ScriptRuntime.gotoScript(this._ScriptFilePath, null);

			//유니티 오브젝트 초기화
			{
				this._MenuPanel.SetActive(false);
				this._BacklogPanel.SetActive(false);

				this._ProceedButton.onClick.AddListener(this.OnProceed);
				this._MenuButton.onClick.AddListener(this.OnMenu);
				this._BacklogButton.onClick.AddListener(this.OnBacklog);
				this._MenuReturnButton.onClick.AddListener(this.OnReturn);
				this._BacklogReturnButton.onClick.AddListener(this.OnReturn);

				this.sWaitManager = this.gameObject.GetComponent<WaitManager>();
				this.sBacklogContentSize = this._BacklogContent.rect.size;
				this.sBacklogContentSize.y = 0;
			}

			UIManager.clearNameText();
			UIManager.clearDialogueText();

			/*
				테스트가 필요하다면 여기부터
			*/

			//테스트...

			/*
				여기까지
			*/

			//스크립트 실행
			ScriptRuntime.runScript();
		}

		private void OnDestroy()
		{
			//파괴할거 여기서 파괴!
			live2d.Live2D.dispose();
			CacheManager.releaseCacheAll();
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
			if(this.sWaitManager.isCanPassWithInput())
			{
				if (this.sWaitManager.IsWaiting)
					this.sWaitManager.passWithInput();

				ScriptRuntime.runScript();
			}
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