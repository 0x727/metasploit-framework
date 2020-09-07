using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace LumiSoft.Net.WebDav
{
	// Token: 0x02000039 RID: 57
	public class WebDav_MultiStatus
	{
		// Token: 0x06000209 RID: 521 RVA: 0x0000CCA8 File Offset: 0x0000BCA8
		public WebDav_MultiStatus()
		{
			this.m_pResponses = new List<WebDav_Response>();
		}

		// Token: 0x0600020A RID: 522 RVA: 0x0000CCC4 File Offset: 0x0000BCC4
		internal static WebDav_MultiStatus Parse(Stream stream)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(stream);
			bool flag2 = !string.Equals(xmlDocument.ChildNodes[1].NamespaceURI + xmlDocument.ChildNodes[1].LocalName, "DAV:multistatus", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Invalid DAV:multistatus value.");
			}
			WebDav_MultiStatus webDav_MultiStatus = new WebDav_MultiStatus();
			foreach (object obj in xmlDocument.ChildNodes[1].ChildNodes)
			{
				XmlNode reponseNode = (XmlNode)obj;
				webDav_MultiStatus.Responses.Add(WebDav_Response.Parse(reponseNode));
			}
			return webDav_MultiStatus;
		}

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x0600020B RID: 523 RVA: 0x0000CDB8 File Offset: 0x0000BDB8
		public List<WebDav_Response> Responses
		{
			get
			{
				return this.m_pResponses;
			}
		}

		// Token: 0x040000E2 RID: 226
		private List<WebDav_Response> m_pResponses = null;
	}
}
