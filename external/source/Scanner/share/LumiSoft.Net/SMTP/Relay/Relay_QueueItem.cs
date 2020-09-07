using System;
using System.IO;

namespace LumiSoft.Net.SMTP.Relay
{
	// Token: 0x02000144 RID: 324
	public class Relay_QueueItem
	{
		// Token: 0x06000CB2 RID: 3250 RVA: 0x0004E778 File Offset: 0x0004D778
		internal Relay_QueueItem(Relay_Queue queue, Relay_SmartHost targetServer, string from, string envelopeID, SMTP_DSN_Ret ret, string to, string originalRecipient, SMTP_DSN_Notify notify, string messageID, Stream message, object tag)
		{
			this.m_pQueue = queue;
			this.m_pTargetServer = targetServer;
			this.m_From = from;
			this.m_EnvelopeID = envelopeID;
			this.m_DSN_Ret = ret;
			this.m_To = to;
			this.m_OriginalRecipient = originalRecipient;
			this.m_DSN_Notify = notify;
			this.m_MessageID = messageID;
			this.m_pMessageStream = message;
			this.m_pTag = tag;
		}

		// Token: 0x1700042B RID: 1067
		// (get) Token: 0x06000CB3 RID: 3251 RVA: 0x0004E83C File Offset: 0x0004D83C
		public Relay_Queue Queue
		{
			get
			{
				return this.m_pQueue;
			}
		}

		// Token: 0x1700042C RID: 1068
		// (get) Token: 0x06000CB4 RID: 3252 RVA: 0x0004E854 File Offset: 0x0004D854
		public Relay_SmartHost TargetServer
		{
			get
			{
				return this.m_pTargetServer;
			}
		}

		// Token: 0x1700042D RID: 1069
		// (get) Token: 0x06000CB5 RID: 3253 RVA: 0x0004E86C File Offset: 0x0004D86C
		public string From
		{
			get
			{
				return this.m_From;
			}
		}

		// Token: 0x1700042E RID: 1070
		// (get) Token: 0x06000CB6 RID: 3254 RVA: 0x0004E884 File Offset: 0x0004D884
		public string EnvelopeID
		{
			get
			{
				return this.m_EnvelopeID;
			}
		}

		// Token: 0x1700042F RID: 1071
		// (get) Token: 0x06000CB7 RID: 3255 RVA: 0x0004E89C File Offset: 0x0004D89C
		public SMTP_DSN_Ret DSN_Ret
		{
			get
			{
				return this.m_DSN_Ret;
			}
		}

		// Token: 0x17000430 RID: 1072
		// (get) Token: 0x06000CB8 RID: 3256 RVA: 0x0004E8B4 File Offset: 0x0004D8B4
		public string To
		{
			get
			{
				return this.m_To;
			}
		}

		// Token: 0x17000431 RID: 1073
		// (get) Token: 0x06000CB9 RID: 3257 RVA: 0x0004E8CC File Offset: 0x0004D8CC
		public string OriginalRecipient
		{
			get
			{
				return this.m_OriginalRecipient;
			}
		}

		// Token: 0x17000432 RID: 1074
		// (get) Token: 0x06000CBA RID: 3258 RVA: 0x0004E8E4 File Offset: 0x0004D8E4
		public SMTP_DSN_Notify DSN_Notify
		{
			get
			{
				return this.m_DSN_Notify;
			}
		}

		// Token: 0x17000433 RID: 1075
		// (get) Token: 0x06000CBB RID: 3259 RVA: 0x0004E8FC File Offset: 0x0004D8FC
		public string MessageID
		{
			get
			{
				return this.m_MessageID;
			}
		}

		// Token: 0x17000434 RID: 1076
		// (get) Token: 0x06000CBC RID: 3260 RVA: 0x0004E914 File Offset: 0x0004D914
		public Stream MessageStream
		{
			get
			{
				return this.m_pMessageStream;
			}
		}

		// Token: 0x17000435 RID: 1077
		// (get) Token: 0x06000CBD RID: 3261 RVA: 0x0004E92C File Offset: 0x0004D92C
		// (set) Token: 0x06000CBE RID: 3262 RVA: 0x0004E944 File Offset: 0x0004D944
		public object Tag
		{
			get
			{
				return this.m_pTag;
			}
			set
			{
				this.m_pTag = value;
			}
		}

		// Token: 0x04000561 RID: 1377
		private Relay_Queue m_pQueue = null;

		// Token: 0x04000562 RID: 1378
		private Relay_SmartHost m_pTargetServer = null;

		// Token: 0x04000563 RID: 1379
		private string m_From = "";

		// Token: 0x04000564 RID: 1380
		private string m_EnvelopeID = null;

		// Token: 0x04000565 RID: 1381
		private SMTP_DSN_Ret m_DSN_Ret = SMTP_DSN_Ret.NotSpecified;

		// Token: 0x04000566 RID: 1382
		private string m_To = "";

		// Token: 0x04000567 RID: 1383
		private string m_OriginalRecipient = null;

		// Token: 0x04000568 RID: 1384
		private SMTP_DSN_Notify m_DSN_Notify = SMTP_DSN_Notify.NotSpecified;

		// Token: 0x04000569 RID: 1385
		private string m_MessageID = "";

		// Token: 0x0400056A RID: 1386
		private Stream m_pMessageStream = null;

		// Token: 0x0400056B RID: 1387
		private object m_pTag = null;
	}
}
