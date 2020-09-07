using System;

namespace SharpCifs.Smb
{
	// Token: 0x020000AD RID: 173
	public interface ISmbFilenameFilter
	{
		// Token: 0x06000563 RID: 1379
		bool Accept(SmbFile dir, string name);
	}
}
