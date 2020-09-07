using System;
using System.Collections;
using System.Collections.Generic;

namespace LumiSoft.Net.POP3.Client
{
	// Token: 0x020000E6 RID: 230
	public class POP3_ClientMessageCollection : IEnumerable, IDisposable
	{
		// Token: 0x06000941 RID: 2369 RVA: 0x00037A68 File Offset: 0x00036A68
		internal POP3_ClientMessageCollection(POP3_Client pop3)
		{
			this.m_pPop3Client = pop3;
			this.m_pMessages = new List<POP3_ClientMessage>();
		}

		// Token: 0x06000942 RID: 2370 RVA: 0x00037A9C File Offset: 0x00036A9C
		public void Dispose()
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				this.m_IsDisposed = true;
				foreach (POP3_ClientMessage pop3_ClientMessage in this.m_pMessages)
				{
					pop3_ClientMessage.Dispose();
				}
				this.m_pMessages = null;
			}
		}

		// Token: 0x06000943 RID: 2371 RVA: 0x00037B10 File Offset: 0x00036B10
		internal void Add(int size)
		{
			this.m_pMessages.Add(new POP3_ClientMessage(this.m_pPop3Client, this.m_pMessages.Count + 1, size));
		}

		// Token: 0x06000944 RID: 2372 RVA: 0x00037B38 File Offset: 0x00036B38
		public IEnumerator GetEnumerator()
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			return this.m_pMessages.GetEnumerator();
		}

		// Token: 0x17000324 RID: 804
		// (get) Token: 0x06000945 RID: 2373 RVA: 0x00037B78 File Offset: 0x00036B78
		public long TotalSize
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				long num = 0L;
				foreach (POP3_ClientMessage pop3_ClientMessage in this.m_pMessages)
				{
					num += (long)pop3_ClientMessage.Size;
				}
				return num;
			}
		}

		// Token: 0x17000325 RID: 805
		// (get) Token: 0x06000946 RID: 2374 RVA: 0x00037BFC File Offset: 0x00036BFC
		public int Count
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pMessages.Count;
			}
		}

		// Token: 0x17000326 RID: 806
		public POP3_ClientMessage this[int index]
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = index < 0 || index > this.m_pMessages.Count;
				if (flag)
				{
					throw new ArgumentOutOfRangeException();
				}
				return this.m_pMessages[index];
			}
		}

		// Token: 0x17000327 RID: 807
		public POP3_ClientMessage this[string uid]
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = !this.m_pPop3Client.IsUidlSupported;
				if (flag)
				{
					throw new NotSupportedException();
				}
				foreach (POP3_ClientMessage pop3_ClientMessage in this.m_pMessages)
				{
					bool flag2 = pop3_ClientMessage.UID == uid;
					if (flag2)
					{
						return pop3_ClientMessage;
					}
				}
				return null;
			}
		}

		// Token: 0x04000426 RID: 1062
		private POP3_Client m_pPop3Client = null;

		// Token: 0x04000427 RID: 1063
		private List<POP3_ClientMessage> m_pMessages = null;

		// Token: 0x04000428 RID: 1064
		private bool m_IsDisposed = false;
	}
}
