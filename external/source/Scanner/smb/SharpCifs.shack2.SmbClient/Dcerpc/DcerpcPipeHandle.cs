using System;
using System.IO;
using SharpCifs.Smb;
using SharpCifs.Util;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Dcerpc
{
	// Token: 0x020000E4 RID: 228
	public class DcerpcPipeHandle : DcerpcHandle
	{
		// Token: 0x06000790 RID: 1936 RVA: 0x000298CC File Offset: 0x00027ACC
		public DcerpcPipeHandle(string url, NtlmPasswordAuthentication auth)
		{
			this.Binding = DcerpcHandle.ParseBinding(url);
			url = "smb://" + this.Binding.Server + "/IPC$/" + Runtime.Substring(this.Binding.Endpoint, 6);
			string text = string.Empty;
			string text2 = (string)this.Binding.GetOption("server");
			bool flag = text2 != null;
			if (flag)
			{
				text = text + "&server=" + text2;
			}
			string str = (string)this.Binding.GetOption("address");
			bool flag2 = text2 != null;
			if (flag2)
			{
				text = text + "&address=" + str;
			}
			bool flag3 = text.Length > 0;
			if (flag3)
			{
				url = url + "?" + Runtime.Substring(text, 1);
			}
			this.Pipe = new SmbNamedPipe(url, 27198979, auth);
		}

		// Token: 0x06000791 RID: 1937 RVA: 0x000299BC File Offset: 0x00027BBC
		protected internal override void DoSendFragment(byte[] buf, int off, int length, bool isDirect)
		{
			bool flag = this.Out != null && !this.Out.IsOpen();
			if (flag)
			{
				throw new IOException("DCERPC pipe is no longer open");
			}
			bool flag2 = this.In == null;
			if (flag2)
			{
				this.In = (SmbFileInputStream)this.Pipe.GetNamedPipeInputStream();
			}
			bool flag3 = this.Out == null;
			if (flag3)
			{
				this.Out = (SmbFileOutputStream)this.Pipe.GetNamedPipeOutputStream();
			}
			if (isDirect)
			{
				this.Out.WriteDirect(buf, off, length, 1);
			}
			else
			{
				this.Out.Write(buf, off, length);
			}
		}

		// Token: 0x06000792 RID: 1938 RVA: 0x00029A68 File Offset: 0x00027C68
		protected internal override void DoReceiveFragment(byte[] buf, bool isDirect)
		{
			bool flag = buf.Length < this.MaxRecv;
			if (flag)
			{
				throw new ArgumentException("buffer too small");
			}
			bool flag2 = this.IsStart && !isDirect;
			int i;
			if (flag2)
			{
				i = this.In.Read(buf, 0, 1024);
			}
			else
			{
				i = this.In.ReadDirect(buf, 0, buf.Length);
			}
			bool flag3 = buf[0] != 5 && buf[1] > 0;
			if (flag3)
			{
				throw new IOException("Unexpected DCERPC PDU header");
			}
			int num = (int)(buf[3] & byte.MaxValue);
			this.IsStart = ((num & DcerpcConstants.DcerpcLastFrag) == DcerpcConstants.DcerpcLastFrag);
			int num2 = (int)Encdec.Dec_uint16le(buf, 8);
			bool flag4 = num2 > this.MaxRecv;
			if (flag4)
			{
				throw new IOException("Unexpected fragment length: " + num2);
			}
			while (i < num2)
			{
				i += this.In.ReadDirect(buf, i, num2 - i);
			}
		}

		// Token: 0x06000793 RID: 1939 RVA: 0x00029B60 File Offset: 0x00027D60
		public override void Close()
		{
			this.State = 0;
			bool flag = this.Out != null;
			if (flag)
			{
				this.Out.Close();
			}
		}

		// Token: 0x040004E9 RID: 1257
		internal SmbNamedPipe Pipe;

		// Token: 0x040004EA RID: 1258
		internal SmbFileInputStream In;

		// Token: 0x040004EB RID: 1259
		internal SmbFileOutputStream Out;

		// Token: 0x040004EC RID: 1260
		internal bool IsStart = true;
	}
}
