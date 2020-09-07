using System;
using System.Text.RegularExpressions;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x02000051 RID: 81
	internal class Matcher
	{
		// Token: 0x060001F3 RID: 499 RVA: 0x000091E9 File Offset: 0x000073E9
		internal Matcher(Regex regex, string str)
		{
			this._regex = regex;
			this._str = str;
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x00009204 File Offset: 0x00007404
		public int End()
		{
			bool flag = this._matches == null || this._current >= this._matches.Count;
			if (flag)
			{
				throw new InvalidOperationException();
			}
			return this._matches[this._current].Index + this._matches[this._current].Length;
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x00009270 File Offset: 0x00007470
		public bool Find()
		{
			bool flag = this._matches == null;
			if (flag)
			{
				this._matches = this._regex.Matches(this._str);
				this._current = 0;
			}
			return this._current < this._matches.Count;
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x000092C4 File Offset: 0x000074C4
		public bool Find(int index)
		{
			this._matches = this._regex.Matches(this._str, index);
			this._current = 0;
			return this._matches.Count > 0;
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x00009304 File Offset: 0x00007504
		public string Group(int n)
		{
			bool flag = this._matches == null || this._current >= this._matches.Count;
			if (flag)
			{
				throw new InvalidOperationException();
			}
			Group group = this._matches[this._current].Groups[n];
			return group.Success ? group.Value : null;
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x00009370 File Offset: 0x00007570
		public bool Matches()
		{
			this._matches = null;
			return this.Find();
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x00009390 File Offset: 0x00007590
		public string ReplaceFirst(string txt)
		{
			return this._regex.Replace(this._str, txt, 1);
		}

		// Token: 0x060001FA RID: 506 RVA: 0x000093B8 File Offset: 0x000075B8
		public Matcher Reset(CharSequence str)
		{
			return this.Reset(str.ToString());
		}

		// Token: 0x060001FB RID: 507 RVA: 0x000093D8 File Offset: 0x000075D8
		public Matcher Reset(string str)
		{
			this._matches = null;
			this._str = str;
			return this;
		}

		// Token: 0x060001FC RID: 508 RVA: 0x000093FC File Offset: 0x000075FC
		public int Start()
		{
			bool flag = this._matches == null || this._current >= this._matches.Count;
			if (flag)
			{
				throw new InvalidOperationException();
			}
			return this._matches[this._current].Index;
		}

		// Token: 0x04000062 RID: 98
		private int _current;

		// Token: 0x04000063 RID: 99
		private MatchCollection _matches;

		// Token: 0x04000064 RID: 100
		private Regex _regex;

		// Token: 0x04000065 RID: 101
		private string _str;
	}
}
