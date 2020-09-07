using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace LumiSoft.Net.UPnP
{
	// Token: 0x02000129 RID: 297
	public class UPnP_Exception : Exception
	{
		// Token: 0x06000BCD RID: 3021 RVA: 0x00048074 File Offset: 0x00047074
		public UPnP_Exception(int errorCode, string errorText) : base(string.Concat(new object[]
		{
			"UPnP error: ",
			errorCode,
			" ",
			errorText,
			"."
		}))
		{
			this.m_ErrorCode = errorCode;
			this.m_ErrorText = errorText;
		}

		// Token: 0x06000BCE RID: 3022 RVA: 0x000480DC File Offset: 0x000470DC
		public static UPnP_Exception Parse(Stream stream)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			int num = -1;
			string text = null;
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(stream);
				List<XmlNode> list = new List<XmlNode>();
				list.Add(xmlDocument);
				while (list.Count > 0)
				{
					XmlNode xmlNode = list[0];
					list.RemoveAt(0);
					bool flag2 = string.Equals("UPnPError", xmlNode.Name, StringComparison.InvariantCultureIgnoreCase);
					if (flag2)
					{
						foreach (object obj in xmlNode.ChildNodes)
						{
							XmlNode xmlNode2 = (XmlNode)obj;
							bool flag3 = string.Equals("errorCode", xmlNode2.Name, StringComparison.InvariantCultureIgnoreCase);
							if (flag3)
							{
								num = Convert.ToInt32(xmlNode2.InnerText);
							}
							else
							{
								bool flag4 = string.Equals("errorDescription", xmlNode2.Name, StringComparison.InvariantCultureIgnoreCase);
								if (flag4)
								{
									text = xmlNode2.InnerText;
								}
							}
						}
						break;
					}
					bool flag5 = xmlNode.ChildNodes.Count > 0;
					if (flag5)
					{
						for (int i = 0; i < xmlNode.ChildNodes.Count; i++)
						{
							list.Insert(i, xmlNode.ChildNodes[i]);
						}
					}
				}
			}
			catch
			{
			}
			bool flag6 = num == -1 || text == null;
			if (flag6)
			{
				throw new ParseException("Failed to parse UPnP error.");
			}
			return new UPnP_Exception(num, text);
		}

		// Token: 0x170003D6 RID: 982
		// (get) Token: 0x06000BCF RID: 3023 RVA: 0x000482B4 File Offset: 0x000472B4
		public int ErrorCode
		{
			get
			{
				return this.m_ErrorCode;
			}
		}

		// Token: 0x170003D7 RID: 983
		// (get) Token: 0x06000BD0 RID: 3024 RVA: 0x000482CC File Offset: 0x000472CC
		public string ErrorText
		{
			get
			{
				return this.m_ErrorText;
			}
		}

		// Token: 0x040004CE RID: 1230
		private int m_ErrorCode = 0;

		// Token: 0x040004CF RID: 1231
		private string m_ErrorText = "";
	}
}
