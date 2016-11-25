using Noir.UI;
using Noir.Util;
using System.Text;

namespace Noir.Script
{
	public class ScriptDialogue : ScriptLine
	{
		public string Dialogue { get { return this.sDialogue; } }

		private string sDialogue;

		public ScriptDialogue(string sScriptFilePath, StringParser sStringParser) : base(sScriptFilePath, sStringParser.Line)
		{
			StringBuilder sDialogueBuilder = new StringBuilder();

			while(sStringParser.tryNotMatchChar("\n[@*"))
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
			UIManager.appendBacklogDialogueLog(this.sDialogue);
		}
	}
}