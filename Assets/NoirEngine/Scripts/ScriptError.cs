using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noir.Script
{
	/// <summary>
	/// 스크립트 시스템에서 발생한 모든 오류를 관리합니다.
	/// </summary>
	public class ScriptError
	{
		public enum ErrorType : uint
		{
			ParsingError,
			RuntimeError
		}

		public struct Error
		{
			public ErrorType eErrorType;
			public string sErrorMessage;
			public string sErrorFilePath;
			public int nErrorLineNumber;
		}

		public delegate void ErrorHandler(ref Error sError);
		public static event ErrorHandler ErrorEvent;

		private static List<Error> sErrorList = new List<Error>();

		public static void pushError(ErrorType eNewErrorType, string sNewErrorMessage, string sNewErrorFilePath, int nNewErrorLineNumber)
		{
			Error sError;
			sError.eErrorType = eNewErrorType;
			sError.sErrorMessage = sNewErrorMessage;
			sError.sErrorFilePath = sNewErrorFilePath;
			sError.nErrorLineNumber = nNewErrorLineNumber;

			if (ScriptError.ErrorEvent != null)
				ScriptError.ErrorEvent(ref sError);

			ScriptError.sErrorList.Add(sError);
		}
	}
}