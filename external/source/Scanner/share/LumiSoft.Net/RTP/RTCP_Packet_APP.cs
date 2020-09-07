using System;

namespace LumiSoft.Net.RTP
{
	// Token: 0x020000D4 RID: 212
	public class RTCP_Packet_APP : RTCP_Packet
	{
		// Token: 0x06000833 RID: 2099 RVA: 0x000307A4 File Offset: 0x0002F7A4
		internal RTCP_Packet_APP()
		{
			this.m_Name = "xxxx";
			this.m_Data = new byte[0];
		}

		// Token: 0x06000834 RID: 2100 RVA: 0x000307F8 File Offset: 0x0002F7F8
		protected override void ParseInternal(byte[] buffer, ref int offset)
		{
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			bool flag2 = offset < 0;
			if (flag2)
			{
				throw new ArgumentException("Argument 'offset' value must be >= 0.");
			}
			int num = offset;
			offset = num + 1;
			this.m_Version = buffer[num] >> 6;
			bool flag3 = Convert.ToBoolean(buffer[offset] >> 5 & 1);
			num = offset;
			offset = num + 1;
			int subType = (int)(buffer[num] & 31);
			num = offset;
			offset = num + 1;
			int num2 = (int)buffer[num];
			num = offset;
			offset = num + 1;
			int num3 = (int)buffer[num] << 8;
			num = offset;
			offset = num + 1;
			int num4 = num3 | (int)buffer[num];
			bool flag4 = flag3;
			if (flag4)
			{
				base.PaddBytesCount = (int)buffer[offset + num4];
			}
			this.m_SubType = subType;
			num = offset;
			offset = num + 1;
			uint num5 = (uint)((uint)buffer[num] << 24);
			num = offset;
			offset = num + 1;
			uint num6 = num5 | (uint)((uint)buffer[num] << 16);
			num = offset;
			offset = num + 1;
			uint num7 = num6 | (uint)((uint)buffer[num] << 8);
			num = offset;
			offset = num + 1;
			this.m_Source = (num7 | (uint)buffer[num]);
			num = offset;
			offset = num + 1;
			char c = (char)buffer[num];
			string str = c.ToString();
			num = offset;
			offset = num + 1;
			c = (char)buffer[num];
			string str2 = c.ToString();
			num = offset;
			offset = num + 1;
			c = (char)buffer[num];
			string str3 = c.ToString();
			num = offset;
			offset = num + 1;
			c = (char)buffer[num];
			this.m_Name = str + str2 + str3 + c.ToString();
			this.m_Data = new byte[num4 - 8];
			Array.Copy(buffer, offset, this.m_Data, 0, this.m_Data.Length);
		}

		// Token: 0x06000835 RID: 2101 RVA: 0x0003098C File Offset: 0x0002F98C
		public override void ToByte(byte[] buffer, ref int offset)
		{
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			bool flag2 = offset < 0;
			if (flag2)
			{
				throw new ArgumentException("Argument 'offset' value must be >= 0.");
			}
			int num = 8 + this.m_Data.Length;
			int num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(128 | (this.m_SubType & 31));
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = 204;
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(num >> 8 | 255);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(num | 255);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_Source >> 24 | 255U);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_Source >> 16 | 255U);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_Source >> 8 | 255U);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_Source | 255U);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)this.m_Name[0];
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)this.m_Name[1];
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)this.m_Name[2];
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)this.m_Name[2];
			Array.Copy(this.m_Data, 0, buffer, offset, this.m_Data.Length);
			offset += this.m_Data.Length;
		}

		// Token: 0x170002B8 RID: 696
		// (get) Token: 0x06000836 RID: 2102 RVA: 0x00030B10 File Offset: 0x0002FB10
		public override int Version
		{
			get
			{
				return this.m_Version;
			}
		}

		// Token: 0x170002B9 RID: 697
		// (get) Token: 0x06000837 RID: 2103 RVA: 0x00030B28 File Offset: 0x0002FB28
		public override int Type
		{
			get
			{
				return 204;
			}
		}

		// Token: 0x170002BA RID: 698
		// (get) Token: 0x06000838 RID: 2104 RVA: 0x00030B40 File Offset: 0x0002FB40
		public int SubType
		{
			get
			{
				return this.m_SubType;
			}
		}

		// Token: 0x170002BB RID: 699
		// (get) Token: 0x06000839 RID: 2105 RVA: 0x00030B58 File Offset: 0x0002FB58
		// (set) Token: 0x0600083A RID: 2106 RVA: 0x00030B70 File Offset: 0x0002FB70
		public uint Source
		{
			get
			{
				return this.m_Source;
			}
			set
			{
				this.m_Source = value;
			}
		}

		// Token: 0x170002BC RID: 700
		// (get) Token: 0x0600083B RID: 2107 RVA: 0x00030B7C File Offset: 0x0002FB7C
		public string Name
		{
			get
			{
				return this.m_Name;
			}
		}

		// Token: 0x170002BD RID: 701
		// (get) Token: 0x0600083C RID: 2108 RVA: 0x00030B94 File Offset: 0x0002FB94
		public byte[] Data
		{
			get
			{
				return this.m_Data;
			}
		}

		// Token: 0x170002BE RID: 702
		// (get) Token: 0x0600083D RID: 2109 RVA: 0x00030BAC File Offset: 0x0002FBAC
		public override int Size
		{
			get
			{
				return 12 + this.m_Data.Length;
			}
		}

		// Token: 0x040003A0 RID: 928
		private int m_Version = 2;

		// Token: 0x040003A1 RID: 929
		private int m_SubType = 0;

		// Token: 0x040003A2 RID: 930
		private uint m_Source = 0U;

		// Token: 0x040003A3 RID: 931
		private string m_Name = "";

		// Token: 0x040003A4 RID: 932
		private byte[] m_Data = null;
	}
}
