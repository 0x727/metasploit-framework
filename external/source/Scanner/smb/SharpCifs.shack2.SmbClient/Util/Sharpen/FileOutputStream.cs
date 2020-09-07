using System;
using System.IO;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x0200003D RID: 61
	internal class FileOutputStream : OutputStream
	{
		// Token: 0x0600017C RID: 380 RVA: 0x00008372 File Offset: 0x00006572
		public FileOutputStream(FilePath file) : this(file.GetPath(), false)
		{
		}

		// Token: 0x0600017D RID: 381 RVA: 0x00008383 File Offset: 0x00006583
		public FileOutputStream(string file) : this(file, false)
		{
		}

		// Token: 0x0600017E RID: 382 RVA: 0x0000838F File Offset: 0x0000658F
		public FileOutputStream(FilePath file, bool append) : this(file.GetPath(), append)
		{
		}

		// Token: 0x0600017F RID: 383 RVA: 0x000083A0 File Offset: 0x000065A0
		public FileOutputStream(string file, bool append)
		{
			try
			{
				if (append)
				{
					this.Wrapped = File.Open(file, FileMode.Append, FileAccess.Write);
				}
				else
				{
					this.Wrapped = File.Open(file, FileMode.Create, FileAccess.Write);
				}
			}
			catch (DirectoryNotFoundException)
			{
				throw new FileNotFoundException("File not found: " + file);
			}
		}
	}
}
