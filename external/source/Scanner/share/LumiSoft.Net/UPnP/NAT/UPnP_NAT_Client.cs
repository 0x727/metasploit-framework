using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Xml;
using LumiSoft.Net.UPnP.Client;

namespace LumiSoft.Net.UPnP.NAT
{
	// Token: 0x0200012B RID: 299
	public class UPnP_NAT_Client
	{
		// Token: 0x06000BE0 RID: 3040 RVA: 0x0004875C File Offset: 0x0004775C
		public UPnP_NAT_Client()
		{
			this.Init();
		}

		// Token: 0x06000BE1 RID: 3041 RVA: 0x00048770 File Offset: 0x00047770
		private void Init()
		{
			try
			{
				UPnP_Client upnP_Client = new UPnP_Client();
				UPnP_Device[] array = null;
				try
				{
					IPAddress ip = null;
					foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
					{
						bool flag = networkInterface.OperationalStatus == OperationalStatus.Up;
						if (flag)
						{
							using (IEnumerator<GatewayIPAddressInformation> enumerator = networkInterface.GetIPProperties().GatewayAddresses.GetEnumerator())
							{
								if (enumerator.MoveNext())
								{
									GatewayIPAddressInformation gatewayIPAddressInformation = enumerator.Current;
									ip = gatewayIPAddressInformation.Address;
								}
							}
							break;
						}
					}
					array = upnP_Client.Search(ip, "urn:schemas-upnp-org:device:InternetGatewayDevice:1", 1200);
				}
				catch
				{
				}
				bool flag2 = array.Length == 0;
				if (flag2)
				{
					array = upnP_Client.Search("urn:schemas-upnp-org:device:InternetGatewayDevice:1", 1200);
				}
				bool flag3 = array.Length != 0;
				if (flag3)
				{
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.LoadXml(array[0].DeviceXml);
					List<XmlNode> list = new List<XmlNode>();
					list.Add(xmlDocument);
					while (list.Count > 0)
					{
						XmlNode xmlNode = list[0];
						list.RemoveAt(0);
						bool flag4 = string.Equals("urn:schemas-upnp-org:service:WANPPPConnection:1", xmlNode.InnerText, StringComparison.InvariantCultureIgnoreCase);
						if (flag4)
						{
							foreach (object obj in xmlNode.ParentNode.ChildNodes)
							{
								XmlNode xmlNode2 = (XmlNode)obj;
								bool flag5 = string.Equals("controlURL", xmlNode2.Name, StringComparison.InvariantCultureIgnoreCase);
								if (flag5)
								{
									UPnP_NAT_Client.m_BaseUrl = array[0].BaseUrl;
									UPnP_NAT_Client.m_ServiceType = "urn:schemas-upnp-org:service:WANPPPConnection:1";
									UPnP_NAT_Client.m_ControlUrl = xmlNode2.InnerText;
									return;
								}
							}
						}
						else
						{
							bool flag6 = string.Equals("urn:schemas-upnp-org:service:WANIPConnection:1", xmlNode.InnerText, StringComparison.InvariantCultureIgnoreCase);
							if (flag6)
							{
								foreach (object obj2 in xmlNode.ParentNode.ChildNodes)
								{
									XmlNode xmlNode3 = (XmlNode)obj2;
									bool flag7 = string.Equals("controlURL", xmlNode3.Name, StringComparison.InvariantCultureIgnoreCase);
									if (flag7)
									{
										UPnP_NAT_Client.m_BaseUrl = array[0].BaseUrl;
										UPnP_NAT_Client.m_ServiceType = "urn:schemas-upnp-org:service:WANIPConnection:1";
										UPnP_NAT_Client.m_ControlUrl = xmlNode3.InnerText;
										return;
									}
								}
							}
							else
							{
								bool flag8 = xmlNode.ChildNodes.Count > 0;
								if (flag8)
								{
									for (int j = 0; j < xmlNode.ChildNodes.Count; j++)
									{
										list.Insert(j, xmlNode.ChildNodes[j]);
									}
								}
							}
						}
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000BE2 RID: 3042 RVA: 0x00048AC4 File Offset: 0x00047AC4
		public IPAddress GetExternalIPAddress()
		{
			string soapData = "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">\r\n<s:Body>\r\n<u:GetExternalIPAddress xmlns:u=\"" + UPnP_NAT_Client.m_ServiceType + "\"></u:GetExternalIPAddress>\r\n</s:Body>\r\n</s:Envelope>\r\n";
			string s = this.SendCommand("GetExternalIPAddress", soapData);
			//XmlReader xmlReader = XmlReader.Create(new StringReader(s));
			XmlReader xmlReader = XmlReader.Create(s);
			while (xmlReader.Read())
			{
				bool flag = string.Equals("NewExternalIPAddress", xmlReader.Name, StringComparison.InvariantCultureIgnoreCase);
				if (flag)
				{
					return IPAddress.Parse(xmlReader.ReadString());
				}
			}
			return null;
		}

		// Token: 0x06000BE3 RID: 3043 RVA: 0x00048B3C File Offset: 0x00047B3C
		public UPnP_NAT_Map[] GetPortMappings()
		{
			List<UPnP_NAT_Map> list = new List<UPnP_NAT_Map>();
			for (int i = 0; i < 100; i++)
			{
				try
				{
					string soapData = string.Concat(new object[]
					{
						"<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">\r\n<s:Body>\r\n<u:GetGenericPortMappingEntry xmlns:u=\"",
						UPnP_NAT_Client.m_ServiceType,
						"\">\r\n<NewPortMappingIndex>",
						i,
						"</NewPortMappingIndex>\r\n</u:GetGenericPortMappingEntry>\r\n</s:Body>\r\n</s:Envelope>\r\n"
					});
					string s = this.SendCommand("GetGenericPortMappingEntry", soapData);
					bool enabled = false;
					string protocol = "";
					string remoteHost = "";
					string externalPort = "";
					string internalHost = "";
					int internalPort = 0;
					string description = "";
					int leaseDuration = 0;
					//XmlReader xmlReader = XmlReader.Create(new StringReader(s));
					XmlReader xmlReader = XmlReader.Create(s);
					while (xmlReader.Read())
					{
						bool flag = string.Equals("NewRemoteHost", xmlReader.Name, StringComparison.InvariantCultureIgnoreCase);
						if (flag)
						{
							remoteHost = xmlReader.ReadString();
						}
						else
						{
							bool flag2 = string.Equals("NewExternalPort", xmlReader.Name, StringComparison.InvariantCultureIgnoreCase);
							if (flag2)
							{
								externalPort = xmlReader.ReadString();
							}
							else
							{
								bool flag3 = string.Equals("NewProtocol", xmlReader.Name, StringComparison.InvariantCultureIgnoreCase);
								if (flag3)
								{
									protocol = xmlReader.ReadString();
								}
								else
								{
									bool flag4 = string.Equals("NewInternalPort", xmlReader.Name, StringComparison.InvariantCultureIgnoreCase);
									if (flag4)
									{
										internalPort = Convert.ToInt32(xmlReader.ReadString());
									}
									else
									{
										bool flag5 = string.Equals("NewInternalClient", xmlReader.Name, StringComparison.InvariantCultureIgnoreCase);
										if (flag5)
										{
											internalHost = xmlReader.ReadString();
										}
										else
										{
											bool flag6 = string.Equals("NewEnabled", xmlReader.Name, StringComparison.InvariantCultureIgnoreCase);
											if (flag6)
											{
												enabled = Convert.ToBoolean(Convert.ToInt32(xmlReader.ReadString()));
											}
											else
											{
												bool flag7 = string.Equals("NewPortMappingDescription", xmlReader.Name, StringComparison.InvariantCultureIgnoreCase);
												if (flag7)
												{
													description = xmlReader.ReadString();
												}
												else
												{
													bool flag8 = string.Equals("NewLeaseDuration", xmlReader.Name, StringComparison.InvariantCultureIgnoreCase);
													if (flag8)
													{
														leaseDuration = Convert.ToInt32(xmlReader.ReadString());
													}
												}
											}
										}
									}
								}
							}
						}
					}
					list.Add(new UPnP_NAT_Map(enabled, protocol, remoteHost, externalPort, internalHost, internalPort, description, leaseDuration));
				}
				catch (WebException ex)
				{
					bool flag9 = ex.Response.ContentType.ToLower().IndexOf("text/xml") > -1;
					if (!flag9)
					{
						throw ex;
					}
					UPnP_Exception ex2 = UPnP_Exception.Parse(ex.Response.GetResponseStream());
					bool flag10 = ex2.ErrorCode != 713;
					if (flag10)
					{
						throw ex2;
					}
					break;
				}
			}
			return list.ToArray();
		}

		// Token: 0x06000BE4 RID: 3044 RVA: 0x00048DEC File Offset: 0x00047DEC
		public void AddPortMapping(bool enabled, string description, string protocol, string remoteHost, int publicPort, IPEndPoint localEP, int leaseDuration)
		{
			bool flag = description == null;
			if (flag)
			{
				throw new ArgumentNullException("description");
			}
			bool flag2 = protocol == null;
			if (flag2)
			{
				throw new ArgumentNullException("protocol");
			}
			bool flag3 = localEP == null;
			if (flag3)
			{
				throw new ArgumentNullException("localEP");
			}
			try
			{
				string soapData = string.Concat(new object[]
				{
					"<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">\r\n<s:Body>\r\n<u:AddPortMapping xmlns:u=\"",
					UPnP_NAT_Client.m_ServiceType,
					"\">\r\n<NewRemoteHost>",
					remoteHost,
					"</NewRemoteHost>\r\n<NewExternalPort>",
					publicPort.ToString(),
					"</NewExternalPort>\r\n<NewProtocol>",
					protocol,
					"</NewProtocol>\r\n<NewInternalPort>",
					localEP.Port.ToString(),
					"</NewInternalPort>\r\n<NewInternalClient>",
					localEP.Address.ToString(),
					"</NewInternalClient>\r\n<NewEnabled>",
					Convert.ToInt32(enabled),
					"</NewEnabled>\r\n<NewPortMappingDescription>",
					description,
					"</NewPortMappingDescription>\r\n<NewLeaseDuration>",
					leaseDuration,
					"</NewLeaseDuration>\r\n</u:AddPortMapping>\r\n</s:Body>\r\n</s:Envelope>\r\n"
				});
				string text = this.SendCommand("AddPortMapping", soapData);
			}
			catch (WebException ex)
			{
				bool flag4 = ex.Response.ContentType.ToLower().IndexOf("text/xml") > -1;
				if (flag4)
				{
					throw UPnP_Exception.Parse(ex.Response.GetResponseStream());
				}
			}
		}

		// Token: 0x06000BE5 RID: 3045 RVA: 0x00048F58 File Offset: 0x00047F58
		public void DeletePortMapping(UPnP_NAT_Map map)
		{
			bool flag = map == null;
			if (flag)
			{
				throw new ArgumentNullException("map");
			}
			this.DeletePortMapping(map.Protocol, map.RemoteHost, Convert.ToInt32(map.ExternalPort));
		}

		// Token: 0x06000BE6 RID: 3046 RVA: 0x00048F98 File Offset: 0x00047F98
		public void DeletePortMapping(string protocol, string remoteHost, int publicPort)
		{
			bool flag = protocol == null;
			if (flag)
			{
				throw new ArgumentNullException("protocol");
			}
			try
			{
				string soapData = string.Concat(new string[]
				{
					"<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">\r\n<s:Body>\r\n<u:DeletePortMapping xmlns:u=\"",
					UPnP_NAT_Client.m_ServiceType,
					"\">\r\n<NewRemoteHost>",
					remoteHost,
					"</NewRemoteHost>\r\n<NewExternalPort>",
					publicPort.ToString(),
					"</NewExternalPort>\r\n<NewProtocol>",
					protocol,
					"</NewProtocol>\r\n</u:DeletePortMapping>\r\n</s:Body>\r\n</s:Envelope>\r\n"
				});
				this.SendCommand("DeletePortMapping", soapData);
			}
			catch (WebException ex)
			{
				bool flag2 = ex.Response.ContentType.ToLower().IndexOf("text/xml") > -1;
				if (flag2)
				{
					throw UPnP_Exception.Parse(ex.Response.GetResponseStream());
				}
			}
		}

		// Token: 0x06000BE7 RID: 3047 RVA: 0x00049064 File Offset: 0x00048064
		private string SendCommand(string method, string soapData)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(soapData);
			WebRequest webRequest = WebRequest.Create(UPnP_NAT_Client.m_BaseUrl + UPnP_NAT_Client.m_ControlUrl);
			webRequest.Method = "POST";
			webRequest.Headers.Add("SOAPAction", UPnP_NAT_Client.m_ServiceType + "#" + method);
			webRequest.ContentType = "text/xml; charset=\"utf-8\";";
			webRequest.ContentLength = (long)bytes.Length;
			webRequest.GetRequestStream().Write(bytes, 0, bytes.Length);
			webRequest.GetRequestStream().Close();
			WebResponse response = webRequest.GetResponse();
			string result;
			using (TextReader textReader = new StreamReader(response.GetResponseStream()))
			{
				result = textReader.ReadToEnd();
			}
			return result;
		}

		// Token: 0x170003E5 RID: 997
		// (get) Token: 0x06000BE8 RID: 3048 RVA: 0x00049130 File Offset: 0x00048130
		public bool IsSupported
		{
			get
			{
				return UPnP_NAT_Client.m_ControlUrl != null;
			}
		}

		// Token: 0x040004DD RID: 1245
		private static string m_BaseUrl = null;

		// Token: 0x040004DE RID: 1246
		private static string m_ServiceType = null;

		// Token: 0x040004DF RID: 1247
		private static string m_ControlUrl = null;
	}
}
