using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Noir.Script;

namespace Assets.Resources.C__Scripts
{
	public class ScriptErrorLogger : MonoBehaviour
	{
		public enum ErrorReportType
		{
			Log,
			Print,
			Conceal
		}

		public ErrorReportType _ErrorReportType;

		private void Start()
		{
			ScriptError.ErrorEvent += this.OnError;
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
	}
}