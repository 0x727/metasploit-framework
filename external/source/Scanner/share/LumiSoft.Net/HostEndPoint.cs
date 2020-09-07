using System;
using System.Net;

namespace LumiSoft.Net
{
	// Token: 0x02000011 RID: 17
	public class HostEndPoint
	{
		// Token: 0x0600003F RID: 63 RVA: 0x00002FC4 File Offset: 0x00001FC4
		public HostEndPoint(string host, int port)
		{
			bool flag = host == null;
			if (flag)
			{
				throw new ArgumentNullException("host");
			}
			bool flag2 = host == "";
			if (flag2)
			{
				throw new ArgumentException("Argument 'host' value must be specified.");
			}
			this.m_Host = host;
			this.m_Port = port;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00003028 File Offset: 0x00002028
		public HostEndPoint(IPEndPoint endPoint)
		{
			bool flag = endPoint == null;
			if (flag)
			{
				throw new ArgumentNullException("endPoint");
			}
			this.m_Host = endPoint.Address.ToString();
			this.m_Port = endPoint.Port;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00003080 File Offset: 0x00002080
		public static HostEndPoint Parse(string value)
		{
			return HostEndPoint.Parse(value, -1);
		}

		// Token: 0x06000042 RID: 66 RVA: 0x0000309C File Offset: 0x0000209C
		public static HostEndPoint Parse(string value, int defaultPort)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			bool flag2 = value == "";
			if (flag2)
			{
				throw new ArgumentException("Argument 'value' value must be specified.");
			}
			bool flag3 = value.IndexOf(':') > -1;
			if (flag3)
			{
				string[] array = value.Split(new char[]
				{
					':'
				}, 2);
				try
				{
					return new HostEndPoint(array[0], Convert.ToInt32(array[1]));
				}
				catch
				{
					throw new ArgumentException("Argument 'value' has invalid value.");
				}
			}
			return new HostEndPoint(value, defaultPort);
		}

		// Token: 0x06000043 RID: 67 RVA: 0x0000313C File Offset: 0x0000213C
		public override string ToString()
		{
			bool flag = this.m_Port == -1;
			string result;
			if (flag)
			{
				result = this.m_Host;
			}
			else
			{
				result = this.m_Host + ":" + this.m_Port.ToString();
			}
			return result;
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000044 RID: 68 RVA: 0x00003184 File Offset: 0x00002184
		public bool IsIPAddress
		{
			get
			{
				return Net_Utils.IsIPAddress(this.m_Host);
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000045 RID: 69 RVA: 0x000031A4 File Offset: 0x000021A4
		public string Host
		{
			get
			{
				return this.m_Host;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000046 RID: 70 RVA: 0x000031BC File Offset: 0x000021BC
		public int Port
		{
			get
			{
				return this.m_Port;
			}
		}

		// Token: 0x0400002F RID: 47
		private string m_Host = "";

		// Token: 0x04000030 RID: 48
		private int m_Port = 0;
	}
}
