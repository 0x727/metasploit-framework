using System;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x02000065 RID: 101
	public class StringTokenizer
	{
		// Token: 0x060002CE RID: 718 RVA: 0x0000B859 File Offset: 0x00009A59
		public StringTokenizer(string text, string delim)
		{
			this._tokens = text.Split(delim);
		}

		// Token: 0x060002CF RID: 719 RVA: 0x0000B870 File Offset: 0x00009A70
		public int CountTokens()
		{
			return this._tokens.Length;
		}

		// Token: 0x060002D0 RID: 720 RVA: 0x0000B88C File Offset: 0x00009A8C
		public string NextToken()
		{
			string result = this._tokens[this._pos];
			this._pos++;
			return result;
		}

		// Token: 0x060002D1 RID: 721 RVA: 0x0000B8BC File Offset: 0x00009ABC
		public bool HasMoreTokens()
		{
			return this._pos < this._tokens.Length;
		}

		// Token: 0x04000084 RID: 132
		private string[] _tokens;

		// Token: 0x04000085 RID: 133
		private int _pos;
	}
}
