using System;
using System.Collections.Generic;
using System.Net;

namespace LumiSoft.Net.TCP
{
	// Token: 0x02000033 RID: 51
	public class TCP_SessionCollection<T> where T : TCP_Session
	{
		// Token: 0x060001E0 RID: 480 RVA: 0x0000C381 File Offset: 0x0000B381
		internal TCP_SessionCollection()
		{
			this.m_pItems = new Dictionary<string, T>();
			this.m_pConnectionsPerIP = new Dictionary<string, long>();
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x0000C3B0 File Offset: 0x0000B3B0
		internal void Add(T session)
		{
			bool flag = session == null;
			if (flag)
			{
				throw new ArgumentNullException("session");
			}
			Dictionary<string, T> pItems = this.m_pItems;
			lock (pItems)
			{
				this.m_pItems.Add(session.ID, session);
				bool flag3 = session.IsConnected && session.RemoteEndPoint != null;
				if (flag3)
				{
					bool flag4 = this.m_pConnectionsPerIP.ContainsKey(session.RemoteEndPoint.Address.ToString());
					if (flag4)
					{
						Dictionary<string, long> pConnectionsPerIP = this.m_pConnectionsPerIP;
						string key = session.RemoteEndPoint.Address.ToString();
						long num = pConnectionsPerIP[key];
						pConnectionsPerIP[key] = num + 1L;
					}
					else
					{
						this.m_pConnectionsPerIP.Add(session.RemoteEndPoint.Address.ToString(), 1L);
					}
				}
			}
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x0000C4CC File Offset: 0x0000B4CC
		internal void Remove(T session)
		{
			bool flag = session == null;
			if (flag)
			{
				throw new ArgumentNullException("session");
			}
			Dictionary<string, T> pItems = this.m_pItems;
			lock (pItems)
			{
				this.m_pItems.Remove(session.ID);
				bool flag3 = session.IsConnected && this.m_pConnectionsPerIP.ContainsKey(session.RemoteEndPoint.Address.ToString());
				if (flag3)
				{
					Dictionary<string, long> pConnectionsPerIP = this.m_pConnectionsPerIP;
					string key = session.RemoteEndPoint.Address.ToString();
					long num = pConnectionsPerIP[key];
					pConnectionsPerIP[key] = num - 1L;
					bool flag4 = this.m_pConnectionsPerIP[session.RemoteEndPoint.Address.ToString()] == 0L;
					if (flag4)
					{
						this.m_pConnectionsPerIP.Remove(session.RemoteEndPoint.Address.ToString());
					}
				}
			}
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x0000C5F8 File Offset: 0x0000B5F8
		internal void Clear()
		{
			Dictionary<string, T> pItems = this.m_pItems;
			lock (pItems)
			{
				this.m_pItems.Clear();
				this.m_pConnectionsPerIP.Clear();
			}
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x0000C650 File Offset: 0x0000B650
		public T[] ToArray()
		{
			Dictionary<string, T> pItems = this.m_pItems;
			T[] result;
			lock (pItems)
			{
				T[] array = new T[this.m_pItems.Count];
				this.m_pItems.Values.CopyTo(array, 0);
				result = array;
			}
			return result;
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x0000C6B8 File Offset: 0x0000B6B8
		public long GetConnectionsPerIP(IPAddress ip)
		{
			bool flag = ip == null;
			if (flag)
			{
				throw new ArgumentNullException("ip");
			}
			long result = 0L;
			this.m_pConnectionsPerIP.TryGetValue(ip.ToString(), out result);
			return result;
		}

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x060001E6 RID: 486 RVA: 0x0000C6F8 File Offset: 0x0000B6F8
		public int Count
		{
			get
			{
				return this.m_pItems.Count;
			}
		}

		// Token: 0x1700008F RID: 143
		public T this[string id]
		{
			get
			{
				return this.m_pItems[id];
			}
		}

		// Token: 0x040000CD RID: 205
		private Dictionary<string, T> m_pItems = null;

		// Token: 0x040000CE RID: 206
		private Dictionary<string, long> m_pConnectionsPerIP = null;
	}
}
