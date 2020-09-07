using System;
using System.IO;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x02000016 RID: 22
	public class BufferedWriter
	{
		// Token: 0x060000CB RID: 203 RVA: 0x0000681B File Offset: 0x00004A1B
		public BufferedWriter(StreamWriter w)
		{
			this._writer = w;
		}

		// Token: 0x060000CC RID: 204 RVA: 0x0000682C File Offset: 0x00004A2C
		public void Write(string s)
		{
			this._writer.Write(s);
		}

		// Token: 0x060000CD RID: 205 RVA: 0x0000683C File Offset: 0x00004A3C
		public void NewLine()
		{
			this._writer.WriteLine();
		}

		// Token: 0x060000CE RID: 206 RVA: 0x0000684B File Offset: 0x00004A4B
		public void Close()
		{
			this._writer.Close();
		}

		// Token: 0x0400004C RID: 76
		private StreamWriter _writer;
	}
}
