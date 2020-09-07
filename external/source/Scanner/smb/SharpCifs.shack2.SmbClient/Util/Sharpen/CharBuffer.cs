using System;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x02000017 RID: 23
	internal class CharBuffer : CharSequence
	{
		// Token: 0x060000CF RID: 207 RVA: 0x0000685C File Offset: 0x00004A5C
		public override string ToString()
		{
			return this.Wrapped;
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x00006874 File Offset: 0x00004A74
		public static CharBuffer Wrap(string str)
		{
			return new CharBuffer
			{
				Wrapped = str
			};
		}

		// Token: 0x0400004D RID: 77
		internal string Wrapped;
	}
}
