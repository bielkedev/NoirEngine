using UnityEngine;
using System.Collections;
using Noir.Script;

public class Tester : MonoBehaviour
{
	// Use this for initialization
	private void Start()
	{
		ScriptError.ErrorEvent += this.OnError;
		ScriptRuntime.runScript("Scenario/main");
	}

	private void OnError(ref ScriptError.Error sError)
	{
		Debug.LogError("Error during " + (sError.eErrorType == ScriptError.ErrorType.ParsingError ? "parsing : " : "running : ") + sError.sErrorMessage + "\n" + sError.sErrorFilePath + " : " + sError.nErrorLineNumber);
	}
}