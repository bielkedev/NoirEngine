using Noir.Unity;
using System.Collections.Generic;

namespace Noir.Script
{
	public class ScriptRuntime
	{
		public struct ScriptCall
		{
			public int nLineIndex;
			public Script sScript;
		}

		public static bool IsRunning { get { return ScriptRuntime.sCurrentScript != null; } }
		public static bool IsSuspended { get { return ScriptRuntime.bSuspendScript; } }
		public static int CurrentLineIndex { get { return ScriptRuntime.nCurrentLineIndex; } }
		public static Script CurrentScript { get { return ScriptRuntime.sCurrentScript; } }

		private static Stack<ScriptCall> sScriptCallStack = new Stack<ScriptCall>();

		/// <summary>
		/// 스크립트 진행을 멈출지 결정하는 플래그입니다.
		/// </summary>
		private static bool bSuspendScript = false;
		/// <summary>
		/// 현재 실행중인 스크립트 명령의 인덱스입니다.
		/// </summary>
		private static int nCurrentLineIndex = 0;
		/// <summary>
		/// 현재 실행중인 스크립트 객체입니다.
		/// </summary>
		private static Script sCurrentScript = null;

		/// <summary>
		/// 특정 스크립트의 특정 부분으로 점프합니다.
		/// </summary>
		/// <param name="sScript">점프할 스크립트 객체입니다.</param>
		/// <param name="nScriptLine">점프할 스크립트 라인 위치입니다.</param>
		public static void gotoScript(Script sScript, int nScriptLine)
		{
			ScriptRuntime.nCurrentLineIndex = nScriptLine;
			ScriptRuntime.sCurrentScript = sScript;
		}

		/// <summary>
		/// 특정 스크립트의 특정 부분으로 점프합니다.
		/// </summary>
		/// <param name="sScript">점프할 스크립트 객체입니다.</param>
		/// <param name="sLabelName">점프할 스크립트의 레이블 이름입니다. null이거나 비어있다면 스크립트의 처음을 나타냅니다.</param>
		/// <returns>성공 여부입니다.</returns>
		public static bool gotoScript(Script sScript, string sLabelName = null)
		{
			int nLineIndex = 0;

			if (string.IsNullOrEmpty(sLabelName) || sScript.ScriptRegionList.TryGetValue(sLabelName, out nLineIndex))
			{
				ScriptRuntime.nCurrentLineIndex = nLineIndex;
				ScriptRuntime.sCurrentScript = sScript;

				return true;
			}

			return false;
		}

		/// <summary>
		/// 특정 스크립트의 특정 부분으로 점프합니다.
		/// </summary>
		/// <param name="sScriptFile">점프할 스크립트 파일의 경로입니다. null이거나 비어있다면 현재 실행중인 스크립트의 부모를 나타냅니다.</param>
		/// <param name="sLabelName">점프할 스크립트의 레이블 이름입니다. null이거나 비어있다면 스크립트의 처음을 나타냅니다.</param>
		/// <returns>성공 여부입니다.</returns>
		public static bool gotoScript(string sScriptFile, string sLabelName)
		{
			Script sTargetScript = string.IsNullOrEmpty(sScriptFile) ? null : CacheManager.loadScript(sScriptFile);

			if (sTargetScript == null)
			{
				if (ScriptRuntime.sCurrentScript == null)
					return false;

				sTargetScript = ScriptRuntime.sCurrentScript.ParentScript;
			}

			int nLineIndex = 0;

			if (string.IsNullOrEmpty(sLabelName) || sTargetScript.ScriptRegionList.TryGetValue(sLabelName, out nLineIndex))
			{
				ScriptRuntime.nCurrentLineIndex = nLineIndex;
				ScriptRuntime.sCurrentScript = sTargetScript;

				return true;
			}

			return false;
		}

