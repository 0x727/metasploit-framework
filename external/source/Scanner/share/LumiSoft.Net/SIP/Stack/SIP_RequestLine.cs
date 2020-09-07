using System;

namespace LumiSoft.Net.SIP.Stack
{
	// Token: 0x0200008C RID: 140
	public class SIP_RequestLine
	{
		// Token: 0x06000538 RID: 1336 RVA: 0x0001C978 File Offset: 0x0001B978
		public SIP_RequestLine(string method, AbsoluteUri uri)
		{
			bool flag = method == null;
			if (flag)
			{
				throw new ArgumentNullException("method");
			}
			bool flag2 = !SIP_Utils.IsToken(method);
			if (flag2)
			{
				throw new ArgumentException("Argument 'method' value must be token.");
			}
			bool flag3 = uri == null;
			if (flag3)
			{
				throw new ArgumentNullException("uri");
			}
			this.m_Method = method.ToUpper();
			this.m_pUri = uri;
			this.m_Version = "SIP/2.0";
		}

		// Token: 0x06000539 RID: 1337 RVA: 0x0001CA0C File Offset: 0x0001BA0C
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				this.m_Method,
				" ",
				this.m_pUri.ToString(),
				" ",
				this.m_Version,
				"\r\n"
			});
		}

		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x0600053A RID: 1338 RVA: 0x0001CA64 File Offset: 0x0001BA64
		// (set) Token: 0x0600053B RID: 1339 RVA: 0x0001CA7C File Offset: 0x0001BA7C
		public string Method
		{
			get
			{
				return this.m_Method;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("Method");
				}
				bool flag2 = !SIP_Utils.IsToken(value);
				if (flag2)
				{
					throw new ArgumentException("Property 'Method' value must be token.");
				}
				this.m_Method = value.ToUpper();
			}
		}

		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x0600053C RID: 1340 RVA: 0x0001CAC4 File Offset: 0x0001BAC4
		// (set) Token: 0x0600053D RID: 1341 RVA: 0x0001CADC File Offset: 0x0001BADC
		public AbsoluteUri Uri
		{
			get
			{
				return this.m_pUri;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("Uri");
				}
				this.m_pUri = value;
			}
		}

		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x0600053E RID: 1342 RVA: 0x0001CB08 File Offset: 0x0001BB08
		// (set) Token: 0x0600053F RID: 1343 RVA: 0x0001CB20 File Offset: 0x0001BB20
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

		// Token: 0x040001BB RID: 443
		private string m_Method = "";

		// Token: 0x040001BC RID: 444
		private AbsoluteUri m_pUri = null;

		// Token: 0x040001BD RID: 445
		private string m_Version = "";
	}
}
