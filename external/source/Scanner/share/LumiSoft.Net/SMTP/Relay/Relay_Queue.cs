using System;
using System.Collections.Generic;
using System.IO;

namespace LumiSoft.Net.SMTP.Relay
{
	// Token: 0x02000143 RID: 323
	public class Relay_Queue : IDisposable
	{
		// Token: 0x06000CAA RID: 3242 RVA: 0x0004E57C File Offset: 0x0004D57C
		public Relay_Queue(string name)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			bool flag2 = name == "";
			if (flag2)
			{
				throw new ArgumentException("Argument 'name' value may not be empty.");
			}
			this.m_Name = name;
			this.m_pQueue = new Queue<Relay_QueueItem>();
		}

		// Token: 0x06000CAB RID: 3243 RVA: 0x000091B8 File Offset: 0x000081B8
		public void Dispose()
		{
		}

		// Token: 0x06000CAC RID: 3244 RVA: 0x0004E5E4 File Offset: 0x0004D5E4
		public void QueueMessage(string from, string to, string messageID, Stream message, object tag)
		{
			this.QueueMessage(null, from, null, SMTP_DSN_Ret.NotSpecified, to, null, SMTP_DSN_Notify.NotSpecified, messageID, message, tag);
		}

		// Token: 0x06000CAD RID: 3245 RVA: 0x0004E608 File Offset: 0x0004D608
		public void QueueMessage(string from, string envelopeID, SMTP_DSN_Ret ret, string to, string originalRecipient, SMTP_DSN_Notify notify, string messageID, Stream message, object tag)
		{
			this.QueueMessage(null, from, envelopeID, ret, to, originalRecipient, notify, messageID, message, tag);
		}

		// Token: 0x06000CAE RID: 3246 RVA: 0x0004E630 File Offset: 0x0004D630
		public void QueueMessage(Relay_SmartHost targetServer, string from, string envelopeID, SMTP_DSN_Ret ret, string to, string originalRecipient, SMTP_DSN_Notify notify, string messageID, Stream message, object tag)
		{
			bool flag = messageID == null;
			if (flag)
			{
				throw new ArgumentNullException("messageID");
			}
			bool flag2 = messageID == "";
			if (flag2)
			{
				throw new ArgumentException("Argument 'messageID' value must be specified.");
			}
			bool flag3 = message == null;
			if (flag3)
			{
				throw new ArgumentNullException("message");
			}
			Queue<Relay_QueueItem> pQueue = this.m_pQueue;
			lock (pQueue)
			{
				this.m_pQueue.Enqueue(new Relay_QueueItem(this, targetServer, from, envelopeID, ret, to, originalRecipient, notify, messageID, message, tag));
			}
		}

		// Token: 0x06000CAF RID: 3247 RVA: 0x0004E6DC File Offset: 0x0004D6DC
		public Relay_QueueItem DequeueMessage()
		{
			Queue<Relay_QueueItem> pQueue = this.m_pQueue;
			Relay_QueueItem result;
			lock (pQueue)
			{
				bool flag2 = this.m_pQueue.Count > 0;
				if (flag2)
				{
					result = this.m_pQueue.Dequeue();
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		// Token: 0x17000429 RID: 1065
		// (get) Token: 0x06000CB0 RID: 3248 RVA: 0x0004E740 File Offset: 0x0004D740
		public string Name
		{
			get
			{
				return this.m_Name;
			}
		}

		// Token: 0x1700042A RID: 1066
		// (get) Token: 0x06000CB1 RID: 3249 RVA: 0x0004E758 File Offset: 0x0004D758
		public int Count
		{
			get
			{
				return this.m_pQueue.Count;
			}
		}

		// Token: 0x0400055F RID: 1375
		private string m_Name = "";

		// Token: 0x04000560 RID: 1376
		private Queue<Relay_QueueItem> m_pQueue = null;
	}
}
