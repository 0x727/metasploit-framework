using System;
using System.IO;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x02000015 RID: 21
	public class BufferedReader : StreamReader
	{
		// Token: 0x060000CA RID: 202 RVA: 0x0000680B File Offset: 0x00004A0B
		public BufferedReader(InputStreamReader r) : base(r.BaseStream)
		{
		}
	}
}
