using System.Collections.Generic;

namespace Noir.Script
{
	public class ScriptLoop
	{
		public static bool IsCurrentLooping { get { return ScriptLoop.sLoopingStack.Peek(); } set { ScriptLoop.sLoopingStack.Pop(); ScriptLoop.sLoopingStack.Push(value); } }

		private static Stack<bool> sLoopingStack = new Stack<bool>();

		public static void initLoop()
		{
			ScriptLoop.sLoopingStack.Clear();
		}

		public static void pushLoop()
		{
			ScriptLoop.sLoopingStack.Push(false);
		}

		public static void popLoop()
		{
			ScriptLoop.sLoopingStack.Pop();
		}
	}
}