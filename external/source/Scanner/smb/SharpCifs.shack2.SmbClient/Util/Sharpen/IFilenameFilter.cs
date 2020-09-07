using System;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x02000048 RID: 72
	public interface IFilenameFilter
	{
		// Token: 0x060001C9 RID: 457
		bool Accept(FilePath dir, string name);
	}
}
