using System;
using System.Text;

namespace LumiSoft.Net.RTP
{
	// Token: 0x020000D8 RID: 216
	public class RTCP_Packet_SDES_Chunk
	{
		// Token: 0x0600085E RID: 2142 RVA: 0x000315EC File Offset: 0x000305EC
		public RTCP_Packet_SDES_Chunk(uint source, string cname)
		{
			bool flag = source == 0U;
			if (flag)
			{
				throw new ArgumentException("Argument 'source' value must be > 0.");
			}
			bool flag2 = string.IsNullOrEmpty(cname);
			if (flag2)
			{
				throw new ArgumentException("Argument 'cname' value may not be null or empty.");
			}
			this.m_Source = source;
			this.m_CName = cname;
		}

		// Token: 0x0600085F RID: 2143 RVA: 0x00031674 File Offset: 0x00030674
		internal RTCP_Packet_SDES_Chunk()
		{
		}

		// Token: 0x06000860 RID: 2144 RVA: 0x000316C4 File Offset: 0x000306C4
		public void Parse(byte[] buffer, ref int offset)
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
			int num2 = offset;
			offset = num2 + 1;
			uint num3 = (uint)((uint)buffer[num2] << 24);
			num2 = offset;
			offset = num2 + 1;
			uint num4 = num3 | (uint)((uint)buffer[num2] << 16);
			num2 = offset;
			offset = num2 + 1;
			uint num5 = num4 | (uint)((uint)buffer[num2] << 8);
			num2 = offset;
			offset = num2 + 1;
			this.m_Source = (num5 | (uint)buffer[num2]);
			while (offset < buffer.Length && buffer[offset] > 0)
			{
				num2 = offset;
				offset = num2 + 1;
				int num6 = (int)buffer[num2];
				num2 = offset;
				offset = num2 + 1;
				int num7 = (int)buffer[num2];
				bool flag3 = num6 == 1;
				if (flag3)
				{
					this.m_CName = Encoding.UTF8.GetString(buffer, offset, num7);
				}
				else
				{
					bool flag4 = num6 == 2;
					if (flag4)
					{
						this.m_Name = Encoding.UTF8.GetString(buffer, offset, num7);
					}
					else
					{
						bool flag5 = num6 == 3;
						if (flag5)
						{
							this.m_Email = Encoding.UTF8.GetString(buffer, offset, num7);
						}
						else
						{
							bool flag6 = num6 == 4;
							if (flag6)
							{
								this.m_Phone = Encoding.UTF8.GetString(buffer, offset, num7);
							}
							else
							{
								bool flag7 = num6 == 5;
								if (flag7)
								{
									this.m_Location = Encoding.UTF8.GetString(buffer, offset, num7);
								}
								else
								{
									bool flag8 = num6 == 6;
									if (flag8)
									{
										this.m_Tool = Encoding.UTF8.GetString(buffer, offset, num7);
									}
									else
									{
										bool flag9 = num6 == 7;
										if (flag9)
										{
											this.m_Note = Encoding.UTF8.GetString(buffer, offset, num7);
										}
										else
										{
											bool flag10 = num6 == 8;
											if (flag10)
											{
											}
										}
									}
								}
							}
						}
					}
				}
				offset += num7;
			}
			offset++;
			offset += (offset - num) % 4;
		}

