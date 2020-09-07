using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace LumiSoft.Net.WebDav
{
	// Token: 0x0200003E RID: 62
	public class WebDav_p_ResourceType : WebDav_p
	{
		// Token: 0x0600021D RID: 541 RVA: 0x0000D1AC File Offset: 0x0000C1AC
		public WebDav_p_ResourceType()
		{
			this.m_pItems = new List<string>();
		}

		// Token: 0x0600021E RID: 542 RVA: 0x0000D1C8 File Offset: 0x0000C1C8
		public bool Contains(string resourceType)
		{
			foreach (string b in this.m_pItems)
			{
				bool flag = string.Equals(resourceType, b, StringComparison.InvariantCultureIgnoreCase);
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600021F RID: 543 RVA: 0x0000D230 File Offset: 0x0000C230
		internal static WebDav_p_ResourceType Parse(XmlNode resourcetypeNode)
		{
			bool flag = resourcetypeNode == null;
			if (flag)
			{
				throw new ArgumentNullException("resourcetypeNode");
			}
			bool flag2 = !string.Equals(resourcetypeNode.NamespaceURI + resourcetypeNode.LocalName, "DAV:resourcetype", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Invalid DAV:resourcetype value.");
			}
			WebDav_p_ResourceType webDav_p_ResourceType = new WebDav_p_ResourceType();
			foreach (object obj in resourcetypeNode.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				webDav_p_ResourceType.m_pItems.Add(xmlNode.NamespaceURI + xmlNode.LocalName);
			}
			return webDav_p_ResourceType;
		}

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x06000220 RID: 544 RVA: 0x0000D2FC File Offset: 0x0000C2FC
		public override string Namespace
		{
			get
			{
				return "DAV:";
			}
		}

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x06000221 RID: 545 RVA: 0x0000D314 File Offset: 0x0000C314
		public override string Name
		{
			get
			{
				return "resourcetype";
			}
		}

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x06000222 RID: 546 RVA: 0x0000D32C File Offset: 0x0000C32C
		public override string Value
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < this.m_pItems.Count; i++)
				{
					bool flag = i == this.m_pItems.Count - 1;
					if (flag)
					{
						stringBuilder.Append(this.m_pItems[i]);
					}
					else
					{
						stringBuilder.Append(this.m_pItems[i] + ";");
					}
				}
				return stringBuilder.ToString();
			}
		}

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x06000223 RID: 547 RVA: 0x0000D3B4 File Offset: 0x0000C3B4
		public string[] ResourceTypes
		{
			get
			{
				return this.m_pItems.ToArray();
			}
		}

		// Token: 0x040000EA RID: 234
		private List<string> m_pItems = null;
	}
}
