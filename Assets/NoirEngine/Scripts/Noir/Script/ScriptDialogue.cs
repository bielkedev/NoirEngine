using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Noir.UI;
using Noir.Util;

namespace Noir.Script
{
	public class ScriptDialogue : ScriptLine
	{
		private string sDialogue;

		public string Dialogue { get { return this.sDialogue; } }

		public ScriptDialogue(string sScriptFilePath, StringParser sStringParser) : base(sScriptFilePath, sStringParser.Line)
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

		public override void runScript()
		{
			UIManager.appendDialogueText(this.sDialogue);
		}
	}
}