using System;
using System.Text;

namespace LumiSoft.Net.RTP
{
	// Token: 0x020000D5 RID: 213
	public class RTCP_Packet_BYE : RTCP_Packet
	{
		// Token: 0x0600083E RID: 2110 RVA: 0x00030BC9 File Offset: 0x0002FBC9
		internal RTCP_Packet_BYE()
		{
		}

		// Token: 0x0600083F RID: 2111 RVA: 0x00030BEC File Offset: 0x0002FBEC
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
			this.m_Version = buffer[offset] >> 6;
			bool flag3 = Convert.ToBoolean(buffer[offset] >> 5 & 1);
			int num = offset;
			offset = num + 1;
			int num2 = (int)(buffer[num] & 31);
			num = offset;
			offset = num + 1;
			int num3 = (int)buffer[num];
			num = offset;
			offset = num + 1;
			int num4 = (int)buffer[num] << 8;
			num = offset;
			offset = num + 1;
			int num5 = num4 | (int)buffer[num];
			int num6 = num5 * 4;
			bool flag4 = flag3;
			if (flag4)
			{
				base.PaddBytesCount = (int)buffer[offset + num5];
			}
			this.m_Sources = new uint[num2];
			for (int i = 0; i < num2; i++)
			{
				uint[] sources = this.m_Sources;
				int num7 = i;
				num = offset;
				offset = num + 1;
				byte b = (byte)(buffer[num] << 24);
				num = offset;
				offset = num + 1;
				byte b2 = (byte)((int)b | (int)buffer[num] << 16);
				num = offset;
				offset = num + 1;
				byte b3 = (byte)((int)b2 | (int)buffer[num] << 8);
				num = offset;
				offset = num + 1;
				sources[num7] = (b3 | buffer[num]);
			}
			bool flag5 = num6 > offset;
			if (flag5)
			{
				num = offset;
				offset = num + 1;
				int num8 = (int)buffer[num];
				this.m_LeavingReason = Encoding.UTF8.GetString(buffer, offset, num8);
				offset += num8;
			}
		}

		// Token: 0x06000840 RID: 2112 RVA: 0x00030D48 File Offset: 0x0002FD48
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
			byte[] array = new byte[0];
			bool flag3 = !string.IsNullOrEmpty(this.m_LeavingReason);
			if (flag3)
			{
				array = Encoding.UTF8.GetBytes(this.m_LeavingReason);
			}
			int num = 0;
			num += this.m_Sources.Length;
			bool flag4 = !string.IsNullOrEmpty(this.m_LeavingReason);
			if (flag4)
			{
				num += (int)Math.Ceiling((4 + array.Length) / 4);
			}
			int num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(128 | (this.m_Sources.Length & 31));
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = 203;
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(num >> 8 & 255);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(num & 255);
			foreach (int num3 in this.m_Sources)
			{
				num2 = offset;
				offset = num2 + 1;
				buffer[num2] = (byte)(((long)num3 & (long)((ulong)-16777216)) >> 24);
				num2 = offset;
				offset = num2 + 1;
				buffer[num2] = (byte)((num3 & 16711680) >> 16);
				num2 = offset;
				offset = num2 + 1;
				buffer[num2] = (byte)((num3 & 65280) >> 8);
				num2 = offset;
				offset = num2 + 1;
				buffer[num2] = (byte)(num3 & 255);
			}
			bool flag5 = !string.IsNullOrEmpty(this.m_LeavingReason);
			if (flag5)
			{
				byte[] bytes = Encoding.UTF8.GetBytes(this.m_LeavingReason);
				num2 = offset;
				offset = num2 + 1;
				buffer[num2] = (byte)bytes.Length;
				Array.Copy(bytes, 0, buffer, offset, bytes.Length);
				offset += bytes.Length;
			}
		}

		// Token: 0x170002BF RID: 703
		// (get) Token: 0x06000841 RID: 2113 RVA: 0x00030F2C File Offset: 0x0002FF2C
		public override int Version
		{
			get
			{
				return this.m_Version;
			}
		}

		// Token: 0x170002C0 RID: 704
		// (get) Token: 0x06000842 RID: 2114 RVA: 0x00030F44 File Offset: 0x0002FF44
		public override int Type
		{
			get
			{
				return 203;
			}
		}

		// Token: 0x170002C1 RID: 705
		// (get) Token: 0x06000843 RID: 2115 RVA: 0x00030F5C File Offset: 0x0002FF5C
		// (set) Token: 0x06000844 RID: 2116 RVA: 0x00030F74 File Offset: 0x0002FF74
		public uint[] Sources
		{
			get
			{
				return this.m_Sources;
			}
			set
			{
				bool flag = value.Length > 31;
				if (flag)
				{
					throw new ArgumentException("Property 'Sources' can accomodate only 31 entries.");
				}
				this.m_Sources = value;
			}
		}

		// Token: 0x170002C2 RID: 706
		// (get) Token: 0x06000845 RID: 2117 RVA: 0x00030FA0 File Offset: 0x0002FFA0
		// (set) Token: 0x06000846 RID: 2118 RVA: 0x00030FB8 File Offset: 0x0002FFB8
		public string LeavingReason
		{
			get
			{
				return this.m_LeavingReason;
			}
			set
			{
				this.m_LeavingReason = value;
			}
		}

		// Token: 0x170002C3 RID: 707
		// (get) Token: 0x06000847 RID: 2119 RVA: 0x00030FC4 File Offset: 0x0002FFC4
		public override int Size
		{
			get
			{
				int num = 4;
				bool flag = this.m_Sources != null;
				if (flag)
				{
					num += 4 * this.m_Sources.Length;
				}
				bool flag2 = !string.IsNullOrEmpty(this.m_LeavingReason);
				if (flag2)
				{
					num++;
					num += Encoding.UTF8.GetByteCount(this.m_LeavingReason);
				}
				return num;
			}
		}

		// Token: 0x040003A5 RID: 933
		private int m_Version = 2;

		// Token: 0x040003A6 RID: 934
		private uint[] m_Sources = null;

		// Token: 0x040003A7 RID: 935
		private string m_LeavingReason = "";
	}
}
