using System;
using System.Text;

namespace LumiSoft.Net.AUTH
{
	// Token: 0x0200026D RID: 621
	public class Auth_HttpDigest
	{
		// Token: 0x06001640 RID: 5696 RVA: 0x0008AEF0 File Offset: 0x00089EF0
		public Auth_HttpDigest(string digestResponse, string requestMethod)
		{
			this.m_Method = requestMethod;
			this.Parse(digestResponse);
		}

		// Token: 0x06001641 RID: 5697 RVA: 0x0008AFA0 File Offset: 0x00089FA0
		public Auth_HttpDigest(string userName, string password, string cnonce, string uri, string digestResponse, string requestMethod)
		{
			this.Parse(digestResponse);
			this.m_UserName = userName;
			this.m_Password = password;
			this.m_Method = requestMethod;
			this.m_Cnonce = cnonce;
			this.m_Uri = uri;
			this.m_Qop = "auth";
			this.m_NonceCount = 1;
			this.m_Response = this.CalculateResponse(this.m_UserName, this.m_Password);
		}

		// Token: 0x06001642 RID: 5698 RVA: 0x0008B098 File Offset: 0x0008A098
		public Auth_HttpDigest(string realm, string nonce, string opaque)
		{
			this.m_Realm = realm;
			this.m_Nonce = nonce;
			this.m_Opaque = opaque;
		}

		// Token: 0x06001643 RID: 5699 RVA: 0x0008B150 File Offset: 0x0008A150
		public bool Authenticate(string userName, string password)
		{
			return this.Response == this.CalculateResponse(userName, password);
		}

