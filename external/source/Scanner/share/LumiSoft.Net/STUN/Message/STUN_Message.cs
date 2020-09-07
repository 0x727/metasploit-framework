using System;
using System.Net;
using System.Text;

namespace LumiSoft.Net.STUN.Message
{
	// Token: 0x02000024 RID: 36
	public class STUN_Message
	{
		// Token: 0x06000112 RID: 274 RVA: 0x00008094 File Offset: 0x00007094
		public STUN_Message()
		{
			this.m_pTransactionID = new byte[12];
			new Random().NextBytes(this.m_pTransactionID);
		}

		// Token: 0x06000113 RID: 275 RVA: 0x00008124 File Offset: 0x00007124
		public void Parse(byte[] data)
		{
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			bool flag2 = data.Length < 20;
			if (flag2)
			{
				throw new ArgumentException("Invalid STUN message value !");
			}
			int num = 0;
			int num2 = (int)data[num++] << 8 | (int)data[num++];
			bool flag3 = num2 == 273;
			if (flag3)
			{
				this.m_Type = STUN_MessageType.BindingErrorResponse;
			}
			else
			{
				bool flag4 = num2 == 1;
				if (flag4)
				{
					this.m_Type = STUN_MessageType.BindingRequest;
				}
				else
				{
					bool flag5 = num2 == 257;
					if (flag5)
					{
						this.m_Type = STUN_MessageType.BindingResponse;
					}
					else
					{
						bool flag6 = num2 == 274;
						if (flag6)
						{
							this.m_Type = STUN_MessageType.SharedSecretErrorResponse;
						}
						else
						{
							bool flag7 = num2 == 2;
							if (flag7)
							{
								this.m_Type = STUN_MessageType.SharedSecretRequest;
							}
							else
							{
								bool flag8 = num2 == 258;
								if (!flag8)
								{
									throw new ArgumentException("Invalid STUN message type value !");
								}
								this.m_Type = STUN_MessageType.SharedSecretResponse;
							}
						}
					}
				}
			}
			int num3 = (int)data[num++] << 8 | (int)data[num++];
			this.m_MagicCookie = ((int)data[num++] << 24 | (int)data[num++] << 16 | (int)data[num++] << 8 | (int)data[num++]);
			this.m_pTransactionID = new byte[12];
			Array.Copy(data, num, this.m_pTransactionID, 0, 12);
			num += 12;
			while (num - 20 < num3)
			{
				this.ParseAttribute(data, ref num);
			}
		}

