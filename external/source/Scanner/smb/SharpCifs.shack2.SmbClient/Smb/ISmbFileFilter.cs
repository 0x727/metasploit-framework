using System;

namespace SharpCifs.Smb
{
	// Token: 0x020000AB RID: 171
	public interface ISmbFileFilter
	{
		// Token: 0x06000556 RID: 1366
		bool Accept(SmbFile file);
	}
}
