using System;
using System.Text;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x02000018 RID: 24
	public class CharSequence
	{
		// Token: 0x060000D2 RID: 210 RVA: 0x000068A0 File Offset: 0x00004AA0
		public static implicit operator CharSequence(string str)
		{
			return new StringCharSequence(str);
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x000068B8 File Offset: 0x00004AB8
		public static implicit operator CharSequence(StringBuilder str)
		{
			return new StringCharSequence(str.ToString());
		}
	}
}