		// Token: 0x06000861 RID: 2145 RVA: 0x000318A0 File Offset: 0x000308A0
		public void ToByte(byte[] buffer, ref int offset)
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
			int num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_Source >> 24 & 255U);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_Source >> 16 & 255U);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_Source >> 8 & 255U);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_Source & 255U);
			bool flag3 = !string.IsNullOrEmpty(this.m_CName);
			if (flag3)
			{
				byte[] bytes = Encoding.UTF8.GetBytes(this.m_CName);
				num2 = offset;
				offset = num2 + 1;
				buffer[num2] = 1;
				num2 = offset;
				offset = num2 + 1;
				buffer[num2] = (byte)bytes.Length;
				Array.Copy(bytes, 0, buffer, offset, bytes.Length);
				offset += bytes.Length;
			}
			bool flag4 = !string.IsNullOrEmpty(this.m_Name);
			if (flag4)
			{
				byte[] bytes2 = Encoding.UTF8.GetBytes(this.m_Name);
				num2 = offset;
				offset = num2 + 1;
				buffer[num2] = 2;
				num2 = offset;
				offset = num2 + 1;
				buffer[num2] = (byte)bytes2.Length;
				Array.Copy(bytes2, 0, buffer, offset, bytes2.Length);
				offset += bytes2.Length;
			}
			bool flag5 = !string.IsNullOrEmpty(this.m_Email);
			if (flag5)
			{
				byte[] bytes3 = Encoding.UTF8.GetBytes(this.m_Email);
				num2 = offset;
				offset = num2 + 1;
				buffer[num2] = 3;
				num2 = offset;
				offset = num2 + 1;
				buffer[num2] = (byte)bytes3.Length;
				Array.Copy(bytes3, 0, buffer, offset, bytes3.Length);
				offset += bytes3.Length;
			}
			bool flag6 = !string.IsNullOrEmpty(this.m_Phone);
			if (flag6)
			{
				byte[] bytes4 = Encoding.UTF8.GetBytes(this.m_Phone);
				num2 = offset;
				offset = num2 + 1;
				buffer[num2] = 4;
				num2 = offset;
				offset = num2 + 1;
				buffer[num2] = (byte)bytes4.Length;
				Array.Copy(bytes4, 0, buffer, offset, bytes4.Length);
				offset += bytes4.Length;
			}
			bool flag7 = !string.IsNullOrEmpty(this.m_Location);
			if (flag7)
			{
				byte[] bytes5 = Encoding.UTF8.GetBytes(this.m_Location);
				num2 = offset;
				offset = num2 + 1;
				buffer[num2] = 5;
				num2 = offset;
				offset = num2 + 1;
				buffer[num2] = (byte)bytes5.Length;
				Array.Copy(bytes5, 0, buffer, offset, bytes5.Length);
				offset += bytes5.Length;
			}
			bool flag8 = !string.IsNullOrEmpty(this.m_Tool);
			if (flag8)
			{
				byte[] bytes6 = Encoding.UTF8.GetBytes(this.m_Tool);
				num2 = offset;
				offset = num2 + 1;
				buffer[num2] = 6;
				num2 = offset;
				offset = num2 + 1;
				buffer[num2] = (byte)bytes6.Length;
				Array.Copy(bytes6, 0, buffer, offset, bytes6.Length);
				offset += bytes6.Length;
			}
			bool flag9 = !string.IsNullOrEmpty(this.m_Note);
			if (flag9)
			{
				byte[] bytes7 = Encoding.UTF8.GetBytes(this.m_Note);
				num2 = offset;
				offset = num2 + 1;
				buffer[num2] = 7;
				num2 = offset;
				offset = num2 + 1;
				buffer[num2] = (byte)bytes7.Length;
				Array.Copy(bytes7, 0, buffer, offset, bytes7.Length);
				offset += bytes7.Length;
			}
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = 0;
			while ((offset - num) % 4 > 0)
			{
				num2 = offset;
				offset = num2 + 1;
				buffer[num2] = 0;
			}
		}

		// Token: 0x170002CE RID: 718
		// (get) Token: 0x06000862 RID: 2146 RVA: 0x00031BF8 File Offset: 0x00030BF8
		public uint Source
		{
			get
			{
				return this.m_Source;
			}
		}

		// Token: 0x170002CF RID: 719
		// (get) Token: 0x06000863 RID: 2147 RVA: 0x00031C10 File Offset: 0x00030C10
		public string CName
		{
			get
			{
				return this.m_CName;
			}
		}

		// Token: 0x170002D0 RID: 720
		// (get) Token: 0x06000864 RID: 2148 RVA: 0x00031C28 File Offset: 0x00030C28
		// (set) Token: 0x06000865 RID: 2149 RVA: 0x00031C40 File Offset: 0x00030C40
		public string Name
		{
			get
			{
				return this.m_Name;
			}
			set
			{
				bool flag = Encoding.UTF8.GetByteCount(value) > 255;
				if (flag)
				{
					throw new ArgumentException("Property 'Name' value must be <= 255 bytes.");
				}
				this.m_Name = value;
			}
		}

		// Token: 0x170002D1 RID: 721
		// (get) Token: 0x06000866 RID: 2150 RVA: 0x00031C78 File Offset: 0x00030C78
		// (set) Token: 0x06000867 RID: 2151 RVA: 0x00031C90 File Offset: 0x00030C90
		public string Email
		{
			get
			{
				return this.m_Email;
			}
			set
			{
				bool flag = Encoding.UTF8.GetByteCount(value) > 255;
				if (flag)
				{
					throw new ArgumentException("Property 'Email' value must be <= 255 bytes.");
				}
				this.m_Email = value;
			}
		}

		// Token: 0x170002D2 RID: 722
		// (get) Token: 0x06000868 RID: 2152 RVA: 0x00031CC8 File Offset: 0x00030CC8
		// (set) Token: 0x06000869 RID: 2153 RVA: 0x00031CE0 File Offset: 0x00030CE0
		public string Phone
		{
			get
			{
				return this.m_Phone;
			}
			set
			{
				bool flag = Encoding.UTF8.GetByteCount(value) > 255;
				if (flag)
				{
					throw new ArgumentException("Property 'Phone' value must be <= 255 bytes.");
				}
				this.m_Phone = value;
			}
		}

		// Token: 0x170002D3 RID: 723
		// (get) Token: 0x0600086A RID: 2154 RVA: 0x00031D18 File Offset: 0x00030D18
		// (set) Token: 0x0600086B RID: 2155 RVA: 0x00031D30 File Offset: 0x00030D30
		public string Location
		{
			get
			{
				return this.m_Location;
			}
			set
			{
				bool flag = Encoding.UTF8.GetByteCount(value) > 255;
				if (flag)
				{
					throw new ArgumentException("Property 'Location' value must be <= 255 bytes.");
				}
				this.m_Location = value;
			}
		}

		// Token: 0x170002D4 RID: 724
		// (get) Token: 0x0600086C RID: 2156 RVA: 0x00031D68 File Offset: 0x00030D68
		// (set) Token: 0x0600086D RID: 2157 RVA: 0x00031D80 File Offset: 0x00030D80
		public string Tool
		{
			get
			{
				return this.m_Tool;
			}
			set
			{
				bool flag = Encoding.UTF8.GetByteCount(value) > 255;
				if (flag)
				{
					throw new ArgumentException("Property 'Tool' value must be <= 255 bytes.");
				}
				this.m_Tool = value;
			}
		}

		// Token: 0x170002D5 RID: 725
		// (get) Token: 0x0600086E RID: 2158 RVA: 0x00031DB8 File Offset: 0x00030DB8
		// (set) Token: 0x0600086F RID: 2159 RVA: 0x00031DD0 File Offset: 0x00030DD0
		public string Note
		{
			get
			{
				return this.m_Note;
			}
			set
			{
				bool flag = Encoding.UTF8.GetByteCount(value) > 255;
				if (flag)
				{
					throw new ArgumentException("Property 'Note' value must be <= 255 bytes.");
				}
				this.m_Note = value;
			}
		}

		// Token: 0x170002D6 RID: 726
		// (get) Token: 0x06000870 RID: 2160 RVA: 0x00031E08 File Offset: 0x00030E08
		public int Size
		{
			get
			{
				int num = 4;
				bool flag = !string.IsNullOrEmpty(this.m_CName);
				if (flag)
				{
					num += 2;
					num += Encoding.UTF8.GetByteCount(this.m_CName);
				}
				bool flag2 = !string.IsNullOrEmpty(this.m_Name);
				if (flag2)
				{
					num += 2;
					num += Encoding.UTF8.GetByteCount(this.m_Name);
				}
				bool flag3 = !string.IsNullOrEmpty(this.m_Email);
				if (flag3)
				{
					num += 2;
					num += Encoding.UTF8.GetByteCount(this.m_Email);
				}
				bool flag4 = !string.IsNullOrEmpty(this.m_Phone);
				if (flag4)
				{
					num += 2;
					num += Encoding.UTF8.GetByteCount(this.m_Phone);
				}
				bool flag5 = !string.IsNullOrEmpty(this.m_Location);
				if (flag5)
				{
					num += 2;
					num += Encoding.UTF8.GetByteCount(this.m_Location);
				}
				bool flag6 = !string.IsNullOrEmpty(this.m_Tool);
				if (flag6)
				{
					num += 2;
					num += Encoding.UTF8.GetByteCount(this.m_Tool);
				}
				bool flag7 = !string.IsNullOrEmpty(this.m_Note);
				if (flag7)
				{
					num += 2;
					num += Encoding.UTF8.GetByteCount(this.m_Note);
				}
				num++;
				while (num % 4 > 0)
				{
					num++;
				}
				return num;
			}
		}

		// Token: 0x040003AC RID: 940
		private uint m_Source = 0U;

		// Token: 0x040003AD RID: 941
		private string m_CName = null;

		// Token: 0x040003AE RID: 942
		private string m_Name = null;

		// Token: 0x040003AF RID: 943
		private string m_Email = null;

		// Token: 0x040003B0 RID: 944
		private string m_Phone = null;

		// Token: 0x040003B1 RID: 945
		private string m_Location = null;

		// Token: 0x040003B2 RID: 946
		private string m_Tool = null;

		// Token: 0x040003B3 RID: 947
		private string m_Note = null;
	}
}