		// Token: 0x06000114 RID: 276 RVA: 0x000082A0 File Offset: 0x000072A0
		public byte[] ToByteData()
		{
			byte[] array = new byte[512];
			int num = 0;
			array[num++] = (byte)((int)this.Type >> 8 & 63);
			array[num++] = (byte)(this.Type & (STUN_MessageType)255);
			array[num++] = 0;
			array[num++] = 0;
			array[num++] = (byte)(this.MagicCookie >> 24 & 255);
			array[num++] = (byte)(this.MagicCookie >> 16 & 255);
			array[num++] = (byte)(this.MagicCookie >> 8 & 255);
			array[num++] = (byte)(this.MagicCookie & 255);
			Array.Copy(this.m_pTransactionID, 0, array, num, 12);
			num += 12;
			bool flag = this.MappedAddress != null;
			if (flag)
			{
				this.StoreEndPoint(STUN_Message.AttributeType.MappedAddress, this.MappedAddress, array, ref num);
			}
			else
			{
				bool flag2 = this.ResponseAddress != null;
				if (flag2)
				{
					this.StoreEndPoint(STUN_Message.AttributeType.ResponseAddress, this.ResponseAddress, array, ref num);
				}
				else
				{
					bool flag3 = this.ChangeRequest != null;
					if (flag3)
					{
						array[num++] = 0;
						array[num++] = 3;
						array[num++] = 0;
						array[num++] = 4;
						array[num++] = 0;
						array[num++] = 0;
						array[num++] = 0;
						array[num++] = (byte)(Convert.ToInt32(this.ChangeRequest.ChangeIP) << 2 | Convert.ToInt32(this.ChangeRequest.ChangePort) << 1);
					}
					else
					{
						bool flag4 = this.SourceAddress != null;
						if (flag4)
						{
							this.StoreEndPoint(STUN_Message.AttributeType.SourceAddress, this.SourceAddress, array, ref num);
						}
						else
						{
							bool flag5 = this.ChangedAddress != null;
							if (flag5)
							{
								this.StoreEndPoint(STUN_Message.AttributeType.ChangedAddress, this.ChangedAddress, array, ref num);
							}
							else
							{
								bool flag6 = this.UserName != null;
								if (flag6)
								{
									byte[] bytes = Encoding.ASCII.GetBytes(this.UserName);
									array[num++] = 0;
									array[num++] = 6;
									array[num++] = (byte)(bytes.Length >> 8);
									array[num++] = (byte)(bytes.Length & 255);
									Array.Copy(bytes, 0, array, num, bytes.Length);
									num += bytes.Length;
								}
								else
								{
									bool flag7 = this.Password != null;
									if (flag7)
									{
										byte[] bytes2 = Encoding.ASCII.GetBytes(this.UserName);
										array[num++] = 0;
										array[num++] = 7;
										array[num++] = (byte)(bytes2.Length >> 8);
										array[num++] = (byte)(bytes2.Length & 255);
										Array.Copy(bytes2, 0, array, num, bytes2.Length);
										num += bytes2.Length;
									}
									else
									{
										bool flag8 = this.ErrorCode != null;
										if (flag8)
										{
											byte[] bytes3 = Encoding.ASCII.GetBytes(this.ErrorCode.ReasonText);
											array[num++] = 0;
											array[num++] = 9;
											array[num++] = 0;
											array[num++] = (byte)(4 + bytes3.Length);
											array[num++] = 0;
											array[num++] = 0;
											array[num++] = (byte)Math.Floor((double)(this.ErrorCode.Code / 100));
											array[num++] = (byte)(this.ErrorCode.Code & 255);
											Array.Copy(bytes3, array, bytes3.Length);
											num += bytes3.Length;
										}
										else
										{
											bool flag9 = this.ReflectedFrom != null;
											if (flag9)
											{
												this.StoreEndPoint(STUN_Message.AttributeType.ReflectedFrom, this.ReflectedFrom, array, ref num);
											}
										}
									}
								}
							}
						}
					}
				}
			}
			array[2] = (byte)(num - 20 >> 8);
			array[3] = (byte)(num - 20 & 255);
			byte[] array2 = new byte[num];
			Array.Copy(array, array2, array2.Length);
			return array2;
		}

