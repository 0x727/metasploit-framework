using System;

namespace SharpCifs.Smb
{
	// Token: 0x02000076 RID: 118
	internal interface IInfo
	{
		// Token: 0x0600034F RID: 847
		int GetAttributes();

		// Token: 0x06000350 RID: 848
		long GetCreateTime();

		// Token: 0x06000351 RID: 849
		long GetLastWriteTime();

		// Token: 0x06000352 RID: 850
		long GetSize();
	}
}
