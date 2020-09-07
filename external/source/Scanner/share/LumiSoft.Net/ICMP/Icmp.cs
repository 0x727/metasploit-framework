using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace LumiSoft.Net.ICMP
{
	// Token: 0x0200002E RID: 46
	public class Icmp
	{
		// Token: 0x0600014F RID: 335 RVA: 0x000095F8 File Offset: 0x000085F8
		public static EchoMessage[] Trace(string destIP)
		{
			return Icmp.Trace(IPAddress.Parse(destIP), 2000);
		}

		// Token: 0x06000150 RID: 336 RVA: 0x0000961C File Offset: 0x0000861C
		public static EchoMessage[] Trace(IPAddress ip, int timeout)
		{
			List<EchoMessage> list = new List<EchoMessage>();
			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);
			IPEndPoint remoteEP = new IPEndPoint(ip, 80);
			EndPoint endPoint = new IPEndPoint(Dns.GetHostEntry(Dns.GetHostName()).AddressList[0], 80);
			ushort id = (ushort)DateTime.Now.Millisecond;
			byte[] array = Icmp.CreatePacket(id);
			int num = 0;
			for (int i = 1; i <= 30; i++)
			{
				byte[] array2 = new byte[1024];
				try
				{
					socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, i);
					socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, timeout);
					socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, timeout);
					DateTime now = DateTime.Now;
					socket.SendTo(array, array.Length, SocketFlags.None, remoteEP);
					socket.ReceiveFrom(array2, array2.Length, SocketFlags.None, ref endPoint);
					TimeSpan timeSpan = DateTime.Now - now;
					list.Add(new EchoMessage(((IPEndPoint)endPoint).Address, i, timeSpan.Milliseconds));
					bool flag = array2[20] == 0;
					if (flag)
					{
						break;
					}
					bool flag2 = array2[20] != 11;
					if (flag2)
					{
						throw new Exception("UnKnown error !");
					}
					num = 0;
				}
				catch
				{
					num++;
				}
				bool flag3 = num >= 3;
				if (flag3)
				{
					break;
				}
			}
			return list.ToArray();
		}

		// Token: 0x06000151 RID: 337 RVA: 0x00009798 File Offset: 0x00008798
		public static EchoMessage Ping(IPAddress ip, int timeout)
		{
			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);
			IPEndPoint remoteEP = new IPEndPoint(ip, 80);
			EndPoint endPoint = new IPEndPoint(Dns.GetHostEntry(Dns.GetHostName()).AddressList[0], 80);
			ushort id = (ushort)DateTime.Now.Millisecond;
			byte[] array = Icmp.CreatePacket(id);
			socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, 30);
			socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, timeout);
			socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, timeout);
			DateTime now = DateTime.Now;
			socket.SendTo(array, array.Length, SocketFlags.None, remoteEP);
			byte[] array2 = new byte[1024];
			socket.ReceiveFrom(array2, array2.Length, SocketFlags.None, ref endPoint);
			bool flag = array2[20] == 0;
			if (!flag)
			{
				bool flag2 = array2[20] != 11;
				if (flag2)
				{
					throw new Exception("UnKnown error !");
				}
			}
			TimeSpan timeSpan = DateTime.Now - now;
			return new EchoMessage(((IPEndPoint)endPoint).Address, 0, timeSpan.Milliseconds);
		}

		// Token: 0x06000152 RID: 338 RVA: 0x000098A8 File Offset: 0x000088A8
		private static byte[] CreatePacket(ushort id)
		{
			byte[] array = new byte[10];
			array[0] = 8;
			array[1] = 0;
			array[2] = 0;
			array[3] = 0;
			array[4] = 0;
			array[5] = 0;
			array[6] = 0;
			array[7] = 0;
			Array.Copy(BitConverter.GetBytes(id), 0, array, 4, 2);
			for (int i = 0; i < 2; i++)
			{
				array[i + 8] = 120;
			}
			int num = 0;
			for (int j = 0; j < array.Length; j += 2)
			{
				num += Convert.ToInt32(BitConverter.ToUInt16(array, j));
			}
			num &= 65535;
			Array.Copy(BitConverter.GetBytes((ushort)(~(ushort)num)), 0, array, 2, 2);
			return array;
		}
	}
}
