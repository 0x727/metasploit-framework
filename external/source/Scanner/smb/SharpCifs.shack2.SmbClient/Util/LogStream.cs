using System;
using System.IO;
using System.Text;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Util
{
	// Token: 0x0200000C RID: 12
	public class LogStream : PrintWriter
	{
		// Token: 0x06000072 RID: 114 RVA: 0x00004ED5 File Offset: 0x000030D5
		public void SetLevel(int level)
		{
			this.Level = level;
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00004EDF File Offset: 0x000030DF
		public LogStream(TextWriter other) : base(other)
		{
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00004EF1 File Offset: 0x000030F1
		public static void SetInstance(TextWriter other)
		{
			LogStream._inst = new LogStream(other);
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00004F00 File Offset: 0x00003100
		public static LogStream GetInstance()
		{
			bool flag = LogStream._inst == null;
			if (flag)
			{
				LogStream.SetInstance(Console.Error);
			}
			return LogStream._inst;
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000076 RID: 118 RVA: 0x00002380 File Offset: 0x00000580
		public override Encoding Encoding
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x04000039 RID: 57
		private static LogStream _inst = null;

		// Token: 0x0400003A RID: 58
		public int Level = 1;
	}
}
