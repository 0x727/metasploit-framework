using System;
using System.Collections.Generic;
using System.Xml;

namespace LumiSoft.Net.WebDav
{
	// Token: 0x0200003B RID: 59
	public class WebDav_Prop
	{
		// Token: 0x06000210 RID: 528 RVA: 0x0000CDD0 File Offset: 0x0000BDD0
		public WebDav_Prop()
		{
			this.m_pProperties = new List<WebDav_p>();
		}

		// Token: 0x06000211 RID: 529 RVA: 0x0000CDEC File Offset: 0x0000BDEC
		internal static WebDav_Prop Parse(XmlNode propNode)
		{
			bool flag = propNode == null;
			if (flag)
			{
				throw new ArgumentNullException("propNode");
			}
			bool flag2 = !string.Equals(propNode.NamespaceURI + propNode.LocalName, "DAV:prop", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Invalid DAV:prop value.");
			}
			WebDav_Prop webDav_Prop = new WebDav_Prop();
			foreach (object obj in propNode.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				bool flag3 = string.Equals(xmlNode.LocalName, "resourcetype", StringComparison.InvariantCultureIgnoreCase);
				if (flag3)
				{
					webDav_Prop.m_pProperties.Add(WebDav_p_ResourceType.Parse(xmlNode));
				}
				else
				{
					webDav_Prop.m_pProperties.Add(new WebDav_p_Default(xmlNode.NamespaceURI, xmlNode.LocalName, xmlNode.InnerXml));
				}
			}
			return webDav_Prop;
		}

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x06000212 RID: 530 RVA: 0x0000CEF0 File Offset: 0x0000BEF0
		public WebDav_p[] Properties
		{
			get
			{
				return this.m_pProperties.ToArray();
			}
		}

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x06000213 RID: 531 RVA: 0x0000CF10 File Offset: 0x0000BF10
		public WebDav_p_ResourceType Prop_ResourceType
		{
			get
			{
				foreach (WebDav_p webDav_p in this.m_pProperties)
				{
					bool flag = webDav_p is WebDav_p_ResourceType;
					if (flag)
					{
						return (WebDav_p_ResourceType)webDav_p;
					}
				}
				return null;
			}
		}

		// Token: 0x040000E3 RID: 227
		private List<WebDav_p> m_pProperties = null;
	}
}
