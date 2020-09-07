using System;

namespace SharpCifs.Smb
{
	// Token: 0x0200006E RID: 110
	internal interface IAllocInfo
	{
		// Token: 0x0600032C RID: 812
		long GetCapacity();

		// Token: 0x0600032D RID: 813
		long GetFree();
	}
}
