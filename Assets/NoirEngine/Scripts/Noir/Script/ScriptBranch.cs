using System.Collections.Generic;

namespace Noir.Script
{
	public class ScriptBranch
	{
		public static bool IsCurrentBranching { get { return ScriptBranch.sBranchingStack.Peek(); } set { ScriptBranch.sBranchingStack.Pop(); ScriptBranch.sBranchingStack.Push(value); } }

		private static Stack<bool> sBranchingStack = new Stack<bool>();

		public static void initBranch()
		{
			ScriptBranch.sBranchingStack.Clear();
		}

		public static void pushBranch()
		{
			ScriptBranch.sBranchingStack.Push(false);
		}

		public static void popBranch()
		{
			ScriptBranch.sBranchingStack.Pop();
		}
	}
}