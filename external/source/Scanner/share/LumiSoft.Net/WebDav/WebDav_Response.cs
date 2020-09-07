using System;
using System.Collections.Generic;
using System.Xml;

namespace LumiSoft.Net.WebDav
{
	// Token: 0x02000040 RID: 64
	public class WebDav_Response
	{
		// Token: 0x06000226 RID: 550 RVA: 0x0000D3FB File Offset: 0x0000C3FB
		internal WebDav_Response()
		{
			this.m_pPropStats = new List<WebDav_PropStat>();
		}

		// Token: 0x06000227 RID: 551 RVA: 0x0000D420 File Offset: 0x0000C420
		internal static WebDav_Response Parse(XmlNode reponseNode)
		{
			bool flag = reponseNode == null;
			if (flag)
			{
				throw new ArgumentNullException("responseNode");
			}
			bool flag2 = !string.Equals(reponseNode.NamespaceURI + reponseNode.LocalName, "DAV:response", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Invalid DAV:response value.");
			}
			WebDav_Response webDav_Response = new WebDav_Response();
			foreach (object obj in reponseNode.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				bool flag3 = string.Equals(xmlNode.LocalName, "href", StringComparison.InvariantCultureIgnoreCase);
				if (flag3)
				{
					webDav_Response.m_HRef = xmlNode.ChildNodes[0].Value;
				}
				else
				{
					bool flag4 = string.Equals(xmlNode.LocalName, "propstat", StringComparison.InvariantCultureIgnoreCase);
					if (flag4)
					{
						webDav_Response.m_pPropStats.Add(WebDav_PropStat.Parse(xmlNode));
					}
				}
			}
			return webDav_Response;
		}

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x06000228 RID: 552 RVA: 0x0000D52C File Offset: 0x0000C52C
		public string HRef
		{
			get
			{
				return this.m_HRef;
			}
		}

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x06000229 RID: 553 RVA: 0x0000D544 File Offset: 0x0000C544
		public WebDav_PropStat[] PropStats
		{
			get
			{
				return this.m_pPropStats.ToArray();
			}
		}

		// Token: 0x040000EF RID: 239
		private string m_HRef = null;

		// Token: 0x040000F0 RID: 240
		private List<WebDav_PropStat> m_pPropStats = null;
	}
}
