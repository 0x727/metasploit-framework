using System;
using System.IO;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x02000040 RID: 64
	internal class FileWriter : StreamWriter
	{
		// Token: 0x060001AE RID: 430 RVA: 0x00008B07 File Offset: 0x00006D07
		public FileWriter(FilePath path) : base(path.GetPath())
		{
		}

		// Token: 0x060001AF RID: 431 RVA: 0x00008B18 File Offset: 0x00006D18
		public FileWriter Append(string sequence)
		{
			this.Write(sequence);
			return this;
		}
	}
}
