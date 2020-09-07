using System;

namespace SharpCifs.Smb
{
	// Token: 0x02000087 RID: 135
	public class SmbAuthException : SmbException
	{
		// Token: 0x060003FC RID: 1020 RVA: 0x00013065 File Offset: 0x00011265
		internal SmbAuthException(int errcode) : base(errcode, null)
		{
		}
	}
}
