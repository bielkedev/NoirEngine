using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noir.Error
{
	public class Error
	{
		public enum ErrorMode : uint
		{
			Print,
			Conceal
		}
		
		public struct ErrorData
		{
			public string ErrorMessage;
			public string FileName;
			public int LineNumber;
		}

		public static ErrorMode sErrorMode = ErrorMode.Print;
		public static List<ErrorData> sErrorList = new List<ErrorData>();

		public static void pushError(string sNewErrorMessage, string sNewFileName, int nNewLineNumber)
		{
			ErrorData sErrorData;
			sErrorData.ErrorMessage = sNewErrorMessage;
			sErrorData.FileName = sNewFileName;
			sErrorData.LineNumber = nNewLineNumber;

			Error.sErrorList.Add(sErrorData);
		}
	}
}