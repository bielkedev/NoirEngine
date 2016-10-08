using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Noir.Util;
using UnityEngine;

namespace Noir.Script
{
	public class ScriptDialogue : IScriptLine
	{
		private string sDialogue;

		public string Dialogue { get { return this.sDialogue; } }

		public ScriptDialogue(StringParser sStringParser)
		{
			StringBuilder sDialogueBuilder = new StringBuilder();

			while(sStringParser.tryNotMatchChar("[@*"))
			{
				if (sStringParser.tryMatchChar('\\'))
				{
					sStringParser.skipWhile(1);

					if (sStringParser.IsRemain)
					{
						sDialogueBuilder.Append(sStringParser.CharacterUnsafe);
						sStringParser.skipWhile(1);
					}
				}
				else
				{
					sDialogueBuilder.Append(sStringParser.CharacterUnsafe);
					sStringParser.skipWhile(1);
				}
			}

			sDialogueBuilder.Replace("\n", "");
			this.sDialogue = sDialogueBuilder.ToString();
		}

		void IScriptLine.runScript()
		{
			Debug.Log("대사 : " + this.sDialogue);
		}
	}
}