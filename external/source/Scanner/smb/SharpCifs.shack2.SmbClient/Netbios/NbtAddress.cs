using System;
using System.Net;
using SharpCifs.Util;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Netbios
{
	// Token: 0x020000D6 RID: 214
	public sealed class NbtAddress
	{
		// Token: 0x06000711 RID: 1809 RVA: 0x00026EEC File Offset: 0x000250EC
		static NbtAddress()
		{
			NbtAddress.AddressCache.Put(NbtAddress.UnknownName, new NbtAddress.CacheEntry(NbtAddress.UnknownName, NbtAddress.UnknownAddress, -1L));
			IPAddress ipaddress = NbtAddress.Client.laddr;
			bool flag = ipaddress == null;
			if (flag)
			{
				try
				{
					ipaddress = Extensions.GetAddressByName("127.0.0.1");
				}
				catch (UnknownHostException)
				{
				}
			}
			string text = Config.GetProperty("jcifs.netbios.hostname", null);
			bool flag2 = string.IsNullOrEmpty(text);
			if (flag2)
			{
				byte[] addressBytes = ipaddress.GetAddressBytes();
				text = "JCIFS_127_0_0_1";
			}
			Name hostName = new Name(text, 0, Config.GetProperty("jcifs.netbios.scope", null));
			NbtAddress.Localhost = new NbtAddress(hostName, ipaddress.GetHashCode(), false, 0, false, false, true, false, NbtAddress.UnknownMacAddress);
			NbtAddress.CacheAddress(hostName, NbtAddress.Localhost, -1L);
		}

		// Token: 0x06000712 RID: 1810 RVA: 0x00027050 File Offset: 0x00025250
		internal static void CacheAddress(Name hostName, NbtAddress addr)
		{
			bool flag = NbtAddress.CachePolicy == 0;
			if (!flag)
			{
				long expiration = -1L;
				bool flag2 = NbtAddress.CachePolicy != -1;
				if (flag2)
				{
					expiration = Runtime.CurrentTimeMillis() + (long)(NbtAddress.CachePolicy * 1000);
				}
				NbtAddress.CacheAddress(hostName, addr, expiration);
			}
		}

		// Token: 0x06000713 RID: 1811 RVA: 0x000270A0 File Offset: 0x000252A0
		internal static void CacheAddress(Name hostName, NbtAddress addr, long expiration)
		{
			bool flag = NbtAddress.CachePolicy == 0;
			if (!flag)
			{
				Hashtable addressCache = NbtAddress.AddressCache;
				lock (addressCache)
				{
					NbtAddress.CacheEntry cacheEntry = (NbtAddress.CacheEntry)NbtAddress.AddressCache.Get(hostName);
					bool flag3 = cacheEntry == null;
					if (flag3)
					{
						cacheEntry = new NbtAddress.CacheEntry(hostName, addr, expiration);
						NbtAddress.AddressCache.Put(hostName, cacheEntry);
					}
					else
					{
						cacheEntry.Address = addr;
						cacheEntry.Expiration = expiration;
					}
				}
			}
		}

		// Token: 0x06000714 RID: 1812 RVA: 0x00027134 File Offset: 0x00025334
		internal static void CacheAddressArray(NbtAddress[] addrs)
		{
			bool flag = NbtAddress.CachePolicy == 0;
			if (!flag)
			{
				long expiration = -1L;
				bool flag2 = NbtAddress.CachePolicy != -1;
				if (flag2)
				{
					expiration = Runtime.CurrentTimeMillis() + (long)(NbtAddress.CachePolicy * 1000);
				}
				Hashtable addressCache = NbtAddress.AddressCache;
				lock (addressCache)
				{
					for (int i = 0; i < addrs.Length; i++)
					{
						NbtAddress.CacheEntry cacheEntry = (NbtAddress.CacheEntry)NbtAddress.AddressCache.Get(addrs[i].HostName);
						bool flag4 = cacheEntry == null;
						if (flag4)
						{
							cacheEntry = new NbtAddress.CacheEntry(addrs[i].HostName, addrs[i], expiration);
							NbtAddress.AddressCache.Put(addrs[i].HostName, cacheEntry);
						}
						else
						{
							cacheEntry.Address = addrs[i];
							cacheEntry.Expiration = expiration;
						}
					}
				}
			}
		}

		// Token: 0x06000715 RID: 1813 RVA: 0x00027230 File Offset: 0x00025430
		internal static NbtAddress GetCachedAddress(Name hostName)
		{
			bool flag = NbtAddress.CachePolicy == 0;
			NbtAddress result;
			if (flag)
			{
				result = null;
			}
			else
			{
				Hashtable addressCache = NbtAddress.AddressCache;
				lock (addressCache)
				{
					NbtAddress.CacheEntry cacheEntry = (NbtAddress.CacheEntry)NbtAddress.AddressCache.Get(hostName);
					bool flag3 = cacheEntry != null && cacheEntry.Expiration < Runtime.CurrentTimeMillis() && cacheEntry.Expiration >= 0L;
					if (flag3)
					{
						cacheEntry = null;
					}
					result = ((cacheEntry != null) ? cacheEntry.Address : null);
				}
			}
			return result;
		}

		// Token: 0x06000716 RID: 1814 RVA: 0x000272D0 File Offset: 0x000254D0
		internal static NbtAddress DoNameQuery(Name name, IPAddress svr)
		{
			bool flag = name.HexCode == 29 && svr == null;
			if (flag)
			{
				svr = NbtAddress.Client.Baddr;
			}
			name.SrcHashCode = ((svr != null) ? svr.GetHashCode() : 0);
			NbtAddress nbtAddress = NbtAddress.GetCachedAddress(name);
			bool flag2 = nbtAddress == null;
			if (flag2)
			{
				bool flag3 = (nbtAddress = (NbtAddress)NbtAddress.CheckLookupTable(name)) == null;
				if (flag3)
				{
					try
					{
						nbtAddress = NbtAddress.Client.GetByName(name, svr);
					}
					catch (UnknownHostException)
					{
						nbtAddress = NbtAddress.UnknownAddress;
					}
					finally
					{
						NbtAddress.CacheAddress(name, nbtAddress);
						NbtAddress.UpdateLookupTable(name);
					}
				}
			}
			bool flag4 = nbtAddress == NbtAddress.UnknownAddress;
			if (flag4)
			{
				throw new UnknownHostException(name.ToString());
			}
			return nbtAddress;
		}

		// Token: 0x06000717 RID: 1815 RVA: 0x000273A8 File Offset: 0x000255A8
		private static object CheckLookupTable(Name name)
		{
			Hashtable lookupTable = NbtAddress.LookupTable;
			lock (lookupTable)
			{
				bool flag2 = !NbtAddress.LookupTable.ContainsKey(name);
				if (flag2)
				{
					NbtAddress.LookupTable.Put(name, name);
					return null;
				}
				while (NbtAddress.LookupTable.ContainsKey(name))
				{
					try
					{
						Runtime.Wait(NbtAddress.LookupTable);
					}
					catch (Exception)
					{
					}
				}
			}
			object cachedAddress = NbtAddress.GetCachedAddress(name);
			bool flag3 = cachedAddress == null;
			if (flag3)
			{
				Hashtable lookupTable2 = NbtAddress.LookupTable;
				lock (lookupTable2)
				{
					NbtAddress.LookupTable.Put(name, name);
				}
			}
			return cachedAddress;
		}

		// Token: 0x06000718 RID: 1816 RVA: 0x0002749C File Offset: 0x0002569C
		private static void UpdateLookupTable(Name name)
		{
			Hashtable lookupTable = NbtAddress.LookupTable;
			lock (lookupTable)
			{
				NbtAddress.LookupTable.Remove(name);
				Runtime.NotifyAll(NbtAddress.LookupTable);
			}
		}

		// Token: 0x06000719 RID: 1817 RVA: 0x000274F4 File Offset: 0x000256F4
		public static NbtAddress GetLocalHost()
		{
			return NbtAddress.Localhost;
		}

		// Token: 0x0600071A RID: 1818 RVA: 0x0002750C File Offset: 0x0002570C
		public static NbtAddress[] GetHosts()
		{
			return new NameServiceClient().GetHosts();
		}

		// Token: 0x0600071B RID: 1819 RVA: 0x00027528 File Offset: 0x00025728
		public static Name GetLocalName()
		{
			return NbtAddress.Localhost.HostName;
		}

		// Token: 0x0600071C RID: 1820 RVA: 0x00027544 File Offset: 0x00025744
		public static NbtAddress GetByName(string host)
		{
			return NbtAddress.GetByName(host, 0, null);
		}

		// Token: 0x0600071D RID: 1821 RVA: 0x00027560 File Offset: 0x00025760
		public static NbtAddress GetByName(string host, int type, string scope)
		{
			return NbtAddress.GetByName(host, type, scope, null);
		}

		// Token: 0x0600071E RID: 1822 RVA: 0x0002757C File Offset: 0x0002577C
		public static NbtAddress GetByName(string host, int type, string scope, IPAddress svr)
		{
			bool flag = string.IsNullOrEmpty(host);
			NbtAddress result;
			if (flag)
			{
				result = NbtAddress.GetLocalHost();
			}
			else
			{
				bool flag2 = !char.IsDigit(host[0]);
				if (flag2)
				{
					result = NbtAddress.DoNameQuery(new Name(host, type, scope), svr);
				}
				else
				{
					int num = 0;
					int num2 = 0;
					char[] array = host.ToCharArray();
					for (int i = 0; i < array.Length; i++)
					{
						char c = array[i];
						bool flag3 = c < '0' || c > '9';
						if (flag3)
						{
							return NbtAddress.DoNameQuery(new Name(host, type, scope), svr);
						}
						int num3 = 0;
						while (c != '.')
						{
							bool flag4 = c < '0' || c > '9';
							if (flag4)
							{
								return NbtAddress.DoNameQuery(new Name(host, type, scope), svr);
							}
							num3 = num3 * 10 + (int)c - 48;
							bool flag5 = ++i >= array.Length;
							if (flag5)
							{
								break;
							}
							c = array[i];
						}
						bool flag6 = num3 > 255;
						if (flag6)
						{
							return NbtAddress.DoNameQuery(new Name(host, type, scope), svr);
						}
						num = (num << 8) + num3;
						num2++;
					}
					bool flag7 = num2 != 4 || host.EndsWith(".");
					if (flag7)
					{
						result = NbtAddress.DoNameQuery(new Name(host, type, scope), svr);
					}
					else
					{
						result = new NbtAddress(NbtAddress.UnknownName, num, false, 0);
					}
				}
			}
			return result;
		}

		// Token: 0x0600071F RID: 1823 RVA: 0x00027700 File Offset: 0x00025900
		public static NbtAddress[] GetAllByName(string host, int type, string scope, IPAddress svr)
		{
			return NbtAddress.Client.GetAllByName(new Name(host, type, scope), svr);
		}

		// Token: 0x06000720 RID: 1824 RVA: 0x00027728 File Offset: 0x00025928
		public static NbtAddress[] GetAllByAddress(string host)
		{
			return NbtAddress.GetAllByAddress(NbtAddress.GetByName(host, 0, null));
		}

		// Token: 0x06000721 RID: 1825 RVA: 0x00027748 File Offset: 0x00025948
		public static NbtAddress[] GetAllByAddress(string host, int type, string scope)
		{
			return NbtAddress.GetAllByAddress(NbtAddress.GetByName(host, type, scope));
		}

		// Token: 0x06000722 RID: 1826 RVA: 0x00027768 File Offset: 0x00025968
		public static NbtAddress[] GetAllByAddress(NbtAddress addr)
		{
			NbtAddress[] result;
			try
			{
				NbtAddress[] nodeStatus = NbtAddress.Client.GetNodeStatus(addr);
				NbtAddress.CacheAddressArray(nodeStatus);
				result = nodeStatus;
			}
			catch (UnknownHostException)
			{
				throw new UnknownHostException(string.Concat(new string[]
				{
					"no name with type 0x",
					Hexdump.ToHexString(addr.HostName.HexCode, 2),
					(addr.HostName.Scope == null || addr.HostName.Scope.Length == 0) ? " with no scope" : (" with scope " + addr.HostName.Scope),
					" for host ",
					addr.GetHostAddress()
				}));
			}
			return result;
		}

		// Token: 0x06000723 RID: 1827 RVA: 0x00027820 File Offset: 0x00025A20
		public static IPAddress GetWinsAddress()
		{
			return (NbtAddress.Nbns.Length == 0) ? null : NbtAddress.Nbns[NbtAddress._nbnsIndex];
		}

		// Token: 0x06000724 RID: 1828 RVA: 0x00027848 File Offset: 0x00025A48
		public static bool IsWins(IPAddress svr)
		{
			int num = 0;
			while (svr != null && num < NbtAddress.Nbns.Length)
			{
				bool flag = svr.GetHashCode() == NbtAddress.Nbns[num].GetHashCode();
				if (flag)
				{
					return true;
				}
				num++;
			}
			return false;
		}

		// Token: 0x06000725 RID: 1829 RVA: 0x00027898 File Offset: 0x00025A98
		internal static IPAddress SwitchWins()
		{
			NbtAddress._nbnsIndex = ((NbtAddress._nbnsIndex + 1 < NbtAddress.Nbns.Length) ? (NbtAddress._nbnsIndex + 1) : 0);
			return (NbtAddress.Nbns.Length == 0) ? null : NbtAddress.Nbns[NbtAddress._nbnsIndex];
		}

		// Token: 0x06000726 RID: 1830 RVA: 0x000278DF File Offset: 0x00025ADF
		internal NbtAddress(Name hostName, int address, bool groupName, int nodeType)
		{
			this.HostName = hostName;
			this.Address = address;
			this.GroupName = groupName;
			this.NodeType = nodeType;
		}

		// Token: 0x06000727 RID: 1831 RVA: 0x00027908 File Offset: 0x00025B08
		internal NbtAddress(Name hostName, int address, bool groupName, int nodeType, bool isBeingDeleted, bool isInConflict, bool isActive, bool isPermanent, byte[] macAddress)
		{
			this.HostName = hostName;
			this.Address = address;
			this.GroupName = groupName;
			this.NodeType = nodeType;
			this.isBeingDeleted = isBeingDeleted;
			this.isInConflict = isInConflict;
			this.isActive = isActive;
			this.isPermanent = isPermanent;
			this.MacAddress = macAddress;
			this.IsDataFromNodeStatus = true;
		}

		// Token: 0x06000728 RID: 1832 RVA: 0x0002796C File Offset: 0x00025B6C
		public string FirstCalledName()
		{
			this.CalledName = this.HostName.name;
			bool flag = char.IsDigit(this.CalledName[0]);
			if (flag)
			{
				int num2;
				int num = num2 = 0;
				int length = this.CalledName.Length;
				char[] array = this.CalledName.ToCharArray();
				while (num2 < length && char.IsDigit(array[num2++]))
				{
					bool flag2 = num2 == length && num == 3;
					if (flag2)
					{
						this.CalledName = NbtAddress.SmbserverName;
						break;
					}
					bool flag3 = num2 < length && array[num2] == '.';
					if (flag3)
					{
						num++;
						num2++;
					}
				}
			}
			else
			{
				switch (this.HostName.HexCode)
				{
				case 27:
				case 28:
				case 29:
					this.CalledName = NbtAddress.SmbserverName;
					break;
				}
			}
			return this.CalledName;
		}

		// Token: 0x06000729 RID: 1833 RVA: 0x00027A60 File Offset: 0x00025C60
		public string NextCalledName()
		{
			bool flag = this.CalledName == this.HostName.name;
			if (flag)
			{
				this.CalledName = NbtAddress.SmbserverName;
			}
			else
			{
				bool flag2 = this.CalledName == NbtAddress.SmbserverName;
				if (flag2)
				{
					try
					{
						NbtAddress[] nodeStatus = NbtAddress.Client.GetNodeStatus(this);
						bool flag3 = this.HostName.HexCode == 29;
						if (flag3)
						{
							for (int i = 0; i < nodeStatus.Length; i++)
							{
								bool flag4 = nodeStatus[i].HostName.HexCode == 32;
								if (flag4)
								{
									return nodeStatus[i].HostName.name;
								}
							}
							return null;
						}
						bool isDataFromNodeStatus = this.IsDataFromNodeStatus;
						if (isDataFromNodeStatus)
						{
							this.CalledName = null;
							return this.HostName.name;
						}
					}
					catch (UnknownHostException)
					{
						this.CalledName = null;
					}
				}
				else
				{
					this.CalledName = null;
				}
			}
			return this.CalledName;
		}

		// Token: 0x0600072A RID: 1834 RVA: 0x00027B7C File Offset: 0x00025D7C
		internal void CheckData()
		{
			bool flag = this.HostName == NbtAddress.UnknownName;
			if (flag)
			{
				NbtAddress.GetAllByAddress(this);
			}
		}

		// Token: 0x0600072B RID: 1835 RVA: 0x00027BA4 File Offset: 0x00025DA4
		internal void CheckNodeStatusData()
		{
			bool flag = !this.IsDataFromNodeStatus;
			if (flag)
			{
				NbtAddress.GetAllByAddress(this);
			}
		}

		// Token: 0x0600072C RID: 1836 RVA: 0x00027BC8 File Offset: 0x00025DC8
		public bool IsGroupAddress()
		{
			this.CheckData();
			return this.GroupName;
		}

		// Token: 0x0600072D RID: 1837 RVA: 0x00027BE8 File Offset: 0x00025DE8
		public int GetNodeType()
		{
			this.CheckData();
			return this.NodeType;
		}

		// Token: 0x0600072E RID: 1838 RVA: 0x00027C08 File Offset: 0x00025E08
		public bool IsBeingDeleted()
		{
			this.CheckNodeStatusData();
			return this.isBeingDeleted;
		}

		// Token: 0x0600072F RID: 1839 RVA: 0x00027C28 File Offset: 0x00025E28
		public bool IsInConflict()
		{
			this.CheckNodeStatusData();
			return this.isInConflict;
		}

		// Token: 0x06000730 RID: 1840 RVA: 0x00027C48 File Offset: 0x00025E48
		public bool IsActive()
		{
			this.CheckNodeStatusData();
			return this.isActive;
		}

		// Token: 0x06000731 RID: 1841 RVA: 0x00027C68 File Offset: 0x00025E68
		public bool IsPermanent()
		{
			this.CheckNodeStatusData();
			return this.isPermanent;
		}

		// Token: 0x06000732 RID: 1842 RVA: 0x00027C88 File Offset: 0x00025E88
		public byte[] GetMacAddress()
		{
			this.CheckNodeStatusData();
			return this.MacAddress;
		}

		// Token: 0x06000733 RID: 1843 RVA: 0x00027CA8 File Offset: 0x00025EA8
		public string GetHostName()
		{
			bool flag = this.HostName == NbtAddress.UnknownName;
			string result;
			if (flag)
			{
				result = this.GetHostAddress();
			}
			else
			{
				result = this.HostName.name;
			}
			return result;
		}

		// Token: 0x06000734 RID: 1844 RVA: 0x00027CE0 File Offset: 0x00025EE0
		public byte[] GetAddress()
		{
			return new byte[]
			{
				(byte)((uint)this.Address >> 24 & 255U),
				(byte)((uint)this.Address >> 16 & 255U),
				(byte)((uint)this.Address >> 8 & 255U),
				(byte)(this.Address & 255)
			};
		}

		// Token: 0x06000735 RID: 1845 RVA: 0x00027D44 File Offset: 0x00025F44
		public IPAddress GetInetAddress()
		{
			return Extensions.GetAddressByName(this.GetHostAddress());
		}

		// Token: 0x06000736 RID: 1846 RVA: 0x00027D64 File Offset: 0x00025F64
		public string GetHostAddress()
		{
			return string.Concat(new object[]
			{
				(int)((uint)this.Address >> 24 & 255U),
				".",
				(int)((uint)this.Address >> 16 & 255U),
				".",
				(int)((uint)this.Address >> 8 & 255U),
				".",
				this.Address & 255
			});
		}

		// Token: 0x06000737 RID: 1847 RVA: 0x00027DF4 File Offset: 0x00025FF4
		public int GetNameType()
		{
			return this.HostName.HexCode;
		}

		// Token: 0x06000738 RID: 1848 RVA: 0x00027E14 File Offset: 0x00026014
		public override int GetHashCode()
		{
			return this.Address;
		}

		// Token: 0x06000739 RID: 1849 RVA: 0x00027E2C File Offset: 0x0002602C
		public override bool Equals(object obj)
		{
			return obj != null && obj is NbtAddress && ((NbtAddress)obj).Address == this.Address;
		}

		// Token: 0x0600073A RID: 1850 RVA: 0x00027E60 File Offset: 0x00026060
		public override string ToString()
		{
			return this.HostName + "/" + this.GetHostAddress();
		}

		// Token: 0x04000479 RID: 1145
		internal static readonly string AnyHostsName = "*\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0";

		// Token: 0x0400047A RID: 1146
		public static readonly string MasterBrowserName = "\u0001\u0002__MSBROWSE__\u0002";

		// Token: 0x0400047B RID: 1147
		public static readonly string SmbserverName = "*SMBSERVER     ";

		// Token: 0x0400047C RID: 1148
		public const int BNode = 0;

		// Token: 0x0400047D RID: 1149
		public const int PNode = 1;

		// Token: 0x0400047E RID: 1150
		public const int MNode = 2;

		// Token: 0x0400047F RID: 1151
		public const int HNode = 3;

		// Token: 0x04000480 RID: 1152
		internal static readonly IPAddress[] Nbns = Config.GetInetAddressArray("jcifs.netbios.wins", ",", new IPAddress[0]);

		// Token: 0x04000481 RID: 1153
		private static readonly NameServiceClient Client = new NameServiceClient();

		// Token: 0x04000482 RID: 1154
		private const int DefaultCachePolicy = 30;

		// Token: 0x04000483 RID: 1155
		private static readonly int CachePolicy = Config.GetInt("jcifs.netbios.cachePolicy", 30);

		// Token: 0x04000484 RID: 1156
		private const int Forever = -1;

		// Token: 0x04000485 RID: 1157
		private static int _nbnsIndex;

		// Token: 0x04000486 RID: 1158
		private static readonly Hashtable AddressCache = new Hashtable();

		// Token: 0x04000487 RID: 1159
		private static readonly Hashtable LookupTable = new Hashtable();

		// Token: 0x04000488 RID: 1160
		internal static readonly Name UnknownName = new Name("0.0.0.0", 0, null);

		// Token: 0x04000489 RID: 1161
		internal static readonly NbtAddress UnknownAddress = new NbtAddress(NbtAddress.UnknownName, 0, false, 0);

		// Token: 0x0400048A RID: 1162
		internal static readonly byte[] UnknownMacAddress = new byte[6];

		// Token: 0x0400048B RID: 1163
		internal static NbtAddress Localhost;

		// Token: 0x0400048C RID: 1164
		internal Name HostName;

		// Token: 0x0400048D RID: 1165
		internal int Address;

		// Token: 0x0400048E RID: 1166
		internal int NodeType;

		// Token: 0x0400048F RID: 1167
		internal bool GroupName;

		// Token: 0x04000490 RID: 1168
		internal bool isBeingDeleted;

		// Token: 0x04000491 RID: 1169
		internal bool isInConflict;

		// Token: 0x04000492 RID: 1170
		internal bool isActive;

		// Token: 0x04000493 RID: 1171
		internal bool isPermanent;

		// Token: 0x04000494 RID: 1172
		internal bool IsDataFromNodeStatus;

		// Token: 0x04000495 RID: 1173
		internal byte[] MacAddress;

		// Token: 0x04000496 RID: 1174
		internal string CalledName;

		// Token: 0x02000129 RID: 297
		internal sealed class CacheEntry
		{
			// Token: 0x06000850 RID: 2128 RVA: 0x0002BDCB File Offset: 0x00029FCB
			internal CacheEntry(Name hostName, NbtAddress address, long expiration)
			{
				this.HostName = hostName;
				this.Address = address;
				this.Expiration = expiration;
			}

			// Token: 0x040005BE RID: 1470
			internal Name HostName;

			// Token: 0x040005BF RID: 1471
			internal NbtAddress Address;

			// Token: 0x040005C0 RID: 1472
			internal long Expiration;
		}
	}
}
