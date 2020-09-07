using System;
using System.Diagnostics;
using System.Net;
using System.Security.Principal;

namespace LumiSoft.Net.Log
{
	// Token: 0x02000038 RID: 56
	public class Logger : IDisposable
	{
		// Token: 0x060001FB RID: 507 RVA: 0x000091B8 File Offset: 0x000081B8
		public void Dispose()
		{
		}

		// Token: 0x060001FC RID: 508 RVA: 0x0000CABD File Offset: 0x0000BABD
		public void AddRead(long size, string text)
		{
			this.OnWriteLog(new LogEntry(LogEntryType.Read, "", size, text));
		}

		// Token: 0x060001FD RID: 509 RVA: 0x0000CAD4 File Offset: 0x0000BAD4
		public void AddRead(string id, GenericIdentity userIdentity, long size, string text, IPEndPoint localEP, IPEndPoint remoteEP)
		{
			this.OnWriteLog(new LogEntry(LogEntryType.Read, id, userIdentity, size, text, localEP, remoteEP, (byte[])null));
		}

		// Token: 0x060001FE RID: 510 RVA: 0x0000CAFC File Offset: 0x0000BAFC
		public void AddRead(string id, GenericIdentity userIdentity, long size, string text, IPEndPoint localEP, IPEndPoint remoteEP, byte[] data)
		{
			this.OnWriteLog(new LogEntry(LogEntryType.Read, id, userIdentity, size, text, localEP, remoteEP, data));
		}

		// Token: 0x060001FF RID: 511 RVA: 0x0000CB22 File Offset: 0x0000BB22
		public void AddWrite(long size, string text)
		{
			this.OnWriteLog(new LogEntry(LogEntryType.Write, "", size, text));
		}

		// Token: 0x06000200 RID: 512 RVA: 0x0000CB3C File Offset: 0x0000BB3C
		public void AddWrite(string id, GenericIdentity userIdentity, long size, string text, IPEndPoint localEP, IPEndPoint remoteEP)
		{
			this.OnWriteLog(new LogEntry(LogEntryType.Write, id, userIdentity, size, text, localEP, remoteEP, (byte[])null));
		}

		// Token: 0x06000201 RID: 513 RVA: 0x0000CB64 File Offset: 0x0000BB64
		public void AddWrite(string id, GenericIdentity userIdentity, long size, string text, IPEndPoint localEP, IPEndPoint remoteEP, byte[] data)
		{
			this.OnWriteLog(new LogEntry(LogEntryType.Write, id, userIdentity, size, text, localEP, remoteEP, data));
		}

		// Token: 0x06000202 RID: 514 RVA: 0x0000CB8A File Offset: 0x0000BB8A
		public void AddText(string text)
		{
			this.OnWriteLog(new LogEntry(LogEntryType.Text, "", 0L, text));
		}

		// Token: 0x06000203 RID: 515 RVA: 0x0000CBA2 File Offset: 0x0000BBA2
		public void AddText(string id, string text)
		{
			this.OnWriteLog(new LogEntry(LogEntryType.Text, id, 0L, text));
		}

		// Token: 0x06000204 RID: 516 RVA: 0x0000CBB8 File Offset: 0x0000BBB8
		public void AddText(string id, GenericIdentity userIdentity, string text, IPEndPoint localEP, IPEndPoint remoteEP)
		{
			this.OnWriteLog(new LogEntry(LogEntryType.Text, id, userIdentity, 0L, text, localEP, remoteEP, (byte[])null));
		}

		// Token: 0x06000205 RID: 517 RVA: 0x0000CBE0 File Offset: 0x0000BBE0
		public void AddException(string id, GenericIdentity userIdentity, string text, IPEndPoint localEP, IPEndPoint remoteEP, Exception exception)
		{
			this.OnWriteLog(new LogEntry(LogEntryType.Exception, id, userIdentity, 0L, text, localEP, remoteEP, exception));
		}

		// Token: 0x1400000A RID: 10
		// (add) Token: 0x06000206 RID: 518 RVA: 0x0000CC08 File Offset: 0x0000BC08
		// (remove) Token: 0x06000207 RID: 519 RVA: 0x0000CC40 File Offset: 0x0000BC40
		
		public event EventHandler<WriteLogEventArgs> WriteLog = null;

		// Token: 0x06000208 RID: 520 RVA: 0x0000CC78 File Offset: 0x0000BC78
		private void OnWriteLog(LogEntry entry)
		{
			bool flag = this.WriteLog != null;
			if (flag)
			{
				this.WriteLog(this, new WriteLogEventArgs(entry));
			}
		}
	}
}