		/// <summary>
		/// 특정 스크립트의 특정 부분을 호출합니다. 호출스택에 기록됩니다.
		/// </summary>
		/// <param name="sScript">호출할 스크립트 객체입니다.</param>
		/// <param name="nScriptLine">호출할 스크립트 라인 위치입니다.</param>
		/// <returns>성공 여부입니다.</returns>
		public static bool callScript(Script sScript, int nScriptLine)
		{
			ScriptCall sCall;

			sCall.nLineIndex = ScriptRuntime.nCurrentLineIndex;
			sCall.sScript = ScriptRuntime.sCurrentScript;

			ScriptRuntime.sScriptCallStack.Push(sCall);

			ScriptRuntime.nCurrentLineIndex = nScriptLine;
			ScriptRuntime.sCurrentScript = sScript;

			return true;
		}

		/// <summary>
		/// 특정 스크립트의 특정 부분을 호출합니다. 호출스택에 기록됩니다.
		/// </summary>
		/// <param name="sScript">호출할 스크립트 객체입니다.</param>
		/// <param name="sLabelName">호출할 스크립트의 레이블 이름입니다. null이거나 비어있다면 스크립트의 처음을 나타냅니다.</param>
		/// <returns>성공 여부입니다.</returns>
		public static bool callScript(Script sScript, string sLabelName = null)
		{
			int nLineIndex = 0;

			if (string.IsNullOrEmpty(sLabelName) || sScript.ScriptRegionList.TryGetValue(sLabelName, out nLineIndex))
			{
				ScriptCall sCall;

				sCall.nLineIndex = ScriptRuntime.nCurrentLineIndex;
				sCall.sScript = ScriptRuntime.sCurrentScript;

				ScriptRuntime.sScriptCallStack.Push(sCall);

				ScriptRuntime.nCurrentLineIndex = nLineIndex;
				ScriptRuntime.sCurrentScript = sScript;

				return true;
			}

			return false;
		}

		/// <summary>
		/// 특정 스크립트의 특정 부분을 호출합니다. 호출스택에 기록됩니다.
		/// </summary>
		/// <param name="sScriptFile">호출할 스크립트 파일의 경로입니다. null이거나 비어있다면 현재 실행중인 스크립트의 부모를 나타냅니다.</param>
		/// <param name="sLabelName">호출할 스크립트의 레이블 이름입니다. null이거나 비어있다면 스크립트의 처음을 나타냅니다.</param>
		/// <returns>성공 여부입니다.</returns>
		public static bool callScript(string sScriptFile, string sLabelName)
		{
			Script sTargetScript = string.IsNullOrEmpty(sScriptFile) ? null : CacheManager.loadScript(sScriptFile);

			if (sTargetScript == null)
			{
				if (ScriptRuntime.sCurrentScript == null)
					return false;

				sTargetScript = ScriptRuntime.sCurrentScript.ParentScript;
			}

			int nLineIndex;

			if (sTargetScript.ScriptRegionList.TryGetValue(sLabelName, out nLineIndex))
			{
				ScriptCall sCall;

				sCall.nLineIndex = ScriptRuntime.nCurrentLineIndex;
				sCall.sScript = ScriptRuntime.sCurrentScript;

				ScriptRuntime.sScriptCallStack.Push(sCall);

				ScriptRuntime.nCurrentLineIndex = nLineIndex;
				ScriptRuntime.sCurrentScript = sTargetScript;

				return true;
			}

			return false;
		}

		/// <summary>
		/// 마지막으로 스크립트를 호출했던 위치로 되돌아갑니다. 호출스택을 되감습니다. 돌아갈 곳이 없다면 스크립팅을 종료합니다.
		/// </summary>
		public static void returnScript()
		{
			if (ScriptRuntime.sCurrentScript.IsMacro)
				Macro.popMacroVariable();

			//돌아갈 곳이 없으면 스크립팅 종료
			if (ScriptRuntime.sScriptCallStack.Count == 0)
			{
				ScriptRuntime.nCurrentLineIndex = 0;
				ScriptRuntime.sCurrentScript = null;

				ScriptRuntime.suspendScript();

				return;
			}

			ScriptCall sCall = ScriptRuntime.sScriptCallStack.Pop();

			ScriptRuntime.nCurrentLineIndex = sCall.nLineIndex;
			ScriptRuntime.sCurrentScript = sCall.sScript;
		}

