using System;
using System.IO;
using System.Xml;

namespace LumiSoft.Net.UPnP
{
	// Token: 0x0200012A RID: 298
	public class UPnP_Device
	{
		// Token: 0x06000BD1 RID: 3025 RVA: 0x000482E4 File Offset: 0x000472E4
		internal UPnP_Device(string url)
		{
			bool flag = url == null;
			if (flag)
			{
				throw new ArgumentNullException("url");
			}
			this.Init(url);
		}

		// Token: 0x06000BD2 RID: 3026 RVA: 0x000483A0 File Offset: 0x000473A0
		private void Init(string url)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(url);
			StringWriter stringWriter = new StringWriter();
			xmlDocument.WriteTo(new XmlTextWriter(stringWriter));
			this.m_DeviceXml = stringWriter.ToString();
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
			xmlNamespaceManager.AddNamespace("n", xmlDocument.ChildNodes[1].NamespaceURI);
			this.m_BaseUrl = ((xmlDocument.SelectSingleNode("n:root/n:URLBase", xmlNamespaceManager) != null) ? xmlDocument.SelectSingleNode("n:root/n:URLBase", xmlNamespaceManager).InnerText : url.Substring(0, url.LastIndexOf("/")));
			this.m_DeviceType = ((xmlDocument.SelectSingleNode("n:root/n:device/n:deviceType", xmlNamespaceManager) != null) ? xmlDocument.SelectSingleNode("n:root/n:device/n:deviceType", xmlNamespaceManager).InnerText : "");
			this.m_FriendlyName = ((xmlDocument.SelectSingleNode("n:root/n:device/n:friendlyName", xmlNamespaceManager) != null) ? xmlDocument.SelectSingleNode("n:root/n:device/n:friendlyName", xmlNamespaceManager).InnerText : "");
			this.m_Manufacturer = ((xmlDocument.SelectSingleNode("n:root/n:device/n:manufacturer", xmlNamespaceManager) != null) ? xmlDocument.SelectSingleNode("n:root/n:device/n:manufacturer", xmlNamespaceManager).InnerText : "");
			this.m_ManufacturerUrl = ((xmlDocument.SelectSingleNode("n:root/n:device/n:manufacturerURL", xmlNamespaceManager) != null) ? xmlDocument.SelectSingleNode("n:root/n:device/n:manufacturerURL", xmlNamespaceManager).InnerText : "");
			this.m_ModelDescription = ((xmlDocument.SelectSingleNode("n:root/n:device/n:modelDescription", xmlNamespaceManager) != null) ? xmlDocument.SelectSingleNode("n:root/n:device/n:modelDescription", xmlNamespaceManager).InnerText : "");
			this.m_ModelName = ((xmlDocument.SelectSingleNode("n:root/n:device/n:modelName", xmlNamespaceManager) != null) ? xmlDocument.SelectSingleNode("n:root/n:device/n:modelName", xmlNamespaceManager).InnerText : "");
			this.m_ModelNumber = ((xmlDocument.SelectSingleNode("n:root/n:device/n:modelNumber", xmlNamespaceManager) != null) ? xmlDocument.SelectSingleNode("n:root/n:device/n:modelNumber", xmlNamespaceManager).InnerText : "");
			this.m_ModelUrl = ((xmlDocument.SelectSingleNode("n:root/n:device/n:modelURL", xmlNamespaceManager) != null) ? xmlDocument.SelectSingleNode("n:root/n:device/n:modelURL", xmlNamespaceManager).InnerText : "");
			this.m_SerialNumber = ((xmlDocument.SelectSingleNode("n:root/n:device/n:serialNumber", xmlNamespaceManager) != null) ? xmlDocument.SelectSingleNode("n:root/n:device/n:serialNumber", xmlNamespaceManager).InnerText : "");
			this.m_UDN = ((xmlDocument.SelectSingleNode("n:root/n:device/n:UDN", xmlNamespaceManager) != null) ? xmlDocument.SelectSingleNode("n:root/n:device/n:UDN", xmlNamespaceManager).InnerText : "");
			this.m_PresentationUrl = ((xmlDocument.SelectSingleNode("n:root/n:device/n:presentationURL", xmlNamespaceManager) != null) ? xmlDocument.SelectSingleNode("n:root/n:device/n:presentationURL", xmlNamespaceManager).InnerText : "");
		}

