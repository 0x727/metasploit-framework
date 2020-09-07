using System;

namespace LumiSoft.Net.UPnP.NAT
{
	// Token: 0x0200012C RID: 300
	public class UPnP_NAT_Map
	{
		// Token: 0x06000BEA RID: 3050 RVA: 0x00049160 File Offset: 0x00048160
		public UPnP_NAT_Map(bool enabled, string protocol, string remoteHost, string externalPort, string internalHost, int internalPort, string description, int leaseDuration)
		{
			this.m_Enabled = enabled;
			this.m_Protocol = protocol;
			this.m_RemoteHost = remoteHost;
			this.m_ExternalPort = externalPort;
			this.m_InternalHost = internalHost;
			this.m_InternalPort = internalPort;
			this.m_Description = description;
			this.m_LeaseDuration = leaseDuration;
		}

		// Token: 0x170003E6 RID: 998
		// (get) Token: 0x06000BEB RID: 3051 RVA: 0x00049200 File Offset: 0x00048200
		public bool Enabled
		{
			get
			{
				return this.m_Enabled;
			}
		}

		// Token: 0x170003E7 RID: 999
		// (get) Token: 0x06000BEC RID: 3052 RVA: 0x00049218 File Offset: 0x00048218
		public string Protocol
		{
			get
			{
				return this.m_Protocol;
			}
		}

		// Token: 0x170003E8 RID: 1000
		// (get) Token: 0x06000BED RID: 3053 RVA: 0x00049230 File Offset: 0x00048230
		public string RemoteHost
		{
			get
			{
				return this.m_RemoteHost;
			}
		}

		// Token: 0x170003E9 RID: 1001
		// (get) Token: 0x06000BEE RID: 3054 RVA: 0x00049248 File Offset: 0x00048248
		public string ExternalPort
		{
			get
			{
				return this.m_ExternalPort;
			}
		}

		// Token: 0x170003EA RID: 1002
		// (get) Token: 0x06000BEF RID: 3055 RVA: 0x00049260 File Offset: 0x00048260
		public string InternalHost
		{
			get
			{
				return this.m_InternalHost;
			}
		}

		// Token: 0x170003EB RID: 1003
		// (get) Token: 0x06000BF0 RID: 3056 RVA: 0x00049278 File Offset: 0x00048278
		public int InternalPort
		{
			get
			{
				return this.m_InternalPort;
			}
		}

		// Token: 0x170003EC RID: 1004
		// (get) Token: 0x06000BF1 RID: 3057 RVA: 0x00049290 File Offset: 0x00048290
		public string Description
		{
			get
			{
				return this.m_Description;
			}
		}

		// Token: 0x170003ED RID: 1005
		// (get) Token: 0x06000BF2 RID: 3058 RVA: 0x000492A8 File Offset: 0x000482A8
		public int LeaseDuration
		{
			get
			{
				return this.m_LeaseDuration;
			}
		}

		// Token: 0x040004E0 RID: 1248
		private bool m_Enabled = false;

		// Token: 0x040004E1 RID: 1249
		private string m_Protocol = "";

		// Token: 0x040004E2 RID: 1250
		private string m_RemoteHost = "";

		// Token: 0x040004E3 RID: 1251
		private string m_ExternalPort = "";

		// Token: 0x040004E4 RID: 1252
		private string m_InternalHost = "";

		// Token: 0x040004E5 RID: 1253
		private int m_InternalPort = 0;

		// Token: 0x040004E6 RID: 1254
		private string m_Description = "";

		// Token: 0x040004E7 RID: 1255
		private int m_LeaseDuration = 0;
	}
}
