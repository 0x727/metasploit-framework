using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace LumiSoft.Net.AUTH
{
	// Token: 0x0200026A RID: 618
	public class AUTH_SASL_DigestMD5_Response
	{
		// Token: 0x0600161D RID: 5661 RVA: 0x0008A014 File Offset: 0x00089014
		public AUTH_SASL_DigestMD5_Response(AUTH_SASL_DigestMD5_Challenge challenge, string realm, string userName, string password, string cnonce, int nonceCount, string qop, string digestUri)
		{
			bool flag = challenge == null;
			if (flag)
			{
				throw new ArgumentNullException("challenge");
			}
			bool flag2 = realm == null;
			if (flag2)
			{
				throw new ArgumentNullException("realm");
			}
			bool flag3 = userName == null;
			if (flag3)
			{
				throw new ArgumentNullException("userName");
			}
			bool flag4 = password == null;
			if (flag4)
			{
				throw new ArgumentNullException("password");
			}
			bool flag5 = cnonce == null;
			if (flag5)
			{
				throw new ArgumentNullException("cnonce");
			}
			bool flag6 = qop == null;
			if (flag6)
			{
				throw new ArgumentNullException("qop");
			}
			bool flag7 = digestUri == null;
			if (flag7)
			{
				throw new ArgumentNullException("digestUri");
			}
			this.m_pChallenge = challenge;
			this.m_Realm = realm;
			this.m_UserName = userName;
			this.m_Password = password;
			this.m_Nonce = this.m_pChallenge.Nonce;
			this.m_Cnonce = cnonce;
			this.m_NonceCount = nonceCount;
			this.m_Qop = qop;
			this.m_DigestUri = digestUri;
			this.m_Response = this.CalculateResponse(userName, password);
			this.m_Charset = challenge.Charset;
		}

		// Token: 0x0600161E RID: 5662 RVA: 0x0008A184 File Offset: 0x00089184
		private AUTH_SASL_DigestMD5_Response()
		{
		}

		// Token: 0x0600161F RID: 5663 RVA: 0x0008A1F4 File Offset: 0x000891F4
		public static AUTH_SASL_DigestMD5_Response Parse(string digestResponse)
		{
			bool flag = digestResponse == null;
			if (flag)
			{
				throw new ArgumentNullException(digestResponse);
			}
			AUTH_SASL_DigestMD5_Response auth_SASL_DigestMD5_Response = new AUTH_SASL_DigestMD5_Response();
			auth_SASL_DigestMD5_Response.m_Realm = "";
			string[] array = TextUtils.SplitQuotedString(digestResponse, ',');
			foreach (string text in array)
			{
				string[] array3 = text.Split(new char[]
				{
					'='
				}, 2);
				string text2 = array3[0].Trim();
				bool flag2 = array3.Length == 2;
				if (flag2)
				{
					bool flag3 = text2.ToLower() == "username";
					if (flag3)
					{
						auth_SASL_DigestMD5_Response.m_UserName = TextUtils.UnQuoteString(array3[1]);
					}
					else
					{
						bool flag4 = text2.ToLower() == "realm";
						if (flag4)
						{
							auth_SASL_DigestMD5_Response.m_Realm = TextUtils.UnQuoteString(array3[1]);
						}
						else
						{
							bool flag5 = text2.ToLower() == "nonce";
							if (flag5)
							{
								auth_SASL_DigestMD5_Response.m_Nonce = TextUtils.UnQuoteString(array3[1]);
							}
							else
							{
								bool flag6 = text2.ToLower() == "cnonce";
								if (flag6)
								{
									auth_SASL_DigestMD5_Response.m_Cnonce = TextUtils.UnQuoteString(array3[1]);
								}
								else
								{
									bool flag7 = text2.ToLower() == "nc";
									if (flag7)
									{
										auth_SASL_DigestMD5_Response.m_NonceCount = int.Parse(TextUtils.UnQuoteString(array3[1]), NumberStyles.HexNumber);
									}
									else
									{
										bool flag8 = text2.ToLower() == "qop";
										if (flag8)
										{
											auth_SASL_DigestMD5_Response.m_Qop = TextUtils.UnQuoteString(array3[1]);
										}
										else
										{
											bool flag9 = text2.ToLower() == "digest-uri";
											if (flag9)
											{
												auth_SASL_DigestMD5_Response.m_DigestUri = TextUtils.UnQuoteString(array3[1]);
											}
											else
											{
												bool flag10 = text2.ToLower() == "response";
												if (flag10)
												{
													auth_SASL_DigestMD5_Response.m_Response = TextUtils.UnQuoteString(array3[1]);
												}
												else
												{
													bool flag11 = text2.ToLower() == "charset";
													if (flag11)
													{
														auth_SASL_DigestMD5_Response.m_Charset = TextUtils.UnQuoteString(array3[1]);
													}
													else
													{
														bool flag12 = text2.ToLower() == "cipher";
														if (flag12)
														{
															auth_SASL_DigestMD5_Response.m_Cipher = TextUtils.UnQuoteString(array3[1]);
														}
														else
														{
															bool flag13 = text2.ToLower() == "authzid";
															if (flag13)
															{
																auth_SASL_DigestMD5_Response.m_Authzid = TextUtils.UnQuoteString(array3[1]);
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
			bool flag14 = string.IsNullOrEmpty(auth_SASL_DigestMD5_Response.UserName);
			if (flag14)
			{
				throw new ParseException("The response-string doesn't contain required parameter 'username' value.");
			}
			bool flag15 = string.IsNullOrEmpty(auth_SASL_DigestMD5_Response.Nonce);
			if (flag15)
			{
				throw new ParseException("The response-string doesn't contain required parameter 'nonce' value.");
			}
			bool flag16 = string.IsNullOrEmpty(auth_SASL_DigestMD5_Response.Cnonce);
			if (flag16)
			{
				throw new ParseException("The response-string doesn't contain required parameter 'cnonce' value.");
			}
			bool flag17 = auth_SASL_DigestMD5_Response.NonceCount < 1;
			if (flag17)
			{
				throw new ParseException("The response-string doesn't contain required parameter 'nc' value.");
			}
			bool flag18 = string.IsNullOrEmpty(auth_SASL_DigestMD5_Response.Response);
			if (flag18)
			{
				throw new ParseException("The response-string doesn't contain required parameter 'response' value.");
			}
			return auth_SASL_DigestMD5_Response;
		}

		// Token: 0x06001620 RID: 5664 RVA: 0x0008A504 File Offset: 0x00089504
		public bool Authenticate(string userName, string password)
		{
			bool flag = userName == null;
			if (flag)
			{
				throw new ArgumentNullException("userName");
			}
			bool flag2 = password == null;
			if (flag2)
			{
				throw new ArgumentNullException("password");
			}
			return this.Response == this.CalculateResponse(userName, password);
		}

		// Token: 0x06001621 RID: 5665 RVA: 0x0008A55C File Offset: 0x0008955C
		public string ToResponse()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("username=\"" + this.UserName + "\"");
			stringBuilder.Append(",realm=\"" + this.Realm + "\"");
			stringBuilder.Append(",nonce=\"" + this.Nonce + "\"");
			stringBuilder.Append(",cnonce=\"" + this.Cnonce + "\"");
			stringBuilder.Append(",nc=" + this.NonceCount.ToString("x8"));
			stringBuilder.Append(",qop=" + this.Qop);
			stringBuilder.Append(",digest-uri=\"" + this.DigestUri + "\"");
			stringBuilder.Append(",response=" + this.Response);
			bool flag = !string.IsNullOrEmpty(this.Charset);
			if (flag)
			{
				stringBuilder.Append(",charset=" + this.Charset);
			}
			bool flag2 = !string.IsNullOrEmpty(this.Cipher);
			if (flag2)
			{
				stringBuilder.Append(",cipher=\"" + this.Cipher + "\"");
			}
			bool flag3 = !string.IsNullOrEmpty(this.Authzid);
			if (flag3)
			{
				stringBuilder.Append(",authzid=\"" + this.Authzid + "\"");
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06001622 RID: 5666 RVA: 0x0008A6E8 File Offset: 0x000896E8
		public string ToRspauthResponse(string userName, string password)
		{
			byte[] value = null;
			bool flag = string.IsNullOrEmpty(this.Qop) || this.Qop.ToLower() == "auth";
			if (flag)
			{
				value = Encoding.UTF8.GetBytes(":" + this.DigestUri);
			}
			else
			{
				bool flag2 = this.Qop.ToLower() == "auth-int" || this.Qop.ToLower() == "auth-conf";
				if (flag2)
				{
					value = Encoding.UTF8.GetBytes(":" + this.DigestUri + ":00000000000000000000000000000000");
				}
			}
			bool flag3 = this.Qop.ToLower() == "auth";
			if (flag3)
			{
				return "rspauth=" + this.hex(this.kd(this.hex(this.h(this.a1(userName, password))), string.Concat(new string[]
				{
					this.m_Nonce,
					":",
					this.NonceCount.ToString("x8"),
					":",
					this.Cnonce,
					":",
					this.Qop,
					":",
					this.hex(this.h(value))
				})));
			}
			throw new ArgumentException("Invalid 'qop' value '" + this.Qop + "'.");
		}

		// Token: 0x06001623 RID: 5667 RVA: 0x0008A86C File Offset: 0x0008986C
		private string CalculateResponse(string userName, string password)
		{
			bool flag = string.IsNullOrEmpty(this.Qop) || this.Qop.ToLower() == "auth";
			if (flag)
			{
				return this.hex(this.kd(this.hex(this.h(this.a1(userName, password))), string.Concat(new string[]
				{
					this.m_Nonce,
					":",
					this.NonceCount.ToString("x8"),
					":",
					this.Cnonce,
					":",
					this.Qop,
					":",
					this.hex(this.h(this.a2()))
				})));
			}
			throw new ArgumentException("Invalid 'qop' value '" + this.Qop + "'.");
		}

		// Token: 0x06001624 RID: 5668 RVA: 0x0008A95C File Offset: 0x0008995C
		private byte[] a1(string userName, string password)
		{
			bool flag = string.IsNullOrEmpty(this.Authzid);
			byte[] result;
			if (flag)
			{
				byte[] array = this.h(Encoding.UTF8.GetBytes(string.Concat(new string[]
				{
					userName,
					":",
					this.Realm,
					":",
					password
				})));
				byte[] bytes = Encoding.UTF8.GetBytes(":" + this.m_Nonce + ":" + this.Cnonce);
				byte[] array2 = new byte[array.Length + bytes.Length];
				Array.Copy(array, 0, array2, 0, array.Length);
				Array.Copy(bytes, 0, array2, array.Length, bytes.Length);
				result = array2;
			}
			else
			{
				byte[] array3 = this.h(Encoding.UTF8.GetBytes(string.Concat(new string[]
				{
					userName,
					":",
					this.Realm,
					":",
					password
				})));
				byte[] bytes2 = Encoding.UTF8.GetBytes(string.Concat(new string[]
				{
					":",
					this.m_Nonce,
					":",
					this.Cnonce,
					":",
					this.Authzid
				}));
				byte[] array4 = new byte[array3.Length + bytes2.Length];
				Array.Copy(array3, 0, array4, 0, array3.Length);
				Array.Copy(bytes2, 0, array4, array3.Length, bytes2.Length);
				result = array4;
			}
			return result;
		}

		// Token: 0x06001625 RID: 5669 RVA: 0x0008AAD8 File Offset: 0x00089AD8
		private byte[] a2()
		{
			bool flag = string.IsNullOrEmpty(this.Qop) || this.Qop.ToLower() == "auth";
			byte[] bytes;
			if (flag)
			{
				bytes = Encoding.UTF8.GetBytes("AUTHENTICATE:" + this.DigestUri);
			}
			else
			{
				bool flag2 = this.Qop.ToLower() == "auth-int" || this.Qop.ToLower() == "auth-conf";
				if (!flag2)
				{
					throw new ArgumentException("Invalid 'qop' value '" + this.Qop + "'.");
				}
				bytes = Encoding.UTF8.GetBytes("AUTHENTICATE:" + this.DigestUri + ":00000000000000000000000000000000");
			}
			return bytes;
		}

		// Token: 0x06001626 RID: 5670 RVA: 0x0008ABA0 File Offset: 0x00089BA0
		private byte[] h(byte[] value)
		{
			byte[] result;
			using (MD5 md = new MD5CryptoServiceProvider())
			{
				result = md.ComputeHash(value);
			}
			return result;
		}

		// Token: 0x06001627 RID: 5671 RVA: 0x0008ABDC File Offset: 0x00089BDC
		private byte[] kd(string secret, string data)
		{
			return this.h(Encoding.UTF8.GetBytes(secret + ":" + data));
		}

		// Token: 0x06001628 RID: 5672 RVA: 0x0008AC0C File Offset: 0x00089C0C
		private string hex(byte[] value)
		{
			return Net_Utils.ToHex(value);
		}

		// Token: 0x1700073C RID: 1852
		// (get) Token: 0x06001629 RID: 5673 RVA: 0x0008AC24 File Offset: 0x00089C24
		public string UserName
		{
			get
			{
				return this.m_UserName;
			}
		}

		// Token: 0x1700073D RID: 1853
		// (get) Token: 0x0600162A RID: 5674 RVA: 0x0008AC3C File Offset: 0x00089C3C
		public string Realm
		{
			get
			{
				return this.m_Realm;
			}
		}

		// Token: 0x1700073E RID: 1854
		// (get) Token: 0x0600162B RID: 5675 RVA: 0x0008AC54 File Offset: 0x00089C54
		public string Nonce
		{
			get
			{
				return this.m_Nonce;
			}
		}

		// Token: 0x1700073F RID: 1855
		// (get) Token: 0x0600162C RID: 5676 RVA: 0x0008AC6C File Offset: 0x00089C6C
		public string Cnonce
		{
			get
			{
				return this.m_Cnonce;
			}
		}

		// Token: 0x17000740 RID: 1856
		// (get) Token: 0x0600162D RID: 5677 RVA: 0x0008AC84 File Offset: 0x00089C84
		public int NonceCount
		{
			get
			{
				return this.m_NonceCount;
			}
		}

		// Token: 0x17000741 RID: 1857
		// (get) Token: 0x0600162E RID: 5678 RVA: 0x0008AC9C File Offset: 0x00089C9C
		public string Qop
		{
			get
			{
				return this.m_Qop;
			}
		}

		// Token: 0x17000742 RID: 1858
		// (get) Token: 0x0600162F RID: 5679 RVA: 0x0008ACB4 File Offset: 0x00089CB4
		public string DigestUri
		{
			get
			{
				return this.m_DigestUri;
			}
		}

		// Token: 0x17000743 RID: 1859
		// (get) Token: 0x06001630 RID: 5680 RVA: 0x0008ACCC File Offset: 0x00089CCC
		public string Response
		{
			get
			{
				return this.m_Response;
			}
		}

		// Token: 0x17000744 RID: 1860
		// (get) Token: 0x06001631 RID: 5681 RVA: 0x0008ACE4 File Offset: 0x00089CE4
		public string Charset
		{
			get
			{
				return this.m_Charset;
			}
		}

		// Token: 0x17000745 RID: 1861
		// (get) Token: 0x06001632 RID: 5682 RVA: 0x0008ACFC File Offset: 0x00089CFC
		public string Cipher
		{
			get
			{
				return this.m_Cipher;
			}
		}

		// Token: 0x17000746 RID: 1862
		// (get) Token: 0x06001633 RID: 5683 RVA: 0x0008AD14 File Offset: 0x00089D14
		public string Authzid
		{
			get
			{
				return this.m_Authzid;
			}
		}

		// Token: 0x040008DB RID: 2267
		private AUTH_SASL_DigestMD5_Challenge m_pChallenge = null;

		// Token: 0x040008DC RID: 2268
		private string m_UserName = null;

		// Token: 0x040008DD RID: 2269
		private string m_Password = null;

		// Token: 0x040008DE RID: 2270
		private string m_Realm = null;

		// Token: 0x040008DF RID: 2271
		private string m_Nonce = null;

		// Token: 0x040008E0 RID: 2272
		private string m_Cnonce = null;

		// Token: 0x040008E1 RID: 2273
		private int m_NonceCount = 0;

		// Token: 0x040008E2 RID: 2274
		private string m_Qop = null;

		// Token: 0x040008E3 RID: 2275
		private string m_DigestUri = null;

		// Token: 0x040008E4 RID: 2276
		private string m_Response = null;

		// Token: 0x040008E5 RID: 2277
		private string m_Charset = null;

		// Token: 0x040008E6 RID: 2278
		private string m_Cipher = null;

		// Token: 0x040008E7 RID: 2279
		private string m_Authzid = null;
	}
}
