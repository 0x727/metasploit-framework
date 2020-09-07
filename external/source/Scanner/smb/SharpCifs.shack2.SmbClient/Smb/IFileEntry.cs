using System;

namespace SharpCifs.Smb
{
	// Token: 0x02000075 RID: 117
	public interface IFileEntry
	{
		// Token: 0x06000349 RID: 841
		string GetName();

		// Token: 0x0600034A RID: 842
		int GetType();

		// Token: 0x0600034B RID: 843
		int GetAttributes();

		// Token: 0x0600034C RID: 844
		long CreateTime();

		// Token: 0x0600034D RID: 845
		long LastModified();

		// Token: 0x0600034E RID: 846
		long Length();
	}
}
