using System;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x0200002F RID: 47
	public class ParseException : Exception
	{
		// Token: 0x06000118 RID: 280 RVA: 0x000071F6 File Offset: 0x000053F6
		public ParseException()
		{
		}

		// Token: 0x06000119 RID: 281 RVA: 0x00007237 File Offset: 0x00005437
		public ParseException(string msg, int errorOffset) : base(string.Format("Msg: {0}. Error Offset: {1}", msg, errorOffset))
		{
		}
	}
}
