using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace LumiSoft.Net
{
	// Token: 0x0200001D RID: 29
	public class IPBindInfo
	{
		// Token: 0x0600009C RID: 156 RVA: 0x00005ACC File Offset: 0x00004ACC
		public IPBindInfo(string hostName, BindInfoProtocol protocol, IPAddress ip, int port)
		{
			bool flag = ip == null;
			if (flag)
			{
				throw new ArgumentNullException("ip");
			}
			this.m_HostName = hostName;
			this.m_Protocol = protocol;
			this.m_pEndPoint = new IPEndPoint(ip, port);
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00005B3F File Offset: 0x00004B3F
		public IPBindInfo(string hostName, IPAddress ip, int port, SslMode sslMode, X509Certificate2 sslCertificate) : this(hostName, BindInfoProtocol.TCP, ip, port, sslMode, sslCertificate)
		{
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00005B54 File Offset: 0x00004B54
		public IPBindInfo(string hostName, BindInfoProtocol protocol, IPAddress ip, int port, SslMode sslMode, X509Certificate2 sslCertificate)
		{
			bool flag = ip == null;
			if (flag)
			{
				throw new ArgumentNullException("ip");
			}
			this.m_HostName = hostName;
			this.m_Protocol = protocol;
			this.m_pEndPoint = new IPEndPoint(ip, port);
			this.m_SslMode = sslMode;
			this.m_pCertificate = sslCertificate;
			bool flag2 = (sslMode == SslMode.SSL || sslMode == SslMode.TLS) && sslCertificate == null;
			if (flag2)
			{
				throw new ArgumentException("SSL requested, but argument 'sslCertificate' is not provided.");
			}
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00005BFC File Offset: 0x00004BFC
		public override bool Equals(object obj)
		{
			bool flag = obj == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = !(obj is IPBindInfo);
				if (flag2)
				{
					result = false;
				}
				else
				{
					IPBindInfo ipbindInfo = (IPBindInfo)obj;
					bool flag3 = ipbindInfo.HostName != this.m_HostName;
					if (flag3)
					{
						result = false;
					}
					else
					{
						bool flag4 = ipbindInfo.Protocol != this.m_Protocol;
						if (flag4)
						{
							result = false;
						}
						else
						{
							bool flag5 = !ipbindInfo.EndPoint.Equals(this.m_pEndPoint);
							if (flag5)
							{
								result = false;
							}
							else
							{
								bool flag6 = ipbindInfo.SslMode != this.m_SslMode;
								if (flag6)
								{
									result = false;
								}
								else
								{
									bool flag7 = !object.Equals(ipbindInfo.Certificate, this.m_pCertificate);
									result = !flag7;
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00005CD0 File Offset: 0x00004CD0
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060000A1 RID: 161 RVA: 0x00005CE8 File Offset: 0x00004CE8
		public string HostName
		{
			get
			{
				return this.m_HostName;
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060000A2 RID: 162 RVA: 0x00005D00 File Offset: 0x00004D00
		public BindInfoProtocol Protocol
		{
			get
			{
				return this.m_Protocol;
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060000A3 RID: 163 RVA: 0x00005D18 File Offset: 0x00004D18
		public IPEndPoint EndPoint
		{
			get
			{
				return this.m_pEndPoint;
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060000A4 RID: 164 RVA: 0x00005D30 File Offset: 0x00004D30
		public IPAddress IP
		{
			get
			{
				return this.m_pEndPoint.Address;
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000A5 RID: 165 RVA: 0x00005D50 File Offset: 0x00004D50
		public int Port
		{
			get
			{
				return this.m_pEndPoint.Port;
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000A6 RID: 166 RVA: 0x00005D70 File Offset: 0x00004D70
		public SslMode SslMode
		{
			get
			{
				return this.m_SslMode;
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000A7 RID: 167 RVA: 0x00005D88 File Offset: 0x00004D88
		[Obsolete("Use property Certificate instead.")]
		public X509Certificate2 SSL_Certificate
		{
			get
			{
				return this.m_pCertificate;
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000A8 RID: 168 RVA: 0x00005DA0 File Offset: 0x00004DA0
		public X509Certificate2 Certificate
		{
			get
			{
				return this.m_pCertificate;
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000A9 RID: 169 RVA: 0x00005DB8 File Offset: 0x00004DB8
		// (set) Token: 0x060000AA RID: 170 RVA: 0x00005DD0 File Offset: 0x00004DD0
		public object Tag
		{
			get
			{
				return this.m_Tag;
			}
			set
			{
				this.m_Tag = value;
			}
		}

		// Token: 0x0400005C RID: 92
		private string m_HostName = "";

		// Token: 0x0400005D RID: 93
		private BindInfoProtocol m_Protocol = BindInfoProtocol.TCP;

		// Token: 0x0400005E RID: 94
		private IPEndPoint m_pEndPoint = null;

		// Token: 0x0400005F RID: 95
		private SslMode m_SslMode = SslMode.None;

		// Token: 0x04000060 RID: 96
		private X509Certificate2 m_pCertificate = null;

		// Token: 0x04000061 RID: 97
		private object m_Tag = null;
	}
}
