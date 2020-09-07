using System;

namespace LumiSoft.Net.SDP
{
	// Token: 0x020000BC RID: 188
	public class SDP_Connection
	{
		// Token: 0x0600072F RID: 1839 RVA: 0x0002C0EC File Offset: 0x0002B0EC
		public SDP_Connection(string netType, string addressType, string address)
		{
			bool flag = netType == null;
			if (flag)
			{
				throw new ArgumentNullException("netType");
			}
			bool flag2 = netType == string.Empty;
			if (flag2)
			{
				throw new ArgumentException("Argument 'netType' value must be specified.");
			}
			bool flag3 = addressType == null;
			if (flag3)
			{
				throw new ArgumentNullException("addressType");
			}
			bool flag4 = addressType == string.Empty;
			if (flag4)
			{
				throw new ArgumentException("Argument 'addressType' value must be specified.");
			}
			bool flag5 = address == null;
			if (flag5)
			{
				throw new ArgumentNullException("address");
			}
			bool flag6 = address == string.Empty;
			if (flag6)
			{
				throw new ArgumentException("Argument 'address' value must be specified.");
			}
			this.m_NetType = netType;
			this.m_AddressType = addressType;
			this.m_Address = address;
		}

		// Token: 0x06000730 RID: 1840 RVA: 0x0002C1C8 File Offset: 0x0002B1C8
		public static SDP_Connection Parse(string cValue)
		{
			StringReader stringReader = new StringReader(cValue);
			stringReader.QuotedReadToDelimiter('=');
			string text = stringReader.ReadWord();
			bool flag = text == null;
			if (flag)
			{
				throw new Exception("SDP message \"c\" field <nettype> value is missing !");
			}
			string netType = text;
			text = stringReader.ReadWord();
			bool flag2 = text == null;
			if (flag2)
			{
				throw new Exception("SDP message \"c\" field <addrtype> value is missing !");
			}
			string addressType = text;
			text = stringReader.ReadWord();
			bool flag3 = text == null;
			if (flag3)
			{
				throw new Exception("SDP message \"c\" field <connection-address> value is missing !");
			}
			string address = text;
			return new SDP_Connection(netType, addressType, address);
		}

		// Token: 0x06000731 RID: 1841 RVA: 0x0002C26C File Offset: 0x0002B26C
		public string ToValue()
		{
			return string.Concat(new string[]
			{
				"c=",
				this.NetType,
				" ",
				this.AddressType,
				" ",
				this.Address,
				"\r\n"
			});
		}

		// Token: 0x1700024B RID: 587
		// (get) Token: 0x06000732 RID: 1842 RVA: 0x0002C2C4 File Offset: 0x0002B2C4
		public string NetType
		{
			get
			{
				return this.m_NetType;
			}
		}

		// Token: 0x1700024C RID: 588
		// (get) Token: 0x06000733 RID: 1843 RVA: 0x0002C2DC File Offset: 0x0002B2DC
		// (set) Token: 0x06000734 RID: 1844 RVA: 0x0002C2F4 File Offset: 0x0002B2F4
		public string AddressType
		{
			get
			{
				return this.m_AddressType;
			}
			set
			{
				bool flag = string.IsNullOrEmpty(value);
				if (flag)
				{
					throw new ArgumentException("Property AddressType can't be null or empty !");
				}
				this.m_AddressType = value;
			}
		}

		// Token: 0x1700024D RID: 589
		// (get) Token: 0x06000735 RID: 1845 RVA: 0x0002C320 File Offset: 0x0002B320
		// (set) Token: 0x06000736 RID: 1846 RVA: 0x0002C338 File Offset: 0x0002B338
		public string Address
		{
			get
			{
				return this.m_Address;
			}
			set
			{
				bool flag = string.IsNullOrEmpty(value);
				if (flag)
				{
					throw new ArgumentException("Property Address can't be null or empty !");
				}
				this.m_Address = value;
			}
		}

		// Token: 0x04000312 RID: 786
		private string m_NetType = "IN";

		// Token: 0x04000313 RID: 787
		private string m_AddressType = "";

		// Token: 0x04000314 RID: 788
		private string m_Address = "";
	}
}
