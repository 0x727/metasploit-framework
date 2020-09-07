using System;
using System.Net;

namespace LumiSoft.Net.ICMP
{
	// Token: 0x0200002D RID: 45
	public class EchoMessage
	{
		// Token: 0x0600014A RID: 330 RVA: 0x000094F3 File Offset: 0x000084F3
		internal EchoMessage(IPAddress ip, int ttl, int time)
		{
			this.m_pIP = ip;
			this.m_TTL = ttl;
			this.m_Time = time;
		}

		// Token: 0x0600014B RID: 331 RVA: 0x00009528 File Offset: 0x00008528
		[Obsolete("Will be removed !")]
		public string ToStringEx()
		{
			return string.Concat(new object[]
			{
				"TTL=",
				this.m_TTL,
				"\tTime=",
				this.m_Time,
				"ms\tIP=",
				this.m_pIP
			});
		}

		// Token: 0x0600014C RID: 332 RVA: 0x00009584 File Offset: 0x00008584
		[Obsolete("Will be removed !")]
		public static string ToStringEx(EchoMessage[] messages)
		{
			string text = "";
			foreach (EchoMessage echoMessage in messages)
			{
				text = text + echoMessage.ToStringEx() + "\r\n";
			}
			return text;
		}

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x0600014D RID: 333 RVA: 0x000095C8 File Offset: 0x000085C8
		public IPAddress IPAddress
		{
			get
			{
				return this.m_pIP;
			}
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x0600014E RID: 334 RVA: 0x000095E0 File Offset: 0x000085E0
		public int ReplyTime
		{
			get
			{
				return this.m_Time;
			}
		}

		// Token: 0x0400009A RID: 154
		private IPAddress m_pIP = null;

		// Token: 0x0400009B RID: 155
		private int m_TTL = 0;

		// Token: 0x0400009C RID: 156
		private int m_Time = 0;
	}
}
