using System;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x02000019 RID: 25
	internal class StringCharSequence : CharSequence
	{
		// Token: 0x060000D5 RID: 213 RVA: 0x000068D5 File Offset: 0x00004AD5
		public StringCharSequence(string str)
		{
			this._str = str;
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x000068E8 File Offset: 0x00004AE8
		public override string ToString()
		{
			return this._str;
		}

		// Token: 0x0400004E RID: 78
		private string _str;
	}
}
