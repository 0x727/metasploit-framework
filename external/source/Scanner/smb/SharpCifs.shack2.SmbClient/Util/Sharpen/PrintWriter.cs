using System;
using System.IO;
using System.Text;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x0200005D RID: 93
	public class PrintWriter : TextWriter
	{
		// Token: 0x06000252 RID: 594 RVA: 0x0000A93E File Offset: 0x00008B3E
		public PrintWriter(FilePath path)
		{
			this._writer = new StreamWriter(path);
		}

		// Token: 0x06000253 RID: 595 RVA: 0x0000A959 File Offset: 0x00008B59
		public PrintWriter(TextWriter other)
		{
			this._writer = other;
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000254 RID: 596 RVA: 0x0000A96C File Offset: 0x00008B6C
		public override Encoding Encoding
		{
			get
			{
				return this._writer.Encoding;
			}
		}

		// Token: 0x06000255 RID: 597 RVA: 0x0000A989 File Offset: 0x00008B89
		public override void Close()
		{
			this._writer.Close();
		}

		// Token: 0x06000256 RID: 598 RVA: 0x0000A998 File Offset: 0x00008B98
		public override void Flush()
		{
			this._writer.Flush();
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000257 RID: 599 RVA: 0x0000A9A8 File Offset: 0x00008BA8
		public override IFormatProvider FormatProvider
		{
			get
			{
				return this._writer.FormatProvider;
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000258 RID: 600 RVA: 0x0000A9C8 File Offset: 0x00008BC8
		// (set) Token: 0x06000259 RID: 601 RVA: 0x0000A9E5 File Offset: 0x00008BE5
		public override string NewLine
		{
			get
			{
				return this._writer.NewLine;
			}
			set
			{
				this._writer.NewLine = value;
			}
		}

		// Token: 0x0600025A RID: 602 RVA: 0x0000A9F5 File Offset: 0x00008BF5
		public override void Write(char[] buffer, int index, int count)
		{
			this._writer.Write(buffer, index, count);
		}

		// Token: 0x0600025B RID: 603 RVA: 0x0000AA07 File Offset: 0x00008C07
		public override void Write(char[] buffer)
		{
			this._writer.Write(buffer);
		}

		// Token: 0x0600025C RID: 604 RVA: 0x0000AA17 File Offset: 0x00008C17
		public new void Write(string format, object arg0, object arg1, object arg2)
		{
			this._writer.Write(format, arg0, arg1, arg2);
		}

		// Token: 0x0600025D RID: 605 RVA: 0x0000AA2B File Offset: 0x00008C2B
		public override void Write(string format, object arg0, object arg1)
		{
			this._writer.Write(format, arg0, arg1);
		}

		// Token: 0x0600025E RID: 606 RVA: 0x0000AA3D File Offset: 0x00008C3D
		public override void Write(string format, object arg0)
		{
			this._writer.Write(format, arg0);
		}

		// Token: 0x0600025F RID: 607 RVA: 0x0000AA4E File Offset: 0x00008C4E
		public override void Write(string format, params object[] arg)
		{
			this._writer.Write(format, arg);
		}

		// Token: 0x06000260 RID: 608 RVA: 0x0000AA5F File Offset: 0x00008C5F
		public override void WriteLine(char[] buffer, int index, int count)
		{
			this._writer.WriteLine(buffer, index, count);
		}

		// Token: 0x06000261 RID: 609 RVA: 0x0000AA71 File Offset: 0x00008C71
		public override void WriteLine(char[] buffer)
		{
			this._writer.WriteLine(buffer);
		}

		// Token: 0x06000262 RID: 610 RVA: 0x0000AA81 File Offset: 0x00008C81
		public new void WriteLine(string format, object arg0, object arg1, object arg2)
		{
			this._writer.WriteLine(format, arg0, arg1, arg2);
		}

		// Token: 0x06000263 RID: 611 RVA: 0x0000AA95 File Offset: 0x00008C95
		public override void WriteLine(string format, object arg0, object arg1)
		{
			this._writer.WriteLine(format, arg0, arg1);
		}

		// Token: 0x06000264 RID: 612 RVA: 0x0000AAA7 File Offset: 0x00008CA7
		public override void WriteLine(string format, object arg0)
		{
			this._writer.WriteLine(format, arg0);
		}

		// Token: 0x06000265 RID: 613 RVA: 0x0000AAB8 File Offset: 0x00008CB8
		public override void WriteLine(string format, params object[] arg)
		{
			this._writer.WriteLine(format, arg);
		}

		// Token: 0x06000266 RID: 614 RVA: 0x0000AAC9 File Offset: 0x00008CC9
		public override void WriteLine(ulong value)
		{
			this._writer.WriteLine(value);
		}

		// Token: 0x06000267 RID: 615 RVA: 0x0000AAD9 File Offset: 0x00008CD9
		public override void WriteLine(uint value)
		{
			this._writer.WriteLine(value);
		}

		// Token: 0x06000268 RID: 616 RVA: 0x0000AAE9 File Offset: 0x00008CE9
		public override void WriteLine(string value)
		{
			this._writer.WriteLine(value);
		}

		// Token: 0x06000269 RID: 617 RVA: 0x0000AAF9 File Offset: 0x00008CF9
		public override void WriteLine(float value)
		{
			this._writer.WriteLine(value);
		}

		// Token: 0x0600026A RID: 618 RVA: 0x0000AB09 File Offset: 0x00008D09
		public override void WriteLine(object value)
		{
			this._writer.WriteLine(value);
		}

		// Token: 0x0600026B RID: 619 RVA: 0x0000AB19 File Offset: 0x00008D19
		public override void WriteLine(long value)
		{
			this._writer.WriteLine(value);
		}

		// Token: 0x0600026C RID: 620 RVA: 0x0000AB29 File Offset: 0x00008D29
		public override void WriteLine(int value)
		{
			this._writer.WriteLine(value);
		}

		// Token: 0x0600026D RID: 621 RVA: 0x0000AB39 File Offset: 0x00008D39
		public override void WriteLine(double value)
		{
			this._writer.WriteLine(value);
		}

		// Token: 0x0600026E RID: 622 RVA: 0x0000AB49 File Offset: 0x00008D49
		public override void WriteLine(decimal value)
		{
			this._writer.WriteLine(value);
		}

		// Token: 0x0600026F RID: 623 RVA: 0x0000AB59 File Offset: 0x00008D59
		public override void WriteLine(char value)
		{
			this._writer.WriteLine(value);
		}

		// Token: 0x06000270 RID: 624 RVA: 0x0000AB69 File Offset: 0x00008D69
		public override void WriteLine(bool value)
		{
			this._writer.WriteLine(value);
		}

		// Token: 0x06000271 RID: 625 RVA: 0x0000AB79 File Offset: 0x00008D79
		public override void WriteLine()
		{
			this._writer.WriteLine();
		}

		// Token: 0x06000272 RID: 626 RVA: 0x0000AB88 File Offset: 0x00008D88
		public override void Write(bool value)
		{
			this._writer.Write(value);
		}

		// Token: 0x06000273 RID: 627 RVA: 0x0000AB98 File Offset: 0x00008D98
		public override void Write(char value)
		{
			this._writer.Write(value);
		}

		// Token: 0x06000274 RID: 628 RVA: 0x0000ABA8 File Offset: 0x00008DA8
		public override void Write(decimal value)
		{
			this._writer.Write(value);
		}

		// Token: 0x06000275 RID: 629 RVA: 0x0000ABB8 File Offset: 0x00008DB8
		public override void Write(double value)
		{
			this._writer.Write(value);
		}

		// Token: 0x06000276 RID: 630 RVA: 0x0000ABC8 File Offset: 0x00008DC8
		public override void Write(int value)
		{
			this._writer.Write(value);
		}

		// Token: 0x06000277 RID: 631 RVA: 0x0000ABD8 File Offset: 0x00008DD8
		public override void Write(long value)
		{
			this._writer.Write(value);
		}

		// Token: 0x06000278 RID: 632 RVA: 0x0000ABE8 File Offset: 0x00008DE8
		public override void Write(object value)
		{
			this._writer.Write(value);
		}

		// Token: 0x06000279 RID: 633 RVA: 0x0000ABF8 File Offset: 0x00008DF8
		public override void Write(float value)
		{
			this._writer.Write(value);
		}

		// Token: 0x0600027A RID: 634 RVA: 0x0000AC08 File Offset: 0x00008E08
		public override void Write(string value)
		{
			this._writer.Write(value);
		}

		// Token: 0x0600027B RID: 635 RVA: 0x0000AC18 File Offset: 0x00008E18
		public override void Write(uint value)
		{
			this._writer.Write(value);
		}

		// Token: 0x0600027C RID: 636 RVA: 0x0000AC28 File Offset: 0x00008E28
		public override void Write(ulong value)
		{
			this._writer.Write(value);
		}

		// Token: 0x0400007A RID: 122
		private TextWriter _writer;
	}
}
