using System;
using System.IO;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x0200003C RID: 60
	public class FileInputStream : InputStream
	{
		// Token: 0x0600017A RID: 378 RVA: 0x00008321 File Offset: 0x00006521
		public FileInputStream(FilePath file) : this(file.GetPath())
		{
		}

		// Token: 0x0600017B RID: 379 RVA: 0x00008334 File Offset: 0x00006534
		public FileInputStream(string file)
		{
			bool flag = !File.Exists(file);
			if (flag)
			{
				throw new FileNotFoundException("File not found", file);
			}
			this.Wrapped = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		}
	}
}
