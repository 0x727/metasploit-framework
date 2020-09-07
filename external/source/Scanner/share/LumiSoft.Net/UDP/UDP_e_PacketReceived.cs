using System;
using System.Net;
using System.Net.Sockets;

namespace LumiSoft.Net.UDP
{
	// Token: 0x0200012F RID: 303
	public class UDP_e_PacketReceived : EventArgs
	{
		// Token: 0x06000C03 RID: 3075 RVA: 0x00049D50 File Offset: 0x00048D50
		internal UDP_e_PacketReceived()
		{
		}

		// Token: 0x06000C04 RID: 3076 RVA: 0x00049D76 File Offset: 0x00048D76
		internal void Reuse(Socket socket, byte[] buffer, int count, IPEndPoint remoteEP)
		{
			this.m_pSocket = socket;
			this.m_pBuffer = buffer;
			this.m_Count = count;
			this.m_pRemoteEP = remoteEP;
		}

		// Token: 0x170003EE RID: 1006
		// (get) Token: 0x06000C05 RID: 3077 RVA: 0x00049D98 File Offset: 0x00048D98
		public Socket Socket
		{
			get
			{
				return this.m_pSocket;
			}
		}

		// Token: 0x170003EF RID: 1007
		// (get) Token: 0x06000C06 RID: 3078 RVA: 0x00049DB0 File Offset: 0x00048DB0
		public byte[] Buffer
		{
			get
			{
				return this.m_pBuffer;
			}
		}

		// Token: 0x170003F0 RID: 1008
		// (get) Token: 0x06000C07 RID: 3079 RVA: 0x00049DC8 File Offset: 0x00048DC8
		public int Count
		{
			get
			{
				return this.m_Count;
			}
		}

		// Token: 0x170003F1 RID: 1009
		// (get) Token: 0x06000C08 RID: 3080 RVA: 0x00049DE0 File Offset: 0x00048DE0
		public IPEndPoint RemoteEP
		{
			get
			{
				return this.m_pRemoteEP;
			}
		}

		// Token: 0x040004F1 RID: 1265
		private Socket m_pSocket = null;

		// Token: 0x040004F2 RID: 1266
		private byte[] m_pBuffer = null;

		// Token: 0x040004F3 RID: 1267
		private int m_Count = 0;

		// Token: 0x040004F4 RID: 1268
		private IPEndPoint m_pRemoteEP = null;
	}
}