		/// <summary>
		/// 스크립트를 실행합니다.
		/// </summary>
		public static void runScript()
		{
			if (!ScriptRuntime.IsRunning)
				return;

			for (;;)
			{
				if (ScriptRuntime.bSuspendScript)
				{
					ScriptRuntime.bSuspendScript = false;
					return;
				}

				while (ScriptRuntime.nCurrentLineIndex < ScriptRuntime.sCurrentScript.ScriptLineList.Count)
				{
					ScriptRuntime.sCurrentScript.ScriptLineList[ScriptRuntime.nCurrentLineIndex++].runScript();

					if (ScriptRuntime.bSuspendScript)
					{
						ScriptRuntime.bSuspendScript = false;
						return;
					}
				}

				//스크립트 실행을 끝냈으면 콜스택 되감기 시도
				ScriptRuntime.returnScript();
			}
		}

		/// <summary>
		/// 스크립트 실행을 중지합니다.
		/// </summary>
		public static void suspendScript()
		{
			foreach (Layer sLayer in Layer.NeedUpdateLayerEnumerable)
				sLayer.applyLayerProperties();

			Layer.clearNeedUpdateLayerList();
			ScriptRuntime.bSuspendScript = true;
		}

		public static void skipScript(int nCount)
		{
			ScriptRuntime.nCurrentLineIndex += nCount;
		}

		public static bool skipScript(string sDelimitTagName)
		{
			while (ScriptRuntime.nCurrentLineIndex < ScriptRuntime.sCurrentScript.ScriptLineList.Count)
			{
				ScriptTag sTag = ScriptRuntime.sCurrentScript.ScriptLineList[ScriptRuntime.nCurrentLineIndex] as ScriptTag;

				if (sTag != null && sTag.Name == sDelimitTagName)
					return true;

				++ScriptRuntime.nCurrentLineIndex;
			}

			return false;
		}

		public static string skipScript(string[] sDelimitTagName)
		{
			while (ScriptRuntime.nCurrentLineIndex < ScriptRuntime.sCurrentScript.ScriptLineList.Count)
			{
				ScriptTag sTag = ScriptRuntime.sCurrentScript.ScriptLineList[ScriptRuntime.nCurrentLineIndex] as ScriptTag;

				if (sTag != null)
					foreach (string sName in sDelimitTagName)
						if (sTag.Name == sName)
							return sName;

				++ScriptRuntime.nCurrentLineIndex;
			}

			return null;
		}

		public static void skipScriptBack(int nCount)
		{
			if ((ScriptRuntime.nCurrentLineIndex -= nCount) < 0)
				ScriptRuntime.nCurrentLineIndex = 0;
		}

		public static bool skipScriptBack(string sDelimitTagName)
		{
			if (ScriptRuntime.CurrentScript.ScriptLineList.Count > 0 && ScriptRuntime.nCurrentLineIndex >= ScriptRuntime.CurrentScript.ScriptLineList.Count)
				ScriptRuntime.nCurrentLineIndex = ScriptRuntime.CurrentScript.ScriptLineList.Count - 1;

			while (ScriptRuntime.nCurrentLineIndex >= 0 && ScriptRuntime.nCurrentLineIndex < ScriptRuntime.CurrentScript.ScriptLineList.Count)
			{
				ScriptTag sTag = ScriptRuntime.sCurrentScript.ScriptLineList[ScriptRuntime.nCurrentLineIndex] as ScriptTag;

				if (sTag != null && sTag.Name == sDelimitTagName)
					return true;

				--ScriptRuntime.nCurrentLineIndex;
			}

			return false;
		}

		public static string skipScriptBack(string[] sDelimitTagName)
		{
			while (ScriptRuntime.nCurrentLineIndex >= 0 && ScriptRuntime.nCurrentLineIndex < ScriptRuntime.CurrentScript.ScriptLineList.Count)
			{
				ScriptTag sTag = ScriptRuntime.sCurrentScript.ScriptLineList[ScriptRuntime.nCurrentLineIndex] as ScriptTag;

				if (sTag != null)
					foreach (string sName in sDelimitTagName)
						if (sTag.Name == sName)
							return sName;

				--ScriptRuntime.nCurrentLineIndex;
			}

			return null;
		}
	}
}