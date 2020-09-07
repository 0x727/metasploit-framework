using System;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x0200003F RID: 63
	public class FileReader : InputStreamReader
	{
		// Token: 0x060001AD RID: 429 RVA: 0x00008AF7 File Offset: 0x00006CF7
		public FileReader(FilePath f) : base(f.GetPath())
		{
		}
	}
}
