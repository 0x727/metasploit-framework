using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace LumiSoft.Net.AUTH
{
	// Token: 0x02000265 RID: 613
	public class AUTH_SASL_Client_Ntlm : AUTH_SASL_Client
	{
		// Token: 0x060015F9 RID: 5625 RVA: 0x000894B4 File Offset: 0x000884B4
		public AUTH_SASL_Client_Ntlm(string domain, string userName, string password)
		{
			bool flag = domain == null;
			if (flag)
			{
				throw new ArgumentNullException("domain");
			}
			bool flag2 = userName == null;
			if (flag2)
			{
				throw new ArgumentNullException("userName");
			}
			bool flag3 = password == null;
			if (flag3)
			{
				throw new ArgumentNullException("password");
			}
			this.m_Domain = domain;
			this.m_UserName = userName;
			this.m_Password = password;
		}

		// Token: 0x060015FA RID: 5626 RVA: 0x00089540 File Offset: 0x00088540
		public override byte[] Continue(byte[] serverResponse)
		{
			bool isCompleted = this.m_IsCompleted;
			if (isCompleted)
			{
				throw new InvalidOperationException("Authentication is completed.");
			}
			bool flag = this.m_State == 0;
			byte[] result;
			if (flag)
			{
				this.m_State++;
				result = new AUTH_SASL_Client_Ntlm.MessageType1(this.m_Domain, Environment.MachineName).ToByte();
			}
			else
			{
				bool flag2 = this.m_State == 1;
				if (!flag2)
				{
					throw new InvalidOperationException("Authentication is completed.");
				}
				this.m_State++;
				this.m_IsCompleted = true;
				byte[] nonce = AUTH_SASL_Client_Ntlm.MessageType2.Parse(serverResponse).Nonce;
				result = new AUTH_SASL_Client_Ntlm.MessageType3(this.m_Domain, this.m_UserName, Environment.MachineName, AUTH_SASL_Client_Ntlm.NTLM_Utils.CalculateLM(nonce, this.m_Password), AUTH_SASL_Client_Ntlm.NTLM_Utils.CalculateNT(nonce, this.m_Password)).ToByte();
			}
			return result;
		}

		// Token: 0x17000724 RID: 1828
		// (get) Token: 0x060015FB RID: 5627 RVA: 0x00089610 File Offset: 0x00088610
		public override bool IsCompleted
		{
			get
			{
				return this.m_IsCompleted;
			}
		}

		// Token: 0x17000725 RID: 1829
		// (get) Token: 0x060015FC RID: 5628 RVA: 0x00089628 File Offset: 0x00088628
		public override string Name
		{
			get
			{
				return "NTLM";
			}
		}

		// Token: 0x17000726 RID: 1830
		// (get) Token: 0x060015FD RID: 5629 RVA: 0x00089640 File Offset: 0x00088640
		public override string UserName
		{
			get
			{
				return this.m_UserName;
			}
		}

		// Token: 0x17000727 RID: 1831
		// (get) Token: 0x060015FE RID: 5630 RVA: 0x00089658 File Offset: 0x00088658
		public override bool SupportsInitialResponse
		{
			get
			{
				return true;
			}
		}

		// Token: 0x040008C2 RID: 2242
		private bool m_IsCompleted = false;

		// Token: 0x040008C3 RID: 2243
		private int m_State = 0;

		// Token: 0x040008C4 RID: 2244
		private string m_Domain = null;

		// Token: 0x040008C5 RID: 2245
		private string m_UserName = null;

		// Token: 0x040008C6 RID: 2246
		private string m_Password = null;

		// Token: 0x02000389 RID: 905
		private class MessageType1
		{
			// Token: 0x06001BDC RID: 7132 RVA: 0x000AC2A0 File Offset: 0x000AB2A0
			public MessageType1(string domain, string host)
			{
				bool flag = domain == null;
				if (flag)
				{
					throw new ArgumentNullException("domain");
				}
				bool flag2 = host == null;
				if (flag2)
				{
					throw new ArgumentNullException("host");
				}
				this.m_Domain = domain;
				this.m_Host = host;
			}

			// Token: 0x06001BDD RID: 7133 RVA: 0x000AC2FC File Offset: 0x000AB2FC
			public byte[] ToByte()
			{
				short num = (short)this.m_Domain.Length;
				short num2 = (short)this.m_Host.Length;
				byte[] array = new byte[(int)(32 + num + num2)];
				array[0] = 78;
				array[1] = 84;
				array[2] = 76;
				array[3] = 77;
				array[4] = 83;
				array[5] = 83;
				array[6] = 80;
				array[7] = 0;
				array[8] = 1;
				array[9] = 0;
				array[10] = 0;
				array[11] = 0;
				array[12] = 3;
				array[13] = 178;
				array[14] = 0;
				array[15] = 0;
				short num3 = (short)(32 + num2);
				array[16] = (byte)num;
				array[17] = (byte)(num >> 8);
				array[18] = array[16];
				array[19] = array[17];
				array[20] = (byte)num3;
				array[21] = (byte)(num3 >> 8);
				array[24] = (byte)num2;
				array[25] = (byte)(num2 >> 8);
				array[26] = array[24];
				array[27] = array[25];
				array[28] = 32;
				array[29] = 0;
				byte[] bytes = Encoding.ASCII.GetBytes(this.m_Host.ToUpper(CultureInfo.InvariantCulture));
				Buffer.BlockCopy(bytes, 0, array, 32, bytes.Length);
				byte[] bytes2 = Encoding.ASCII.GetBytes(this.m_Domain.ToUpper(CultureInfo.InvariantCulture));
				Buffer.BlockCopy(bytes2, 0, array, (int)num3, bytes2.Length);
				return array;
			}

			// Token: 0x04000CFC RID: 3324
			private string m_Domain = null;

			// Token: 0x04000CFD RID: 3325
			private string m_Host = null;
		}

		// Token: 0x0200038A RID: 906
		private class MessageType2
		{
			// Token: 0x06001BDE RID: 7134 RVA: 0x000AC43C File Offset: 0x000AB43C
			public MessageType2(byte[] nonce)
			{
				bool flag = nonce == null;
				if (flag)
				{
					throw new ArgumentNullException("nonce");
				}
				bool flag2 = nonce.Length != 8;
				if (flag2)
				{
					throw new ArgumentException("Argument 'nonce' value must be 8 bytes value.", "nonce");
				}
				this.m_Nonce = nonce;
			}

			// Token: 0x06001BDF RID: 7135 RVA: 0x000AC494 File Offset: 0x000AB494
			public static AUTH_SASL_Client_Ntlm.MessageType2 Parse(byte[] data)
			{
				bool flag = data == null;
				if (flag)
				{
					throw new ArgumentNullException("data");
				}
				byte[] array = new byte[8];
				Buffer.BlockCopy(data, 24, array, 0, 8);
				return new AUTH_SASL_Client_Ntlm.MessageType2(array);
			}

			// Token: 0x170008A0 RID: 2208
			// (get) Token: 0x06001BE0 RID: 7136 RVA: 0x000AC4D4 File Offset: 0x000AB4D4
			public byte[] Nonce
			{
				get
				{
					return this.m_Nonce;
				}
			}

			// Token: 0x04000CFE RID: 3326
			private byte[] m_Nonce = null;
		}

		// Token: 0x0200038B RID: 907
		private class MessageType3
		{
			// Token: 0x06001BE1 RID: 7137 RVA: 0x000AC4EC File Offset: 0x000AB4EC
			public MessageType3(string domain, string user, string host, byte[] lm, byte[] nt)
			{
				bool flag = domain == null;
				if (flag)
				{
					throw new ArgumentNullException("domain");
				}
				bool flag2 = user == null;
				if (flag2)
				{
					throw new ArgumentNullException("user");
				}
				bool flag3 = host == null;
				if (flag3)
				{
					throw new ArgumentNullException("host");
				}
				bool flag4 = lm == null;
				if (flag4)
				{
					throw new ArgumentNullException("lm");
				}
				bool flag5 = nt == null;
				if (flag5)
				{
					throw new ArgumentNullException("nt");
				}
				this.m_Domain = domain;
				this.m_User = user;
				this.m_Host = host;
				this.m_LM = lm;
				this.m_NT = nt;
			}

			// Token: 0x06001BE2 RID: 7138 RVA: 0x000AC5B4 File Offset: 0x000AB5B4
			public byte[] ToByte()
			{
				byte[] bytes = Encoding.Unicode.GetBytes(this.m_Domain.ToUpper(CultureInfo.InvariantCulture));
				byte[] bytes2 = Encoding.Unicode.GetBytes(this.m_User);
				byte[] bytes3 = Encoding.Unicode.GetBytes(this.m_Host.ToUpper(CultureInfo.InvariantCulture));
				byte[] array = new byte[64 + bytes.Length + bytes2.Length + bytes3.Length + 24 + 24];
				array[0] = 78;
				array[1] = 84;
				array[2] = 76;
				array[3] = 77;
				array[4] = 83;
				array[5] = 83;
				array[6] = 80;
				array[7] = 0;
				array[8] = 3;
				array[9] = 0;
				array[10] = 0;
				array[11] = 0;
				short num = (short)(64 + bytes.Length + bytes2.Length + bytes3.Length);
				array[12] = 24;
				array[13] = 0;
				array[14] = 24;
				array[15] = 0;
				array[16] = (byte)num;
				array[17] = (byte)(num >> 8);
				short num2 = (short)(num + 24);
				array[20] = 24;
				array[21] = 0;
				array[22] = 24;
				array[23] = 0;
				array[24] = (byte)num2;
				array[25] = (byte)(num2 >> 8);
				short num3 = (short)bytes.Length;
				short num4 = 64;
				array[28] = (byte)num3;
				array[29] = (byte)(num3 >> 8);
				array[30] = array[28];
				array[31] = array[29];
				array[32] = (byte)num4;
				array[33] = (byte)(num4 >> 8);
				short num5 = (short)bytes2.Length;
				short num6 = (short)(num4 + num3);
				array[36] = (byte)num5;
				array[37] = (byte)(num5 >> 8);
				array[38] = array[36];
				array[39] = array[37];
				array[40] = (byte)num6;
				array[41] = (byte)(num6 >> 8);
				short num7 = (short)bytes3.Length;
				short num8 = (short)(num6 + num5);
				array[44] = (byte)num7;
				array[45] = (byte)(num7 >> 8);
				array[46] = array[44];
				array[47] = array[45];
				array[48] = (byte)num8;
				array[49] = (byte)(num8 >> 8);
				short num9 = (short)array.Length;
				array[56] = (byte)num9;
				array[57] = (byte)(num9 >> 8);
				array[60] = 1;
				array[61] = 130;
				array[62] = 0;
				array[63] = 0;
				Buffer.BlockCopy(bytes, 0, array, (int)num4, bytes.Length);
				Buffer.BlockCopy(bytes2, 0, array, (int)num6, bytes2.Length);
				Buffer.BlockCopy(bytes3, 0, array, (int)num8, bytes3.Length);
				Buffer.BlockCopy(this.m_LM, 0, array, (int)num, 24);
				Buffer.BlockCopy(this.m_NT, 0, array, (int)num2, 24);
				return array;
			}

			// Token: 0x04000CFF RID: 3327
			private string m_Domain = null;

			// Token: 0x04000D00 RID: 3328
			private string m_User = null;

			// Token: 0x04000D01 RID: 3329
			private string m_Host = null;

			// Token: 0x04000D02 RID: 3330
			private byte[] m_LM = null;

			// Token: 0x04000D03 RID: 3331
			private byte[] m_NT = null;
		}

		// Token: 0x0200038C RID: 908
		private class NTLM_Utils
		{
			// Token: 0x06001BE3 RID: 7139 RVA: 0x000AC7FC File Offset: 0x000AB7FC
			public static byte[] CalculateLM(byte[] nonce, string password)
			{
				bool flag = nonce == null;
				if (flag)
				{
					throw new ArgumentNullException("nonce");
				}
				bool flag2 = password == null;
				if (flag2)
				{
					throw new ArgumentNullException("password");
				}
				byte[] array = new byte[21];
				byte[] inputBuffer = new byte[]
				{
					75,
					71,
					83,
					33,
					64,
					35,
					36,
					37
				};
				byte[] src = new byte[]
				{
					170,
					211,
					180,
					53,
					181,
					20,
					4,
					238
				};
				DES des = DES.Create();
				des.Mode = CipherMode.ECB;
				bool flag3 = password.Length < 1;
				if (flag3)
				{
					Buffer.BlockCopy(src, 0, array, 0, 8);
				}
				else
				{
					des.Key = AUTH_SASL_Client_Ntlm.NTLM_Utils.PasswordToKey(password, 0);
					des.CreateEncryptor().TransformBlock(inputBuffer, 0, 8, array, 0);
				}
				bool flag4 = password.Length < 8;
				if (flag4)
				{
					Buffer.BlockCopy(src, 0, array, 8, 8);
				}
				else
				{
					des.Key = AUTH_SASL_Client_Ntlm.NTLM_Utils.PasswordToKey(password, 7);
					des.CreateEncryptor().TransformBlock(inputBuffer, 0, 8, array, 8);
				}
				return AUTH_SASL_Client_Ntlm.NTLM_Utils.calc_resp(nonce, array);
			}

			// Token: 0x06001BE4 RID: 7140 RVA: 0x000AC8FC File Offset: 0x000AB8FC
			public static byte[] CalculateNT(byte[] nonce, string password)
			{
				bool flag = nonce == null;
				if (flag)
				{
					throw new ArgumentNullException("nonce");
				}
				bool flag2 = password == null;
				if (flag2)
				{
					throw new ArgumentNullException("password");
				}
				byte[] array = new byte[21];
				_MD4 md = _MD4.Create();
				byte[] src = md.ComputeHash(Encoding.Unicode.GetBytes(password));
				Buffer.BlockCopy(src, 0, array, 0, 16);
				return AUTH_SASL_Client_Ntlm.NTLM_Utils.calc_resp(nonce, array);
			}

			// Token: 0x06001BE5 RID: 7141 RVA: 0x000AC970 File Offset: 0x000AB970
			private static byte[] calc_resp(byte[] nonce, byte[] data)
			{
				byte[] array = new byte[24];
				DES des = DES.Create();
				des.Mode = CipherMode.ECB;
				des.Key = AUTH_SASL_Client_Ntlm.NTLM_Utils.setup_des_key(data, 0);
				ICryptoTransform cryptoTransform = des.CreateEncryptor();
				cryptoTransform.TransformBlock(nonce, 0, 8, array, 0);
				des.Key = AUTH_SASL_Client_Ntlm.NTLM_Utils.setup_des_key(data, 7);
				cryptoTransform = des.CreateEncryptor();
				cryptoTransform.TransformBlock(nonce, 0, 8, array, 8);
				des.Key = AUTH_SASL_Client_Ntlm.NTLM_Utils.setup_des_key(data, 14);
				cryptoTransform = des.CreateEncryptor();
				cryptoTransform.TransformBlock(nonce, 0, 8, array, 16);
				return array;
			}

			// Token: 0x06001BE6 RID: 7142 RVA: 0x000ACA00 File Offset: 0x000ABA00
			private static byte[] setup_des_key(byte[] key56bits, int position)
			{
				return new byte[]
				{
					key56bits[position],
					(byte)((int)key56bits[position] << 7 | key56bits[position + 1] >> 1),
					(byte)((int)key56bits[position + 1] << 6 | key56bits[position + 2] >> 2),
					(byte)((int)key56bits[position + 2] << 5 | key56bits[position + 3] >> 3),
					(byte)((int)key56bits[position + 3] << 4 | key56bits[position + 4] >> 4),
					(byte)((int)key56bits[position + 4] << 3 | key56bits[position + 5] >> 5),
					(byte)((int)key56bits[position + 5] << 2 | key56bits[position + 6] >> 6),
					(byte)(key56bits[position + 6] << 1)
				};
			}

			// Token: 0x06001BE7 RID: 7143 RVA: 0x000ACA9C File Offset: 0x000ABA9C
			private static byte[] PasswordToKey(string password, int position)
			{
				byte[] array = new byte[7];
				int charCount = Math.Min(password.Length - position, 7);
				Encoding.ASCII.GetBytes(password.ToUpper(CultureInfo.CurrentCulture), position, charCount, array, 0);
				return AUTH_SASL_Client_Ntlm.NTLM_Utils.setup_des_key(array, 0);
			}
		}
	}
}
