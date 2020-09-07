using System;
using System.Xml;

namespace LumiSoft.Net.WebDav
{
	// Token: 0x0200003C RID: 60
	public class WebDav_PropStat
	{
		// Token: 0x06000214 RID: 532 RVA: 0x0000CF80 File Offset: 0x0000BF80
		internal WebDav_PropStat()
		{
		}

		// Token: 0x06000215 RID: 533 RVA: 0x0000CFA0 File Offset: 0x0000BFA0
		internal static WebDav_PropStat Parse(XmlNode propstatNode)
		{
			bool flag = propstatNode == null;
			if (flag)
			{
				throw new ArgumentNullException("propstatNode");
			}
			bool flag2 = !string.Equals(propstatNode.NamespaceURI + propstatNode.LocalName, "DAV:propstat", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Invalid DAV:propstat value.");
			}
			WebDav_PropStat webDav_PropStat = new WebDav_PropStat();
			foreach (object obj in propstatNode.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				bool flag3 = string.Equals(xmlNode.LocalName, "status", StringComparison.InvariantCultureIgnoreCase);
				if (flag3)
				{
					webDav_PropStat.m_Status = xmlNode.ChildNodes[0].Value;
				}
				else
				{
					bool flag4 = string.Equals(xmlNode.LocalName, "prop", StringComparison.InvariantCultureIgnoreCase);
					if (flag4)
					{
						webDav_PropStat.m_pProp = WebDav_Prop.Parse(xmlNode);
					}
				}
			}
			return webDav_PropStat;
		}

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x06000216 RID: 534 RVA: 0x0000D0A8 File Offset: 0x0000C0A8
		public string Status
		{
			get
			{
				return this.m_Status;
			}
		}

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x06000217 RID: 535 RVA: 0x0000D0C0 File Offset: 0x0000C0C0
		public string ResponseDescription
		{
			get
			{
				return this.m_ResponseDescription;
			}
		}

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x06000218 RID: 536 RVA: 0x0000D0D8 File Offset: 0x0000C0D8
		public WebDav_Prop Prop
		{
			get
			{
				return this.m_pProp;
			}
		}

		// Token: 0x040000E4 RID: 228
		private string m_Status = null;

		// Token: 0x040000E5 RID: 229
		private string m_ResponseDescription = null;

		// Token: 0x040000E6 RID: 230
		private WebDav_Prop m_pProp = null;
	}
}