		// Token: 0x170003D8 RID: 984
		// (get) Token: 0x06000BD3 RID: 3027 RVA: 0x00048624 File Offset: 0x00047624
		public string BaseUrl
		{
			get
			{
				return this.m_BaseUrl;
			}
		}

		// Token: 0x170003D9 RID: 985
		// (get) Token: 0x06000BD4 RID: 3028 RVA: 0x0004863C File Offset: 0x0004763C
		public string DeviceType
		{
			get
			{
				return this.m_DeviceType;
			}
		}

		// Token: 0x170003DA RID: 986
		// (get) Token: 0x06000BD5 RID: 3029 RVA: 0x00048654 File Offset: 0x00047654
		public string FriendlyName
		{
			get
			{
				return this.m_FriendlyName;
			}
		}

		// Token: 0x170003DB RID: 987
		// (get) Token: 0x06000BD6 RID: 3030 RVA: 0x0004866C File Offset: 0x0004766C
		public string Manufacturer
		{
			get
			{
				return this.m_Manufacturer;
			}
		}

		// Token: 0x170003DC RID: 988
		// (get) Token: 0x06000BD7 RID: 3031 RVA: 0x00048684 File Offset: 0x00047684
		public string ManufacturerUrl
		{
			get
			{
				return this.m_ManufacturerUrl;
			}
		}

		// Token: 0x170003DD RID: 989
		// (get) Token: 0x06000BD8 RID: 3032 RVA: 0x0004869C File Offset: 0x0004769C
		public string ModelDescription
		{
			get
			{
				return this.m_ModelDescription;
			}
		}

		// Token: 0x170003DE RID: 990
		// (get) Token: 0x06000BD9 RID: 3033 RVA: 0x000486B4 File Offset: 0x000476B4
		public string ModelName
		{
			get
			{
				return this.m_ModelName;
			}
		}

		// Token: 0x170003DF RID: 991
		// (get) Token: 0x06000BDA RID: 3034 RVA: 0x000486CC File Offset: 0x000476CC
		public string ModelNumber
		{
			get
			{
				return this.m_ModelNumber;
			}
		}

		// Token: 0x170003E0 RID: 992
		// (get) Token: 0x06000BDB RID: 3035 RVA: 0x000486E4 File Offset: 0x000476E4
		public string ModelUrl
		{
			get
			{
				return this.m_ModelUrl;
			}
		}

		// Token: 0x170003E1 RID: 993
		// (get) Token: 0x06000BDC RID: 3036 RVA: 0x000486FC File Offset: 0x000476FC
		public string SerialNumber
		{
			get
			{
				return this.m_SerialNumber;
			}
		}

		// Token: 0x170003E2 RID: 994
		// (get) Token: 0x06000BDD RID: 3037 RVA: 0x00048714 File Offset: 0x00047714
		public string UDN
		{
			get
			{
				return this.m_UDN;
			}
		}

		// Token: 0x170003E3 RID: 995
		// (get) Token: 0x06000BDE RID: 3038 RVA: 0x0004872C File Offset: 0x0004772C
		public string PresentationUrl
		{
			get
			{
				return this.m_PresentationUrl;
			}
		}

		// Token: 0x170003E4 RID: 996
		// (get) Token: 0x06000BDF RID: 3039 RVA: 0x00048744 File Offset: 0x00047744
		public string DeviceXml
		{
			get
			{
				return this.m_DeviceXml;
			}
		}

		// Token: 0x040004D0 RID: 1232
		private string m_BaseUrl = "";

		// Token: 0x040004D1 RID: 1233
		private string m_DeviceType = "";

		// Token: 0x040004D2 RID: 1234
		private string m_FriendlyName = "";

		// Token: 0x040004D3 RID: 1235
		private string m_Manufacturer = "";

		// Token: 0x040004D4 RID: 1236
		private string m_ManufacturerUrl = "";

		// Token: 0x040004D5 RID: 1237
		private string m_ModelDescription = "";

		// Token: 0x040004D6 RID: 1238
		private string m_ModelName = "";

		// Token: 0x040004D7 RID: 1239
		private string m_ModelNumber = "";

		// Token: 0x040004D8 RID: 1240
		private string m_ModelUrl = "";

		// Token: 0x040004D9 RID: 1241
		private string m_SerialNumber = "";

		// Token: 0x040004DA RID: 1242
		private string m_UDN = "";

		// Token: 0x040004DB RID: 1243
		private string m_PresentationUrl = "";

		// Token: 0x040004DC RID: 1244
		private string m_DeviceXml = null;
	}
}
