using System;
using System.IO;

namespace SharpCifs.Dcerpc.Ndr
{
	// Token: 0x020000EA RID: 234
	public class NdrException : IOException
	{
		// Token: 0x060007BF RID: 1983 RVA: 0x000062BC File Offset: 0x000044BC
		public NdrException(string msg) : base(msg)
		{
		}

		// Token: 0x040004F6 RID: 1270
		public static readonly string NoNullRef = "ref pointer cannot be null";

		// Token: 0x040004F7 RID: 1271
		public static readonly string InvalidConformance = "invalid array conformance";
	}
}
