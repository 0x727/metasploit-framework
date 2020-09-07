using System;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x02000034 RID: 52
	internal class UriSyntaxException : Exception
	{
		// Token: 0x06000123 RID: 291 RVA: 0x00007262 File Offset: 0x00005462
		public UriSyntaxException(string s, string msg) : base(s + " " + msg)
		{
		}
	}
}