		// Token: 0x06000115 RID: 277 RVA: 0x00008654 File Offset: 0x00007654
		private void ParseAttribute(byte[] data, ref int offset)
		{
			int num = offset;
			offset = num + 1;
			STUN_Message.AttributeType attributeType = (STUN_Message.AttributeType)(data[num] << 8);
			num = offset;
			offset = num + 1;
			STUN_Message.AttributeType attributeType2 = attributeType | (STUN_Message.AttributeType)data[num];
			num = offset;
			offset = num + 1;
			int num2 = (int)data[num] << 8;
			num = offset;
			offset = num + 1;
			int num3 = num2 | (int)data[num];
			bool flag = attributeType2 == STUN_Message.AttributeType.MappedAddress;
			if (flag)
			{
				this.m_pMappedAddress = this.ParseEndPoint(data, ref offset);
			}
			else
			{
				bool flag2 = attributeType2 == STUN_Message.AttributeType.ResponseAddress;
				if (flag2)
				{
					this.m_pResponseAddress = this.ParseEndPoint(data, ref offset);
				}
				else
				{
					bool flag3 = attributeType2 == STUN_Message.AttributeType.ChangeRequest;
					if (flag3)
					{
						offset += 3;
						this.m_pChangeRequest = new STUN_t_ChangeRequest((data[offset] & 4) > 0, (data[offset] & 2) > 0);
						offset++;
					}
					else
					{
						bool flag4 = attributeType2 == STUN_Message.AttributeType.SourceAddress;
						if (flag4)
						{
							this.m_pSourceAddress = this.ParseEndPoint(data, ref offset);
						}
						else
						{
							bool flag5 = attributeType2 == STUN_Message.AttributeType.ChangedAddress;
							if (flag5)
							{
								this.m_pChangedAddress = this.ParseEndPoint(data, ref offset);
							}
							else
							{
								bool flag6 = attributeType2 == STUN_Message.AttributeType.Username;
								if (flag6)
								{
									this.m_UserName = Encoding.Default.GetString(data, offset, num3);
									offset += num3;
								}
								else
								{
									bool flag7 = attributeType2 == STUN_Message.AttributeType.Password;
									if (flag7)
									{
										this.m_Password = Encoding.Default.GetString(data, offset, num3);
										offset += num3;
									}
									else
									{
										bool flag8 = attributeType2 == STUN_Message.AttributeType.MessageIntegrity;
										if (flag8)
										{
											offset += num3;
										}
										else
										{
											bool flag9 = attributeType2 == STUN_Message.AttributeType.ErrorCode;
											if (flag9)
											{
												int code = (int)((data[offset + 2] & 7) * 100 + (data[offset + 3] & byte.MaxValue));
												this.m_pErrorCode = new STUN_t_ErrorCode(code, Encoding.Default.GetString(data, offset + 4, num3 - 4));
												offset += num3;
											}
											else
											{
												bool flag10 = attributeType2 == STUN_Message.AttributeType.UnknownAttribute;
												if (flag10)
												{
													offset += num3;
												}
												else
												{
													bool flag11 = attributeType2 == STUN_Message.AttributeType.ReflectedFrom;
													if (flag11)
													{
														this.m_pReflectedFrom = this.ParseEndPoint(data, ref offset);
													}
													else
													{
														bool flag12 = attributeType2 == STUN_Message.AttributeType.ServerName;
														if (flag12)
														{
															this.m_ServerName = Encoding.Default.GetString(data, offset, num3);
															offset += num3;
														}
														else
														{
															offset += num3;
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000116 RID: 278 RVA: 0x0000886C File Offset: 0x0000786C
		private IPEndPoint ParseEndPoint(byte[] data, ref int offset)
		{
			offset++;
			offset++;
			int num = offset;
			offset = num + 1;
			int num2 = (int)data[num] << 8;
			num = offset;
			offset = num + 1;
			int port = num2 | (int)data[num];
			byte[] array = new byte[4];
			byte[] array2 = array;
			int num3 = 0;
			num = offset;
			offset = num + 1;
			array2[num3] = data[num];
			byte[] array3 = array;
			int num4 = 1;
			num = offset;
			offset = num + 1;
			array3[num4] = data[num];
			byte[] array4 = array;
			int num5 = 2;
			num = offset;
			offset = num + 1;
			array4[num5] = data[num];
			byte[] array5 = array;
			int num6 = 3;
			num = offset;
			offset = num + 1;
			array5[num6] = data[num];
			return new IPEndPoint(new IPAddress(array), port);
		}

		// Token: 0x06000117 RID: 279 RVA: 0x000088F0 File Offset: 0x000078F0
		private void StoreEndPoint(STUN_Message.AttributeType type, IPEndPoint endPoint, byte[] message, ref int offset)
		{
			int num = offset;
			offset = num + 1;
			message[num] = (byte)((int)type >> 8);
			num = offset;
			offset = num + 1;
			message[num] = (byte)(type & (STUN_Message.AttributeType)255);
			num = offset;
			offset = num + 1;
			message[num] = 0;
			num = offset;
			offset = num + 1;
			message[num] = 8;
			num = offset;
			offset = num + 1;
			message[num] = 0;
			num = offset;
			offset = num + 1;
			message[num] = 1;
			num = offset;
			offset = num + 1;
			message[num] = (byte)(endPoint.Port >> 8);
			num = offset;
			offset = num + 1;
			message[num] = (byte)(endPoint.Port & 255);
			byte[] addressBytes = endPoint.Address.GetAddressBytes();
			num = offset;
			offset = num + 1;
			message[num] = addressBytes[0];
			num = offset;
			offset = num + 1;
			message[num] = addressBytes[1];
			num = offset;
			offset = num + 1;
			message[num] = addressBytes[2];
			num = offset;
			offset = num + 1;
			message[num] = addressBytes[3];
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x06000118 RID: 280 RVA: 0x000089D8 File Offset: 0x000079D8
		// (set) Token: 0x06000119 RID: 281 RVA: 0x000089F0 File Offset: 0x000079F0
		public STUN_MessageType Type
		{
			get
			{
				return this.m_Type;
			}
			set
			{
				this.m_Type = value;
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x0600011A RID: 282 RVA: 0x000089FC File Offset: 0x000079FC
		public int MagicCookie
		{
			get
			{
				return this.m_MagicCookie;
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x0600011B RID: 283 RVA: 0x00008A14 File Offset: 0x00007A14
		public byte[] TransactionID
		{
			get
			{
				return this.m_pTransactionID;
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x0600011C RID: 284 RVA: 0x00008A2C File Offset: 0x00007A2C
		// (set) Token: 0x0600011D RID: 285 RVA: 0x00008A44 File Offset: 0x00007A44
		public IPEndPoint MappedAddress
		{
			get
			{
				return this.m_pMappedAddress;
			}
			set
			{
				this.m_pMappedAddress = value;
			}
		}

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x0600011E RID: 286 RVA: 0x00008A50 File Offset: 0x00007A50
		// (set) Token: 0x0600011F RID: 287 RVA: 0x00008A68 File Offset: 0x00007A68
		public IPEndPoint ResponseAddress
		{
			get
			{
				return this.m_pResponseAddress;
			}
			set
			{
				this.m_pResponseAddress = value;
			}
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x06000120 RID: 288 RVA: 0x00008A74 File Offset: 0x00007A74
		// (set) Token: 0x06000121 RID: 289 RVA: 0x00008A8C File Offset: 0x00007A8C
		public STUN_t_ChangeRequest ChangeRequest
		{
			get
			{
				return this.m_pChangeRequest;
			}
			set
			{
				this.m_pChangeRequest = value;
			}
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x06000122 RID: 290 RVA: 0x00008A98 File Offset: 0x00007A98
		// (set) Token: 0x06000123 RID: 291 RVA: 0x00008AB0 File Offset: 0x00007AB0
		public IPEndPoint SourceAddress
		{
			get
			{
				return this.m_pSourceAddress;
			}
			set
			{
				this.m_pSourceAddress = value;
			}
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x06000124 RID: 292 RVA: 0x00008ABC File Offset: 0x00007ABC
		// (set) Token: 0x06000125 RID: 293 RVA: 0x00008AD4 File Offset: 0x00007AD4
		public IPEndPoint ChangedAddress
		{
			get
			{
				return this.m_pChangedAddress;
			}
			set
			{
				this.m_pChangedAddress = value;
			}
		}

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x06000126 RID: 294 RVA: 0x00008AE0 File Offset: 0x00007AE0
		// (set) Token: 0x06000127 RID: 295 RVA: 0x00008AF8 File Offset: 0x00007AF8
		public string UserName
		{
			get
			{
				return this.m_UserName;
			}
			set
			{
				this.m_UserName = value;
			}
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x06000128 RID: 296 RVA: 0x00008B04 File Offset: 0x00007B04
		// (set) Token: 0x06000129 RID: 297 RVA: 0x00008B1C File Offset: 0x00007B1C
		public string Password
		{
			get
			{
				return this.m_Password;
			}
			set
			{
				this.m_Password = value;
			}
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x0600012A RID: 298 RVA: 0x00008B28 File Offset: 0x00007B28
		// (set) Token: 0x0600012B RID: 299 RVA: 0x00008B40 File Offset: 0x00007B40
		public STUN_t_ErrorCode ErrorCode
		{
			get
			{
				return this.m_pErrorCode;
			}
			set
			{
				this.m_pErrorCode = value;
			}
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x0600012C RID: 300 RVA: 0x00008B4C File Offset: 0x00007B4C
		// (set) Token: 0x0600012D RID: 301 RVA: 0x00008B64 File Offset: 0x00007B64
		public IPEndPoint ReflectedFrom
		{
			get
			{
				return this.m_pReflectedFrom;
			}
			set
			{
				this.m_pReflectedFrom = value;
			}
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x0600012E RID: 302 RVA: 0x00008B70 File Offset: 0x00007B70
		// (set) Token: 0x0600012F RID: 303 RVA: 0x00008B88 File Offset: 0x00007B88
		public string ServerName
		{
			get
			{
				return this.m_ServerName;
			}
			set
			{
				this.m_ServerName = value;
			}
		}

		// Token: 0x04000074 RID: 116
		private STUN_MessageType m_Type = STUN_MessageType.BindingRequest;

		// Token: 0x04000075 RID: 117
		private int m_MagicCookie = 0;

		// Token: 0x04000076 RID: 118
		private byte[] m_pTransactionID = null;

		// Token: 0x04000077 RID: 119
		private IPEndPoint m_pMappedAddress = null;

		// Token: 0x04000078 RID: 120
		private IPEndPoint m_pResponseAddress = null;

		// Token: 0x04000079 RID: 121
		private STUN_t_ChangeRequest m_pChangeRequest = null;

		// Token: 0x0400007A RID: 122
		private IPEndPoint m_pSourceAddress = null;

		// Token: 0x0400007B RID: 123
		private IPEndPoint m_pChangedAddress = null;

		// Token: 0x0400007C RID: 124
		private string m_UserName = null;

		// Token: 0x0400007D RID: 125
		private string m_Password = null;

		// Token: 0x0400007E RID: 126
		private STUN_t_ErrorCode m_pErrorCode = null;

		// Token: 0x0400007F RID: 127
		private IPEndPoint m_pReflectedFrom = null;

		// Token: 0x04000080 RID: 128
		private string m_ServerName = null;

		// Token: 0x02000275 RID: 629
		private enum AttributeType
		{
			// Token: 0x04000934 RID: 2356
			MappedAddress = 1,
			// Token: 0x04000935 RID: 2357
			ResponseAddress,
			// Token: 0x04000936 RID: 2358
			ChangeRequest,
			// Token: 0x04000937 RID: 2359
			SourceAddress,
			// Token: 0x04000938 RID: 2360
			ChangedAddress,
			// Token: 0x04000939 RID: 2361
			Username,
			// Token: 0x0400093A RID: 2362
			Password,
			// Token: 0x0400093B RID: 2363
			MessageIntegrity,
			// Token: 0x0400093C RID: 2364
			ErrorCode,
			// Token: 0x0400093D RID: 2365
			UnknownAttribute,
			// Token: 0x0400093E RID: 2366
			ReflectedFrom,
			// Token: 0x0400093F RID: 2367
			XorMappedAddress = 32800,
			// Token: 0x04000940 RID: 2368
			XorOnly = 33,
			// Token: 0x04000941 RID: 2369
			ServerName = 32802
		}

		// Token: 0x02000276 RID: 630
		private enum IPFamily
		{
			// Token: 0x04000943 RID: 2371
			IPv4 = 1,
			// Token: 0x04000944 RID: 2372
			IPv6
		}
	}
}
