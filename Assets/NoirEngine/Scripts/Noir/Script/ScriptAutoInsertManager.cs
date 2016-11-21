using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Noir.Util;

namespace Noir.Script
{
	public class ScriptAutoInsertManager
	{
		public static Script LineHeadScript { get { return ScriptAutoInsertManager.sLineHeadScript; } }
		public static Script LineEndScript { get { return ScriptAutoInsertManager.sLineEndScript; } }

		private static List<ScriptLine> sLineHeadTagList = new List<ScriptLine>();
		private static List<ScriptLine> sLineEndTagList = new List<ScriptLine>();
		private static Script sLineHeadScript = null;
		private static Script sLineEndScript = null;
		
		public static void setLineHeadTagList(string sCommand)
		{
			ScriptAutoInsertManager.sLineHeadTagList.Clear();

			StringParser sParser = new StringParser(sCommand);

			for (ScriptLine sLine = Script.parseLine(sParser); sLine != null; sLine = Script.parseLine(sParser))
				ScriptAutoInsertManager.sLineHeadTagList.Add(sLine);

			ScriptAutoInsertManager.sLineHeadScript = new Script(ScriptRuntime.CurrentScript, ScriptAutoInsertManager.sLineHeadTagList);
		}

		public static void setLineEndTagList(string sCommand)
		{
			ScriptAutoInsertManager.sLineEndTagList.Clear();

			StringParser sParser = new StringParser(sCommand);

			for (ScriptLine sLine = Script.parseLine(sParser); sLine != null; sLine = Script.parseLine(sParser))
				ScriptAutoInsertManager.sLineEndTagList.Add(sLine);

			ScriptAutoInsertManager.sLineEndScript = new Script(ScriptRuntime.CurrentScript, ScriptAutoInsertManager.sLineEndTagList);
		}
	}
}