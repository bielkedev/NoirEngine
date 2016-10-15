using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

		private static Stack<ScriptCall> sScriptCallStack = new Stack<ScriptCall>();
		private static Dictionary<string, Script> sScriptMap = new Dictionary<string, Script>();

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
		/// 특정 스크립트를 로드 후 캐싱합니다. 이미 로드되어있다면 캐싱된 스크립트 객체를 불러옵니다.
		/// </summary>
		/// <param name="sNewScriptPath">로드할 스크립트 파일 경로입니다.</param>
		/// <returns>로드된 스크립트 객체입니다.</returns>
		public static Script loadScript(string sNewScriptPath)
		{
			Script sResult;

			if (ScriptRuntime.sScriptMap.TryGetValue(sNewScriptPath, out sResult))
				return sResult;
			
			ScriptRuntime.sScriptMap.Add(sNewScriptPath, sResult = new Script(sNewScriptPath));

			return sResult;
		}

		/// <summary>
		/// 특정 스크립트의 특정 구역을 호출합니다. 스크립트가 실행중이라면 호출스택에 기록됩니다. 실행중이 아니라면 기록하지 않습니다.
		/// </summary>
		/// <param name="sScriptPath">호출할 스크립트의 파일 경로입니다. null일 경우 현재 실행중인 스크립트를 대상으로 합니다.</param>
		/// <param name="sRegionName">호출할 구역의 이름입니다. null일 경우 스크립트를 처음부터 실행합니다.</param>
		public static void callScript(string sScriptPath, string sRegionName)
		{
			Script sTargetScript = null;

			if(sScriptPath == null)
			{
				if(ScriptRuntime.sCurrentScript == null)
				{
					ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "실행중인 스크립트가 없습니다.", "EMPTY", -1);
					return;
				}

				sTargetScript = ScriptRuntime.sCurrentScript;
			}
			else
				sTargetScript = ScriptRuntime.loadScript(sScriptPath);

			int nTargetLineIndex;

			if (sRegionName == null)
				nTargetLineIndex = 0;
			else
			{
				if(!sTargetScript.RegionList.TryGetValue(sRegionName, out nTargetLineIndex))
				{
					ScriptError.pushError(ScriptError.ErrorType.RuntimeError, "레이블이 정의되지 않았습니다.", "EMPTY", -1);
					return;
				}
			}

			//스크립트가 실행중이라면 콜스택 푸시
			if(ScriptRuntime.IsRunning)
			{
				ScriptCall sCall;

				sCall.nLineIndex = ScriptRuntime.nCurrentLineIndex;
				sCall.sScript = ScriptRuntime.sCurrentScript;

				ScriptRuntime.sScriptCallStack.Push(sCall);
			}

			ScriptRuntime.nCurrentLineIndex = nTargetLineIndex;
			ScriptRuntime.sCurrentScript = sTargetScript;
		}

		/// <summary>
		/// 마지막으로 스크립트를 호출했던 위치로 되돌아갑니다. 호출스택을 되감습니다. 돌아갈 곳이 없다면 스크립팅을 종료합니다.
		/// </summary>
		public static void returnScript()
		{
			//돌아갈 곳이 없으면 스크립팅 종료
			if(ScriptRuntime.sScriptCallStack.Count == 0)
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

			if (ScriptRuntime.bSuspendScript)
			{
				ScriptRuntime.bSuspendScript = false;
				return;
			}

			while(ScriptRuntime.nCurrentLineIndex < ScriptRuntime.sCurrentScript.ScriptLineList.Count)
			{
				ScriptRuntime.sCurrentScript.ScriptLineList[ScriptRuntime.nCurrentLineIndex++].runScript();

				if (ScriptRuntime.bSuspendScript)
				{
					ScriptRuntime.bSuspendScript = false;
					return;
				}
			}
		}

		/// <summary>
		/// 연속적인 스크립트 실행을 중지합니다.
		/// </summary>
		public static void suspendScript()
		{
			ScriptRuntime.bSuspendScript = true;
		}
	}
}