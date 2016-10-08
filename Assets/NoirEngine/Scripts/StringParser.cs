using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noir.Util
{
	/// <summary>
	/// 문자열 파싱용 유틸리티 클래스입니다.
	/// </summary>
	public class StringParser
	{
		private int nLine;
		private int nIndex;
		private string sString;

		/// <summary>
		/// 현재 처리중인 줄 번호입니다.
		/// </summary>
		public int Line { get { return this.nLine; } }

		/// <summary>
		/// 현재 처리중인 위치입니다.
		/// </summary>
		public int Index { get { return this.nIndex; } }

		/// <summary>
		/// 현재 처리중인 전체 문자열입니다.
		/// </summary>
		public string String { get { return this.sString; } }

		/// <summary>
		/// 처리 완료 여부입니다.
		/// </summary>
		public bool IsEnd { get { return this.nIndex >= this.sString.Length; } }

		/// <summary>
		/// 처리 진행 여부입니다.
		/// </summary>
		public bool IsRemain { get { return this.nIndex < this.sString.Length; } }

		/// <summary>
		/// 현재 처리중인 문자가 여백문자가 아닌지의 여부입니다.
		/// </summary>
		public bool IsBlackSpace { get { return this.IsRemain && !Char.IsWhiteSpace(this.CharacterUnsafe); } }

		/// <summary>
		/// 현재 처리중인 문자가 여백문자인지의 여부입니다.
		/// </summary>
		public bool IsWhiteSpace { get { return this.IsRemain && Char.IsWhiteSpace(this.CharacterUnsafe); } }

		/// <summary>
		/// 남은 문자의 개수를 가져옵니다.
		/// </summary>
		public int RemainCount { get { return Math.Max(this.sString.Length - this.nIndex, 0); } }

		/// <summary>
		/// 현재 처리중인 문자를 가져옵니다.
		/// </summary>
		public int Character { get { return this.IsRemain ? this.sString[this.nIndex] : -1; } }

		/// <summary>
		/// 현재 처리중인 문자를 가져옵니다.
		/// </summary>
		public char CharacterUnsafe { get { return this.sString[this.nIndex]; } }

		/// <summary>
		/// 새 StringParser 인스턴스를 주어진 문자열로 초기화합니다.
		/// </summary>
		/// <param name="sTarget">처리할 대상 문자열입니다.</param>
		public StringParser(string sTarget)
		{
			this.nLine = 1;
			this.nIndex = 0;
			this.sString = sTarget;
		}

		/// <summary>
		/// 현재 처리중인 문자가 지정된 문자와 일치하는지 여부를 가져옵니다.
		/// </summary>
		/// <param name="nChar">비교할 문자입니다.</param>
		/// <returns>처리가 완료된 상태거나 두 문자가 다르다면 false, 같다면 true를 반환합니다.</returns>
		public bool tryMatchChar(char nChar)
		{
			return this.IsRemain && this.CharacterUnsafe == nChar;
		}

		/// <summary>
		/// 현재 처리중인 문자가 지정된 문자들과 일치하는지 여부를 가져옵니다.
		/// </summary>
		/// <param name="sChar">비교할 문자들입니다.</param>
		/// <returns>처리가 완료된 상태거나 일치하는 문자가 없다면 false, 하나라도 일치한다면 true를 반환합니다.</returns>
		public bool tryMatchChar(string sChar)
		{
			return this.IsRemain && sChar.IndexOf(this.CharacterUnsafe) >= 0;
		}

		/// <summary>
		/// 현재 처리중인 문자열이 지정된 문자열과 일치하는지 여부를 가져옵니다.
		/// </summary>
		/// <param name="sTarget">비교할 문자열입니다.</param>
		/// <returns>처리가 완료된 상태거나 일치하지 않는다면 false, 일치한다면 true를 반환합니다.</returns>
		public bool tryMatchStr(string sTarget)
		{
			if (this.RemainCount < sTarget.Length)
				return false;

			for (int nFirst = this.nIndex, nSecond = 0; nSecond < sString.Length; ++nFirst, ++nSecond)
				if (this.sString[nFirst] != sTarget[nSecond])
					return false;

			return true;
		}

		/// <summary>
		/// 현재 처리중인 문자가 지정된 문자와 일치하지 않는지 여부를 가져옵니다.
		/// </summary>
		/// <param name="nChar">비교할 문자입니다.</param>
		/// <returns>처리가 완료된 상태거나 두 문자가 같다면 true, 다르다면 false를 반환합니다.</returns>
		public bool tryNotMatchChar(char nChar)
		{
			return this.IsRemain && this.CharacterUnsafe != nChar;
		}

		/// <summary>
		/// 현재 처리중인 문자가 지정된 문자들과 일치하지 않는지 여부를 가져옵니다.
		/// </summary>
		/// <param name="sChar">비교할 문자들입니다.</param>
		/// <returns>처리가 완료된 상태거나 하나라도 일치한다면 false, 일치하는 문자가 없다면 true를 반환합니다.</returns>
		public bool tryNotMatchChar(string sChar)
		{
			return this.IsRemain && sChar.IndexOf(this.CharacterUnsafe) < 0;
		}

		/// <summary>
		/// 현재 처리중인 문자와 지정된 문자가 일치하는 동안 문자를 무시합니다.
		/// </summary>
		/// <param name="nChar">무시할 문자입니다.</param>
		public void skipWhile(char nChar)
		{
			while (this.tryMatchChar(nChar))
			{
				if (this.CharacterUnsafe == '\n')
					++this.nLine;

				++this.nIndex;
			}
		}

		/// <summary>
		/// 현재 처리중인 문자와 지정된 문자들이 일치하는 동안 문자를 무시합니다.
		/// </summary>
		/// <param name="sChar">무시할 문자들입니다.</param>
		public void skipWhile(string sChar)
		{
			while (this.tryMatchChar(sChar))
			{
				if (this.CharacterUnsafe == '\n')
					++this.nLine;

				++this.nIndex;
			}
		}

		/// <summary>
		/// 문자를 일정 개수만큼 무시합니다.
		/// </summary>
		/// <param name="nCount">무시할 문자의 개수입니다.</param>
		public void skipWhile(int nCount)
		{
			for (; this.IsRemain && nCount > 0; --nCount)
			{
				if (this.CharacterUnsafe == '\n')
					++this.nLine;

				++this.nIndex;
			}
		}

		/// <summary>
		/// 여백문자가 나타날 때 까지 모든 문자를 무시합니다.
		/// </summary>
		public void skipBlackspace()
		{
			while (this.IsBlackSpace)
				++this.nIndex;
		}

		/// <summary>
		/// 여백문자가 아닌 문자가 나타날 때 까지 모든 문자를 무시합니다.
		/// </summary>
		public void skipWhitespace()
		{
			while (this.IsWhiteSpace)
			{
				if (this.CharacterUnsafe == '\n')
					++this.nLine;

				++this.nIndex;
			}
		}

		/// <summary>
		/// 특정 문자가 나올 때 까지 문자를 무시합니다.
		/// </summary>
		/// <param name="nChar">나오기를 기다릴 문자입니다.</param>
		public void skipUntil(char nChar)
		{
			while (this.tryNotMatchChar(nChar))
			{
				if (this.CharacterUnsafe == '\n')
					++this.nLine;

				++this.nIndex;
			}
		}

		/// <summary>
		/// 특정 문자들이 나올 때 까지 문자를 무시합니다.
		/// </summary>
		/// <param name="sChar">나오기를 기다릴 문자들입니다.</param>
		public void skipUntil(string sChar)
		{
			while (this.tryNotMatchChar(sChar))
			{
				if (this.CharacterUnsafe == '\n')
					++this.nLine;

				++this.nIndex;
			}
		}

		/// <summary>
		/// 특정 문자가 나올 때 까지 문자를 병합해 가져옵니다.
		/// </summary>
		/// <param name="nChar">나오기를 기다릴 문자입니다.</param>
		/// <returns>병합된 문자열입니다.</returns>
		public string mergeUntil(char nChar)
		{
			StringBuilder sBuilder = new StringBuilder();

			while (this.tryMatchChar(nChar))
			{
				sBuilder.Append(this.CharacterUnsafe);

				if (this.CharacterUnsafe == '\n')
					++this.nLine;

				++this.nIndex;
			}

			return sBuilder.ToString();
		}

		/// <summary>
		/// 특정 문자들이 나올 때 까지 문자를 병합해 가져옵니다.
		/// </summary>
		/// <param name="sChar">나오기를 기다릴 문자들입니다.</param>
		/// <returns>병합된 문자열입니다.</returns>
		public string mergeUntil(string sChar)
		{
			StringBuilder sBuilder = new StringBuilder();

			while (this.tryMatchChar(sChar))
			{
				sBuilder.Append(this.CharacterUnsafe);

				if (this.CharacterUnsafe == '\n')
					++this.nLine;

				++this.nIndex;
			}

			return sBuilder.ToString();
		}

		/// <summary>
		/// 여백문자가 나타날 때 까지 모든 문자를 병합해 가져옵니다.
		/// </summary>
		/// <returns>병합된 문자열입니다.</returns>
		public string mergeBlackspace()
		{
			StringBuilder sBuilder = new StringBuilder();

			while (this.IsBlackSpace)
			{
				sBuilder.Append(this.CharacterUnsafe);
				++this.nIndex;
			}

			return sBuilder.ToString();
		}

		/// <summary>
		/// 여백문자 또는 특정 문자들이 나타날 때 까지 모든 문자를 병합해 가져옵니다.
		/// </summary>
		/// <param name="sChar">나타나기를 기다릴 문자들입니다.</param>
		/// <returns>병합된 문자열입니다.</returns>
		public string mergeBlackspace(string sChar)
		{
			StringBuilder sBuilder = new StringBuilder();

			while (this.IsBlackSpace && this.tryNotMatchChar(sChar))
			{
				sBuilder.Append(this.CharacterUnsafe);
				++this.nIndex;
			}

			return sBuilder.ToString();
		}

		/// <summary>
		/// 여백문자가 아닌 문자가 나타날 때 까지 모든 문자를 병합해 가져옵니다.
		/// </summary>
		/// <returns>병합된 문자열입니다.</returns>
		public string mergeWhitespace()
		{
			StringBuilder sBuilder = new StringBuilder();

			while (this.IsWhiteSpace)
			{
				sBuilder.Append(this.CharacterUnsafe);

				if (this.CharacterUnsafe == '\n')
					++this.nLine;

				++this.nIndex;
			}

			return sBuilder.ToString();
		}

		/// <summary>
		/// 여백문자가 아닌 문자 또는 특정 문자들이 나타날 때 까지 모든 문자를 병합해 가져옵니다.
		/// </summary>
		/// <param name="sChar">나타나기를 기다릴 문자들입니다.</param>
		/// <returns>병합된 문자열입니다.</returns>
		public string mergeWhitespace(string sChar)
		{
			StringBuilder sBuilder = new StringBuilder();

			while (this.IsWhiteSpace && this.tryNotMatchChar(sChar))
			{
				sBuilder.Append(this.CharacterUnsafe);

				if (this.CharacterUnsafe == '\n')
					++this.nLine;

				++this.nIndex;
			}

			return sBuilder.ToString();
		}
	}
}