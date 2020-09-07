using System;
using System.Collections.Generic;
using System.IO;
using LumiSoft.Net.TCP;

namespace LumiSoft.Net.NNTP.Client
{
	// Token: 0x0200002B RID: 43
	public class NNTP_Client : TCP_Client
	{
		// Token: 0x06000146 RID: 326 RVA: 0x000092D8 File Offset: 0x000082D8
		public override void Disconnect()
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("NNTP client is not connected.");
			}
			try
			{
				base.WriteLine("QUIT");
			}
			catch
			{
			}
			try
			{
				base.Disconnect();
			}
			catch
			{
			}
		}

		// Token: 0x06000147 RID: 327 RVA: 0x00009360 File Offset: 0x00008360
		public string[] GetNewsGroups()
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("NNTP client is not connected.");
			}
			base.WriteLine("LIST");
			string text = base.ReadLine();
			bool flag2 = !text.StartsWith("215");
			if (flag2)
			{
				throw new Exception(text);
			}
			List<string> list = new List<string>();
			text = base.ReadLine();
			while (text != ".")
			{
				list.Add(text.Split(new char[]
				{
					' '
				})[0]);
				text = base.ReadLine();
			}
			return list.ToArray();
		}

		// Token: 0x06000148 RID: 328 RVA: 0x00009424 File Offset: 0x00008424
		public void PostMessage(string newsgroup, Stream message)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("NNTP client is not connected.");
			}
			base.WriteLine("POST");
			string text = base.ReadLine();
			bool flag2 = !text.StartsWith("340");
			if (flag2)
			{
				throw new Exception(text);
			}
			this.TcpStream.WritePeriodTerminated(message);
			text = base.ReadLine();
			bool flag3 = !text.StartsWith("240");
			if (flag3)
			{
				throw new Exception(text);
			}
		}

		// Token: 0x06000149 RID: 329 RVA: 0x000094C4 File Offset: 0x000084C4
		protected override void OnConnected()
		{
			string text = base.ReadLine();
			bool flag = !text.StartsWith("200");
			if (flag)
			{
				throw new Exception(text);
			}
		}
	}
}
