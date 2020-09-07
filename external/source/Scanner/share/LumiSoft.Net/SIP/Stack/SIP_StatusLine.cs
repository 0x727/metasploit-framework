using System;

namespace LumiSoft.Net.SIP.Stack
{
	// Token: 0x0200008F RID: 143
	public class SIP_StatusLine
	{
		// Token: 0x06000555 RID: 1365 RVA: 0x0001E2DC File Offset: 0x0001D2DC
		public SIP_StatusLine(int statusCode, string reason)
		{
			bool flag = statusCode < 100 || statusCode > 699;
			if (flag)
			{
				throw new ArgumentException("Argument 'statusCode' value must be >= 100 and <= 699.");
			}
			bool flag2 = reason == null;
			if (flag2)
			{
				throw new ArgumentNullException("reason");
			}
			this.m_Version = "SIP/2.0";
			this.m_StatusCode = statusCode;
			this.m_Reason = reason;
		}

		// Token: 0x06000556 RID: 1366 RVA: 0x0001E35C File Offset: 0x0001D35C
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				this.m_Version,
				" ",
				this.m_StatusCode,
				" ",
				this.m_Reason,
				"\r\n"
			});
		}

		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x06000557 RID: 1367 RVA: 0x0001E3B4 File Offset: 0x0001D3B4
		// (set) Token: 0x06000558 RID: 1368 RVA: 0x0001E3CC File Offset: 0x0001D3CC
		public string Version
		{
			get
			{
				return this.m_Version;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("Version");
				}
				bool flag2 = value == "";
				if (flag2)
				{
					throw new ArgumentException("Property 'Version' value must be specified.");
				}
				this.m_Version = value;
			}
		}

		// Token: 0x170001B7 RID: 439
		// (get) Token: 0x06000559 RID: 1369 RVA: 0x0001E410 File Offset: 0x0001D410
		// (set) Token: 0x0600055A RID: 1370 RVA: 0x0001E428 File Offset: 0x0001D428
		public int StatusCode
		{
			get
			{
				return this.m_StatusCode;
			}
			set
			{
				bool flag = value < 100 || value > 699;
				if (flag)
				{
					throw new ArgumentException("Argument 'statusCode' value must be >= 100 and <= 699.");
				}
				this.m_StatusCode = value;
			}
		}

		// Token: 0x170001B8 RID: 440
		// (get) Token: 0x0600055B RID: 1371 RVA: 0x0001E460 File Offset: 0x0001D460
		// (set) Token: 0x0600055C RID: 1372 RVA: 0x0001E478 File Offset: 0x0001D478
		public string Reason
		{
			get
			{
				return this.m_Reason;
			}
			set
			{
				bool flag = this.Reason == null;
				if (flag)
				{
					throw new ArgumentNullException("Reason");
				}
				this.m_Reason = value;
			}
		}

		// Token: 0x040001C8 RID: 456
		private string m_Version = "";

		// Token: 0x040001C9 RID: 457
		private int m_StatusCode = 0;

		// Token: 0x040001CA RID: 458
		private string m_Reason = "";
	}
}
