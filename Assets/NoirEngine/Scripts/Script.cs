using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Noir.Util;

namespace Noir.Script
{
	/// <summary>
	/// 스크립트를 관리합니다.
	/// </summary>
	public class Script
	{
		private List<IScriptLine> sScriptLineList = new List<IScriptLine>();
		private Dictionary<string, int> sRegionList = new Dictionary<string, int>();

		public Script(string sScriptFilePath)
		{
			var sAsset = Resources.Load<TextAsset>(sScriptFilePath);
			
			if(sAsset == null)
			{
				//TODO : Report an error.
				return;
			}

			List<string> sScript = new List<string>();
			StringParser sParser = new StringParser(sAsset.text);

			while(sParser.IsRemain)
			{
				sParser.skipWhile('\t');

				if (sParser.tryMatchStr("//"))
				{
					sParser.skipUntil('\n');
					sParser.skipWhile(1);
					continue;
				}


			}
		}
	}
}