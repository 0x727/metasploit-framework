using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LumiSoft.Net.UPnP.Client
{
	// Token: 0x0200012D RID: 301
	public class UPnP_Client
	{
		// Token: 0x06000BF4 RID: 3060 RVA: 0x000492C0 File Offset: 0x000482C0
		public UPnP_Device[] Search(int timeout)
		{
			return this.Search("upnp:rootdevice", timeout);
		}

		// Token: 0x06000BF5 RID: 3061 RVA: 0x000492E0 File Offset: 0x000482E0
		public UPnP_Device[] Search(string deviceType, int timeout)
		{
			bool flag = timeout < 1;
			if (flag)
			{
				timeout = 1;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("M-SEARCH * HTTP/1.1\r\n");
			stringBuilder.Append("HOST: 239.255.255.250:1900\r\n");
			stringBuilder.Append("MAN: \"ssdp:discover\"\r\n");
			stringBuilder.Append("MX: 1\r\n");
			stringBuilder.Append("ST: " + deviceType + "\r\n");
			stringBuilder.Append("\r\n");
			UPnP_Device[] result;
			using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
			{
				socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
				socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 2);
				socket.SendTo(Encoding.UTF8.GetBytes(stringBuilder.ToString()), new IPEndPoint(IPAddress.Broadcast, 1900));
				List<string> list = new List<string>();
				byte[] array = new byte[32000];
				while (DateTime.Now.AddMilliseconds((double)timeout) > DateTime.Now)
				{
					bool flag2 = socket.Poll(1, SelectMode.SelectRead);
					if (flag2)
					{
						int count = socket.Receive(array);
						string[] array2 = Encoding.UTF8.GetString(array, 0, count).Split(new char[]
						{
							'\n'
						});
						foreach (string text in array2)
						{
							string[] array4 = text.Split(new char[]
							{
								':'
							}, 2);
							bool flag3 = string.Equals(array4[0], "location", StringComparison.InvariantCultureIgnoreCase);
							if (flag3)
							{
								list.Add(array4[1].Trim());
							}
						}
					}
				}
				List<UPnP_Device> list2 = new List<UPnP_Device>();
				foreach (string url in list)
				{
					try
					{
						list2.Add(new UPnP_Device(url));
					}
					catch
					{
					}
				}
				result = list2.ToArray();
			}
			return result;
		}

		// Token: 0x06000BF6 RID: 3062 RVA: 0x0004952C File Offset: 0x0004852C
		public UPnP_Device[] Search(IPAddress ip, string deviceType, int timeout)
		{
			bool flag = ip == null;
			if (flag)
			{
				throw new ArgumentNullException("ip");
			}
			bool flag2 = timeout < 1;
			if (flag2)
			{
				timeout = 1;
			}
			UPnP_Device[] result;
			using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
			{
				socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 2);
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("M-SEARCH * HTTP/1.1\r\n");
				stringBuilder.Append("MAN: \"ssdp:discover\"\r\n");
				stringBuilder.Append("MX: 1\r\n");
				stringBuilder.Append("ST: " + deviceType + "\r\n");
				stringBuilder.Append("\r\n");
				socket.SendTo(Encoding.UTF8.GetBytes(stringBuilder.ToString()), new IPEndPoint(ip, 1900));
				List<string> list = new List<string>();
				byte[] array = new byte[32000];
				while (DateTime.Now.AddMilliseconds((double)timeout) > DateTime.Now)
				{
					bool flag3 = socket.Poll(1, SelectMode.SelectRead);
					if (flag3)
					{
						int count = socket.Receive(array);
						string[] array2 = Encoding.UTF8.GetString(array, 0, count).Split(new char[]
						{
							'\n'
						});
						foreach (string text in array2)
						{
							string[] array4 = text.Split(new char[]
							{
								':'
							}, 2);
							bool flag4 = string.Equals(array4[0], "location", StringComparison.InvariantCultureIgnoreCase);
							if (flag4)
							{
								list.Add(array4[1].Trim());
							}
						}
					}
				}
				List<UPnP_Device> list2 = new List<UPnP_Device>();
				foreach (string url in list)
				{
					try
					{
						list2.Add(new UPnP_Device(url));
					}
					catch
					{
					}
				}
				result = list2.ToArray();
			}
			return result;
		}
	}
}