		// Token: 0x06001644 RID: 5700 RVA: 0x0008B180 File Offset: 0x0008A180
		private void Parse(string digestResponse)
		{
			string[] array = TextUtils.SplitQuotedString(digestResponse, ',');
			foreach (string text in array)
			{
				string[] array3 = text.Split(new char[]
				{
					'='
				}, 2);
				string a = array3[0].Trim();
				bool flag = array3.Length == 2;
				if (flag)
				{
					bool flag2 = string.Equals(a, "realm", StringComparison.InvariantCultureIgnoreCase);
					if (flag2)
					{
						this.m_Realm = TextUtils.UnQuoteString(array3[1]);
					}
					else
					{
						bool flag3 = string.Equals(a, "nonce", StringComparison.InvariantCultureIgnoreCase);
						if (flag3)
						{
							this.m_Nonce = TextUtils.UnQuoteString(array3[1]);
						}
						else
						{
							bool flag4 = string.Equals(a, "uri", StringComparison.InvariantCultureIgnoreCase) || string.Equals(a, "digest-uri", StringComparison.InvariantCultureIgnoreCase);
							if (flag4)
							{
								this.m_Uri = TextUtils.UnQuoteString(array3[1]);
							}
							else
							{
								bool flag5 = string.Equals(a, "qop", StringComparison.InvariantCultureIgnoreCase);
								if (flag5)
								{
									this.m_Qop = TextUtils.UnQuoteString(array3[1]);
								}
								else
								{
									bool flag6 = string.Equals(a, "nc", StringComparison.InvariantCultureIgnoreCase);
									if (flag6)
									{
										this.m_NonceCount = Convert.ToInt32(TextUtils.UnQuoteString(array3[1]));
									}
									else
									{
										bool flag7 = string.Equals(a, "cnonce", StringComparison.InvariantCultureIgnoreCase);
										if (flag7)
										{
											this.m_Cnonce = TextUtils.UnQuoteString(array3[1]);
										}
										else
										{
											bool flag8 = string.Equals(a, "response", StringComparison.InvariantCultureIgnoreCase);
											if (flag8)
											{
												this.m_Response = TextUtils.UnQuoteString(array3[1]);
											}
											else
											{
												bool flag9 = string.Equals(a, "opaque", StringComparison.InvariantCultureIgnoreCase);
												if (flag9)
												{
													this.m_Opaque = TextUtils.UnQuoteString(array3[1]);
												}
												else
												{
													bool flag10 = string.Equals(a, "username", StringComparison.InvariantCultureIgnoreCase);
													if (flag10)
													{
														this.m_UserName = TextUtils.UnQuoteString(array3[1]);
													}
													else
													{
														bool flag11 = string.Equals(a, "algorithm", StringComparison.InvariantCultureIgnoreCase);
														if (flag11)
														{
															this.m_Algorithm = TextUtils.UnQuoteString(array3[1]);
														}
														else
														{
															bool flag12 = string.Equals(a, "charset", StringComparison.InvariantCultureIgnoreCase);
															if (flag12)
															{
																this.m_Charset = TextUtils.UnQuoteString(array3[1]);
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
		}

		// Token: 0x06001645 RID: 5701 RVA: 0x0008B3B4 File Offset: 0x0008A3B4
		public string CalculateRspAuth(string userName, string password)
		{
			bool flag = string.IsNullOrEmpty(this.Algorithm) || string.Equals(this.Algorithm, "md5", StringComparison.InvariantCultureIgnoreCase);
			string text;
			if (flag)
			{
				text = string.Concat(new string[]
				{
					userName,
					":",
					this.Realm,
					":",
					password
				});
			}
			else
			{
				bool flag2 = string.Equals(this.Algorithm, "md5-sess", StringComparison.InvariantCultureIgnoreCase);
				if (!flag2)
				{
					throw new ArgumentException("Invalid Algorithm value '" + this.Algorithm + "' !");
				}
				text = string.Concat(new string[]
				{
					Net_Utils.ComputeMd5(string.Concat(new string[]
					{
						userName,
						":",
						this.Realm,
						":",
						password
					}), false),
					":",
					this.Nonce,
					":",
					this.CNonce
				});
			}
			bool flag3 = string.IsNullOrEmpty(this.Qop) || string.Equals(this.Qop, "auth", StringComparison.InvariantCultureIgnoreCase);
			if (flag3)
			{
				string text2 = ":" + this.Uri;
				bool flag4 = !string.IsNullOrEmpty(this.Qop);
				string result;
				if (flag4)
				{
					result = Net_Utils.ComputeMd5(string.Concat(new string[]
					{
						Net_Utils.ComputeMd5(text, true),
						":",
						this.Nonce,
						":",
						this.NonceCount.ToString("x8"),
						":",
						this.CNonce,
						":",
						this.Qop,
						":",
						Net_Utils.ComputeMd5(text2, true)
					}), true);
				}
				else
				{
					result = Net_Utils.ComputeMd5(string.Concat(new string[]
					{
						Net_Utils.ComputeMd5(text, true),
						":",
						this.Nonce,
						":",
						Net_Utils.ComputeMd5(text2, true)
					}), true);
				}
				return result;
			}
			throw new ArgumentException("Invalid qop value '" + this.Qop + "' !");
		}

		// Token: 0x06001646 RID: 5702 RVA: 0x0008B5FC File Offset: 0x0008A5FC
		public string CalculateResponse(string userName, string password)
		{
			bool flag = string.IsNullOrEmpty(this.Algorithm) || string.Equals(this.Algorithm, "md5", StringComparison.InvariantCultureIgnoreCase);
			string value;
			if (flag)
			{
				value = string.Concat(new string[]
				{
					userName,
					":",
					this.Realm,
					":",
					password
				});
			}
			else
			{
				bool flag2 = string.Equals(this.Algorithm, "md5-sess", StringComparison.InvariantCultureIgnoreCase);
				if (!flag2)
				{
					throw new ArgumentException("Invalid 'algorithm' value '" + this.Algorithm + "'.");
				}
				value = string.Concat(new string[]
				{
					this.H(string.Concat(new string[]
					{
						userName,
						":",
						this.Realm,
						":",
						password
					})),
					":",
					this.Nonce,
					":",
					this.CNonce
				});
			}
			bool flag3 = string.IsNullOrEmpty(this.Qop) || string.Equals(this.Qop, "auth", StringComparison.InvariantCultureIgnoreCase);
			if (flag3)
			{
				string value2 = this.RequestMethod + ":" + this.Uri;
				bool flag4 = string.Equals(this.Qop, "auth", StringComparison.InvariantCultureIgnoreCase) || string.Equals(this.Qop, "auth-int", StringComparison.InvariantCultureIgnoreCase);
				string result;
				if (flag4)
				{
					result = this.KD(this.H(value), string.Concat(new string[]
					{
						this.Nonce,
						":",
						this.NonceCount.ToString("x8"),
						":",
						this.CNonce,
						":",
						this.Qop,
						":",
						this.H(value2)
					}));
				}
				else
				{
					bool flag5 = string.IsNullOrEmpty(this.Qop);
					if (!flag5)
					{
						throw new ArgumentException("Invalid 'qop' value '" + this.Qop + "'.");
					}
					result = this.KD(this.H(value), this.Nonce + ":" + this.H(value2));
				}
				return result;
			}
			throw new ArgumentException("Invalid 'qop' value '" + this.Qop + "'.");
		}

		// Token: 0x06001647 RID: 5703 RVA: 0x0008B864 File Offset: 0x0008A864
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("realm=\"" + this.m_Realm + "\",");
			stringBuilder.Append("username=\"" + this.m_UserName + "\",");
			bool flag = !string.IsNullOrEmpty(this.m_Qop);
			if (flag)
			{
				stringBuilder.Append("qop=\"" + this.m_Qop + "\",");
			}
			stringBuilder.Append("nonce=\"" + this.m_Nonce + "\",");
			stringBuilder.Append("nc=\"" + this.m_NonceCount + "\",");
			stringBuilder.Append("cnonce=\"" + this.m_Cnonce + "\",");
			stringBuilder.Append("response=\"" + this.m_Response + "\",");
			stringBuilder.Append("opaque=\"" + this.m_Opaque + "\",");
			stringBuilder.Append("uri=\"" + this.m_Uri + "\"");
			return stringBuilder.ToString();
		}

		// Token: 0x06001648 RID: 5704 RVA: 0x0008B998 File Offset: 0x0008A998
		public string ToChallenge()
		{
			return this.ToChallenge(true);
		}

		// Token: 0x06001649 RID: 5705 RVA: 0x0008B9B4 File Offset: 0x0008A9B4
		public string ToChallenge(bool addAuthMethod)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (addAuthMethod)
			{
				stringBuilder.Append("digest ");
			}
			stringBuilder.Append("realm=" + TextUtils.QuoteString(this.m_Realm) + ",");
			bool flag = !string.IsNullOrEmpty(this.m_Qop);
			if (flag)
			{
				stringBuilder.Append("qop=" + TextUtils.QuoteString(this.m_Qop) + ",");
			}
			stringBuilder.Append("nonce=" + TextUtils.QuoteString(this.m_Nonce) + ",");
			stringBuilder.Append("opaque=" + TextUtils.QuoteString(this.m_Opaque));
			return stringBuilder.ToString();
		}

		// Token: 0x0600164A RID: 5706 RVA: 0x0008BA78 File Offset: 0x0008AA78
		public string ToAuthorization()
		{
			return this.ToAuthorization(true);
		}

		// Token: 0x0600164B RID: 5707 RVA: 0x0008BA94 File Offset: 0x0008AA94
		public string ToAuthorization(bool addAuthMethod)
		{
			bool flag = string.IsNullOrEmpty(this.m_Password);
			string str;
			if (flag)
			{
				str = this.m_Response;
			}
			else
			{
				str = this.CalculateResponse(this.m_UserName, this.m_Password);
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (addAuthMethod)
			{
				stringBuilder.Append("Digest ");
			}
			stringBuilder.Append("realm=\"" + this.m_Realm + "\",");
			stringBuilder.Append("username=\"" + this.m_UserName + "\",");
			stringBuilder.Append("nonce=\"" + this.m_Nonce + "\",");
			bool flag2 = !string.IsNullOrEmpty(this.m_Uri);
			if (flag2)
			{
				stringBuilder.Append("uri=\"" + this.m_Uri + "\",");
			}
			bool flag3 = !string.IsNullOrEmpty(this.m_Qop);
			if (flag3)
			{
				stringBuilder.Append("qop=" + this.m_Qop + ",");
			}
			bool flag4 = !string.IsNullOrEmpty(this.m_Qop);
			if (flag4)
			{
				stringBuilder.Append("nc=" + this.m_NonceCount.ToString("x8") + ",");
			}
			bool flag5 = !string.IsNullOrEmpty(this.m_Cnonce);
			if (flag5)
			{
				stringBuilder.Append("cnonce=\"" + this.m_Cnonce + "\",");
			}
			stringBuilder.Append("response=\"" + str + "\",");
			bool flag6 = !string.IsNullOrEmpty(this.m_Algorithm);
			if (flag6)
			{
				stringBuilder.Append("algorithm=\"" + this.m_Algorithm + "\",");
			}
			bool flag7 = !string.IsNullOrEmpty(this.m_Opaque);
			if (flag7)
			{
				stringBuilder.Append("opaque=\"" + this.m_Opaque + "\",");
			}
			bool flag8 = !string.IsNullOrEmpty(this.m_Charset);
			if (flag8)
			{
				stringBuilder.Append("charset=" + this.m_Charset + ",");
			}
			string text = stringBuilder.ToString().Trim();
			bool flag9 = text.EndsWith(",");
			if (flag9)
			{
				text = text.Substring(0, text.Length - 1);
			}
			return text;
		}

		// Token: 0x0600164C RID: 5708 RVA: 0x0008BCFC File Offset: 0x0008ACFC
		private string H(string value)
		{
			return Net_Utils.ComputeMd5(value, true);
		}

		// Token: 0x0600164D RID: 5709 RVA: 0x0008BD18 File Offset: 0x0008AD18
		private string KD(string key, string data)
		{
			return this.H(key + ":" + data);
		}

		// Token: 0x0600164E RID: 5710 RVA: 0x0008BD3C File Offset: 0x0008AD3C
		public static string CreateNonce()
		{
			return Guid.NewGuid().ToString().Replace("-", "");
		}

		// Token: 0x0600164F RID: 5711 RVA: 0x0008BD70 File Offset: 0x0008AD70
		public static string CreateOpaque()
		{
			return Guid.NewGuid().ToString().Replace("-", "");
		}

		// Token: 0x1700074E RID: 1870
		// (get) Token: 0x06001650 RID: 5712 RVA: 0x0008BDA4 File Offset: 0x0008ADA4
		// (set) Token: 0x06001651 RID: 5713 RVA: 0x0008BDBC File Offset: 0x0008ADBC
		public string RequestMethod
		{
			get
			{
				return this.m_Method;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					value = "";
				}
				this.m_Method = value;
			}
		}

		// Token: 0x1700074F RID: 1871
		// (get) Token: 0x06001652 RID: 5714 RVA: 0x0008BDE4 File Offset: 0x0008ADE4
		// (set) Token: 0x06001653 RID: 5715 RVA: 0x0008BDFC File Offset: 0x0008ADFC
		public string Realm
		{
			get
			{
				return this.m_Realm;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					value = "";
				}
				this.m_Realm = value;
			}
		}

		// Token: 0x17000750 RID: 1872
		// (get) Token: 0x06001654 RID: 5716 RVA: 0x0008BE24 File Offset: 0x0008AE24
		// (set) Token: 0x06001655 RID: 5717 RVA: 0x0008BE3C File Offset: 0x0008AE3C
		public string Nonce
		{
			get
			{
				return this.m_Nonce;
			}
			set
			{
				bool flag = string.IsNullOrEmpty(value);
				if (flag)
				{
					throw new ArgumentException("Nonce value can't be null or empty !");
				}
				this.m_Nonce = value;
			}
		}

		// Token: 0x17000751 RID: 1873
		// (get) Token: 0x06001656 RID: 5718 RVA: 0x0008BE68 File Offset: 0x0008AE68
		// (set) Token: 0x06001657 RID: 5719 RVA: 0x0008BE80 File Offset: 0x0008AE80
		public string Opaque
		{
			get
			{
				return this.m_Opaque;
			}
			set
			{
				this.m_Opaque = value;
			}
		}

		// Token: 0x17000752 RID: 1874
		// (get) Token: 0x06001658 RID: 5720 RVA: 0x0008BE8C File Offset: 0x0008AE8C
		// (set) Token: 0x06001659 RID: 5721 RVA: 0x0008BEA4 File Offset: 0x0008AEA4
		public string Algorithm
		{
			get
			{
				return this.m_Algorithm;
			}
			set
			{
				this.m_Algorithm = value;
			}
		}

		// Token: 0x17000753 RID: 1875
		// (get) Token: 0x0600165A RID: 5722 RVA: 0x0008BEB0 File Offset: 0x0008AEB0
		public string Response
		{
			get
			{
				return this.m_Response;
			}
		}

		// Token: 0x17000754 RID: 1876
		// (get) Token: 0x0600165B RID: 5723 RVA: 0x0008BEC8 File Offset: 0x0008AEC8
		// (set) Token: 0x0600165C RID: 5724 RVA: 0x0008BEE0 File Offset: 0x0008AEE0
		public string UserName
		{
			get
			{
				return this.m_UserName;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					value = "";
				}
				this.m_UserName = value;
			}
		}

		// Token: 0x17000755 RID: 1877
		// (get) Token: 0x0600165D RID: 5725 RVA: 0x0008BF08 File Offset: 0x0008AF08
		// (set) Token: 0x0600165E RID: 5726 RVA: 0x0008BF20 File Offset: 0x0008AF20
		public string Password
		{
			get
			{
				return this.m_Password;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					value = "";
				}
				this.m_Password = value;
			}
		}

		// Token: 0x17000756 RID: 1878
		// (get) Token: 0x0600165F RID: 5727 RVA: 0x0008BF48 File Offset: 0x0008AF48
		// (set) Token: 0x06001660 RID: 5728 RVA: 0x0008BF60 File Offset: 0x0008AF60
		public string Uri
		{
			get
			{
				return this.m_Uri;
			}
			set
			{
				this.m_Uri = value;
			}
		}

		// Token: 0x17000757 RID: 1879
		// (get) Token: 0x06001661 RID: 5729 RVA: 0x0008BF6C File Offset: 0x0008AF6C
		// (set) Token: 0x06001662 RID: 5730 RVA: 0x0008BF84 File Offset: 0x0008AF84
		public string Qop
		{
			get
			{
				return this.m_Qop;
			}
			set
			{
				this.m_Qop = value;
			}
		}

		// Token: 0x17000758 RID: 1880
		// (get) Token: 0x06001663 RID: 5731 RVA: 0x0008BF90 File Offset: 0x0008AF90
		// (set) Token: 0x06001664 RID: 5732 RVA: 0x0008BFA8 File Offset: 0x0008AFA8
		public string CNonce
		{
			get
			{
				return this.m_Cnonce;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					value = "";
				}
				this.m_Cnonce = value;
			}
		}

		// Token: 0x17000759 RID: 1881
		// (get) Token: 0x06001665 RID: 5733 RVA: 0x0008BFD0 File Offset: 0x0008AFD0
		// (set) Token: 0x06001666 RID: 5734 RVA: 0x0008BFE8 File Offset: 0x0008AFE8
		public int NonceCount
		{
			get
			{
				return this.m_NonceCount;
			}
			set
			{
				this.m_NonceCount = value;
			}
		}

		// Token: 0x06001667 RID: 5735 RVA: 0x0008BFF4 File Offset: 0x0008AFF4
		[Obsolete("Mispell error, use ToChallenge method instead.")]
		public string ToChallange()
		{
			return this.ToChallange(true);
		}

		// Token: 0x06001668 RID: 5736 RVA: 0x0008C010 File Offset: 0x0008B010
		[Obsolete("Mispell error, use ToChallenge method instead.")]
		public string ToChallange(bool addAuthMethod)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (addAuthMethod)
			{
				stringBuilder.Append("digest ");
			}
			stringBuilder.Append("realm=" + TextUtils.QuoteString(this.m_Realm) + ",");
			bool flag = !string.IsNullOrEmpty(this.m_Qop);
			if (flag)
			{
				stringBuilder.Append("qop=" + TextUtils.QuoteString(this.m_Qop) + ",");
			}
			stringBuilder.Append("nonce=" + TextUtils.QuoteString(this.m_Nonce) + ",");
			stringBuilder.Append("opaque=" + TextUtils.QuoteString(this.m_Opaque));
			return stringBuilder.ToString();
		}

		// Token: 0x040008EF RID: 2287
		private string m_Method = "";

		// Token: 0x040008F0 RID: 2288
		private string m_Realm = "";

		// Token: 0x040008F1 RID: 2289
		private string m_Nonce = "";

		// Token: 0x040008F2 RID: 2290
		private string m_Opaque = "";

		// Token: 0x040008F3 RID: 2291
		private string m_Algorithm = "";

		// Token: 0x040008F4 RID: 2292
		private string m_Response = "";

		// Token: 0x040008F5 RID: 2293
		private string m_UserName = "";

		// Token: 0x040008F6 RID: 2294
		private string m_Password = "";

		// Token: 0x040008F7 RID: 2295
		private string m_Uri = "";

		// Token: 0x040008F8 RID: 2296
		private string m_Qop = "";

		// Token: 0x040008F9 RID: 2297
		private string m_Cnonce = "";

		// Token: 0x040008FA RID: 2298
		private int m_NonceCount = 1;

		// Token: 0x040008FB RID: 2299
		private string m_Charset = "";
	}
}
