using System;
using System.Diagnostics;
using LumiSoft.Net.Mail;

namespace LumiSoft.Net.IMAP.Server
{
	// Token: 0x02000212 RID: 530
	public class IMAP_e_Fetch : EventArgs
	{
		// Token: 0x06001316 RID: 4886 RVA: 0x00076958 File Offset: 0x00075958
		internal IMAP_e_Fetch(IMAP_MessageInfo[] messagesInfo, IMAP_Fetch_DataType fetchDataType, IMAP_r_ServerStatus response)
		{
			bool flag = messagesInfo == null;
			if (flag)
			{
				throw new ArgumentNullException("messagesInfo");
			}
			bool flag2 = response == null;
			if (flag2)
			{
				throw new ArgumentNullException("response");
			}
			this.m_pMessagesInfo = messagesInfo;
			this.m_FetchDataType = fetchDataType;
			this.m_pResponse = response;
		}

		// Token: 0x06001317 RID: 4887 RVA: 0x000769C6 File Offset: 0x000759C6
		internal void AddData(IMAP_MessageInfo msgInfo)
		{
			this.OnNewMessageData(msgInfo, null);
		}

		// Token: 0x06001318 RID: 4888 RVA: 0x000769D4 File Offset: 0x000759D4
		public void AddData(IMAP_MessageInfo msgInfo, Mail_Message msgData)
		{
			bool flag = msgInfo == null;
			if (flag)
			{
				throw new ArgumentNullException("msgInfo");
			}
			bool flag2 = msgData == null;
			if (flag2)
			{
				throw new ArgumentNullException("msgData");
			}
			this.OnNewMessageData(msgInfo, msgData);
		}

		// Token: 0x17000636 RID: 1590
		// (get) Token: 0x06001319 RID: 4889 RVA: 0x00076A14 File Offset: 0x00075A14
		// (set) Token: 0x0600131A RID: 4890 RVA: 0x00076A2C File Offset: 0x00075A2C
		public IMAP_r_ServerStatus Response
		{
			get
			{
				return this.m_pResponse;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("value");
				}
				this.m_pResponse = value;
			}
		}

		// Token: 0x17000637 RID: 1591
		// (get) Token: 0x0600131B RID: 4891 RVA: 0x00076A58 File Offset: 0x00075A58
		public IMAP_MessageInfo[] MessagesInfo
		{
			get
			{
				return this.m_pMessagesInfo;
			}
		}

		// Token: 0x17000638 RID: 1592
		// (get) Token: 0x0600131C RID: 4892 RVA: 0x00076A70 File Offset: 0x00075A70
		public IMAP_Fetch_DataType FetchDataType
		{
			get
			{
				return this.m_FetchDataType;
			}
		}

		// Token: 0x14000076 RID: 118
		// (add) Token: 0x0600131D RID: 4893 RVA: 0x00076A88 File Offset: 0x00075A88
		// (remove) Token: 0x0600131E RID: 4894 RVA: 0x00076AC0 File Offset: 0x00075AC0
		
		internal event EventHandler<IMAP_e_Fetch.e_NewMessageData> NewMessageData = null;

		// Token: 0x0600131F RID: 4895 RVA: 0x00076AF8 File Offset: 0x00075AF8
		private void OnNewMessageData(IMAP_MessageInfo msgInfo, Mail_Message msgData)
		{
			bool flag = this.NewMessageData != null;
			if (flag)
			{
				this.NewMessageData(this, new IMAP_e_Fetch.e_NewMessageData(msgInfo, msgData));
			}
		}

		// Token: 0x0400076E RID: 1902
		private IMAP_r_ServerStatus m_pResponse = null;

		// Token: 0x0400076F RID: 1903
		private IMAP_MessageInfo[] m_pMessagesInfo = null;

		// Token: 0x04000770 RID: 1904
		private IMAP_Fetch_DataType m_FetchDataType = IMAP_Fetch_DataType.FullMessage;

		// Token: 0x02000322 RID: 802
		internal class e_NewMessageData : EventArgs
		{
			// Token: 0x06001A49 RID: 6729 RVA: 0x000A2684 File Offset: 0x000A1684
			public e_NewMessageData(IMAP_MessageInfo msgInfo, Mail_Message msgData)
			{
				bool flag = msgInfo == null;
				if (flag)
				{
					throw new ArgumentNullException("msgInfo");
				}
				this.m_pMsgInfo = msgInfo;
				this.m_pMsgData = msgData;
			}

			// Token: 0x17000861 RID: 2145
			// (get) Token: 0x06001A4A RID: 6730 RVA: 0x000A26CC File Offset: 0x000A16CC
			public IMAP_MessageInfo MessageInfo
			{
				get
				{
					return this.m_pMsgInfo;
				}
			}

			// Token: 0x17000862 RID: 2146
			// (get) Token: 0x06001A4B RID: 6731 RVA: 0x000A26E4 File Offset: 0x000A16E4
			public Mail_Message MessageData
			{
				get
				{
					return this.m_pMsgData;
				}
			}

			// Token: 0x04000BF6 RID: 3062
			private IMAP_MessageInfo m_pMsgInfo = null;

			// Token: 0x04000BF7 RID: 3063
			private Mail_Message m_pMsgData = null;
		}
	}
}
